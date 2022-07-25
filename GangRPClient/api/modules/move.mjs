import alt from "alt"
import native from "natives"
import player from "/api/modules/player.mjs"
import animation from "./animation.mjs";

class Move {
    constructor() {
        this.currentStance = "Standing"
        this.interval = null
        // native.requestAnimSet("move_ped_crouched")
        // native.requestAnimSet("move_m@drunk@verydrunk")
        // native.requestAnimSet("move_m@drunk@moderatedrunk")
        // native.requestAnimSet("move_m@drunk@slightlydrunk")
        
        
        
        //native.requestAnimSet("move_ped_crouched_strafing")
        
        
        //MAYBE ACTIVATE AGAIN
        // native.requestAnimDict("move_crawl")
        // native.requestAnimDict("get_up@directional@transition@prone_to_knees@crawl")
        // native.requestAnimDict("move_jump")
        //
        // native.requestAnimDict("missfinale_c2mcs_1")
        // native.requestAnimDict("nm")

        alt.onServer("Carry", this.carry)
        alt.onServer("Carried", this.carried)
    }

    setStance(stance) {
        switch (stance) {
            case "Standing":
                if (this.interval != null) {
                    alt.clearInterval(this.interval)
                    this.interval = null
                }
                native.resetPedMovementClipset(alt.Player.local.scriptID, 0.25)
                //native.resetPedStrafeClipset(alt.Player.local.scriptID)
                native.clearPedTasks(alt.Player.local.scriptID)
                this.currentStance = "Standing"
                break

            case "Crouching":
                if (this.interval != null) {
                    alt.clearInterval(this.interval)
                    this.interval = null
                }
                //native.setPedMovementClipset(alt.Player.local.scriptID, "move_ped_crouched", 0.25)
                this.setClipset("move_ped_crouched", 0.25);
                //native.setPedStrafeClipset(alt.Player.local.scriptID, "move_ped_crouched_strafing")
                this.currentStance = "Crouching"
                break

            case "Prone":
                this.currentStance = "Jump"
                //native.taskPlayAnim(alt.Player.local.scriptID, "move_jump", "dive_start_run", 8, -4, -1, 0, 0, false, false, false)
                animation.playAnim("move_jump", "dive_start_run", 0);

                alt.setTimeout(() => {
                    //native.clearPedTasks(alt.Player.local.scriptID)
                    //native.taskPlayAnim(alt.Player.local.scriptID, "move_crawl", "onfront_fwd", 8, -4, -1, 2, 1, false, false, false)
                    animation.playAnim("move_crawl", "onfront_fwd", 2)
                    this.interval = alt.setInterval(this.proneMovement.bind(this), 0)
                    this.currentStance = "Prone"
                }, 1000)
                break

            case "Standup":
                //native.taskPlayAnim(alt.Player.local.scriptID, "get_up@directional@transition@prone_to_knees@crawl", "front", 8, -8, -1, 0, 0, false, false, false)
                animation.playAnim("get_up@directional@transition@prone_to_knees@crawl", "front", 0)
                if (this.interval != null) {
                    alt.clearInterval(this.interval)
                    this.interval = null
                }
                this.currentStance = "Standup"
                break
        
            default:
                break
        }
    }

    proneMovement() {
      //if (this.currentStance != "Prone") return

    //   if (native.isControlPressed(0, 34))//left
    //   {
    //     const heading = native.getEntityHeading(alt.Player.local.scriptID) + 1.0
    //     native.setEntityHeading(alt.Player.local.scriptID, heading)
    //     //TODO getEntityRotation
    //   }
    //   else if (native.isControlPressed(0, 35))//right
    //   {
    //     const heading = native.getEntityHeading(alt.Player.local.scriptID) - 1.0
    //     native.setEntityHeading(alt.Player.local.scriptID, heading)
    //     //TODO getEntityRotation
	//   }
	  
      if (native.isControlJustPressed(0, 32))//up
      {
        //native.taskPlayAnim(alt.Player.local.scriptID, "move_crawl", "onfront_fwd", 8, -4, -1, 9, 0, false, false, false) //flag 33
          animation.playAnim("move_crawl", "onfront_fwd", 9)
      }
      else if (native.isControlJustPressed(0, 33))//down
      {
		//native.taskPlayAnim(alt.Player.local.scriptID, "move_crawl", "onfront_bwd", 8, -4, -1, 9, 0, false, false, false) //flag 33
          animation.playAnim("move_crawl", "onfront_bwd", 9)
      }
      else if (native.isControlJustReleased(0, 32) || native.isControlJustReleased(0, 33))
      {
        //native.taskPlayAnim(alt.Player.local.scriptID, "move_crawl", "onfront_fwd", 8, -4, -1, 2, 1, false, false, false) //flag 34
          animation.playAnim("move_crawl", "onfront_fwd", 2)
      }

      if (native.getEntityHeightAboveGround(alt.Player.local.scriptID) > 2) {
          this.setStance("Standing")
      }
    }

    carry(toggel) {
        //if (toggel) native.taskPlayAnim(alt.Player.local.scriptID, "missfinale_c2mcs_1", "fin_c2_mcs_1_camman", 8, -4, -1, 50, 0, false, false, false)
        if (toggel) animation.playAnim("missfinale_c2mcs_1", "fin_c2_mcs_1_camman", 50)
        else native.clearPedTasks(alt.Player.local.scriptID)
        player.disableActions = toggel
    }

    carried(carrier) {
        if (carrier == undefined) {
            native.detachEntity(alt.Player.local.scriptID, true, true)
            native.clearPedTasks(alt.Player.local.scriptID)
        } else {
            //alt.log("attachEntityToEntity " + carrier.scriptID)
            native.attachEntityToEntity(alt.Player.local.scriptID, carrier.scriptID, 0, 0.27, 0.15, 0.63, 0.5, 0.5, 180, true, false, true, false, 2, true)
            //alt.log("taskPlayAnim")
            //native.taskPlayAnim(alt.Player.local.scriptID, "nm", "firemans_carry", 8, -4, -1, 1, 0, false, false, false)
            animation.playAnim("nm", "firemans_carry", 1)
            //alt.log("finish")
        }
    }

    loadAnimSet(animSet) {
        return new Promise((resolve, reject) => {
            if (!native.hasClipSetLoaded(animSet)) {
                native.requestClipSet(animSet)
                let tryAmount = 0;
                const interval = alt.setInterval(() => {
                    if (native.hasClipSetLoaded(animSet)) {
                        alt.clearInterval(interval)
                        return resolve(true)
                    }
                    tryAmount++;
                    if (tryAmount > 100) {
                        alt.clearInterval(interval)
                        return reject(false)
                    }
                }, 50)
            }
            else return resolve(true)
        })
    }

    setClipset(animSet, value) {
        this.loadAnimSet(animSet).then(val => {
            alt.log("USE MOVEMENT CLIPSET " + animSet + " VALUE " + value)
            native.setPedMovementClipset(alt.Player.local.scriptID, animSet, value)
            //native.removeAnimSet(animSet)
        })
    }

    async goToCoord(pos, speed, timeout, targetHeading, distanceToSlide) {
        native.taskGoStraightToCoord(alt.Player.local.scriptID, parseFloat(pos.x), parseFloat(pos.y), parseFloat(pos.z), parseFloat(speed), parseInt(timeout), parseFloat(targetHeading), parseFloat(distanceToSlide));
        return new Promise((resolve, reject) => {
            let tryAmount = 0;
            let playerCoord = alt.Player.local.pos;
            const interval = alt.setInterval(() => {
                if (this.getDistance(playerCoord, pos) < 0.2) {
                    alt.clearInterval(interval);
                    return resolve(true);
                }
                tryAmount++;
                if (tryAmount > 100) {
                    alt.clearInterval(interval)
                    return reject(true)
                }
            }, 50)
        })
    }

    getDistance(vector1, vector2)
    {
        return Math.sqrt((vector1.x - vector2.x) * (vector1.x - vector2.x) + (vector1.y - vector2.y) * (vector1.y - vector2.y))
    }
}

export default new Move()