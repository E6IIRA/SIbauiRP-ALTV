import * as alt from "alt-client";
import * as native from "natives";
import webview from "/api/modules/webview.mjs"

class Speedlimiter {
    constructor() {
        this.enabled = false;
        this.speed = 1.0;
        this.vehicle = null;
    }

    toggle() {
        if(this.enabled)
        {
            this.stop()
        }
        else
        {
            this.start()
        }
    }

    increaseSpeed() {
        if(alt.Player.local.vehicle != null)
        {
            let estimatedMaxSpeed = native.getVehicleEstimatedMaxSpeed(alt.Player.local.vehicle.scriptID)
            if(this.speed >= estimatedMaxSpeed)
            {
                this.speed = estimatedMaxSpeed;
            }
            else
            {
                this.speed += 1 / 3.6;
            }
            native.setVehicleMaxSpeed(alt.Player.local.vehicle.scriptID, this.speed)
        }
        else
        {
            this.stop()
        }
    }

    decreaseSpeed() {
        if(this.speed * 3.6 > 5)
        {
            this.speed -= 1 / 3.6;
            native.setVehicleMaxSpeed(alt.Player.local.vehicle.scriptID, this.speed)
        }

    }

    init() {
        this.enabled = false
        this.speed = 1
        this.vehicle = null
    }

    start() {
        if (this.enabled || alt.Player.local.vehicle == null) return;
        this.speed = Math.floor(native.getEntitySpeed(alt.Player.local.vehicle.scriptID))
        this.vehicle = alt.Player.local.vehicle

        if(this.speed * 3.6 < 5)
        {
            webview.updateView("AddNotify", ["Geschwindigkeit zu niedrig für Speedlimiter (< 5 km/h)", "#00a6cc"])
            return;
        }

        this.enabled = true;
        native.setVehicleMaxSpeed(alt.Player.local.vehicle.scriptID, this.speed)
        webview.updateView("AddNotify", ["Speedlimiter bei " + (this.speed * 3.6).toFixed(0) + " km/h aktiviert! Mit + und - kannst du das Tempo anpassen.", "#00a6cc"])
        //this.everyTick = alt.everyTick(this.keyHandler.bind(this));
    }
    stop() {
        if (!this.enabled) return;
        this.enabled = false;
        if(alt.Player.local.vehicle == null) return;
        if(alt.Player.local.vehicle == this.vehicle)
        {
            native.setVehicleMaxSpeed(alt.Player.local.vehicle.scriptID, -1)
            webview.updateView("AddNotify", ["Speedlimiter deaktiviert!", "#00a6cc"])
        }
        else
        {
            this.start()
        }
    }
}

export default new Speedlimiter()