import * as alt from "alt-client";
import * as native from "natives";
import webview from "/api/modules/webview.mjs";
import player from "/api/modules/player.mjs";

class Cam {
  constructor() {
    this.enabled = false;
    this.everyTick = null;
    this.camera = null;
    this.oldPos = null;
    this.cams = null;
    this.activeCamindex = 0;
    this.centralPosition = 0;
    this.centralRange = 0;
    this.interval = null;
  }

  start(cams, x, y, z, r) {
    this.cams = cams;
    this.centralPosition = new alt.Vector3(x, y, z);
    this.centralRange = r;
    if (this.getDistance(this.centralPosition) > this.centralRange) {
      webview.updateView("AddNotify", [
        "Zu weit von der Position entfernt!",
        "#00a6cc"
      ]);
      return;
    }
    this.activeCamindex = 0;
    if (this.enabled) return;
    this.enabled = true;
    this.oldPos = alt.Player.local.pos;
    this.camera = native.createCamWithParams(
      "DEFAULT_SCRIPTED_CAMERA",
      parseFloat(this.cams[this.activeCamindex].x),
      parseFloat(this.cams[this.activeCamindex].y),
      parseFloat(this.cams[this.activeCamindex].z),
      parseFloat(this.cams[this.activeCamindex].rx),
      parseFloat(this.cams[this.activeCamindex].ry),
      parseFloat(this.cams[this.activeCamindex].rz),
      parseFloat(this.cams[this.activeCamindex].f),
      true,
      0
    );
    //native.pointCamAtCoord(this.camera, pos.x, pos.y, pos.z)
    native.renderScriptCams(true, false, 0, true, false, 0)
    native.freezeEntityPosition(alt.Player.local.scriptID, true);
    native.animpostfxPlay("MP_OrbitalCannon", 0, true);
    this.interval = alt.setInterval(this.intervalHandler.bind(this), 10000);

    //native.setEntityVisible(alt.Player.local.scriptID, false, false)
    //native.setEntityCoords(alt.Player.local.scriptID, 241.967, 214.86, 108.45 + 100, false, false, false, false)
  }
  stop() {
    if (!this.enabled) return;
    this.enabled = false;
    native.setCamActive(this.camera, false);
    native.destroyCam(this.camera, false);
    native.renderScriptCams(false, false, 0, true, false, 0)
    native.freezeEntityPosition(alt.Player.local.scriptID, false);
    alt.clearInterval(this.interval);
    native.animpostfxStop("MP_OrbitalCannon");
    //native.setEntityVisible(alt.Player.local.scriptID, true, false)
    //native.setEntityCoords(alt.Player.local.scriptID, this.oldPos.x, this.oldPos.y, this.oldPos.z - 1.0, false, false, false, false)
    alt.emitServer("StopCCTV");
  }
  showCam() {
    let oldCam = this.camera;
    // alt.log(JSON.stringify(this.cams[this.activeCamindex]))
    this.camera = native.createCamWithParams(
      "DEFAULT_SCRIPTED_CAMERA",
      parseFloat(this.cams[this.activeCamindex].x),
      parseFloat(this.cams[this.activeCamindex].y),
      parseFloat(this.cams[this.activeCamindex].z),
      parseFloat(this.cams[this.activeCamindex].rx),
      parseFloat(this.cams[this.activeCamindex].ry),
      parseFloat(this.cams[this.activeCamindex].rz),
      parseFloat(this.cams[this.activeCamindex].f),
      true,
      0
    );
    native.setCamActive(this.camera, true);
    native.destroyCam(oldCam, false);
    native.renderScriptCams(true, false, 0, true, false, 0)
    //native.setEntityCoords(alt.Player.local.scriptID, this.cams[this.activeCamindex].x, this.cams[this.activeCamindex].y, this.cams[this.activeCamindex].z + 100, false, false, false, false)
  }
  nextCam() {
    this.activeCamindex++;
    if (this.activeCamindex >= this.cams.length) this.activeCamindex = 0;
    this.showCam();
  }
  previousCam() {
    this.activeCamindex--;
    if (this.activeCamindex < 0) this.activeCamindex = this.cams.length - 1;
    this.showCam();
  }
  getDistance(position) {
    const playerPosition = alt.Player.local.pos;
    return Math.sqrt(
      (playerPosition.x - position.x) * (playerPosition.x - position.x) +
        (playerPosition.y - position.y) * (playerPosition.y - position.y) +
        (playerPosition.z - position.z) * (playerPosition.z - position.z)
    );
  }
  intervalHandler() {
    if (
      this.getDistance(this.centralPosition) > this.centralRange ||
      player.isInjured ||
      player.isCuffed ||
      native.isPedBeingStunned(alt.Player.local.scriptID, 0)
    ) {
      webview.closeInterface();
      this.stop();
      return;
    }
  }
}

export default new Cam();
