import alt from "alt"
import native from "natives"
import player from "/api/modules/player.mjs"
import animation from "/api/modules/animation.mjs"
import NoClip from "/api/modules/noclip.mjs"
import Cam from "/api/modules/cam.mjs"
import webview from "/api/modules/webview.mjs"
import HackingGame from "/api/modules/hackinggame.mjs"
import vehicle from "/api/modules/vehicle.mjs"

alt.onServer("log", (log) => {
    alt.log(JSON.stringify(log))
})

//let first = true
let oldWeather = undefined
let currentWeather = undefined;

alt.onServer("SetWeather", (weather) => {
    
    let snowWeathers = ["XMAS"]
    let time = 30
    if (currentWeather == undefined) {
        native.setWeatherTypeNowPersist(weather);

        oldWeather = weather
        currentWeather = weather;

        if(snowWeathers.includes(currentWeather))
        {
            native.setForceVehicleTrails(true)
            native.setForcePedFootstepsTracks(true)
            native.requestScriptAudioBank("ICE_FOOTSTEPS", false, 0)
            native.requestScriptAudioBank("SNOW_FOOTSTEPS", false, 0)

            native.requestNamedPtfxAsset("core_snow");

            let timer = alt.setInterval(() => {
                if(native.hasNamedPtfxAssetLoaded("core_snow")){
                    native.useParticleFxAsset("core_snow");
                    alt.clearInterval(timer);
                }
            }, 1);
        }
        else
        {
            native.setForceVehicleTrails(false)
            native.setForcePedFootstepsTracks(false)
        }
        return;
    }
    
    currentWeather = weather;

    alt.log("CHANGE WEATHER: " + oldWeather + " to " + weather)

    if (oldWeather != currentWeather) {
        native.clearWeatherTypePersist()
        let i = 0;
        let inter = alt.setInterval(() => {
            i++;
            if(i < 100) {
                native.setWeatherTypeTransition(native.getHashKey(oldWeather), native.getHashKey(currentWeather), (i / 100));
            } else {
                //alt.log("CHANGE WEATHER FINISHED")
                alt.clearInterval(inter)
                native.setWeatherTypeNowPersist(currentWeather)
                oldWeather = currentWeather;
            }
        }, (time * 10))

        if(snowWeathers.includes(currentWeather))
        {
            native.setForceVehicleTrails(true)
            native.setForcePedFootstepsTracks(true)
            native.requestScriptAudioBank("ICE_FOOTSTEPS", false, 0)
            native.requestScriptAudioBank("SNOW_FOOTSTEPS", false, 0)

            native.requestNamedPtfxAsset("core_snow");

            let timer = alt.setInterval(() => {
                if(native.hasNamedPtfxAssetLoaded("core_snow")){
                    native.useParticleFxAsset("core_snow");
                    alt.clearInterval(timer);
                }
            }, 1);
        }
        else if (snowWeathers.includes(oldWeather))
        {
            native.setForceVehicleTrails(false)
            native.setForcePedFootstepsTracks(false)
        }
    }
})

alt.onServer("SetGPS", (x, y) => {
    native.setNewWaypoint(x, y)
})

alt.onServer("AddAreaBlip", (pos, radius, color, duration) => {
    let blip = new alt.RadiusBlip(pos.x, pos.y, pos.z, radius);
    blip.color = color;
    blip.alpha = 100;
    alt.setTimeout(() => {
        // alt.log("Destroy Blip");
        blip.destroy();
    }, duration)
})

alt.onServer("VehicleSpeed", (speed) => {
    if (alt.Player.local.vehicle == null) return
    native.modifyVehicleTopSpeed(alt.Player.local.vehicle.scriptID, speed)
})

alt.onServer("PlayAnimDebug", async (animDict, animName, flag) => {
    //await animation.loadAnim(animDict)
    //native.taskPlayAnim(alt.Player.local.scriptID, animDict, animName, speed, speedMultiplier, duration, flag, 0, false, false, false)
    //native.taskPlayAnim(alt.Player.local.scriptID, animDict, animName, 8, -4, -1, flag, 0, false, false, false)
    animation.playAnim(animDict, animName, flag)
})

alt.onServer("PlayAnimFarming", async (animDict, animName, flag) => {
    //await animation.loadAnim(animDict)
    player.cantCancelAnimation = true
    //native.taskPlayAnim(alt.Player.local.scriptID, animDict, animName, 8, -4, -1, flag, 0, false, false, false)
    animation.playAnim(animDict, animName, flag)
})

alt.onServer("PlayAnim", async (id, duration) => {
    player.cantCancelAnimation = true
    await animation.play(id)
    if (duration > 0) webview.updateView("Bar", [duration])
    player.playAnim = true
    if (duration != 0) player.disableMovement = true
})

alt.onServer("PlayAnimAtCoords", async (id, pos, rot) => {
    player.cantCancelAnimation = true
    await animation.playAtCoords(id, pos, rot)
    player.playAnim = true
    player.disableMovement = true
})

alt.onServer("PlayAnimOnce", async (id) => {
    await animation.play(id)
})

alt.onServer("ShowProgress", (duration) => {
    webview.updateView("Bar", [duration])
})

alt.onServer("StopAnim", (animDict, animName, p3, unfreeze) => {
    player.cantCancelAnimation = false
    native.stopAnimTask(alt.Player.local.scriptID, animDict, animName, p3)
    if (unfreeze) player.isFreezed = false
})

alt.onServer("ClearTasks", () => {
    player.clearTasks();
})

alt.onServer("PlayScenario", (scenarioName, duration) => {
    player.cantCancelAnimation = true
    native.taskStartScenarioInPlace(alt.Player.local.scriptID, scenarioName, 0, true)
    if (duration > 0) webview.updateView("Bar", [duration])
})

alt.onServer("ADuty", status => {
    player.setAdminDuty(status)
})

alt.onServer("SetBlip", (x, y, z, sprite, color, name) => {
    const blip = new alt.PointBlip(x, y,z)
    blip.sprite = sprite
    blip.color = color
    blip.name = name
    blip.shortRange = true
})

alt.onServer("SetPlayerBlips", (blipDataListe) => {
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = blipDataListe.s
        blip.color = blipDataListe.c
        blip.name = obj.n
        blip.shortRange = true
    }
})

alt.onServer("SetPlayerBlipsSpriteColor", (blipDataListe) => {
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        // alt.log(obj.n + ": " + obj.s)
        blip.sprite = obj.s
        blip.color = obj.c
        blip.name = obj.n
        blip.shortRange = true
    }
})

alt.onServer("SetBlips", (blipDataListe) => {
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = blipDataListe.s
        blip.color = blipDataListe.c
        blip.name = obj.n
        blip.shortRange = true
    }
})

let GDBlips = []

alt.onServer("SetGDBlips", (blipDataListe) => {
    player.unloadMapBlips()
    for(const obj of GDBlips) {
        obj.destroy()
    }
    GDBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = obj.s
        blip.color = obj.c
        blip.name = obj.n
        blip.shortRange = true
        GDBlips.push(blip)
    }
})


alt.onServer("ClearGDBlips", () => {
    for(const obj of GDBlips) {
        obj.destroy()
    }
    GDBlips = []
    player.loadMapBlips()
})

let temporaryBlips = []

alt.onServer("SetTemporaryBlips", (blipDataListe) => {
    for(const obj of temporaryBlips) {
        obj.destroy()
    }
    temporaryBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = obj.s
        blip.color = obj.c
        blip.name = obj.n
        blip.shortRange = true
        temporaryBlips.push(blip)
    }
})

alt.onServer("ClearTemporaryBlips", () => {
    for(const obj of temporaryBlips) {
        obj.destroy()
    }
    temporaryBlips = []
})

let teamVehicleBlips = []

alt.onServer("SetTeamVehicleBlips", (blipDataListe) => {
    player.unloadMapBlips()

    for(const obj of teamVehicleBlips) {
        obj.destroy()
    }
    teamVehicleBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = obj.s
        blip.color = blipDataListe.c
        blip.name = obj.n
        blip.shortRange = true
        teamVehicleBlips.push(blip)
    }
    webview.updateView("AddNotify", ["Leitstellenkarte aktualisiert!", "#c72020"])
})

alt.onServer("ClearTeamVehicleBlips", () => {
    for(const obj of teamVehicleBlips) {
        obj.destroy()
    }
    teamVehicleBlips = []
    for(const obj of serviceBlips) {
        obj.destroy()
    }
    serviceBlips = []
    player.loadMapBlips()
    webview.updateView("AddNotify", ["Leitstellenkarte deaktiviert!", "#c72020"])
})


let maklerBlips = []

alt.onServer("SetMaklerBlips", (idList) => {
    player.unloadMapBlips()

    for(const obj of maklerBlips) {
        obj.destroy()
    }
    maklerBlips = []
    const houses = JSON.parse(alt.File.read("@SibauiRP_Assets/data/house.json"))
    for (const house of houses) {
        if(!idList.d.includes(house.i))
        {
            const blip = new alt.PointBlip(house.x, house.y ,house.z)
            blip.sprite = 374
            blip.color = 2
            blip.name = "Freies Haus"
            blip.shortRange = true
            maklerBlips.push(blip)
        }
        else
        {
            const blip = new alt.PointBlip(house.x, house.y, house.z)
            blip.sprite = 374
            blip.color = 1
            blip.name = "Verkauftes Haus"
            blip.shortRange = true
            maklerBlips.push(blip)
        }
    }
    webview.updateView("AddNotify", ["Maklerkarte aktiviert!", "#c72020"])
})

alt.onServer("ClearMaklerBlips", () => {
    for(const obj of maklerBlips) {
        obj.destroy()
    }
    maklerBlips = []
    player.loadMapBlips()
    webview.updateView("AddNotify", ["Maklerkarte deaktiviert!", "#c72020"])
})

let serviceBlips = []

alt.onServer("SetServiceBlips", (blipDataListe) => {
    for(const obj of serviceBlips) {
        obj.destroy()
    }
    serviceBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = blipDataListe.s
        blip.color = blipDataListe.c
        blip.name = obj.n
        blip.shortRange = true
        serviceBlips.push(blip)
    }
})

let deliverPointsBlips = []

alt.onServer("SetDeliveryWaypoints", (deliveryPointsListe) => {
    // alt.log("SetDeliveryWaypoints Event")
    for(const obj of deliverPointsBlips) {
        obj.destroy()
    }
    deliverPointsBlips = []
    if(deliveryPointsListe != false)
    {
        for (const obj of deliveryPointsListe) {
            const blip = new alt.PointBlip(obj.x, obj.y, obj.z)
            blip.sprite = 501
            blip.color = 5
            blip.name = "Auslieferungspunkt"
            blip.shortRange = true

            deliverPointsBlips.push(blip)
        }
    }
})

alt.onServer("ClearDeliveryWaypoints", () => {
    for(const obj of deliverPointsBlips) {
        obj.destroy()
    }
    deliverPointsBlips = []
})

let tempBlips = []

alt.onServer("SetTempPlayerBlips", (blipDataListe) => {
    for(const obj of tempBlips) {
        obj.destroy()
    }
    tempBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = blipDataListe.s
        blip.color = blipDataListe.c
        blip.name = obj.n
        blip.shortRange = true
        tempBlips.push(blip)
    }
})


let gangwarBlips = []

alt.onServer("SetGangwarBlips", (blipDataListe) => {
    for(const obj of gangwarBlips) {
        obj.destroy()
    }
    gangwarBlips = []
    for (const obj of blipDataListe.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = 543
        blip.color = obj.c
        blip.name = obj.n
        blip.shortRange = true
        gangwarBlips.push(blip)
    }
})

alt.onServer("ClearGangwarBlips", () => {
    for(const obj of gangwarBlips) {
        obj.destroy()
    }
    gangwarBlips = []
})

alt.onServer("SetStat", (statName, value) => {
    alt.setStat(statName, value)
})
alt.onServer("AClothes", (equippedClothesList) => {
    //player.cloth = {}
    for(const obj of equippedClothesList.data) {
        //alt.log(obj.c)
        native.setPedComponentVariation(alt.Player.local.scriptID, obj.c, obj.d, obj.t, 0)
    }
})
alt.onServer("SetClothes", (equippedClothesList) => {
    player.cloth = {}
    //webview.clearProps();
    //webview.clearAllPedProps();
    for(const obj of equippedClothesList.data) {
        //alt.log(obj)
        if (player.cloth[obj.c] == undefined)
            player.cloth[obj.c] = { w: obj.c != 1 }

        // w = wear
        player.cloth[obj.c].c = obj.c
        player.cloth[obj.c].d = obj.d
        player.cloth[obj.c].t = obj.t
    }

    player.setClothes()
})
alt.onServer("SetProps", (equippedPropsList) => {
    player.props = {}
    for(const obj of equippedPropsList.data) {
        //alt.log(obj.c)
        if (player.props[obj.c] == undefined)
            player.props[obj.c] = { w: true }

        player.props[obj.c].c = obj.c // Kategorie
        player.props[obj.c].d = obj.d // Drawable
        player.props[obj.c].t = obj.t // Texture
    }

    player.setProps()
})
alt.onServer("SetJail", () => {
    player.setJailClothes()
})
alt.onServer("SetDive", (color) => {
    player.setDiveClothes(color)
})

alt.onServer("SetPlayerCloth", (component, drawable, texture) => {
    native.setPedComponentVariation(alt.Player.local.scriptID, component, drawable, texture, 0)
})

alt.onServer("SetPlayerProps", (equippedPropsList) => {
    for (const obj of equippedPropsList.data) {
        native.setPedPropIndex(alt.Player.local.scriptID, obj.c, obj.d, obj.t, true)
    }
})

alt.onServer("SetPlayerProp", (component, drawable, texture) => {
    native.setPedPropIndex(alt.Player.local.scriptID, component, drawable, texture, true)
})

alt.onServer("ClearProps", () => {
    native.clearAllPedProps(alt.Player.local.scriptID)
})

alt.onServer("WarpIntoVeh", (vehicle, seatId) => {
    let cleared = false
    const interval = alt.setInterval(() => {
        const vehicleScriptId = vehicle.scriptID
        if (vehicleScriptId) {
            native.setPedIntoVehicle(alt.Player.local.scriptID, vehicleScriptId, seatId)
            alt.clearInterval(interval)
            cleared = true
        }
    }, 10)

    alt.setTimeout(() => {
        if (!cleared) {
            alt.clearInterval(interval)
        }
    }, 5000)
})

alt.onServer("WarpOutOfVeh", async vehicle => {
    native.taskLeaveVehicle(alt.Player.local.scriptID, vehicle.scriptID, 4160)

    if (player.isTied) await animation.play(1)
    else if (player.isCuffed) await animation.play(0)
    //else if (player.isInjured) alt.setTimeout(async () => await animation.play(17), 150)
    else if (player.isInjured) {
        let cleared = false
        const interval = alt.setInterval(async () => {
            if (alt.Player.local.vehicle == null) {
                await animation.play(17)
                alt.clearInterval(interval)
                cleared = true
            }
        }, 10)
    
        alt.setTimeout(() => {
            if (!cleared) {
                alt.clearInterval(interval)
            }
        }, 5000)
    }
})

alt.onServer("GetVehicleSpeed", (speedCamId) => {
    let vehicle = alt.Player.local.vehicle
    if (vehicle == undefined || vehicle == null)return
    let kmh = Math.floor(native.getEntitySpeed(alt.Player.local.vehicle.scriptID) * 3.6)
    // alt.log("KMH: "+ kmh)

    alt.emitServer("SendVehicleSpeed", speedCamId, Math.round(kmh))
})


alt.onServer("SetStateOfDoor", (modelHash, locked, positionX, positionY, positionZ) => {
    native.doorControl(modelHash, positionX, positionY, positionZ, locked, 0, 0, 0)
    if(locked == true)
    {
        native.setStateOfClosestDoorOfType(modelHash, positionX, positionY, positionZ, true, 0, 0)
    }
})


alt.onServer("DoorState", (modelHash, locked, positionX, positionY, positionZ, heading) => {
    // alt.log("Modelhash: " + modelHash + ", locked: " + locked + ", heading: " + heading)
    native.setStateOfClosestDoorOfType(modelHash, positionX, positionY, positionZ, locked, heading, 0)
})


/*alt.onServer("PaletobankDoors", (firstDoorState, secondDoorState, thirdDoorState) => {
    let modelHash = 3109761617
    let posX = -104.6049
    let posY = 6473.443
    let posZ = 31.79532
    let locked = true
    let heading = 0.1
    if(firstDoorState == true)
    {
        heading = -1.167
    }
    native.setStateOfClosestDoorOfType(modelHash, posX, posY, posZ, true, heading, 0)

    modelHash = 1622278560
    posX = -104.8136
    posY = 6473.646
    posZ = 31.9548
    locked = true
    heading = 0.29

    if(secondDoorState == true)
    {
        locked = true
        native.setStateOfClosestDoorOfType(modelHash, posX, posY, posZ, true, heading, 0)
    }
    else
    {
        native.doorControl(modelHash, posX, posY, posZ, locked, 0, 0, 0)
    }

    modelHash = 1309269072
    posX = -106.4713
    posY = 6476.157
    posZ = 31.9548
    locked = false

    if(thirdDoorState == true)
    {
        locked = true
    }
    native.doorControl(modelHash, posX, posY, posZ, locked, 0, 0, 0)
})*/

alt.onServer("GetGroundZFrom3DCoord", (pos, name) => {
    const [toggle, z] = native.getGroundZFor3dCoord(pos.x, pos.y, pos.z, undefined, true)
    // alt.log(JSON.stringify(z))
    // alt.log(JSON.stringify(toggle))
	alt.emitServer('GetGroundZFrom3DCoord', parseFloat(z), name)
})

alt.onServer("GivePlayerRagdollControl", (toggle) => {
    // alt.log(JSON.stringify("GivePlayerRagdollControl CLientevent"))
    if(toggle == 1)
        native.givePlayerRagdollControl(alt.Player.local.scriptID, true)
    else
        native.givePlayerRagdollControl(alt.Player.local.scriptID, false)
})

alt.onServer("SetPlayerTargetingMode", (toggle) => {
    //0 = Traditional GTA
    //1 = Assisted Aiming
    //2 = Free Aim
    alt.log(JSON.stringify("SetPlayerTargetingMode CLientevent"))
    if(toggle == 0)
        native.setPlayerTargetingMode(0)
    else if (toggle == 1)
        native.setPlayerTargetingMode(1)
    else if (toggle == 2) 
        native.setPlayerTargetingMode(2)
})

alt.onServer("SetSwimMultiplierForPlayer", (multiplier) => {
    alt.log(JSON.stringify("SetSwimMultiplierForPlayer CLientevent"))
    native.setSwimMultiplierForPlayer(alt.Player.local, parseFloat(multiplier))
})

alt.onServer("SetRunSprintMultiplierForPlayer", (multiplier) => {
    alt.log(JSON.stringify("SetRunSprintMultiplierForPlayer CLientevent"))
    native.setRunSprintMultiplierForPlayer(alt.Player.local, parseFloat(multiplier))
})

alt.onServer("SetPlayerMaxArmour", (maxArmor) => {
    // alt.log(JSON.stringify("SetPlayerMaxArmour CLientevent"))
    //default 100
    native.setPlayerMaxArmour(alt.Player.local, parseInt(maxArmor))
    var z = native.getPlayerMaxArmour(alt.Player.local)
    // alt.log("armor" + z)
})

alt.onServer("SetPlayerMaxHealth", (maxHealth) => {
    // alt.log(JSON.stringify("SetPlayerMaxHealth CLientevent"))
    //default 100
    native.setPedMaxHealth(alt.Player.local.scriptID, parseInt(maxHealth))
    var z = native.getEntityMaxHealth(alt.Player.local.scriptID)
    // alt.log("health" + z)
})


alt.onServer("SetNightvision", (toggle) => {
    alt.log(JSON.stringify("SetNightvision CLientevent"))
    if(toggle == 1)
        native.setNightvision(1)
    else
        native.setNightvision(0)
})

alt.onServer("SetFlash", () => {
    alt.log(JSON.stringify("SetFlash CLientevent"))
    native.setFlash(parseFloat(0), parseFloat(0), parseFloat(1000), parseFloat(3000), parseFloat(1000))
})

alt.onServer("SetArtificialLightsState", (toggle) => {
    alt.log(JSON.stringify("SetArtificialLightsState CLientevent"))
    if(toggle == 1)
        native.setArtificialLightsState(1)
    else
        native.setArtificialLightsState(0)
})

alt.onServer("SetSeethrough", (toggle) => {
    alt.log(JSON.stringify("SetSeethrough CLientevent"))
    if(toggle == 1)
        native.setSeethrough(1)
    else
        native.setSeethrough(0)
})

alt.onServer("SetSeethroughScale", (index, heatScale) => {
    alt.log(JSON.stringify("CLientevent"))
    native.seethroughSetHeatscale(index, heatScale)
})

alt.onServer("BlurIn", (transitionTime) => {
    webview.hidePhone()
    webview.hideFunk()
    webview.closeInterface()
    native.triggerScreenblurFadeIn(transitionTime)
})

alt.onServer("BlurOut", (transitionTime) => {
    let cleared = false
    const interval = alt.setInterval(() => {
        const isRunning = native.isScreenblurFadeRunning()
        if (!isRunning) {
            native.triggerScreenblurFadeOut(transitionTime)
            alt.clearInterval(interval)
            cleared = true
        }
    }, 1000)

    alt.setTimeout(() => {
        if (!cleared) {
            alt.clearInterval(interval)
        }
    }, 10000)
})

alt.onServer("EnableClownBloodVfx", (toggle) => {
    alt.log(JSON.stringify("EnableClownBloodVfx CLientevent"))
    if(toggle == 1)
        native.enableClownBloodVfx(true)
    else
        native.enableClownBloodVfx(false)
})

alt.onServer("SetPedMaxTimeUnderwater", (time) => {
    alt.log(JSON.stringify("setPedMaxTimeUnderwater CLientevent"))
    native.setPedMaxTimeUnderwater(alt.Player.local.scriptID, parseFloat(time))
})

alt.onServer("CreateAreaBlip", (pos, radius, color, alpha) => {
    alt.log("CreateAreaBlip Clientevent")
    /*for(var i=-10; i < 10; i++)
    {
        for(var j = -10; j < 10; j ++)
        {
            const blip = new alt.AreaBlip(pos.x + 25*i, pos.y + 25*j, pos.z, width, height)
            blip.color = 1
            blip.alpha = 100
            blip.heading = heading
            blip.shortRange = true
        }
    }*/
    /*const blip = new alt.PointBlip(pos.x, pos.y, pos.z)
    blip.sprite = 5
    blip.color = 1
    blip.name = name
    blip.alpha = 100
    blip.heading = 0*/
    const blip = new alt.RadiusBlip(pos.x, pos.y, pos.z, radius)
    blip.color = color
    blip.alpha = alpha
    blip.heading = 0
    blip.shortRange = true
})

alt.onServer("GetZoneName", (pos) => {
    alt.log(JSON.stringify("GetZoneAtCoords CLientevent"))
    var zoneNameId = native.getNameOfZone(pos.x, pos.y, pos.z)
	alt.emitServer('GetZoneName', zoneNameId)
})

alt.onServer("GetFishingZone", (pos) => {
    var zoneNameId = native.getNameOfZone(pos.x, pos.y, pos.z)
    //var isInWater = native.isEntityInWater(alt.Player.local.scriptID)
    //if(!isInWater && (zoneNameId == "OCEANA" || zoneNameId == "ALAMO"))
    //    zoneNameId = "NotInWater"
	alt.emitServer('ReturnFishingZone', zoneNameId)
})

alt.onServer("LoadIpl", (name) => {
    alt.requestIpl(name)
})

alt.onServer("UnloadIpl", (name) => {
    alt.removeIpl(name)
})


alt.onServer("LoadProp", (pos, name, color) => {
    var interior = native.getInteriorAtCoords(pos.x, pos.y, pos.z)
    native.activateInteriorEntitySet(interior, name)
    native.setInteriorEntitySetColor(interior, name, color)
    native.refreshInterior(interior)
})

alt.onServer("UnloadProp", (pos, name) => {
    var interior = native.getInteriorAtCoords(pos.x, pos.y, pos.z)
    native.deactivateInteriorEntitySet(interior, name)
    native.refreshInterior(interior)
})

alt.onServer("StartFire", (pos) => {
    native.startScriptFire(pos.x, pos.y, pos.z - 1.0, 25, false)
})

alt.onServer("StopFire", (pos) => {
    native.stopFireInRange(pos.x, pos.y, pos.z -1.0, 3)
})

alt.onServer("StartCmdEffect", (name, duration, looped) => {
    native.animpostfxStopAll()
    if(looped == 1)
        native.animpostfxPlay(name, duration, true)
    else
        native.animpostfxPlay(name, duration, false)
})

alt.onServer("StopCmdEffect", () => {
    native.animpostfxStopAll()
})

var storageCrates = []
var warehouse = JSON.parse(alt.File.read("/api/assets/storageroom.json"))

alt.onServer("LoadStorageroom", (upgradeLevel) => {
    //storageCrates.push(native.createObject(alt.hash("p_secret_weapon_02"), 1026.771, -3091.579, -39.99937, false, false, false))
    let type = 1
    for(var i = 0; i < upgradeLevel; i++)
    {
        let obj = native.createObject(alt.hash(warehouse.storagerooms[type].crates[i].Propname), warehouse.storagerooms[type].crates[i].Position.X, warehouse.storagerooms[type].crates[i].Position.Y, warehouse.storagerooms[type].crates[i].Position.Z, false, false, false);
        native.setEntityRotation(obj, warehouse.storagerooms[type].crates[i].Rotation.X, warehouse.storagerooms[type].crates[i].Rotation.Y, warehouse.storagerooms[type].crates[i].Rotation.Z, 2, true)
        storageCrates.push(obj)
    }
})

alt.onServer("UnloadStorageroom", () => {
    storageCrates.forEach(obj => {
        native.deleteObject(obj)
    })
})

alt.onServer("ReloadStorageroom", (upgradeLevel) => {
    storageCrates.forEach(obj => {
        native.deleteObject(obj)
    })
    let type = 1
    for(var i = 0; i < upgradeLevel; i++)
    {
        storageCrates.push(native.createObject(alt.hash(warehouse.storagerooms[type].crates[i].Propname), warehouse.storagerooms[type].crates[i].Position.X, warehouse.storagerooms[type].crates[i].Position.Y, warehouse.storagerooms[type].crates[i].Position.Z, false, false, false))
    }
})

var storageEveryTick = null;

alt.onServer("ShowStorageMarker", (positionList) => {
    if(storageEveryTick != null)
    {
        alt.clearEveryTick(storageEveryTick);
        storageEveryTick = null;
    }
    storageEveryTick = alt.everyTick(() => {
        for (const obj of positionList.pos) {
            native.drawMarker(
                1, obj.x, obj.y, obj.z -1.0,
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
                255, 20, 147, 255,
                !!false, !!false, 2, !!false, null, null, !!false
            );
        }
        
    });

    alt.setTimeout(() => {
        alt.clearEveryTick(storageEveryTick);
        storageEveryTick = null;
    }, 30000)
})

var CamperObjects = []
var Kolben = null

alt.onServer("LoadCamper", (isTeam, isCooking) => {
    native.doorControl(132154435, 1972.769, 3815.366, 33.66326, true, 0, 0, 0)
    if(isCooking == true)
    {
        Kolben = native.createObject(alt.hash("prop_drug_erlenmeyer"), 1975.788, 3817.745, 33.4249, false, false, false)
    }
    else 
    {
        Kolben = native.createObject(alt.hash("prop_drug_erlenmeyer"), 1976.096, 3818.717, 33.4249, false, false, false)

    }

    native.freezeEntityPosition(Kolben, true)
    native.setEntityCollision(Kolben, false, true)
    native.setObjectLightColor(Kolben, true, 255, 0, 0)
    CamperObjects.push(Kolben)
    var obj2 = native.createObject(alt.hash("prop_drug_package"), 1976.354, 3821.494, 33.17837, false, false, false)
    native.setEntityRotation(obj2, 0, 0, 0.7192099, 0, true)
    native.freezeEntityPosition(obj2, true)
    CamperObjects.push(obj2)
})

alt.onServer("UnloadCamper", () => {
    CamperObjects.forEach(obj => {
        native.deleteObject(obj)
    })
})

alt.onServer("LoadTerbyte", () => {
    
    var interior = native.getInteriorAtCoords(-1421.0150, -3012.5870, -80.0)
    alt.log("interior: " + JSON.stringify(interior))
    native.activateInteriorEntitySet(interior, "int_03_ba_drone")
    native.activateInteriorEntitySet(interior, "int_03_ba_weapons_mod")
    native.setInteriorEntitySetColor(interior, "int_03_ba_weapons_mod", 9)
    native.activateInteriorEntitySet(interior, "int_03_ba_tint")
    native.setInteriorEntitySetColor(interior, "int_03_ba_tint", 9)
    native.activateInteriorEntitySet(interior, "int_03_ba_design_12")
    native.activateInteriorEntitySet(interior, "int_03_ba_bikemod")
    native.setInteriorEntitySetColor(interior, "int_03_ba_bikemod", 9)
    native.activateInteriorEntitySet(interior, "int_03_ba_light_rig1")
    native.refreshInterior(interior)
})

alt.onServer("ToggleCamperProcess", (toggle) => {
    if(toggle == true)
    {
        native.setEntityCoordsNoOffset(Kolben, 1975.788, 3817.745, 33.4249, true, true, true)
    }
    else
    {
        native.setEntityCoordsNoOffset(Kolben, 1976.096, 3818.717, 33.4249, true, true, true)
    }
})

alt.onServer("SetRagdoll", (time1, time2, ragdollType, p4, p5, p6) => {
    native.setPedToRagdoll(alt.Player.local.scriptID, time1, time2, ragdollType, p4, p5, p6)
})

alt.onServer("SpikeStripEvent", () => {
    let vehicle = alt.Player.local.vehicle
    if (vehicle == undefined || vehicle == null) return
    native.setVehicleTyreBurst(vehicle.scriptID, Math.floor(Math.random() * 6), true, 1000.0)
    for (let index = 0; index < 6; index++) {
        if(Math.random() < 0.3)
        {
        native.setVehicleTyreBurst(vehicle.scriptID, index, true, 1000.0)
        }
    }
})

alt.onServer("NoClip", () => {
    if(NoClip.enabled)
    {
        NoClip.stop()
    }
    else
    {
        NoClip.start()
    }
});

alt.onServer('AttachToFlatBed', (entity, AttachedVeh, value) => {
    if (value) {
        const boneIndex = native.getEntityBoneIndexByName(entity.scriptID, "bodyshell");
        native.attachEntityToEntity(AttachedVeh.scriptID, entity.scriptID, boneIndex, 0, -2, 1.0, 0, 0, 0, true, false, true, false, 0, true);
    } else {
        native.detachEntity(AttachedVeh.scriptID, true, true);
    }
});

// alt.onServer('RepairVeh', (vehicle) => {
//     native.setVehicleFixed(vehicle.scriptID)
//     native.setVehicleDeformationFixed(vehicle.scriptID)
//     native.setVehicleDirtLevel(vehicle.scriptID, 0)
// });

alt.onServer('ShakeEvent', (toggle, type, amplitude) => {
    if(toggle == true)
    {
        alt.log("ShakeEvent active: " + type + ", amplitude: " + amplitude)
        native.shakeGameplayCam(type, amplitude)
    }
    else
    {
        alt.log("ShakeEvent inactive")
        native.stopGameplayCamShaking(true)
    }
});

alt.onServer('StartCCTV', (cams) => {
    Cam.start(cams)
});

alt.onServer('AtmBomb', (pos) => {
    native.addExplosion(pos.x, pos.y, pos.z, 50, 1.0, true, false, 1.0, false)
});

alt.onServer('Bomb', (pos, type, damageScale, camShake, noDamage) => {
    native.addExplosion(pos.x, pos.y, pos.z, type, parseFloat(damageScale), true, false, parseFloat(camShake), noDamage)
});

alt.onServer('OwnedBomb', (pos, type, damageScale, camShake) => {
    native.addOwnedExplosion(alt.Player.local.scriptID, pos.x, pos.y, pos.z, type, parseFloat(damageScale), true, false, parseFloat(camShake))
});
var shieldList = []
alt.onServer("Shield", (objectname, boneIndex, xPos, yPos, zPos, xRot, yRot, zRot) => {
    for(const obj of shieldList) {
        native.deleteObject(obj)
    }
    shieldList = []
    alt.log("boneIndex: " + boneIndex)
    alt.log("xPos: " + xPos)
    alt.log("yPos: " + yPos)
    alt.log("zPos: " + zPos)
    alt.log("xRot: " + xRot)
    alt.log("yRot: " + yRot)
    alt.log("zRot: " + zRot) // 180
    let pos = alt.Player.local.pos
    let shield = native.createObject(native.getHashKey(objectname), pos.x, pos.y, pos.z, false, false, false);
    shieldList.push(shield)
    //alt.log("shield: " + this.shield.scriptID)
    //native.attachEntityToEntity(shield, alt.Player.local.scriptID, boneIndex, xPos, yPos, zPos, xRot, yRot, zRot, true, false, true, false, 2, true)

    native.attachEntityToEntity(shield, alt.Player.local.scriptID, native.getEntityBoneIndexByName(alt.Player.local.scriptID, "IK_L_Hand"), 0.0, -0.05, -0.10, -30.0, 180.0, 40.0, 0, 0, 1, 0, 0, 1)
    //native.taskPlayAnim(alt.Player.local.scriptID, "combat@gestures@gang@pistol_1h@beckon", "0", 8.0, -8.0, -1, (2 + 16 + 32), 0.0, 0, 0, 0)
    animation.playAnim("combat@gestures@gang@pistol_1h@beckon", 50)
    native.setWeaponAnimationOverride(alt.Player.local.scriptID, native.getHashKey("Gang1H"))
    native.giveWeaponToPed(alt.Player.local.scriptID, native.getHashKey("WEAPON_PISTOL"), 300, 0, 1)
    native.setCurrentPedWeapon(alt.Player.local.scriptID, native.getHashKey("WEAPON_PISTOL"), 1)
});


alt.onServer('goToGps', () => {
    
    let waypoint = native.getFirstBlipInfoId(8);
    if (!native.doesBlipExist(waypoint)) { return; }
    const player = alt.Player.local.scriptID;
    const WaypointPosition = native.getBlipInfoIdCoord(waypoint);

    native.setFocusPosAndVel(WaypointPosition.x, WaypointPosition.y, WaypointPosition.z, 0 ,0 ,0);
    native.requestCollisionAtCoord(WaypointPosition.x, WaypointPosition.y, WaypointPosition.z);

    let z = Number(WaypointPosition.z);
    let IsGround = native.getGroundZFor3dCoord(WaypointPosition.x, WaypointPosition.y, WaypointPosition.z, undefined, undefined)[0];
    
    setTimeout(() => 
    {
        while(!IsGround) {
            IsGround = native.getGroundZFor3dCoord(WaypointPosition.x, WaypointPosition.y, z, undefined, undefined)[0];
            z++;
            if(z > 1000) {break;}
        }
    }, 1000);

      setTimeout(() => 
      {
        native.setFocusEntity(player);
        if(!IsGround) { return;}
        native.setPedCoordsKeepVehicle(alt.Player.local, WaypointPosition.x, WaypointPosition.y,z); 
      }, 1400); 
  });

alt.onServer("Blackout", state => {
    native.setArtificialLightsState(state)
});
    

alt.onServer("StartParticleCoord", (dict, name, duration, scale, x, y, z) => {
    alt.log("dict: " + dict + ", name: " + name + ", duration: " + duration + ", scale: " + scale)
    const particles = [];
    //if (name.includes('scr')) return; // scr particles break things easily.
    const interval = alt.setInterval(() => {
        native.requestPtfxAsset(dict);
        native.useParticleFxAsset(dict);
        const particle = native.startParticleFxLoopedAtCoord(
            name,
            x,
            y,
            z,
            0,
            0,
            0,
            scale,
            0,
            0,
            0,
            0
        );
        particles.push(particle);
    }, 0);

    alt.setTimeout(() => {
        alt.clearInterval(interval);
        native.stopFireInRange(x, y, z, 10);
        alt.setTimeout(() => {
            particles.forEach(particle => {
                alt.nextTick(() => {
                    native.stopParticleFxLooped(particle, false);
                });
            });
        }, duration * 2);
    }, duration);
});

alt.onServer('CreateNPC', (type, model, pos) => {
    // alt.log("CreateNPC Event: " + "type: " + type + ", model: " + model + ", hashKey: " + alt.hash( model ))
    native.requestModel(alt.hash( model ))
    native.createPed( type, alt.hash( model ), pos.x, pos.y, pos.z, 0, false, false);
});

let drunkstate = false
alt.onServer('Drunk', () => {
    drunkstate = !drunkstate
    alt.log("Drunkevent: " + drunkstate)
    native.setPedIsDrunk(alt.Player.local.scriptID, drunkstate)
});

let exhauststate = false
alt.onServer('Exhaust', () => {
    if (alt.Player.local.vehicle == null) return
    exhauststate = !exhauststate
    alt.log("Exhaustevent: " + exhauststate)
    native.enableVehicleExhaustPops(alt.Player.local.vehicle.scriptID, exhauststate)
});

let sirenstate = false
alt.onServer('Siren', () => {
    if (alt.Player.local.vehicle == null) return
    sirenstate = !sirenstate
    alt.log("sirenstateevent: " + sirenstate)
    native.setSirenWithNoDriver(alt.Player.local.vehicle.scriptID, sirenstate)
});

alt.onServer('Alarm', (name) => {
    alt.log("Alarmevent: " + name)
    native.stopAllAlarms(true)
    native.startAlarm(name, 1)
});

alt.onServer('Gravity', (level) => {
    alt.log("Gravityevent: " + level)
    native.setGravityLevel(level)
});

let tennisstate = false;
alt.onServer('Tennis', () => {
    alt.log("Tennis: ")
    tennisstate = !tennisstate
    native.enableTennisMode(alt.Player.local.scriptID, tennisstate, 0)
});

alt.onServer('OceanWave', (value) => {
    alt.log("OceanWave: ")
    native.setDeepOceanScaler(value)
});


let vehicleGripBool = false;
alt.onServer('Troll', (type) => {

    let pos = alt.Player.local.pos;
    switch(type) {
        case 1: 
            native.setHighFallTask(alt.Player.local.scriptID, true, true, false)
            break;
        case 2:
            native.taskClimb(alt.Player.local.scriptID, false)
            break;
        case 3:
            native.taskSwapWeapon(alt.Player.local.scriptID, true)
            break;
        case 4:
            native.taskReloadWeapon(alt.Player.local.scriptID, true)
            break;
        case 5:
            native.taskLeaveAnyVehicle(alt.Player.local.scriptID, 0, 0)
            break;
        case 6:
            native.taskSmartFleeCoord(alt.Player.local.scriptID, alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z, 100, 5000, true, true)
            break;
        case 7:
            native.taskAchieveHeading(alt.Player.local.scriptID, Math.floor(Math.random() * Math.floor(360)), 1000)
            break;
        case 8:
            native.taskSkyDive(alt.Player.local.scriptID, true)
            break;
        case 9:
            native.taskCower(alt.Player.local.scriptID, 2500)
            break;
        case 10:
			native.setPedHeadBlendData(alt.Player.local.scriptID, 0, 0, 0, 0, 0, 0, 0, 0, 0, false)
            native.setPedHeadBlendData(alt.Player.local.scriptID, 12, 0, 0, 11, 19, 0, 0.7, 0.6, 0, false)
			native.setPedComponentVariation(alt.Player.local.scriptID, 2, 33, 0, 0)
            native.setPedHairColor(alt.Player.local.scriptID, 3, 3)
            native.setPedEyeColor(alt.Player.local.scriptID, 3)
			
			var features = [-0.6,-0.2,0.3,-0.5,-0.3,-0.1,-0.2,-0.7,0.0,-0.2,0.5,-0.5,0.3,-1.0,0.1,0.0,-0.4,-0.4,-0.3,0.6]
			
			for (const index in features) {
                native.setPedFaceFeature(alt.Player.local.scriptID, index, features[index])
            }
			
			var appearance = [{"Value":255,"Opacity":0.99},{"Value":18,"Opacity":0.54},{"Value":26,"Opacity":0.99},{"Value":8,"Opacity":0.57},{"Value":255,"Opacity":0.99},{"Value":255,"Opacity":0.14},{"Value":255,"Opacity":0.99},{"Value":3,"Opacity":0.36},{"Value":255,"Opacity":0.99},{"Value":0,"Opacity":0.1},{"Value":1,"Opacity":0.44}]
			
			for (const index in appearance) {
                const part = appearance[index]
                if (part.Value == 255) continue
                native.setPedHeadOverlay(alt.Player.local.scriptID, index, part.Value, part.Opacity)
            }
			
			native.setPedHeadOverlayColor(alt.Player.local.scriptID, 2, 1, 2, 0)
            native.setPedHeadOverlayColor(alt.Player.local.scriptID, 1, 1, 56, 0)
            native.setPedHeadOverlayColor(alt.Player.local.scriptID, 10, 1, 0, 0)
            native.setPedHeadOverlayColor(alt.Player.local.scriptID, 5, 2, 8, 0)
            native.setPedHeadOverlayColor(alt.Player.local.scriptID, 8, 2, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 1, 0, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 3, 14, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 4, 21, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 6, 91, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 7, 128, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 8, 15, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 9, 0, 0, 0)
            native.setPedComponentVariation(alt.Player.local.scriptID, 11, 35, 3, 0)

			break;

        case 11:
            webview.closeInterface()
            webview.showInterface(["Afk"])
            break;
        
        case 12:
            native.taskUseNearestScenarioToCoord(alt.Player.local.scriptID, pos.x, pos.y, pos.z, 30, 10000)
            break;
        case 13:
            var x = 0
            var y = 0
            var z = 1000
            native.applyForceToEntityCenterOfMass(alt.Player.local.scriptID, 1, x, y, z, false, true, true, false)
            break;
        case 14:
            z = 10
            y = -100
            native.applyForceToEntityCenterOfMass(alt.Player.local.scriptID, 1, x, y, z, false, true, true, false)
            break;
        case 15:
            native.taskPlantBomb(alt.Player.local.scriptID, pos.x, pos.y, pos.z, 0);
            break;
        case 16:
            native.setPedRagdollOnCollision(alt.Player.local.scriptID, true)
            break;
        case 17:
            native.startEntityFire(alt.Player.local);
            
            alt.setTimeout(() => {
                native.stopEntityFire(alt.Player.local);
            }, 50 * (alt.Player.local.health - 100));
            break;
        case 18:
            if(alt.Player.local.vehicle != null)
            {
                let entityVelocity = native.getEntityVelocity(alt.Player.local.vehicle)
                native.setEntityVelocity(alt.Player.local.vehicle, entityVelocity.x * 1.5, entityVelocity.y * 1.5, entityVelocity.z)
            }
            break;
        case 19:
            if(alt.Player.local.vehicle != null)
            {
                let driver = native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false);
                let seats = native.getVehicleModelNumberOfSeats(alt.Player.local.vehicle.model);
                let randomSeat = Math.floor(Math.random() * (seats - 0 + 1) + 0);
                if(driver == alt.Player.local.scriptID) {
                    if(native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, randomSeat, false)) {
                        return;
                    }
                    native.taskShuffleToNextVehicleSeat(alt.Player.local, alt.Player.local.vehicle, randomSeat);
                }
            }
            break;
        case 20:
            if(alt.Player.local.vehicle != null)
            {
                native.startVehicleHorn(alt.Player.local.vehicle, 2000, alt.hash('HELDDOWN'), false);
            }
            break;
        case 21:
            if(alt.Player.local.vehicle != null)
            {
                let bool = !vehicleGripBool;
                vehicleGripBool = bool;
                native.setVehicleReduceGrip(alt.Player.local.vehicle, bool);
            }
            break;
        // case 22:
        //     let posPed = alt.Player.local.pos;
        //     let closestPlayer = native.getClosestPed(posPed.x, posPed.y, posPed.z, 10.0, true, false, alt.Player.local, false, false, -1);
        //     native.taskShootAtEntity(alt.Player.local, closestPlayer,5000, alt.hash('FIRING_PATTERN_FULL_AUTO'));
        //     break;
        // case 23:
        //     let posPedVeh = alt.Player.local.pos;
        //     let closestVeh = native.getClosestVehicle(posPedVeh.x, posPedVeh.y, posPedVeh.z, 10.0, 0, 70);
        //     native.taskEnterVehicle(alt.Player.local, closestVeh, 2000, 0, 2.0, 1, 0);
        //     break;
        // case 24:
        //     webview.showInterface(["KnockKnock"]);
        //     break;
    }
});

alt.onServer('TestSimon', (vehicle, value) => {
    native.taskVehicleDriveWander(alt.Player.local.scriptID, vehicle.scriptID, parseFloat(50.0), parseFloat(value))
});

alt.onServer('TestSimon2', (vehicle) => {
    native.taskVehicleChase(alt.Player.local.scriptID, vehicle.scriptID)
    native.setTaskVehicleChaseIdealPursuitDistance(alt.Player.local.scriptID, 10)
    native.setTaskVehicleChaseBehaviorFlag(alt.Player.local.scriptID, 1, true)
});

alt.onServer('TestSimon3', (vehicle, player) => {
    native.taskVehicleFollow(alt.Player.local.scriptID, vehicle.scriptID, player.scriptID, parseFloat(100), 1, 10)
});

alt.onServer('TestSimon4', (vehicle) => {
    native.taskVehicleDriveToCoordLongrange(alt.Player.local.scriptID, vehicle.scriptID, parseFloat(0), parseFloat(0), parseFloat(74), parseFloat(100.0), 0, parseFloat(5.0))
});

alt.onServer('TestSimon5', (vehicle) => {
    native.taskVehicleDriveToCoord(alt.Player.local.scriptID, vehicle.scriptID, parseFloat(0), parseFloat(0), parseFloat(74), parseFloat(100.0), parseFloat(1.0), alt.hash("Zentorno"), 0, parseFloat(5.0), true)
    native.setPedKeepTask(alt.Player.local.scriptID, true)
});

alt.onServer('TestSimon6', (player) => {
    native.taskArrestPed(alt.Player.local.scriptID, player.scriptID)
});

alt.onServer("UpdateFitness", (fitness) => {
    player.setFitness(fitness)
})


alt.onServer("CarWash", (vehicle) => {

    let startX = 171.613;
    for (let i = 0; i < 25; i++) {
        alt.nextTick(() => {
            native.requestPtfxAsset("core");
            native.useParticleFxAsset("core");
            native.startParticleFxLoopedAtCoord("ent_sht_water_tower", startX, -1738.81, 33.2799, 0,0,0, 1,0, 0, 0, 0);
            startX += 0.3;
        });
    }
    
    
})


alt.onServer("StartCarwash", (isDriver) => {
    if(isDriver) {
        player.disableMovement = true;
        
        native.taskVehicleDriveToCoord(alt.Player.local.scriptID, alt.Player.local.vehicle.scriptID, 36.7912, -1391.82, 28.926, 3.0, 0, alt.Player.local.vehicle.model, 4194304, 0.2, 0)

    }
    
    alt.setTimeout(() => {
        let start = -1393.19;
        for (let i = 0; i < 10; i++) {
            alt.nextTick(() => {
                native.requestPtfxAsset("core");
                native.useParticleFxAsset("core");
                native.startParticleFxLoopedAtCoord("ent_sht_water_tower", 36.8703, start, 30.5304, 0,0,0, 5,0, 0, 0, 0);
                start += 0.3;
            });
        }
    }, 1500)
    
    
    alt.setTimeout(() => {
        if (isDriver)native.taskVehicleDriveToCoord(alt.Player.local.scriptID, alt.Player.local.vehicle.scriptID, 28.8791, -1391.74, 28.926, 3.0, 0, alt.Player.local.vehicle.model, 4194304, 0.2, 0)

        alt.setTimeout(() => {
            let start = -1393.4;
            for (let i = 0; i < 10; i++) {
                alt.nextTick(() => {
                    native.requestPtfxAsset("core");
                    native.useParticleFxAsset("core");
                    let id = native.startParticleFxLoopedAtCoord("ent_amb_waterfall_splash_p", 28.7341, start, 30.5304, 0,0,0, 1,0, 0, 0, 0);
                    start += 0.3;
                });
            }
            alt.setTimeout(() => {
                alt.nextTick(() => {
                    native.removeParticleFxInRange(28.7341, start, 30.5304, 6.0)
                })
            }, 4000)
        }, 1500)
        
        alt.setTimeout(() => {
            if (isDriver)native.taskVehicleDriveToCoord(alt.Player.local.scriptID, alt.Player.local.vehicle.scriptID, 20.7165, -1391.82, 28.926, 3.0, 0, alt.Player.local.vehicle.model, 4194304, 0.2, 0)

            alt.setTimeout(() => {
                let start = -1393.4;
                for (let i = 0; i < 10; i++) {
                    alt.nextTick(() => {
                        native.requestPtfxAsset("core");
                        native.useParticleFxAsset("core");
                        let id = native.startParticleFxLoopedAtCoord("ent_amb_waterfall_splash_p", 20.9802, start, 28.5304, 0,0,0, 1,0, 0, 0, 0);
                        start += 0.3;
                    });
                }
                alt.setTimeout(() => {
                    alt.nextTick(() => {
                        native.removeParticleFxInRange(20.7341, start, 28.5304, 6.0)
                    })
                }, 4000)
            }, 1500)
            alt.setTimeout(() => {
                if(isDriver)native.taskVehicleDriveToCoord(alt.Player.local.scriptID, alt.Player.local.vehicle.scriptID, 13.9385, -1391.82, 28.926, 3.0, 0, alt.Player.local.vehicle.model, 4194304, 0.2, 0)

                alt.setTimeout(() => {
                    let start = -1393.19;
                    for (let i = 0; i < 10; i++) {
                        alt.nextTick(() => {
                            native.requestPtfxAsset("core");
                            native.useParticleFxAsset("core");
                            native.startParticleFxLoopedAtCoord("ent_sht_water_tower", 13.8725, start, 30.5304, 0,0,0, 5,0, 0, 0, 0);
                            start += 0.3;
                        });
                    }
                }, 1500)
                
                
                alt.setTimeout(() => {
                    if(isDriver) {
                        native.taskVehicleDriveToCoord(alt.Player.local.scriptID, alt.Player.local.vehicle.scriptID, 6.05275, -1391.82, 28.926, 3.0, 0, alt.Player.local.vehicle.model, 4194304, 0.2, 0)
                        player.disableMovement = false;
                        alt.emitServer("FinishCarWash")
                    }
                }, 5000)
            }, 5000)
        }, 5000)
    }, 5000)
    
})






let checkPoint = undefined
let blip = undefined
alt.onServer("SetRoute", (x, y, z, name, sprite = 80, color = 46) => {
    // alt.log(JSON.stringify(blip))
    if (blip != undefined) {
        blip.destroy()
        blip = undefined
    }
    blip = new alt.PointBlip(x, y,z)
    blip.shortRange = true
    blip.sprite = sprite
    blip.color = color
    blip.name = name
    blip.route = true

    // alt.log("checkPoint = " + checkPoint)

    if (checkPoint != undefined) {
        native.deleteCheckpoint(checkPoint)
    }
    checkPoint = native.createCheckpoint(47, x, y, z, x, y, z, 2, 255, 255, 0, 255, 0)
    native.setCheckpointCylinderHeight(checkPoint, 0.5, 0.5, 10)
    // alt.log("checkPoint = " + checkPoint)

})

alt.onServer("RemoveRoute", () => {
    if (blip != undefined) {
        blip.destroy()
        blip = undefined
    }
    if (checkPoint != undefined) {
        native.deleteCheckpoint(checkPoint)
    }
})

alt.onServer("StartTraining", (scenarioName, pos, heading, teleport, duration) => {
    native.taskStartScenarioAtPosition(alt.Player.local.scriptID, scenarioName, pos.x, pos.y, pos.z, heading, duration, false, teleport)
    // alt.log("Duration:" + duration)
    player.playAnim = true;
    if (duration > 0) webview.updateView("Bar", [duration])
})



// const whitelistedMaterialHashes = [
//     {
//         "id" : 11,
//         "materialHashes" : [
//             3833216577,
//             2409420175,
//             2352068586,

//         ]
//     }
// ]


//Sonst erstmal einfach nur einbauen, ob EntityInWater ist
// alt.onServer("PlantSeed", (weedTypeId) => {
//     let startPos = alt.Player.local.pos
//     let endPos  = alt.Player.local.pos 
//     let rayHandle = native.startExpensiveSynchronousShapeTestLosProbe(startPos.x, startPos.y, startPos.z, endPos.x, endPos.y, endPos.z - 1.2, 1, alt.Player.local.scriptID, 1)
//     alt.log(rayHandle)
//     let [handle, hit, endCoords, surfaceNormal, materialHash, entityHit] = native.getShapeTestResultIncludingMaterial(rayHandle, null, null, null, null, null)
//     alt.log("MaterialHash: " + materialHash)
//     alt.log("hit : " + hit)
//     alt.log("entityHit : " + entityHit)
//     alt.log("weedTypeId: " + weedTypeId)
//     let plantable = false
//     if(whitelistedMaterialHashes.find(p => p.id == weedTypeId).materialHashes.includes(materialHash))
//     {
//         plantable = true
//     }
//     alt.log("plantable:" + plantable)
//     alt.emitServer("PlantSeedReturn", weedTypeId, plantable)
// })

/*alt.on("HackingGame:Start", StartHackingGame);
alt.on("HackingGame:Result", StopHackingGame);


 * Trigger a new hacking Game if none is running
 * @param {string} word 8 Char string that will be the Solution 
 * @param {number} lives [OPTIONAL] Number of lives. Default 3 
 * @param {number} minSpeed [OPTIONAL] minimum Rotation Speed of column - Default 10
 * @param {number} maxSpeed [OPTIONAL] maximum Rotation Speed of column. Default 100
 * @returns {boolean} false if game is already running
 
function StartHackingGame(word, lives = 3, minSpeed = 10, maxSpeed = 100) {
    if(ActualHackingGame !== null) return false;
    ActualHackingGame = new HackingGame(word, lives, minSpeed, maxSpeed);
    ActualHackingGame.Start();
    return true;
}


 Hacking Game has ended. Clear var
 
function StopHackingGame(result) {
    ActualHackingGame = null;
    alt.log("Result of Hacking Game => Successfull: "+result);
}*/


alt.onServer("hackinggame", (word, lives, minSpeed, maxSpeed, event) => {
    HackingGame.init(word, lives, minSpeed, maxSpeed, event);
    HackingGame.start()
})

alt.onServer("DailyRollStart", async (priceIndex, reward) => {

    let pos = alt.Player.local.pos
    if(reward)
    {
        let male = native.isPedMale(alt.Player.local.scriptID)
        let lib = "anim_casino_a@amb@casino@games@lucky7wheel@female"
        let anim = "enter_right_to_baseidle"
        if(male)
            lib = "anim_casino_a@amb@casino@games@lucky7wheel@male"
        // let targetPos = new alt.Vector3(1110.24, 228.949, -49.6447)
        // let targetRot = new alt.Vector3(0, 0, 34.53)

        let targetPos = new alt.Vector3(193.582, -927.19, 30.6783)
        let targetRot = new alt.Vector3(0, 0, 34.53)
        
        player.disableActions = true
        player.cantCancelAnimation = true
        await animation.playAnimAtCoordsAndWait(lib, anim, 0, targetPos, targetRot, 5000)
        await animation.playAnimAndWait(lib, "enter_to_armraisedidle", 0, 5000)
        await animation.playAnimAndWait(lib, "armraisedidle_to_spinningidle_high", 0, 5000)
    }

    let wheel = native.getClosestObjectOfType(pos.x, pos.y, pos.z, 10.0, alt.hash("vw_prop_vw_luckywheel_02a"), false, false, false)

    let speedIntCnt = 2
    let rollSpeed = 1.0
    native.setEntityRotation(wheel, 0, 0, 0, 1, true)
    let startAngle = native.getEntityRotation(wheel, 1).y
    let winAngle = ((priceIndex + 18) * 18) % 360
    let rollAngle = winAngle + (360 * 8)
	if(rollAngle <= 0)
	{
		rollAngle = rollAngle + 360
	}
	let angleToRoll = rollAngle
    let midLength = (rollAngle / 2)
    let intCnt = 0
	let rollSum = 0
    let interval = alt.setInterval(() => {
        let retval = native.getEntityRotation(wheel, 1)
        if(rollAngle > midLength)
        {
            speedIntCnt = speedIntCnt + 1
        }
        else
        {
            speedIntCnt = speedIntCnt -1
            if(speedIntCnt < 1)
            {
                speedIntCnt = 1
            }
        }
        intCnt = intCnt + 1
        rollSpeed = speedIntCnt / 10
        let y = retval.y - rollSpeed
        rollAngle = rollAngle - rollSpeed
		rollSum = rollSum + rollSpeed
        native.setEntityRotation(wheel, retval.x, y, retval.z, 1, true)
        if(Math.abs(rollSum - angleToRoll) < 5 || intCnt > 10000)
        {
            if(reward)
            {
                alt.emitServer("DailyRollFinished", priceIndex)
                player.disableActions = false
                player.cantCancelAnimation = false
            }
			alt.clearInterval(interval)
        }
    }, 10)
    
})

alt.onServer("SetBlackscreen", (state) => {
    if(state)
    {
        native.doScreenFadeOut(2000)
        //blackscreen.start()
    }
    else   
    {
        native.doScreenFadeIn(5000)
        //blackscreen.stop()
    }
})

alt.onServer("RqLoadVehTable", (id, hash, multi) => {
    if(alt.Player.local.vehicle != null && (alt.Player.local.vehicle.model == hash || alt.Player.local.vehicle.model == alt.hash(hash)))
    {
        native.modifyVehicleTopSpeed(alt.Player.local.vehicle.scriptID, multi)
        let maxSpeed = native.getVehicleEstimatedMaxSpeed(alt.Player.local.vehicle.scriptID)
        //let maxTraction = native.getVehicleMaxTraction(alt.Player.local.vehicle.scriptID)
        let acceleration = native.getVehicleAcceleration(alt.Player.local.vehicle.scriptID)
        //let maxBreaking = native.getVehicleMaxBraking(alt.Player.local.vehicle.scriptID)
        alt.log("getVehicleEstimatedMaxSpeed: " + maxSpeed)
        alt.emitServer("RsLoadVehTable", id, maxSpeed, acceleration)

        //alt.emitServer("RsLoadVehTable", id, hash, maxSpeed, maxTraction, acceleration, maxBreaking)
    }
})

let vehDataInterval = null
alt.onServer("vehdata", () => {
    if(vehDataInterval != null) 
    {
        webview.updateView("ToggleVehData", [false]);
        alt.clearInterval(vehDataInterval)
    }
    else
    {
        if(alt.Player.local.vehicle != null)
        {
            webview.updateView("ToggleVehData", [true]);
            vehDataInterval = alt.setInterval(() => {
                if(alt.Player.local.vehicle == null) {
                    alt.clearInterval(vehDataInterval)
                    webview.updateView("ToggleVehData", [false]);
                }
                else
                {
                    webview.updateView("UpdateSpeed", [Math.floor(alt.Player.local.vehicle.speed * 3.6)]);
                }
            }, 1)
        }
    }
})

alt.onServer("RqCreateTreasure", (hash) => {
    let pos = alt.Player.local.pos
    let obj = native.createObject(alt.hash(hash), pos.x, pos.y, pos.z, false, false, true);
    native.placeObjectOnGroundProperly(obj)
    let pos_found = native.getEntityCoords(obj, true)
    let rot_found = native.getEntityRotation(obj, 2)
    alt.log(JSON.stringify(pos_found))
    alt.log(JSON.stringify(rot_found))
    
    native.deleteObject(obj)
    alt.emitServer("RsCreateTreasure", hash, pos_found, rot_found)
})

alt.onServer("Spectate", target => {
    let cleared = false
    player.isSpectating = true
    const interval = alt.setInterval(() => {
        if (target.scriptID) {
            player.setInvincible(true)
            native.setEntityVisible(alt.Player.local.scriptID, false, false)
            native.attachEntityToEntity(alt.Player.local.scriptID, target.scriptID, 0, 0, 0, 0, 0, 0, 0, true, false, true, false, 2, true)
            native.setEntityCollision(alt.Player.local.scriptID, false, false)
            player.disableMovement = true
            alt.clearInterval(interval)
            cleared = true
        }
    }, 10)

    alt.setTimeout(() => {
        if (!cleared) {
            alt.clearInterval(interval)
        }
    }, 5000)
})

alt.onServer("StopSpec", pos => {
    native.setEntityCollision(alt.Player.local.scriptID, true, true)
    native.detachEntity(alt.Player.local.scriptID, true, true)
    native.setEntityCoords(alt.Player.local.scriptID, pos.x, pos.y, pos.z, false, false, false, false)
    if(!player.isInAdminDuty)
    {
        player.setInvincible(false)
    }
    native.setEntityVisible(alt.Player.local.scriptID, true, true)
    player.disableMovement = false
    player.isSpectating = false
})

alt.onServer("SetStabilized", () => {
    player.isStabilized = true;
})

alt.onServer("testmoses", (entity) => {
    alt.log("TestMoses Event with entity: " + entity)
    for (const player of alt.Player.all) {
        if (player.scriptID == entity.id) {
            alt.log("Player found!")
            native.taskShootAtEntity(alt.Player.local.scriptID, player, 5000, alt.hash("FIRING_PATTERN_FULL_AUTO"))
        }
    }
})

alt.onServer("LootVitrine", async (pos, heading) => {
    let rot = new alt.Vector3(0, 0, heading)
    //let soundId = 1
    //native.playSoundFromCoord(soundId, "Glass_Smash", pos.x, pos.y, pos.z, "FBI_HEIST_RAID", false, 20, true)
    let soundId = native.getSoundId()
    native.playSoundFromEntity(soundId, "Glass_Smash", alt.Player.local.scriptID, "", true, 0);

    await animation.playAnimAtCoordsAndWait("missheist_jewel", "smash_case", 0, pos, rot, 5000)
    native.stopSound(soundId);
    native.releaseSoundId(soundId);
    //[12:34:40] LootVitrineEvent: 3, {"x":-626.8250122070312,"y":-235.3470001220703,"z":38.05699920654297}, 33.744998931884766
    //await animation.playAnimAtCoordsAndWait(lib, anim, 0, targetPos, targetRot, 5000)
})


alt.onServer("Invincible", (duration) => {
    player.setInvincible(true)
    alt.setTimeout(() => {
        player.setInvincible(false)
    }, duration);
})

const whitelistedMaterialHashes = JSON.parse(alt.File.read("@SibauiRP_Assets/data/material.json"))

alt.onServer("PlantCheck", (itemId, type) => {
    let startPos = alt.Player.local.pos
    let endPos  = alt.Player.local.pos 
    let rayHandle = native.startExpensiveSynchronousShapeTestLosProbe(startPos.x, startPos.y, startPos.z, endPos.x, endPos.y, endPos.z - 1.2, 1, alt.Player.local.scriptID, 1)
    let [handle, hit, endCoords, surfaceNormal, materialHash, entityHit] = native.getShapeTestResultIncludingMaterial(rayHandle, null, null, null, null, null)
    let plantable = false
    if(whitelistedMaterialHashes.includes(materialHash.toString()))
    {
        plantable = true
    }
    let entityHeight = native.getEntityHeightAboveGround(alt.Player.local.scriptID)
    if(entityHeight > 2 || entityHeight < -2)
        plantable = false

    if(native.isEntityInAir(alt.Player.local.scriptID) || native.isPedClimbing(alt.Player.local.scriptID) || native.isPedSwimming(alt.Player.local.scriptID) || native.isPedSwimmingUnderWater(alt.Player.local.scriptID))
        plantable = false
    alt.emitServer("RsPlantCheck", itemId, type, plantable)
})


let matEveryTick = null;
alt.onServer("ShowMaterial", (materialList) => {
    alt.clearEveryTick(matEveryTick);
    if(materialList.length == 0)
    {
        matEveryTick = null
    }
    else
    {
        matEveryTick = alt.everyTick(() => {
            let startPos = alt.Player.local.pos
            
            for(var i = 0; i < 17; i ++)
            {
                let rad = (360 / 16) * i * Math.PI / 180
                let newStartPosX = startPos.x
                let newStartPosY = startPos.y
                
                if(i < 16)
                {
                    newStartPosX = startPos.x + 3 * Math.sin(rad)
                    newStartPosY = startPos.y + 3 * Math.cos(rad)
                }

                let rayHandle = native.startExpensiveSynchronousShapeTestLosProbe(newStartPosX, newStartPosY, startPos.z + 8.0, newStartPosX, newStartPosY, startPos.z - 2.0, 511, alt.Player.local.scriptID, 1)
                let [handle, hit, endCoords, surfaceNormal, materialHash, entityHit] = native.getShapeTestResultIncludingMaterial(rayHandle, null, null, null, null, null)
                
                if(materialHash != 0)
                {
                    native.setDrawOrigin(endCoords.x, endCoords.y, endCoords.z, 0);
                    native.beginTextCommandDisplayText( 'STRING' );
                    native.addTextComponentSubstringPlayerName( "Material: " + materialHash);
                    native.setTextFont( 4 );
                    var size = 0.7 / 2.3;
                    native.setTextScale( 1, size );
                    native.setTextWrap( 0.0, 1.0 );
                    native.setTextCentre( true );
                    native.setTextProportional( true );
                    if(materialList.includes(materialHash.toString()))
                    {
                        native.setTextColour( 0, 255, 0, 255 );
                    }
                    else
                    {
                        native.setTextColour( 255, 0, 0, 255 );
                    }
                    native.setTextOutline();
                    native.setTextDropShadow();

                    native.endTextCommandDisplayText( 0, 0, 0 );
                    native.clearDrawOrigin();
                    
                    if(materialList.includes(materialHash.toString()))
                    {
                        native.drawMarker(
                            25, endCoords.x, endCoords.y, endCoords.z + 0.1,
                            0, 0, 0,
                            0, 0, 0,
                            1, 1, 1,
                            0, 255, 0, 255,
                            !!false, !!false, 2, !!false, null, null, !!false
                        );
                    }
                    else
                    {
                        native.drawMarker(
                            25, endCoords.x, endCoords.y, endCoords.z + 0.1,
                            0, 0, 0,
                            0, 0, 0,
                            1, 1, 1,
                            255, 0, 0, 255,
                            !!false, !!false, 2, !!false, null, null, !!false
                        );
                    }
                }
            }
        })
    }
})


alt.onServer("RqLocateSmartphone", () => {
    let pos = alt.Player.local.pos
    let zoneId = native.getNameOfZone(pos.x, pos.y, pos.z)
    //let signalStrength = native.getZoneScumminess(zoneId)
    alt.emitServer("RsLocateSmartphone", zoneId)
});


var deliverMarkerTick = null;
var deliverBlips = []

alt.onServer("DeliverMarker", (dataList) => {
    if(deliverMarkerTick != null)
    {
        alt.clearEveryTick(deliverMarkerTick);
        deliverMarkerTick = null;
    }
    for(const obj of deliverBlips) {
        obj.destroy()
    }
    deliverBlips = []
    deliverMarkerTick = alt.everyTick(() => {
        for (const obj of dataList.data) {
            native.drawMarker(
                obj.t, obj.x, obj.y, obj.z -1.0,
                0, 0, 0,
                0, 0, 0,
                obj.sx, obj.sy, obj.sz,
                obj.r, obj.g, obj.b, obj.a,
                !!false, !!false, 2, !!false, null, null, !!false
            );
        }
        
    });

    for (const obj of dataList.data) {
        const blip = new alt.PointBlip(obj.x, obj.y,obj.z)
        blip.sprite = 1
        blip.color = 46
        blip.name = "Entladepunkt"
        blip.shortRange = true
        deliverBlips.push(blip)
    }
        
    webview.updateView("AddNotify", ["Ausladepunkte in der Nähe werden jetzt angezeigt!", "#0db600"])

    alt.setTimeout(() => {
        
        for(const obj of deliverBlips) {
            obj.destroy()
        }
        deliverBlips = []
        alt.clearEveryTick(deliverMarkerTick);
        deliverMarkerTick = null;
        webview.updateView("AddNotify", ["Ausladepunkte in der Nähe werden nicht mehr angezeigt!", "#c72020"])
    }, dataList.d)
})

const atmOffsets = [
    {key: 3424098598 /* prop_atm_01 */, x: 0.0040, y: -0.8275, z: 0}, 
    {key: 3168729781 /* prop_atm_02 */, x: 0.0040, y: -0.4809, z: 0}, 
    {key: 2930269768 /* prop_atm_03 */, x: 0.0198, y: -0.4723, z: 0}, 
    {key: 506770882 /* prop_fleeca_atm */, x: 0.0427, y: -0.4621, z: 0}
];

alt.onServer("SlideCard", () => {
    const localPlayer = alt.Player.local;
    
    for (let i = 0; i <= atmOffsets.length; i++) {
        let element = atmOffsets[i];
        let object = native.getClosestObjectOfType(localPlayer.pos.x, localPlayer.pos.y, localPlayer.pos.z, 3, element.key, false, false, false)

        if (object === 0) {
            continue;
        }

        let offsetPos = native.getOffsetFromEntityInWorldCoords(object, element.x, element.y, element.z);

        if (native.getInteriorFromEntity(localPlayer) === 0) {
            native.taskGoToCoordAnyMeans(localPlayer, offsetPos.x, offsetPos.y, offsetPos.z, 1, 0, 0, 786603, 0);
        } else {
            native.setEntityCoordsNoOffset(localPlayer, offsetPos.x, offsetPos.y, localPlayer.pos.z, 0, 0, native.getEntityHeading(object));
        }

        let count = 0;

        let interval = alt.setInterval(() => {
            if (native.getIsTaskActive(localPlayer, 224) === false) {
                alt.clearInterval(interval);
                native.taskTurnPedToFaceEntity(localPlayer, object, 1000);
                if (!player.isInAtm) return;
                alt.setTimeout(() => {
                    animation.playAnim("amb@prop_human_atm@male@enter", "enter", 0);
                    alt.setTimeout(() => {
                        if (!player.isInAtm) return;
                        animation.playAnim("amb@prop_human_atm@male@idle_a", "idle_a", 1);
                    }, 4000);
                }, 1005);
            }

            if (count > 1000) {
                alt.clearInterval(interval);
                return;
            }
            count++;
        }, 20);
        return;
    }
});

let hackingInterval;

alt.onServer("StartHacking", () => {
    player.isInHacking = true;
    animation.playAnim("anim@gangops@facility@servers@", "hotwire", 17);

    hackingInterval = alt.setInterval(() => {
        if (alt.isKeyDown(69)) {
            alt.clearInterval(hackingInterval);
            alt.emitServer("CancelHacking");
            native.clearPedTasks(alt.Player.local.scriptID);
            player.isInHacking = false;
        }
    }, 50);
});

alt.onServer("StopHacking", () => {
    alt.clearInterval(hackingInterval);
    native.clearPedTasks(alt.Player.local.scriptID);
    player.isInHacking = false;
});
