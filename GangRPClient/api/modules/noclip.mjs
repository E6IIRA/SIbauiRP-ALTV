import * as alt from "alt-client";
import * as native from "natives";
import player from "/api/modules/player.mjs";

class NoClip {
    constructor() {
        this.enabled = false;
        this.speed = 1.0;
        this.everyTick = null;
        this.camera = null
        this.KEYS = {
            FORWARD: 32,
            BACKWARD: 33,
            LEFT: 34,
            RIGHT: 35,
            UP: 22,
            DOWN: 36,
            SHIFT: 21,
            ARROW_UP: 172,
            ARROW_DOWN: 173,
            ARROW_LEFT: 174,
            ARROW_RIGHT: 175,
            R: 45,
            E: 38,
            H: 101,
            Q: 44,
            CAPSLOCK: 171,
            SCROLLWHEEL_UP: 261,
            SCROLLWHEEL_DOWN: 262
        };

        alt.onServer("UpdatePosition", (pos) => {
            native.setEntityCoords(alt.Player.local.scriptID, pos.x, pos.y, pos.z, false, false, false, false)
            native.setCamCoord(this.camera, pos.x, pos.y, pos.z)
        });

    }

    start() {
        if (this.enabled) return;
        this.enabled = true;
        player.isInvincible = true;
        const pos = alt.Player.local.pos
        const rot = alt.Player.local.rot //cam 0 3 0.5 40
        const test = native.getOffsetFromEntityInWorldCoords(alt.Player.local.scriptID, 0, 3, 0.5)
        this.camera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", parseFloat(test.x), parseFloat(test.y), parseFloat(test.z), parseFloat(rot.x), parseFloat(rot.y), parseFloat(rot.z), 40, true, 0)
        //native.pointCamAtCoord(this.camera, pos.x, pos.y, pos.z)
        native.renderScriptCams(true, false, 0, true, false, 0)
        native.freezeEntityPosition(alt.Player.local.scriptID, true);
        native.setEntityCollision(alt.Player.local.scriptID, false, false)
        player.setInvincible(true)
        native.setEntityVisible(alt.Player.local.scriptID, false, false)     
        native.setEntityCoords(alt.Player.local.scriptID, parseFloat(pos.x), parseFloat(pos.y), parseFloat(pos.z), false, false, false, false)
        this.everyTick = alt.everyTick(this.keyHandler.bind(this));
    }
    stop() {
        if (!this.enabled) return;
        this.enabled = false;
        if(!player.isInAdminDuty)
        {
            player.setInvincible(false)
        }     
        let pos = alt.Player.local.pos  
        native.setEntityCoords(alt.Player.local.scriptID, parseFloat(pos.x), parseFloat(pos.y), parseFloat(pos.z), false, false, false, false)
        native.setCamActive(this.camera, false)
        native.destroyCam(this.camera, false)
        native.renderScriptCams(false, false, 0, true, false, 0)
        native.freezeEntityPosition(alt.Player.local.scriptID, false);
        native.setEntityCollision(alt.Player.local.scriptID, true, true)
        native.setEntityVisible(alt.Player.local.scriptID, true, false)
        alt.clearEveryTick(this.everyTick);

    }

    addSpeedToVector(vector1, vector2, speed, lr = false) {
        return new alt.Vector3(
            vector1.x + vector2.x * speed,
            vector1.y + vector2.y * speed,
            lr === true ? vector1.z : vector1.z + vector2.z * speed
        );
    }

    camVectorForward(camRot) {
        let rotInRad = {
            x: camRot.x * (Math.PI / 180),
            y: camRot.y * (Math.PI / 180),
            z: camRot.z * (Math.PI / 180) + Math.PI / 2,
        };

        let camDir = {
            x: Math.cos(rotInRad.z),
            y: Math.sin(rotInRad.z),
            z: Math.sin(rotInRad.x),
        };

        return camDir;
    }

    camVectorRight(camRot) {
        let rotInRad = {
            x: camRot.x * (Math.PI / 180),
            y: camRot.y * (Math.PI / 180),
            z: camRot.z * (Math.PI / 180),
        };

        var camDir = {
            x: Math.cos(rotInRad.z),
            y: Math.sin(rotInRad.z),
            z: Math.sin(rotInRad.x),
        };

        return camDir;
    }

    isVectorEqual(vector1, vector2) {
        return (
            vector1.x === vector2.x &&
            vector1.y === vector2.y &&
            vector1.z === vector2.z
        );
    }
    keyHandler() {
        native.blockWeaponWheelThisFrame()
        native.clearPedTasksImmediately(alt.Player.local.scriptID)

        let currentPos = native.getCamCoord(this.camera)
        let speed = this.speed;
        //let rot = native.getCamRot(this.camera, 2)
        let rot = native.getGameplayCamRot(2)
        let dirForward = this.camVectorForward(rot);
        let dirRight = this.camVectorRight(rot);
        let changed = false;
        if (native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_UP))
        {
            this.speed *= 1.2
            native.beginTextCommandDisplayHelp("STRING");
            native.addTextComponentSubstringPlayerName("Speed : " + this.speed);
            native.endTextCommandDisplayHelp(0, false, false, 0);
        }
        if (native.isDisabledControlPressed(0, this.KEYS.SCROLLWHEEL_DOWN))
        {
            if(this.speed > 1)
            {
                this.speed /= 1.2
            }
            native.beginTextCommandDisplayHelp("STRING");
            native.addTextComponentSubstringPlayerName("Speed : " + this.speed);
            native.endTextCommandDisplayHelp(0, false, false, 0);
        }
        if (native.isDisabledControlPressed(0, this.KEYS.SHIFT))
        {
            speed = speed * 5;
        }
        else if (native.isDisabledControlPressed(0, this.KEYS.CAPSLOCK))
            speed = speed / 50;

        if (native.isDisabledControlPressed(0, this.KEYS.FORWARD))
        {
            currentPos = this.addSpeedToVector(currentPos, dirForward, speed);
            changed = true
        }
        if (native.isDisabledControlPressed(0, this.KEYS.BACKWARD))
        {
            currentPos = this.addSpeedToVector(currentPos, dirForward, -speed);
            changed = true
        }
        if (native.isDisabledControlPressed(0, this.KEYS.LEFT))
        {
            currentPos = this.addSpeedToVector(currentPos, dirRight, -speed, true);
            changed = true
        }
        if (native.isDisabledControlPressed(0, this.KEYS.RIGHT))
        {
            currentPos = this.addSpeedToVector(currentPos, dirRight, speed, true);
            changed = true
        }
        let zModifier = 0;
        if (native.isDisabledControlPressed(0, this.KEYS.UP))
        {
            changed = true
            zModifier += speed;
        }
        if (native.isDisabledControlPressed(0, this.KEYS.DOWN))
        {
            changed = true
            zModifier -= speed;
        }
        if (native.isDisabledControlPressed(0, this.KEYS.Q))
        {
            let newFov = native.getCamFov(this.camera) + 0.1 * speed;
            if(newFov >= 130.0)
                newFov = 130
            native.setCamFov(this.camera, newFov)
        }
        if (native.isDisabledControlPressed(0, this.KEYS.E))
        {
            let newFov = native.getCamFov(this.camera) - 0.1 * speed;
            if(newFov <= 1)
                newFov = 1
            native.setCamFov(this.camera, newFov)
        }
        currentPos = new alt.Vector3(currentPos.x, currentPos.y, currentPos.z + zModifier)
        if(changed == true)
        {
            native.setCamCoord(this.camera, parseFloat(currentPos.x), parseFloat(currentPos.y), parseFloat(currentPos.z))
            native.setEntityCoords(alt.Player.local.scriptID, parseFloat(currentPos.x), parseFloat(currentPos.y), parseFloat(currentPos.z), false, false, false, false)
        }
        native.setCamRot(this.camera, rot.x, rot.y, rot.z, 2)
        //native.setEntityRotation(alt.Player.local.scriptID, rot.x, rot.y, rot.z, 2, true)
        // native.beginTextCommandDisplayHelp("STRING");
        // native.addTextComponentSubstringPlayerName("W/A/S/D - Bewegen, CTRL/LEER - Runter/Hoch, SHIFT/CAPS - Schnell/Langsam, Q/E - FOV, H - Log");
        // native.endTextCommandDisplayHelp(0, false, false, 0);

        if (native.isDisabledControlJustPressed(0, this.KEYS.H))
        {
            alt.log("CamCoord:" + JSON.stringify(native.getCamCoord(this.camera)))
            alt.log("CamRot: " + JSON.stringify(native.getCamRot(this.camera, 2)))
            alt.log("FoV: " + JSON.stringify(native.getCamFov(this.camera)))
        }
    }
}

export default new NoClip()