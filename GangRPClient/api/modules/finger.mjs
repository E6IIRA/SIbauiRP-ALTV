import alt from "alt"
import native from "natives"

class Finger {
    constructor() {
        this.active = false
        this.interval = null
        this.cleanStart = false
        //this.gameplayCam = native.createCameraWithParams("gameplay")
        this.localPlayer = alt.Player.local
    }

    start() {
        if (!this.active) {
            this.active = true

            this.requestAnimDictPromise("anim@mp_point").then(()=>{
                //native.setPedCurrentWeaponVisible(this.localPlayer.scriptID, false, true, true, true)
                native.setPedConfigFlag(this.localPlayer.scriptID, 36, true)
                native.taskMoveNetworkByName(this.localPlayer.scriptID,"task_mp_pointing", 0.5, false, "anim@mp_point", 24)
                native.removeAnimDict("anim@mp_point")
                this.cleanStart = true
                this.interval = alt.setInterval(this.process.bind(this), 20)
            }).catch(()=>{alt.log('Promise rejected')})

        }
    }

    stop() {
        if (this.active) {
            if(this.interval) {
                alt.clearInterval(this.interval)
            }
            this.interval = null

            this.active = false

            if(this.cleanStart){
                this.cleanStart = false
                native.requestTaskMoveNetworkStateTransition(this.localPlayer.scriptID, "Stop")

                if (!native.isPedInjured(this.localPlayer.scriptID)) {
                    native.clearPedSecondaryTask(this.localPlayer.scriptID)
                }
                // if (!native.isPedInAnyVehicle(this.localPlayer.scriptID, true)) {
                //     native.setPedCurrentWeaponVisible(this.localPlayer.scriptID, true, true, true, true)
                // }
                native.setPedConfigFlag(this.localPlayer.scriptID, 36, false)
                native.clearPedSecondaryTask(this.localPlayer.scriptID)
            }
        }
    }

    getRelativePitch () {
        const camRot = native.getFinalRenderedCamRot(2)
        return camRot.x - native.getEntityPitch(this.localPlayer.scriptID)
    }

    process () {
        if (this.active) {

            native.isTaskMoveNetworkActive(this.localPlayer.scriptID)

            let camPitch = this.getRelativePitch()

            if (camPitch < -70.0) {
                camPitch = -70.0
            }
            else if (camPitch > 42.0) {
                camPitch = 42.0
            }
            camPitch = (camPitch + 70.0) / 112.0

            let camHeading = native.getGameplayCamRelativeHeading()

            // let cosCamHeading = Math.cos(camHeading)
            // let sinCamHeading = Math.sin(camHeading)

            if (camHeading < -180.0) {
                camHeading = -180.0
            }
            else if (camHeading > 180.0) {
                camHeading = 180.0
            }
            camHeading = (camHeading + 180.0) / 360.0

            // let coords = native.getOffsetFromEntityInWorldCoords(this.localPlayer.scriptID, (cosCamHeading * -0.2) - (sinCamHeading *
            // (0.4 * camHeading + 0.3)), (sinCamHeading * -0.2) + (cosCamHeading * (0.4 * camHeading + 0.3)), 0.6)

            // let ray = native.startShapeTestCapsule(coords.x, coords.y, coords.z - 0.2, coords.x, coords.y, coords.z + 0.2, 1.0, 95, this.localPlayer.scriptID, 7)
            // let [_, blocked, coords1, coords2, entity] = native.getShapeTestResult(ray, false, null, null, null)

            native.setTaskMoveNetworkSignalFloat(this.localPlayer.scriptID, "Pitch",  parseFloat(camPitch))
            native.setTaskMoveNetworkSignalFloat(this.localPlayer.scriptID, "Heading", parseFloat(camHeading) * -1.0 + 1.0)
            //native.setTaskMoveNetworkSignalBool(this.localPlayer.scriptID, "isBlocked", blocked)
            native.setTaskMoveNetworkSignalBool(this.localPlayer.scriptID, "isFirstPerson", native.getCamViewModeForContext(native.getCamActiveViewModeContext()) === 4)
        }
    }

    requestAnimDictPromise(dict) {
        native.requestAnimDict(dict)
        return new Promise((resolve, reject) => {
            const check = alt.setInterval(() => {
                if (native.hasAnimDictLoaded(dict)) {
                    alt.clearInterval(check)
                    resolve(true)
                }
            },(5))
        })
    }
}

export default new Finger()