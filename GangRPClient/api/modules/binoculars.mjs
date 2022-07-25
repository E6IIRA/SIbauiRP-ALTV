import * as alt from "alt-client";
import * as native from "natives";
import webview from "/api/modules/webview.mjs";

class Binoculars {
  constructor() {
    this.enabled = false;
    this.scaleform = null;
    this.everyTick = null;
    this.cam = null;
    this.hasScaleFormLoaded = false;
    this.fovMax = 80.0;
    this.fovMin = 5.0; //Max Zoom Level (smaller fov is more zoom)
    this.fov = 37.5;
    this.visionState = 0; // 0 is normal, 1 is nightmode, 2 is thermal vision
    this.speedLR = 8.0; //Cam Left Right Speed
    this.speedUD = 8.0; //Cam Up Down Speed
    this.zoomSpeed = 4.0; //Cam Zoomspeed
    this.zoomValue = 0;
    this.type = 0;
    this.KEYS = {
      SCROLLWHEEL_UP: 241,
      SCROLLWHEEL_DOWN: 242
    };

    alt.onServer("StartBinoculars", type => {
      this.type = type;
      this.start();
    });
  }

  start(tryNumber = 0) {
    alt.toggleGameControls(false);
    if (tryNumber == 0) {
      if (this.enabled) return;
      this.enabled = true;
      native.taskStartScenarioInPlace(
        alt.Player.local.scriptID,
        "WORLD_HUMAN_BINOCULARS",
        0,
        1
      );
      native.playPedAmbientSpeechNative(
        alt.Player.local.scriptID,
        "GENERIC_CURSE_MED",
        "SPEECH_PARAMS_FORCE",
        0
      );
      alt.setTimeout(() => {
        native.setTimecycleModifier("heilGunCam");
        native.setTimecycleModifierStrength(0.3);
      }, 2000);

      this.scaleform = native.requestScaleformMovie("HELI_CAM");
    }
    if (!native.hasScaleformMovieLoaded(this.scaleform)) {
      if (tryNumber > 100) {
        alt.toggleGameControls(true);
        return;
      }

      alt.setTimeout(() => {
        this.start(++tryNumber);
      }, 25);
      return;
    }

    alt.setTimeout(() => {
      if (this.enabled) {
        native.playSoundFrontend(
          -1,
          "SELECT",
          "HUD_FRONTEND_DEFAULT_SOUNDSET",
          false
        );

        this.cam = native.createCam("DEFAULT_SCRIPTED_FLY_CAMERA", true);
        native.attachCamToEntity(
          this.cam,
          alt.Player.local.scriptID,
          0.0,
          0.5,
          0.7,
          true
        );
        native.setCamRot(
          this.cam,
          0.0,
          0.0,
          native.getEntityHeading(alt.Player.local.scriptID),
          2
        );
        native.setCamFov(this.cam, this.fov);
        native.renderScriptCams(true, false, 0, 1, 0, 0)

        native.beginScaleformMovieMethod(this.scaleform, "SET_CAM_LOGO");
        if (this.type == 1) {
          native.scaleformMovieMethodAddParamInt(0);
        } else if (this.type == 2) {
          native.scaleformMovieMethodAddParamInt(1);
        }
        native.endScaleformMovieMethod();

        native.drawScaleformMovieFullscreen(
          this.scaleform,
          255,
          255,
          255,
          255,
          0
        );

        native.displayRadar(false);
        webview.updateView("ShowHud", [false]);

        alt.toggleGameControls(true);
        this.everyTick = alt.everyTick(this.keyHandler.bind(this));
      }
    }, 800);
  }

  stop() {
    if (!this.enabled) return;
    this.enabled = false;
    alt.clearEveryTick(this.everyTick);

    native.playSoundFrontend(
      -1,
      "SELECT",
      "HUD_FRONTEND_DEFAULT_SOUNDSET",
      false
    );
    native.clearPedTasks(alt.Player.local.scriptID);

    native.clearTimecycleModifier();
    this.fov = (this.fovMax + this.fovMin) * 0.5;
    native.renderScriptCams(false, false, 0, 1, 0, 0)
    native.setScaleformMovieAsNoLongerNeeded(this.scaleform);
    native.destroyCam(this.cam, false);
    native.setNightvision(false);
    native.setSeethrough(false);

    native.displayRadar(true);
    webview.updateView("ShowHud", [true]);
  }

  keyHandler() {
    native.blockWeaponWheelThisFrame();
    native.disableControlAction(1, 99, true);
    native.disableControlAction(1, 200, true);

    if (native.isEntityDead(alt.Player.local.scriptID, true) || !this.enabled) {
      this.stop();
    }

    this.zoomValue =
      (1.0 / (this.fovMax - this.fovMin)) * (this.fov - this.fovMin);

    this.checkInputRotation();
    this.handleZoom();

    native.beginScaleformMovieMethod(this.scaleform, "SET_ALT_FOV_HEADING");
    native.scaleformMovieMethodAddParamFloat(native.getEntityCoords(alt.Player.local.pos, true).z)
    native.scaleformMovieMethodAddParamFloat(this.zoomValue);
    native.scaleformMovieMethodAddParamFloat(native.getCamRot(this.cam, 2).z);
    native.endScaleformMovieMethod();
    native.drawScaleformMovieFullscreen(this.scaleform, 255, 255, 255, 255, 0);
  }

  changeVision() {
    if (this.visionState == 0) {
      native.setNightvision(true);
      this.visionState = 1;
    } else if (this.visionState == 1) {
      native.setNightvision(false);
      if (this.type == 1) {
        this.visionState = 0;
        return;
      }
      native.setSeethrough(true);
      this.visionState = 2;
    } else if (this.visionState == 2) {
      native.setSeethrough(false);
      this.visionState = 0;
    }
  }

  checkInputRotation() {
    let rightAxisX = native.getDisabledControlNormal(0, 220);
    let rightAxisY = native.getDisabledControlNormal(0, 221);
    let rotation = native.getCamRot(this.cam, 2);
    if (rightAxisX != 0.0 || rightAxisY != 0.0) {
      let new_z =
        rotation.z + -1 * rightAxisX * this.speedUD * (this.zoomValue + 0.1);
      let new_x = Math.max(
        Math.min(
          70.0,
          rotation.x + -1 * rightAxisY * this.speedLR * (this.zoomValue + 0.1)
        ),
        -80.0
      );
      native.setCamRot(this.cam, new_x, 0.0, new_z, 2);
      native.setEntityHeading(alt.Player.local.scriptID, new_z);
    }
  }

  handleZoom() {
    if (native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_UP)) {
      this.fov = Math.max(this.fov - this.zoomSpeed, this.fovMin);
    }
    if (native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_DOWN)) {
      this.fov = Math.min(this.fov + this.zoomSpeed, this.fovMax);
    }
    let currentFov = native.getCamFov(this.cam);
    if (Math.abs(this.fov - currentFov) < 0.1) {
      this.fov = currentFov;
    }
    native.setCamFov(this.cam, currentFov + (this.fov - currentFov) * 0.05);
  }

  rotAnglesToVec(rot) {
    let z = (rot.z * Math.PI) / 180;
    let x = (rot.x * Math.PI) / 180;
    let num = Math.abs(Math.cos(x));
    return new alt.Vector3(-Math.sin(z) * num, Math.cos(z) * num, Math.sin(x));
  }
}
export default new Binoculars();
