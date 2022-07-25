import * as alt from "alt-client";
import * as native from "natives";
import player from "/api/modules/player.mjs"
import animation from "/api/modules/animation.mjs"

class Rappel {
    constructor() {

        this.isActive = false;

        alt.onServer("StartRappel", async (pos, rot) => {
            native.setGravityLevel(3)
            native.requestAnimDict("missrappel")
            //await animation.playAnimAtCoordsFullParamsAndWait("mp_common_heist", "rappel_intro", 8, pos, rot, 1, 1, -1, 0)
            alt.setTimeout(() => {
                animation.playAnimAtCoordsFullParams("missrappel", "rope_slide", 5, pos, rot, 1, 1, -1, 0)
                this.isActive = true;
                alt.setTimeout(() => {
                    let everyTick = alt.everyTick(() => {
                        let playerPos = alt.Player.local.pos
                        //animation.playAnimAtCoordsFullParams("missrappel", "rope_slide", 5, playerPos, rot, 1, 1, -1, 0)
                        let height = native.getEntityHeightAboveGround(alt.Player.local.scriptID)
                        if(height < 1.2) {
                            native.clearPedTasks(alt.Player.local.scriptID)
                            native.setGravityLevel(0)
                            //alt.removeAnimDict("missrappel")
                            alt.clearEveryTick(everyTick)
                            return;
                        }
                        let zChange = 6 * native.getFrameTime()
                        native.setEntityCoords(alt.Player.local.scriptID, pos.x, pos.y, playerPos.z - 1 - zChange, true, true, true, false)
                    });
                
                }, 200);
                
            }, 200);

        //     const interval = alt.setInterval(() => {
        //         let height = native.getEntityHeightAboveGround(alt.Player.local.scriptID)
        //         alt.log("height: " + height)
        //         if(height < 1) {
        //             native.clearPedTasks(alt.Player.local.scriptID)
        //             native.setGravityLevel(0)
        //             this.isActive = false;
        //             alt.clearInterval(interval)
        //         }
        //         let playerPos = alt.Player.local.pos
        //         let zChange = 2 * native.getFrameTime()
        //         let newZ = playerPos.z - 1 - zChange
        //         alt.log("newZ: " + newZ)
        //         native.setEntityCoords(alt.Player.local.scriptID, playerPos.x, playerPos.y, newZ, true, true, true, false)
        //     }, 200)
        })
    }
}

export default new Rappel()