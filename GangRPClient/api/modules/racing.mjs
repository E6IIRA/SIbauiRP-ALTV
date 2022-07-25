import alt from "alt";
import native from "natives";
import webview from "/api/modules/webview.mjs"

class Racing {
    constructor() {
        this.enabled = false;
        // this.speed = 1.0;
        this.vehicle = null;
        this.checkpointData = [];
        this.interval = null;
        this.everyTick = null;
        this.startTime = 0
        this.racingId = 0;
        this.checkpointId = undefined
        this.blip = null;
        this.lastTime = undefined;
        this.cancelCounter = 0;

        alt.onServer("StartRacing", (vehicle, data) => {
            this.cancelCounter = 0;
            this.vehicle = vehicle
            this.enabled = true
            this.checkpointData = [];
            if (this.checkpointId != undefined)native.deleteCheckpoint(this.checkpointId);
            let cleared = false
            let vehicleScriptId = undefined
            this.everyTick = alt.everyTick(this.drawNames.bind(this));
            native.setCurrentPedWeapon(alt.Player.local.scriptID, 0xA2719263, true);
            native.setCanPedEquipAllWeapons(alt.Player.local.scriptID, false);
            const tempInterval = alt.setInterval(() => {
                vehicleScriptId = vehicle.scriptID
                if (vehicleScriptId) {
                    native.setPedIntoVehicle(alt.Player.local.scriptID, vehicleScriptId, -1)
                    native.setVehicleEngineOn(vehicleScriptId, false, true, true)
                    alt.clearInterval(tempInterval)
                    if(native.getEntityModel(vehicleScriptId) == 4008920556)
                    {
                        native.freezeEntityPosition(vehicleScriptId, true)
                    }
                    cleared = true
                }
            }, 10)
        
            alt.setTimeout(() => {
                if (!cleared) {
                    alt.clearInterval(tempInterval)
                    alt.clearEveryTick(this.everyTick)
                }
            }, 5000)

            webview.updateView("CloseIF")
            webview.closeInterface();
            webview.updateView("StartRaceHud", [data.d.length]);
            this.startTime = Date.now();
            this.lastTime = this.startTime;
            this.racingId = data.i;
            let i = 9;
            const interval = alt.setInterval(() => {
                if (i == 0) {
                    native.setVehicleEngineOn(vehicleScriptId, true, true, false)
                    if(native.getEntityModel(vehicleScriptId) == 4008920556)
                    {
                        native.freezeEntityPosition(vehicleScriptId, false)
                    }
                    alt.clearInterval(interval)
                }
                i--;
            }, 1000)

            for (const obj of data.d) {
                var temp = {};
                temp["pos"] = new alt.Vector3(obj.x, obj.y, obj.z);
                temp["range"] = obj.r;
                temp["order"] = obj.o;
                this.checkpointData.push(temp);
            }

            this.blip = new alt.PointBlip(this.checkpointData[0].pos.x, this.checkpointData[0].pos.y,this.checkpointData[0].pos.z)
            this.blip.shortRange = false
            this.blip.sprite = 38
            this.blip.color = 57
            this.blip.route = true
            this.checkpointId = native.createCheckpoint(1, this.checkpointData[0].pos.x, this.checkpointData[0].pos.y, this.checkpointData[0].pos.z-5, this.checkpointData[1].pos.x, this.checkpointData[1].pos.y, this.checkpointData[1].pos.z, this.checkpointData[0].range, 57, 192, 216, 150, 0);
            this.interval = alt.setInterval(this.checkForNextCheckpoint.bind(this), 250);
        })

        alt.onServer("StopRacing", () => {
            this.stop();
        })
    }

    distance(vector1, vector2) {
        return Math.sqrt(
          Math.pow(vector1.x - vector2.x, 2) +
            Math.pow(vector1.y - vector2.y, 2) +
            Math.pow(vector1.z - vector2.z, 2)
        );
      }

    drawNames() {
        for (const player of alt.Player.streamedIn) {
          if (player == alt.Player.local || player.scriptID == 0) continue;
          if (this.distance(player.pos, alt.Player.local.pos) > 50) continue;
          if (!native.isEntityOnScreen(player.scriptID)) continue;
    
          const playerPos = player.pos;
          const entity = player.vehicle ? player.vehicle.scriptID : player.scriptID;
          const vector = native.getEntityVelocity(entity);
          const frameTime = native.getFrameTime();
          native.setDrawOrigin(
            playerPos.x + vector.x * frameTime,
            playerPos.y + vector.y * frameTime,
            playerPos.z + vector.z * frameTime + 1.1,
            0
          );
          native.beginTextCommandDisplayText("STRING");
          native.setTextFont(4);
          native.setTextCentre(true);
          native.setTextOutline();
    
          const fontSize =
            (1 - (0.8 * this.distance(player.pos, alt.Player.local.pos)) / 100) *
            0.4;
          native.setTextScale(fontSize, fontSize);
          native.setTextProportional(true);
          //native.setTextColour(255, 255, 255, 255)
          native.addTextComponentSubstringPlayerName(
            `~w~${
              player.name
            }`
          );
          native.endTextCommandDisplayText(0, 0, 0);
          native.clearDrawOrigin();
        }
      }

    stop() {
        this.enabled = false
        this.vehicle = null
        if (this.checkpointId != undefined) native.deleteCheckpoint(this.checkpointId);
        if (this.checkpointData.length != 0) {
            if (this.blip.valid) this.blip.destroy();
        }
        this.checkpointData = [];
        native.setCanPedEquipAllWeapons(alt.Player.local.scriptID, true);
        alt.clearEveryTick(this.everyTick)
        alt.clearInterval(this.interval)
    }

    checkForNextCheckpoint() {
        let now = Date.now()
        if(this.lastTime > now)
        {
            this.stop();
            //alt.log("TIME ERROR RACING")
            alt.emitServer("TimeErrorRacing", this.racingId, this.lastTime, now)
        }
        else
        {
            this.lastTime = now
        }
        
        if (alt.Player.local.vehicle != null && alt.Player.local.vehicle.scriptID == this.vehicle.scriptID) {
            this.cancelCounter = 0;
            let distance = alt.Player.local.pos.distanceTo(this.checkpointData[0].pos);
            if(distance < this.checkpointData[0].range)
            {
                if (this.checkpointId != undefined) native.deleteCheckpoint(this.checkpointId);
                this.checkpointData.shift();
                let amount = this.checkpointData.length;
                webview.updateView("UpdateRaceHud", []);
                this.blip.destroy();
                if(amount == 0)
                {
                    this.blip = 0;
                    this.stop();
                    let time = now - this.startTime;
                    alt.emitServer("FinishedRacing", this.racingId, time);
                } else {
                    this.blip = new alt.PointBlip(this.checkpointData[0].pos.x, this.checkpointData[0].pos.y,this.checkpointData[0].pos.z)
                    this.blip.shortRange = false
                    this.blip.sprite = 38
                    this.blip.color = 57
                    this.blip.route = true
    
                    if (amount == 1) 
                    {
                        this.checkpointId = native.createCheckpoint(4, this.checkpointData[0].pos.x, this.checkpointData[0].pos.y, this.checkpointData[0].pos.z-5, 0, 0, 0, this.checkpointData[0].range, 255, 0, 0, 150, 0);
                    } else {
                        this.checkpointId = native.createCheckpoint(1, this.checkpointData[0].pos.x, this.checkpointData[0].pos.y, this.checkpointData[0].pos.z-5, this.checkpointData[1].pos.x, this.checkpointData[1].pos.y, this.checkpointData[1].pos.z-10, this.checkpointData[1].range, 57, 192, 216, 150, 0);
                    }
                }
            }
        } else {
            this.cancelCounter++;
            if (this.cancelCounter >= 4 * 20) {
                if (this.checkpointId != undefined )native.deleteCheckpoint(this.checkpointId);
                if (this.checkpointData.length != 0) {
                    if (this.blip.valid) this.blip.destroy();
                }
                this.stop();
                alt.emitServer("CancelRacing", this.racingId)
            } else {
                if (this.cancelCounter % 4 == 0) {
                    webview.updateView("AddNotify", ["Kehre zurück zu deinem Fahrzeug! " + (20 - this.cancelCounter/4).toFixed(0)  + " Sekunden verbleibend", "#c72020"])
                }
            }
        }
    }

}

export default new Racing()