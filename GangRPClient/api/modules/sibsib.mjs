import alt from "alt-client";
import native from "natives";
import player from "/api/modules/player.mjs"
import noclip from "api/modules/noclip.mjs"
import webview from "/api/modules/webview.mjs"
import interfaces from "/api/modules/interfaces.mjs"
import vehicle from "/api/modules/vehicle.mjs"
import rappel from "/api/modules/rappel.mjs";
import prophunt from "./prophunt.mjs";
import racing from "./racing.mjs";
import paintball from "./paintball.mjs";
import cam from "./cam.mjs"

class Sibsib {
    constructor() {
        this.timeBetweenClicks = []
        this.lastClick = null
        this.isPressed = false
        this.allowedHealth = null
        this.healthAlarmMsg = []
        this.allowedArmor = null
        this.armorAlarmMsg = []
        this.allowedModel = null
        this.modelAlarmMsg = []
        this.lastChange = false
        this.hpAndArmorBlockCounter = 0
        this.isSuspect = 0
        this.interval1ms = null
        this.interval5000ms = null
        this.interval60000ms = null
        this.interval250ms = null
        this.threshold = 20
        this.oldCarHealth = null
        this.blockFlyCheck = 0
        this.aimTime = 0
        this.lastAimCheck = 0
        this.lastPosition = null
        this.lastPositionCounter = 0
        this.entityAimingAt = null
        this.aimTimes = []
        this.hittedShots = 0
        this.missedShots = 0
        this.hittedBones = []
        this.oldPosition = null
        this.oldSpeed = null
        this.insertCounter = 0
        this.endCounter = 0
        this.weaponInformation = []
        this.vehFlyCounter = 0
        this.minMinutes = 25
        this.maxMinutes = 35
        this.nextAfkCounter = this.minMinutes
        this.vehicleInfoBlockCounter = 0
        this.pos = []

        this.vehicleData = JSON.parse(alt.File.read("@SibauiRP_Assets/data/vehicleData.json"))

        // this.allowedResources = JSON.parse(alt.File.read("@SibauiRP_Assets/data/info.json"))
        
        // alt.on("anyResourceStart", name => {
        //     if(!this.allowedResources.some(x => x === name)) return alt.emitServer("SibsibResource", name);

        //     alt.log("Ressource: " + name + " started but was found in allowedResources!")
        // });
        
        /*let weapons = JSON.parse(alt.File.read("@SibauiRP_Assets/data/weapons.json"))

        for (const weapon of weapons) {
            alt.log(alt.hash(weapon.name), weapon.damage)
            this.weaponInformation[alt.hash(weapon.name)] = weapon.damage
        }*/
    

        //native.setPlayerWeaponDefenseModifier?
        //native.setPedCanSwitchWeapon auf false setzen, wenn Cheater in der Nähe und
        //native.setCurrentPedWeapon auf Nahkampf / Faust setzen
        //native.getWeaponDamage --> dmg checken

        alt.onServer("AfkFuchs", () => {
            this.nextAfkCounter = 0;
            this.lastPosition = alt.Player.local.pos;
        });

        alt.onServer("SetModel", model => {
            native.requestModel(model)
            alt.setTimeout(() => {
                this.allowedModel = model
                native.setPlayerModel(alt.Player.local.scriptID, model)
            }, 1000);
        })

        alt.onServer("SetHP", hp => {
            this.setHP(hp)
        })

        alt.onServer("SetArmor", armor => {
            this.setArmor(armor)
        })

        alt.onServer("SetHPandArmor", (hp, armor) => {
            this.setHP(hp)
            this.setArmor(armor)
        })

        alt.onServer("DisableFly", () => {
            this.blockFlyCheck = 180
        })

        alt.onServer("GetSibSibSpeed", () => {
            alt.emitServer("SendSibSibSpeedi", this.timeBetweenClicks)
            this.timeBetweenClicks = []
        })

        alt.onServer("GetScreenView", () => {
            alt.takeScreenshot()
        })

        alt.onServer("DisableVehInfo", () => {
            this.vehicleInfoBlockCounter = 15
        })

        alt.onServer("RqPositionLog", (id) => {
            alt.emitServer("RsPositionLog", id, this.pos)
        })
    }

    randomIntFromInterval(min, max) { // min and max included 
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    setHP(hp) {
        this.lastChange = true;
        this.allowedHealth = hp;
        if (hp > 200) native.setPedMaxHealth(alt.Player.local.scriptID, hp);
        else native.setPedMaxHealth(alt.Player.local.scriptID, 200);
        native.setEntityHealth(alt.Player.local.scriptID, hp, 0);
        
        player.applyDamageEffect();
    }

    setDamage(damage, weaponHash) {
        if(!player.isInjured)
        {
            let actualHealth = native.getEntityHealth(alt.Player.local.scriptID);
            let newHealth = actualHealth - damage;
            if(newHealth > 100)
            {
                native.setEntityHealth(alt.Player.local.scriptID, newHealth, 0);
            }
            else
            {
                alt.emitServer("KillPlayer", weaponHash)
            }
        }
    }

    setArmor(armor) {
        this.lastChange = true;
        this.allowedArmor = armor;
        if (armor > 100) native.setPlayerMaxArmour(alt.Player.local, armor);
        else native.setPlayerMaxArmour(alt.Player.local, 100);
        native.setPedArmour(alt.Player.local.scriptID, armor);
    }

    start(data) {
        this.allowedHealth = native.getEntityHealth(alt.Player.local.scriptID);
        this.allowedArmor = native.getPedArmour(alt.Player.local.scriptID);
        this.allowedModel = native.getEntityModel(alt.Player.local.scriptID);
        this.oldPosition = native.getEntityCoords(alt.Player.local.scriptID, true);
        this.oldSpeed = native.getEntitySpeed(alt.Player.local.scriptID);
        this.nextAfkCounter = this.randomIntFromInterval(this.minMinutes, this.maxMinutes);
        this.lastPosition = alt.Player.local.pos;
        this.oldPosition = null;
        this.interval1ms = alt.setInterval(() => {
            if(!player.isInjured)
            {
                this.checkSpeedi();
                this.checkShots();
            }
        }, 1)
        this.interval1000ms = alt.setInterval(() => {
            if(this.lastChange == false && this.hpAndArmorBlockCounter <= 0)
            {
                this.checkHealth();
                this.checkArmor();
            }
            else
            {
                this.lastChange = false;
                this.hpAndArmorBlockCounter--;
            }
        }, 1000)
        this.interval5000ms = alt.setInterval(() => {
            this.checkForMessage();
            this.checkForFly();
            this.checkForTeleport();
            //this.checkForCam();
        }, 5000)
        this.interval60000ms = alt.setInterval(() => {
            this.checkForAimMessage();
            this.checkForShotsMessage();
            this.checkForObjectModel();
            this.checkForVehicleData();
            if(!player.isInjured)
            {
                this.checkForPosition();
            }
            this.logPos();
        }, 60000)
        this.threshold = data;
        this.interval250ms = alt.setInterval(() => {
            player.applyDamageEffect();
        }, 250)
    }

    stop () {
        this.checkForAimMessage()
        this.checkForShotsMessage()
        alt.clearInterval(this.interval1ms)
        alt.clearInterval(this.interval1000ms)
        alt.clearInterval(this.interval5000ms)
        alt.clearInterval(this.interval60000ms)
    }

    insertAlert() {
        this.insertCounter++
    }

    endAlert() {
        this.endCounter++
    }

    AltF4Alert() {
        alt.emitServer("SibsibAltF4")
    }

    getDistance(vector1, vector2)
    {
        return Math.sqrt((vector1.x - vector2.x) * (vector1.x - vector2.x) + (vector1.y - vector2.y) * (vector1.y - vector2.y))
    }

    /*checkDamage(weapon) {
        //const weapon = native.getSelectedPedWeapon(alt.Player.local.scriptID)
        alt.log("WeaponHash: " + weapon)
        let damage = native.getWeaponDamage(weapon, null)
        alt.log("Client Damage: " + damage)
        alt.log("Server Damage: " + this.weaponInformation[weapon])
    }*/

    logPos() {
        this.pos.push(alt.Player.local.pos)
        if(this.pos.length > 120)
        {
            this.pos.shift()
        }
    }

    checkForVehicleData() {
        if(this.vehicleInfoBlockCounter > 0)
        {
            this.vehicleInfoBlockCounter--;
            return;
        }
        if(alt.Player.local.vehicle != null && alt.Player.local.scriptID == native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false))
        {
            let vehInfo = this.vehicleData.find(veh => veh.hash == alt.Player.local.vehicle.model || alt.hash(veh.hash) == alt.Player.local.vehicle.model)
            if(vehInfo != null)
            {
                if (vehInfo.maxAcceleration == 0 || vehInfo.maxSpeed == 0) return;
                let maxSpeed = native.getVehicleEstimatedMaxSpeed(alt.Player.local.vehicle.scriptID)
                let maxTraction = native.getVehicleMaxTraction(alt.Player.local.vehicle.scriptID)
                let acceleration = native.getVehicleAcceleration(alt.Player.local.vehicle.scriptID)
                let maxBreaking = native.getVehicleMaxBraking(alt.Player.local.vehicle.scriptID)
                let agility = native.getVehicleModelEstimatedAgility(alt.Player.local.vehicle.scriptID)
                if(maxSpeed - vehInfo.maxSpeed > 0.1)
                {
                    alt.emitServer("SibsibVehicleData", vehInfo.id, 1, maxSpeed, vehInfo.maxSpeed)
                }
                else if(maxTraction - vehInfo.maxTraction > 0.1)
                {
                    alt.emitServer("SibsibVehicleData", vehInfo.id, 2, maxTraction, vehInfo.maxTraction)
                }
                else if(acceleration - vehInfo.maxAcceleration > 0.1)
                {
                    alt.emitServer("SibsibVehicleData", vehInfo.id, 3, acceleration, vehInfo.maxAcceleration)
                }
                else if(maxBreaking - vehInfo.maxBreaking > 0.1)
                {
                    alt.emitServer("SibsibVehicleData", vehInfo.id, 4, maxBreaking, vehInfo.maxBreaking)
                }
                else if(agility - vehInfo.agility > 0.1)
                {
                    alt.emitServer("SibsibVehicleData", vehInfo.id, 5, agility, vehInfo.agility)
                }
            }
        }
    }

    checkForVehSpeed() {
        if(alt.Player.local.vehicle != null)
        {
            let maxSpeed = native.getVehicleEstimatedMaxSpeed(alt.Player.local.vehicle.scriptID)
            let actualSpeed = native.getEntitySpeed(alt.Player.local.scriptID)
            let lookUpSpeed = 10000;
            //alt.log("maxSpeed: " + maxSpeed + ", actualSpeed: " + actualSpeed)
            if(maxSpeed > lookUpSpeed || actualSpeed > 1.2 * maxSpeed)
            {
                alt.emitServer("SibsibVehSpeed", (actualSpeed / maxSpeed))
            }
        }
    }

    checkForObjectModel() {
        let modelHash = "h4_prop_h4_chest_01a";
        let [result, minVector, maxVector] = native.getModelDimensions(alt.hash(modelHash), null, null)
        if(result != null)
        {
            let dimension = Math.sqrt(
                (maxVector.x - minVector.x) * (maxVector.x - minVector.x) +
                  (maxVector.y - minVector.y) * (maxVector.y - minVector.y) +
                  (maxVector.z - minVector.z) * (maxVector.z - minVector.z)
              );
            if(dimension > 0.621)
            {
                alt.emitServer("SibsibObjectModel", modelHash, dimension)
            }
        }
    }

    checkForPosition() {
        let pos = alt.Player.local.pos
        let distance = this.getDistance(pos, this.lastPosition)
        if(distance < 3)
        {
            this.lastPositionCounter += 1;
            if(this.lastPositionCounter > this.nextAfkCounter)
            // if(this.lastPositionCounter >= 1) //Zum Testen jede Minute, sonst obiges (alle 30 Minuten)
            {
                
                if(player.adutyInterval != null) return;
                let playerNearby = 0
                for (const player of alt.Player.streamedIn) {
                    if (this.getDistance(player.pos, pos) < 5) {
                        playerNearby++
                    }
                }
                if(playerNearby < 5 && !interfaces.isOpen("Tuning"))
                {
                    if(interfaces.isOpen("Fishing"))
                    {
                        alt.emitServer("StopFishing")
                    }
                    else if (interfaces.isOpen("GoldFarming"))
                    {
                        alt.emitServer("StopGoldFarming")
                    }
                    else if (interfaces.isOpen("SlotMachine"))
                    {
                        webview.updateView("StopSlotMachine", []);
                    }

                    webview.closeInterface()
                    alt.setTimeout(() => {
                        webview.showInterface(["Afk"])
                    }, 500);
                    this.lastPositionCounter = 0
                    this.nextAfkCounter = this.randomIntFromInterval(this.minMinutes, this.maxMinutes)
                }
            }
        }
        else
        {
            this.lastPositionCounter = 0
        }
        this.lastPosition = pos
    }

    checkForCam() {
        if (noclip.enabled || cam.enabled) return;
        const interior = native.getInteriorFromEntity(alt.Player.local.scriptID)
        if(interior != 0)
        {
            return;
        }

        const camPos = native.getFinalRenderedCamCoord();
        const gameplayCamPos = native.getGameplayCamCoord();
        const pos = alt.Player.local.pos;
        const distanceBetweenPlayerAndCam = this.getDistance(camPos, pos)
        const distanceBetweenActiveCamAndGameplayCam = this.getDistance(camPos, gameplayCamPos)
        if (distanceBetweenActiveCamAndGameplayCam > 6 || (alt.Player.local.vehicle == null && distanceBetweenPlayerAndCam > 10) || (alt.Player.local.vehicle != null && distanceBetweenPlayerAndCam > 100)) 
        {
            if(distanceBetweenPlayerAndCam < 2) return;
            alt.emitServer("SibsibCam", distanceBetweenPlayerAndCam, distanceBetweenActiveCamAndGameplayCam);
        }
    }

    checkForTeleport() {
        const interior = native.getInteriorFromEntity(alt.Player.local.scriptID)
        if(interior != 0)
        {
            this.oldPosition = null;
            return;
        }
        const newPos = native.getEntityCoords(alt.Player.local.scriptID, true)
        let newSpeed = native.getEntitySpeed(alt.Player.local.scriptID)
        if(alt.Player.local.vehicle != null)
        {
            newSpeed = native.getEntitySpeed(alt.Player.local.vehicle)
        }
        let maxSpeed = newSpeed > this.oldSpeed ? newSpeed : this.oldSpeed
        if(this.oldPosition != null)
        {
            const now = Date.now();
            const distance = Math.sqrt((newPos.x - this.oldPosition.x) * (newPos.x - this.oldPosition.x) + (newPos.y - this.oldPosition.y) * (newPos.y - this.oldPosition.y))
            //alt.log("distance: " + distance + ", limit: " + (maxSpeed * 20))
            //alt.log("LastCommand: " + ((now - player.lastCommand) / 1000) + " seconds ago")
            //alt.log("LastInteraction: " + ((now - player.lastInteraction) / 1000) + " seconds ago")
            if(distance > maxSpeed * 20 && distance > 150 && !noclip.enabled && !player.isSpectating &&
                !prophunt.enabled && !racing.enabled && !paintball.active &&
                now - player.lastCommand > 6000 && now - player.lastInteraction > 6000 && native.getPedParachuteState(alt.Player.local.scriptID) < 1)
            {
                //alt.log("Alarm")
                alt.emitServer("SibsibTp", this.oldPosition, newPos)
                //native.setEntityCoords(alt.Player.local.scriptID, this.oldPosition.x, this.oldPosition.y, this.oldPosition.z, false, false, false, false)
            }
        }
        this.oldSpeed = newSpeed
        this.oldPosition = newPos
    }

    checkShots() {
        if(native.isPedShooting(alt.Player.local.scriptID))
        {
            let [found, entity] = native.getEntityPlayerIsFreeAimingAt(alt.Player.local.scriptID, null)
            if(native.isEntityAPed(entity) && (native.getEntityModel(entity) == -1667301416 || native.getEntityModel(entity) == 1885233650))
            {
                this.hittedShots++;
                let [damaged, bone] = native.getPedLastDamageBone(entity, null)
                if(damaged == true)
                {
                    this.hittedBones.push(bone)
                }
            }
            else
            {
                this.missedShots++;
            }
        }
    }

    checkForModel() {
        let model = native.getEntityModel(alt.Player.local.scriptID)
        if(model != this.allowedModel)
        {
            this.modelAlarmMsg.push(this.allowedModel.toString + " -> " + this.model.toString())
            native.setPlayerModel(alt.Player.local.scriptID, this.allowedModel)
        }
    }

    checkForFly() {
        if(this.blockFlyCheck <= 0)
        {
            if (alt.Player.local.vehicle == null)
            {
                this.vehFlyCounter = 0
                if(noclip.enabled || player.isSpectating || player.isInjured || rappel.isActive)
                {
                    return;
                }

                let entityHeight = native.getEntityHeightAboveGround(alt.Player.local.scriptID)
                if(entityHeight < 5 && entityHeight > -3)
                    return;

                if(native.isEntityInAir(alt.Player.local.scriptID))
                    return;

                if(native.isPedClimbing(alt.Player.local.scriptID))
                    return;

                if(native.isPedSwimming(alt.Player.local.scriptID))
                    return;

                if(native.isPedSwimmingUnderWater(alt.Player.local.scriptID))
                    return;
                
                let pos = alt.Player.local.pos

                let toggle1 = false //?? World: wenn true und die Position + Range enthält ein Mapobjekt / Wasser etc., dann zählt die Position als occupied
                let toggle2 = true  //Müssten Vehicles sein
                let toggle3 = false //Entity: wenn true und ein Entity ist in der Position + Range, dann zählt die Position als occupied
                let toggle4 = true //World: Ist auf dem Boden / in Bodennähe?
                let toggle5 = false
                let toggle6 = 0
                let toggle7 = false
                let range = 6
                if(native.isPositionOccupied(pos.x, pos.y, pos.z, range, toggle1, toggle2, toggle3, toggle4, toggle5, toggle6, toggle7))
                {
                    return;
                }

                for(var i = 0; i < 17; i ++)
                {
                    let rad = (360 / 16) * i * Math.PI / 180
                    let newStartPosX = pos.x
                    let newStartPosY = pos.y
                    
                    if(i < 16)
                    {
                        newStartPosX = pos.x + 0.2 * Math.sin(rad)
                        newStartPosY = pos.y + 0.2 * Math.cos(rad)
                    }

                    let rayHandle = native.startExpensiveSynchronousShapeTestLosProbe(newStartPosX, newStartPosY, pos.z, newStartPosX, newStartPosY, pos.z - 2, 511, alt.Player.local.scriptID, 1)
                    let [handle, hit, endCoords, surfaceNormal, materialHash, entityHit] = native.getShapeTestResultIncludingMaterial(rayHandle, null, null, null, null, null)
                    if(hit)
                    {
                        return
                    }
                }
                alt.emitServer("SibsibFly", alt.Player.local.pos)
            }
            else
            {
                if(native.isEntityInAir(alt.Player.local.vehicle.scriptID))
                {
                    let speedVector = native.getEntitySpeedVector(alt.Player.local.vehicle.scriptID, true)
                    if(speedVector.z == 0)
                    {
                        this.vehFlyCounter += 1;
                        if(this.vehFlyCounter > 1)
                        {
                            alt.emitServer("SibsibFly", alt.Player.local.pos)
                        }
                    }
                }
                else
                {
                    this.vehFlyCounter = 0;
                }
            }
        }
        else
        {
            this.blockFlyCheck--;
        }
    }

    checkForAimMessage() {
        if(this.aimTimes.length > 0)
        {
            alt.emitServer("SibsibAim", this.aimTimes)
            this.aimTimes = []
        }
    }

    checkForShotsMessage() {
        if(this.hittedShots + this.missedShots > 10)
        {
            alt.emitServer("SibsibShots", this.hittedShots, this.missedShots)
            this.hittedShots = 0
            this.missedShots = 0
        }
        if(this.hittedBones.length > 5)
        {
            alt.emitServer("SibsibBones", this.hittedBones)
            this.hittedBones = []
        }
    }

    checkForMessage() {
        if(this.healthAlarmMsg.length > 0)
        {
            alt.emitServer("SibsibHealth", this.healthAlarmMsg)
            this.healthAlarmMsg = []
        }

        if(this.armorAlarmMsg.length > 0)
        {
            alt.emitServer("SibsibArmor", this.armorAlarmMsg)
            this.armorAlarmMsg = []
        }

        if(this.modelAlarmMsg.length > 0)
        {
            alt.emitServer("SibsibModel", this.modelAlarmMsg)
            this.modelAlarmMsg = []
        }

        if(this.isSuspect >= 5 && this.timeBetweenClicks.length > 0 && alt.Player.local.vehicle == null)
        {
            alt.emitServer("SibsibSpeedi", this.timeBetweenClicks);
        }
        this.timeBetweenClicks = [];
        this.isSuspect = 0;

        if(this.insertCounter > 0)
        {
            alt.emitServer("SibsibInsert", this.insertCounter);
            this.insertCounter = 0
        }
        if (this.endCounter > 0) {
            alt.emitServer("SibsibEnd", this.endCounter);
            this.endCounter = 0
        }
    }

    checkSpeedi() {
        if(alt.Player.local.vehicle == null)
        {
            if (native.isControlPressed(0, 24))
            {
                if(!this.isPressed)
                {
                    this.isPressed = true
                    const t = Date.now()
                    if(this.lastClick != null)
                    {
                        const timeBetween = t - this.lastClick
                        
                        if(this.timeBetweenClicks.length < 50)
                        {
                            this.timeBetweenClicks.push(timeBetween)
                        }
                        if(timeBetween < this.threshold)
                        {
                            this.isSuspect++
                        }
                    }
                    this.lastClick = t
                }
            }
            else
            {
                this.isPressed = false
            }
        }
    }

    checkHealth() {
        const currentHealth = native.getEntityHealth(alt.Player.local.scriptID)
        if(this.allowedHealth < currentHealth)
        {
            if(this.allowedHealth == 0 && (currentHealth == 200 || currentHealth == 230) || this.allowedHealth == 100 && currentHealth == 101)
            {
                return;
            }
            if(this.healthAlarmMsg.length < 50)
            {
                this.healthAlarmMsg.push(this.allowedHealth.toString() + " HP -> " + currentHealth + " HP")
            }
            native.setEntityHealth(alt.Player.local.scriptID, this.allowedHealth, 0)
        }
        else if (this.allowedHealth > currentHealth)
        {
            this.allowedHealth = currentHealth;
            player.applyDamageEffect();
        }
    }

    checkArmor() {
        const currentArmor = native.getPedArmour(alt.Player.local.scriptID)
        if(this.allowedArmor < currentArmor)
        {
            if(this.armorAlarmMsg.length < 50)
            {
                this.armorAlarmMsg.push(this.allowedArmor.toString() + " Armor -> " + currentArmor + " Armor")
            }
            native.setPedArmour(alt.Player.local.scriptID, this.allowedArmor)
        }
        else
        {
            this.allowedArmor = currentArmor
        }
    }

    checkInvincible() {
        if(native.getPlayerInvincible(alt.Player.local.scriptID) == true && player.isInvincible == false)
        {
            alt.emitServer("SibsibGod")
            player.setInvincible(false)
        }
    }
}




export default new Sibsib()