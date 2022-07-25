import * as alt from "alt-client";
import * as native from "natives";
import webview from "/api/modules/webview.mjs"
import vehicle from "/api/modules/vehicle.mjs"

class Helicam {
    constructor() {
        this.enabled = false
        this.scaleform = null
        this.everyTick = null;
        this.cam = null
        this.hasScaleFormLoaded = false
        this.fovMax = 80.0
        this.fovMin = 5.0 //Max Zoom Level (smaller fov is more zoom)
        this.fov = 45
        this.visionState = 0 // 0 is normal, 1 is nightmode, 2 is thermal vision
        this.speedLR = 6.0 //Cam Left Right Speed
        this.speedUD = 6.0 //Cam Up Down Speed
        this.zoomSpeed = 2.0 //Cam Zoomspeed
        this.lockedOnVeh = null
        this.zoomValue = 0
        this.tempVeh = null
        this.isCastingEMP = false
        this.empVeh = null
        this.empMessage = "READY"
        this.empCooldown = 60000
        this.empDurations = [
            1000,
            1000,
            1000,
            1000,
            1000,
            1000,
        ]
        this.KEYS = {
            FORWARD: 32,
            BACKWARD: 33,
            LEFT: 34,
            RIGHT: 35,
            E: 38,
            H: 101,
            Q: 44,
            SCROLLWHEEL_UP: 241,
            SCROLLWHEEL_DOWN: 242,
            SPACEBAR: 255
        };
    }

    start(tryNumber = 0) {

        if(!this.isHeliHighEnough()) {
            webview.updateView("AddNotify", ["Helikopter ist nicht hoch genug!", "#c72020"])
            return
        }
        if(tryNumber == 0)
        {
            if (this.enabled) return;
            this.enabled = true;
            //native.taskStartScenarioInPlace(alt.Player.local.scriptID, "WORLD_HUMAN_BINOCULARS", 0, 1)
            //native.playPedAmbientSpeechNative(alt.Player.local.scriptID, "GENERIC_CURSE_MED", "SPEECH_PARAMS_FORCE")
            alt.setTimeout(() => {
                native.setTimecycleModifier("heilGunCam")
                native.setTimecycleModifierStrength(0.3)
            }, 2000);

            this.scaleform = native.requestScaleformMovie("HELI_CAM")
        }
        if(!native.hasScaleformMovieLoaded(this.scaleform)) {
            if(tryNumber > 100) { 
                return;
            }

            alt.setTimeout(() => {
                this.start(++tryNumber);
            },25)
            return;
        }


        native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)

        this.cam = native.createCam("DEFAULT_SCRIPTED_FLY_CAMERA", true)
        native.attachCamToEntity(this.cam, alt.Player.local.vehicle.scriptID, 0.0, 3.0, -1.5, true)
        native.setCamRot(this.cam, 0.0, 0.0, native.getEntityHeading(alt.Player.local.scriptID), 2)
        native.setCamFov(this.cam, parseFloat(this.fov))
        native.renderScriptCams(true, false, 0, 1, 0, 0)


        native.beginScaleformMovieMethod(this.scaleform, "SET_CAM_LOGO");
        native.scaleformMovieMethodAddParamInt(1);
        native.endScaleformMovieMethod();

        native.drawScaleformMovieFullscreen(this.scaleform, 255, 255, 255, 255, 0)
        

        native.displayRadar(false)
        webview.updateView("ShowHud", [false])
        this.everyTick = alt.everyTick(this.keyHandler.bind(this));
    }

    stop() {
        if (!this.enabled) return;
        this.enabled = false;
        alt.clearEveryTick(this.everyTick);

        native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
        native.clearPedTasks(alt.Player.local.scriptID)
            

        native.clearTimecycleModifier()
        this.fov = (this.fovMax + this.fovMin) * 0.5
        native.renderScriptCams(false, false, 0, 1, 0, 0)
        native.setScaleformMovieAsNoLongerNeeded(this.scaleform)
        native.destroyCam(this.cam, false)
        native.setNightvision(false)
        native.setSeethrough(false)
        

        native.displayRadar(true)
        webview.updateView("ShowHud", [true])
        webview.updateView("AddNotify", ["Kamera gestoppt!", "#c72020"])
    }

    keyHandler() {
        
        native.disableControlAction(1, 99, true)
        native.disableControlAction(1, 100, true)
        native.disableControlAction(1, 200, true)
        native.disableControlAction(1, 81, true)

        if(native.isEntityDead(alt.Player.local.scriptID, true) || alt.Player.local.vehicle == null || !this.enabled || !this.isHeliHighEnough())
        {
            this.stop()
        }

        this.zoomValue = (1.0/(this.fovMax - this.fovMin)) * (this.fov - this.fovMin)


        this.checkInputRotation()
        let vehicleDetected = this.getVehicleInView()
        this.renderVehicleInfo(vehicleDetected)
        if(vehicleDetected != null && native.doesEntityExist(vehicleDetected)) {
            this.tempVeh = vehicleDetected 
        }
        else
        {
            this.tempVeh = null
        }

        this.handleZoom()
        
        native.beginScaleformMovieMethod(this.scaleform, "SET_ALT_FOV_HEADING");
        native.scaleformMovieMethodAddParamFloat(native.getEntityCoords(alt.Player.local.pos, true).z)
        native.scaleformMovieMethodAddParamFloat(this.zoomValue)
        native.scaleformMovieMethodAddParamFloat(native.getCamRot(this.cam, 2).z)
        native.endScaleformMovieMethod();
        native.drawScaleformMovieFullscreen(this.scaleform, 255, 255, 255, 255, 0)
    }

    triggerEMP() {
        if(this.tempVeh != null && !this.isCastingEMP)
        {
            native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
            this.isCastingEMP = true
            this.empVeh = this.tempVeh
            this.empStep(0)
        }
    }

    empStep(step) {
        if(step >= this.empDurations.length)
        {
            let veh = null
            for (const vehicle of alt.Vehicle.streamedIn) {
                if (vehicle.scriptID == this.empVeh) {
                    veh = vehicle
                    break
                }
            }
            if(veh != null)
            {
                alt.emitServer("EMPVehicle", veh)
                native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
                this.empMessage = "VEHICLE DISABLED"
                alt.setTimeout(() => {
                    this.empMessage = "RECHARGING..."
                }, 5000)
            }
            alt.setTimeout(() => {
                this.isCastingEMP = false
                this.empMessage = "READY"
            }, this.empCooldown)
            return;
        }
        alt.setTimeout(() => {
            if(this.tempVeh == this.empVeh)
            {
                step = step + 1
                this.empMessage = step + "/" + this.empDurations.length
                native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
                if(step == 1)
                {
                    for (const vehicle of alt.Vehicle.streamedIn) {
                        if (vehicle.scriptID == this.empVeh) {
                            alt.emitServer("EMPVehicleWarn", vehicle)
                            break
                        }
                    }
                }
                this.empStep(step)
                return;
            }
            else
            {
                this.empMessage = "READY"
                this.isCastingEMP = false
                this.empVeh = null
                return;
            }
        }, this.empDurations[step])
    }

    isHeliHighEnough()
    {
        //return true;
        return native.getEntityHeightAboveGround(alt.Player.local.vehicle.scriptID) > 1.5
    }

    changeVision() {
        if (this.visionState == 0)
        {
            native.setNightvision(true)
            this.visionState = 1
        }
        else if (this.visionState == 1)
        {
            native.setNightvision(false)
            native.seethroughSetMaxThickness(5)
            native.setSeethrough(true)
            this.visionState = 2
        }
        else if (this.visionState == 2)
        {
            native.setSeethrough(false)
            this.visionState = 0
        }
    }

    checkInputRotation()
    {
        let rightAxisX = native.getDisabledControlNormal(0, 220)
        let rightAxisY = native.getDisabledControlNormal(0, 221)
        let rotation = native.getCamRot(this.cam, 2)
        if (rightAxisX != 0.0 || rightAxisY != 0.0) {
            let new_z = rotation.z + (-1) * rightAxisX * this.speedUD * (this.zoomValue + 0.1)
            let new_x = Math.max(Math.min(20.0, rotation.x + (-1) * rightAxisY * this.speedLR * (this.zoomValue + 0.1)), -89.5)
            native.setCamRot(this.cam, new_x, 0.0, new_z, 2)
        }
    }

    handleZoom()
    {
        if(native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_UP))
        {
            this.fov = Math.max(this.fov - this.zoomSpeed, this.fovMin)
        }
        if(native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_DOWN))
        {
            this.fov = Math.min(this.fov + this.zoomSpeed, this.fovMax)
        }
        let currentFov = native.getCamFov(this.cam)
        if(Math.abs(this.fov - currentFov) < 0.1)
        {
            this.fov = currentFov
        }
        native.setCamFov(this.cam, currentFov + (this.fov - currentFov)*0.05)
    }

    getVehicleInView() {
        let coords = native.getCamCoord(this.cam)
        let forwardVector = this.rotAnglesToVec(native.getCamRot(this.cam, 2))
        let raycast = native.startExpensiveSynchronousShapeTestLosProbe(
            coords.x,
            coords.y,
            coords.z,
            coords.x + forwardVector.x * 200,
            coords.y + forwardVector.y * 200,
            coords.z + forwardVector.z * 200,
            10,
            alt.Player.local.vehicle,
            0,
            0
        )
        
        const [_, hasHit, endCoords, surfaceNormal, entity] = native.getShapeTestResult(raycast)
        
        if (entity > 0 && native.isEntityAVehicle(entity))
        {
            return entity
        } 
        else
        {
            return null
        }
    }

    renderVehicleInfo(veh)
    {
        let model
        let vehname
        let licenseplate
        let speed
        // alt.log(vehicle)
        if(veh != undefined)
        {
            model = native.getEntityModel(veh)
            let vehinfo = vehicle.vehicleData.find(veh => veh.hash == model || alt.hash(veh.hash) == model)
            vehname = vehinfo.name
            licenseplate = native.getVehicleNumberPlateText(veh)
            speed = Math.round(native.getEntitySpeed(veh) * 3.6)
        }
        else
        {
            model = "-"
            vehname = "-"
            licenseplate = "-"
            speed = "-"
        }
        native.beginTextCommandDisplayText('STRING')
        native.setTextFont(0)
        native.setTextProportional(1)
        native.setTextScale(0.0, 0.45)
        native.setTextColour(255, 255, 255, 255)
        native.setTextDropshadow(0, 0, 0, 0, 255)
        native.setTextEdge(1, 0, 0, 0, 255)
        native.setTextDropShadow()
        native.setTextOutline()
        native.addTextComponentSubstringPlayerName("Model: " + vehname + "\nPlate: " + licenseplate + "  |  Speed: " + speed + "km/h" + "\nEMP: " + this.empMessage)
        native.endTextCommandDisplayText(0.45, 0.9, 0.0)
    }

    rotAnglesToVec(rot)
    {
        let z = rot.z * Math.PI / 180
        let x = rot.x * Math.PI / 180
        let num = Math.abs(Math.cos(x))
        return new alt.Vector3(-Math.sin(z)*num, Math.cos(z)*num, Math.sin(x))
    }
}
export default new Helicam()