import alt from "alt"
import native from "natives"
import webview from "/api/modules/webview.mjs"
import voice from "/api/modules/voice.mjs"
import player from "/api/modules/player.mjs"
import propsync from "/api/modules/propsync.mjs"
import vehicle from "/api/modules/vehicle.mjs"

const metaChangeHandlers = {}
const onMetaChange = (key, handler) => {
    metaChangeHandlers[key] = handler
}

const trailerModels = new Set([
    0xCBB2BE0E, //trailers
    0xA1DA3C91, //trailers2
    0x8548036D, //trailers3
    0xBE66F5AA, //trailers4
])

alt.on("gameEntityCreate", entity => {
     //alt.log("Type: " + entity.type)
     if (entity.type == 1 ) {
         //const speed = entity.getStreamSyncedMeta("speed")
         //if (speed != undefined) native.modifyVehicleTopSpeed(entity.scriptID, speed)
         if (entity.getStreamSyncedMeta("shopCar")) {
            native.setEntityInvincible(entity.scriptID, true)
            alt.setTimeout(() => {
                native.freezeEntityPosition(entity.scriptID, true)
            }, 2000)
         }
         if (entity.getStreamSyncedMeta("trunk")) {
             vehicle.openTrunk(entity, true)
            }
         if (entity.getStreamSyncedMeta("mutedSiren")) native.setVehicleHasMutedSirens(entity.scriptID, true)
         if (trailerModels.has(entity.model))
         {
            native.setEntityInvincible(entity.scriptID, true)
         }
      } else if (entity.type == 0) {
        voice.OnStreamIn(entity)
        //   if (entity.getStreamSyncedMeta("dead")) voice.OnPlayerDied(entity.id)
        //   const range = entity.getStreamSyncedMeta("range")
        //   voice.OnUpdateVoiceClient(entity.id, range ? range : 8)
      }
})

alt.on("gameEntityDestroy", entity => {
    if (entity.type == 0) {
        voice.OnStreamOut(entity)
        propsync.stop(entity)
    }
})

alt.on("streamSyncedMetaChange", (entity, key, value, oldValue) => {
     //alt.log("Change: " + key + " " + value)
     if (metaChangeHandlers[key] != undefined) {
        metaChangeHandlers[key](entity, value, oldValue)
     }
})

onMetaChange("trunk", (entity, value, oldValue) => {
    if (value) {
        vehicle.openTrunk(entity, false)
    }
    else {
        vehicle.closeTrunk(entity, false)
    }
    if (alt.Player.local.vehicle == null || alt.Player.local.vehicle.scriptID != entity.scriptID) return
    webview.updateView("UpdateVehHUD", ["trunkOpen", value])
})


onMetaChange("slotMachine", (entity, value, oldValue) => {
    const slotMachines = JSON.parse(alt.File.read("@SibauiRP_Assets/data/slotmachines.json"))
    if (value) {
        let slotMachine = slotMachines.find(d => d.id == value)
        let testObj = native.getClosestObjectOfType(slotMachine.x, slotMachine.y, slotMachine.z, 0.5, slotMachine.hash, false, false, false)
        native.setEntityNoCollisionEntity(entity.scriptID, testObj, false);
    }
})

onMetaChange("propSync", async (entity, value, oldValue) => {
    if (value) {
        if (oldValue != undefined && oldValue > 0) {
            propsync.stop(entity)
            await propsync.start(entity, value-1)
            return;
        }
        await propsync.start(entity, value-1)
    } else {
        propsync.stop(entity)
    }
})

onMetaChange("dead", (entity, value, oldValue) => {
    if (value) voice.OnPlayerDied(entity.id)
    else voice.OnPlayerRevived(entity.id)
})

onMetaChange("range", (entity, value, oldValue) => {
    voice.OnUpdateVoiceClient(entity.id, value)
})

onMetaChange("mutedSiren", (entity, value, oldValue) => {
    if (value) native.setVehicleHasMutedSirens(entity.scriptID, true)
    else native.setVehicleHasMutedSirens(entity.scriptID, false)
})

onMetaChange("invisible", (entity, value, oldValue) => {
    const interval = alt.setInterval(()  => {
        const playerScriptId = entity.scriptID
        if (playerScriptId) {

            if (value) 
            {
                //alt.log("Make Ped invisible")
                native.setEntityCollision(playerScriptId, false, false)
                native.setEntityAlpha(playerScriptId, 0, false)
            }
            else {
                //alt.log("Make Ped visible")
                native.setEntityCollision(playerScriptId, true, true)
                native.setEntityAlpha(playerScriptId, 255, false)
            }
            alt.clearInterval(interval)
            return
        }
    }, 10)
    
})

alt.on("consoleCommandD", (name, ...args) => {
    //get all vehicles in streaming range
})

let cam = null
let matEveryTick = null
let matEveryTickCounter = 0

alt.on("consoleCommandD", (name, ...args) => {
    alt.log("COMMAND " + name + " " + JSON.stringify(args))
    if (name == "combat") {
        alt.log("combat " + alt.Player.local.scriptID)
        native.setPedUsingActionMode(alt.Player.local.scriptID, false, -1, "DEFAULT_ACTION")
    }
    else if (name == "hash") {
        if(alt.Player.local.vehicle != null) {
            alt.log(alt.Player.local.vehicle.model)
        }
    } else if (name == "test") {
        // alt.log("Test " + alt.Player.local.scriptID)
        // native.setPedConfigFlag(alt.Player.local.scriptID, 184, true)
        // native.setPedSuffersCriticalHits(alt.Player.local.scriptID, false)
        // native.setBlockingOfNonTemporaryEvents(alt.Player.local.scriptID, true)
        // native.taskSetBlockingOfNonTemporaryEvents(alt.Player.local.scriptID, true)
        // native.setPedFleeAttributes(alt.Player.local.scriptID, 0, false)
        // native.setPedConfigFlag(alt.Player.local.scriptID, 281, true)
    }

    else if (name == "cam") {
        const pos = alt.Player.local.pos
        const rot = alt.Player.local.rot
        const test = native.getOffsetFromEntityInWorldCoords(alt.Player.local.scriptID, parseFloat(args[0]), parseFloat(args[1]), parseFloat(args[2]))
        cam = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", test.x, test.y, test.z, rot.x, rot.y, rot.z, parseFloat(args[3]), true, 0)
        //cam 0 3 0 45 0 0 0
        // native.setCamRot(cam, parseFloat(args[4]), parseFloat(args[5]), cam.z, 2)
        // native.setCamRot(cam, rot.x, rot.y, cam.z, 2)
        //native.pointCamAtEntity(cam, alt.Player.local.scriptID, parseFloat(args[4]), parseFloat(args[5]), parseFloat(args[6]), 1)
        // cam 1 3 0.5 45 1 0 0
        const coord = native.getOffsetFromEntityInWorldCoords(alt.Player.local.scriptID, parseFloat(args[4]), parseFloat(args[5]), parseFloat(args[6]))
        native.pointCamAtCoord(cam, coord.x, coord.y, coord.z)
        // const cords = native.getCamCoord(cam)
        // native.setCamCoord(cords.x + parseFloat(args[4]), cords.y + parseFloat(args[5]), cords.z)
        native.renderScriptCams(true, false, 0, true, false, 0)
    }

    else if (name == "scam") {
        native.setCamActive(cam, false)
        native.destroyCam(cam, false)
        native.renderScriptCams(false, false, 0, true, false, 0)
    }

    else if (name == "f") {
        native.freezeEntityPosition(alt.Player.local.scriptID, parseInt(args[0]))
    }

    else if (name == "rot") {
        native.setEntityRotation(alt.Player.local.scriptID, parseFloat(args[0]), parseFloat(args[1]), parseFloat(args[2]), 2, 1)
    }

    else if (name == "h") {
        native.setEntityHeading(alt.Player.local.scriptID, parseFloat(args[0]))
    }

    else if (name == "stask") {
        native.clearPedSecondaryTask(alt.Player.local.scriptID)
    }

    else if (name == "record") {
        native.startRecording(1)
    }

    else if (name == "stop") {
        native.stopRecordingAndSaveClip()
    }
    else if (name == "time") {
        let hour = native.getClockHours();
        let minute = native.getClockMinutes();
        let second = native.getClockSeconds();
        alt.log("Uhrzeit: " + hour + ":" + minute + ":" + second)
        //Uhrzeit: 4:33:10
    }
    else if (name == "hp") {
        native.setEntityHealth(alt.Player.local.scriptID, 200, 0)
    }
    else if (name == "armor") {
        native.setPedArmour(alt.Player.local.scriptID, 100)
    }
    else if (name == "god") {
        player.setInvincible(true)
    }
    else if (name == "pos") {
        native.freezeEntityPosition(alt.Player.local.scriptID, true);
        let currentPos = alt.Player.local.pos
        native.setEntityCoords(alt.Player.local.scriptID, currentPos.x + parseFloat(args[0]), currentPos.y + parseFloat(args[1]), currentPos.z + parseFloat(args[2]), false, false, false, false)
        alt.log("Position: " + JSON.stringify(native.getEntityCoords(alt.Player.local.scriptID, true)) + "heading: " + native.getEntityHeading(alt.Player.local.scriptID))
    }
    else if (name == "heading") {
        native.freezeEntityPosition(alt.Player.local.scriptID, true);
        let currentHeading = native.getEntityHeading(alt.Player.local.scriptID)
        native.setEntityHeading(alt.Player.local.scriptID, currentHeading + parseFloat(args[0]))
        alt.log("Position: " + JSON.stringify(native.getEntityCoords(alt.Player.local.scriptID, true)) + "heading: " + native.getEntityHeading(alt.Player.local.scriptID))
    }
    else if (name == "blips") {
        var startX = -5000
        var startY = -1000
        var actX = startX
        var actY = startY

        for(var i = 1; i <= 669; i++)
        {
            const blip = new alt.PointBlip(actX, actY, 0)
            blip.sprite = i
            blip.color = 4
            blip.name = "ID: " + i
            blip.shortRange = true
            actX += 60
            if(i % 50 == 0)
            {
                actY -= 80
                actX = startX
            }
        }

        actY -= 240
        actX = startX

        for(var j = 1; j <= 85; j++)
        {
            const blip = new alt.PointBlip(actX, actY, 0)
            blip.sprite = 1
            blip.color = j
            blip.name = "Color: " + j
            blip.shortRange = true
            actX += 60
            if(j % 10 == 0)
            {
                actY -= 80
                actX = startX
            }
        }
    }
    else if (name == "t") {
        native.addPedDecorationFromHashes(alt.Player.local.scriptID, alt.hash(args[0]), alt.hash(args[1]))
    }
    else if (name == "ct") {
        native.clearPedDecorations(alt.Player.local.scriptID)
    }
    else if (name == "mat") {
        alt.log("mat command")
        if(matEveryTick != null)
        {
            alt.log("stopped mat")
            alt.clearEveryTick(matEveryTick);
            matEveryTick = null
        }
        else
        {
            alt.log("started mat")
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
                    
                    matEveryTickCounter++
                    if(matEveryTickCounter == 100)
                    {
                        alt.log("hit: " + hit +  ", materialHash: " + materialHash + ", entityHit: " + entityHit)
                        matEveryTickCounter = 0
                    }
                    
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
    
                        native.setTextColour( 255, 255, 0, 255 );
                        native.setTextOutline();
                        native.setTextDropShadow();
    
                        native.endTextCommandDisplayText( 0, 0, 0 );
                        native.clearDrawOrigin();
                        native.drawMarker(
                            25, endCoords.x, endCoords.y, endCoords.z + 0.1,
                            0, 0, 0,
                            0, 0, 0,
                            1, 1, 1,
                            255, 255, 0, 255,
                            !!false, !!false, 2, !!false, null, null, !!false
                        );
                    }
                }
            })
        }
        /*let startPos = alt.Player.local.pos
        let endPos  = alt.Player.local.pos 
        let rayHandle = native.startExpensiveSynchronousShapeTestLosProbe(startPos.x, startPos.y, startPos.z, endPos.x, endPos.y, endPos.z - 1.2, 1, alt.Player.local.scriptID, 1)
        alt.log(rayHandle)
        let [handle, hit, endCoords, surfaceNormal, materialHash, entityHit] = native.getShapeTestResultIncludingMaterial(rayHandle, null, null, null, null, null)
        alt.log("MaterialHash: " + materialHash)
        alt.log("hit : " + hit)
        alt.log("entityHit : " + entityHit)*/
    }
    else if (name == "showweapon")
    {
        const weaponNames = [
            "WEAPON_REVOLVER", "WEAPON_PISTOL",
            "WEAPON_PISTOL_MK2","WEAPON_COMBATPISTOL","WEAPON_APPISTOL",
            "WEAPON_PISTOL50","WEAPON_SNSPISTOL","WEAPON_HEAVYPISTOL",
            "WEAPON_VINTAGEPISTOL","WEAPON_STUNGUN","WEAPON_FLAREGUN",
            "WEAPON_MARKSMANPISTOL","WEAPON_MICROSMG","WEAPON_MINISMG",
            "WEAPON_SMG","WEAPON_SMG_MK2","WEAPON_ASSAULTSMG","WEAPON_MG",
            "WEAPON_COMBATMG","WEAPON_COMBATMG_MK2","WEAPON_COMBATPDW",
            "WEAPON_GUSENBERG","WEAPON_MACHINEPISTOL","WEAPON_ASSAULTRIFLE",
            "WEAPON_ASSAULTRIFLE_MK2","WEAPON_CARBINERIFLE","WEAPON_CARBINERIFLE_MK2",
            "WEAPON_ADVANCEDRIFLE","WEAPON_SPECIALCARBINE","WEAPON_BULLPUPRIFLE",
            "WEAPON_COMPACTRIFLE","WEAPON_PUMPSHOTGUN","WEAPON_SWEEPERSHOTGUN",
            "WEAPON_SAWNOFFSHOTGUN","WEAPON_BULLPUPSHOTGUN","WEAPON_ASSAULTSHOTGUN",
            "WEAPON_MUSKET","WEAPON_HEAVYSHOTGUN","WEAPON_DBSHOTGUN","WEAPON_SNIPERRIFLE",
            "WEAPON_HEAVYSNIPER","WEAPON_HEAVYSNIPER_MK2","WEAPON_MARKSMANRIFLE",
            "WEAPON_MINIGUN", "WEAPON_SNSPISTOL_MK2", "WEAPON_REVOLVER_MK2","WEAPON_DOUBLEACTION","WEAPON_SPECIALCARBINE_MK2",
            "WEAPON_BULLPUPRIFLE_MK2","WEAPON_PUMPSHOTGUN_MK2","WEAPON_MARKSMANRIFLE_MK2",
            "WEAPON_RAYPISTOL","WEAPON_RAYCARBINE","WEAPON_RAYMINIGUN","WEAPON_NAVYREVOLVER",
            "WEAPON_CERAMICPISTOL"
            ]
            weaponNames.forEach(hash => alt.log(hash + " : " + native.getWeaponDamage(alt.hash(hash), null)));
    }
    else if (name == "kill") {
        native.setPedStealthMovement(alt.Player.local.scriptID, false, 0)
    }
    else if (name == "wep") {
        const weaponNames = [
            "WEAPON_REVOLVER", "WEAPON_PISTOL",
            "WEAPON_PISTOL_MK2","WEAPON_COMBATPISTOL","WEAPON_APPISTOL",
            "WEAPON_PISTOL50","WEAPON_SNSPISTOL","WEAPON_HEAVYPISTOL",
            "WEAPON_VINTAGEPISTOL","WEAPON_STUNGUN","WEAPON_FLAREGUN",
            "WEAPON_MARKSMANPISTOL","WEAPON_MICROSMG","WEAPON_MINISMG",
            "WEAPON_SMG","WEAPON_SMG_MK2","WEAPON_ASSAULTSMG","WEAPON_MG",
            "WEAPON_COMBATMG","WEAPON_COMBATMG_MK2","WEAPON_COMBATPDW",
            "WEAPON_GUSENBERG","WEAPON_MACHINEPISTOL","WEAPON_ASSAULTRIFLE",
            "WEAPON_ASSAULTRIFLE_MK2","WEAPON_CARBINERIFLE","WEAPON_CARBINERIFLE_MK2",
            "WEAPON_ADVANCEDRIFLE","WEAPON_SPECIALCARBINE","WEAPON_BULLPUPRIFLE",
            "WEAPON_COMPACTRIFLE","WEAPON_PUMPSHOTGUN","WEAPON_SWEEPERSHOTGUN",
            "WEAPON_SAWNOFFSHOTGUN","WEAPON_BULLPUPSHOTGUN","WEAPON_ASSAULTSHOTGUN",
            "WEAPON_MUSKET","WEAPON_HEAVYSHOTGUN","WEAPON_DBSHOTGUN","WEAPON_SNIPERRIFLE",
            "WEAPON_HEAVYSNIPER","WEAPON_HEAVYSNIPER_MK2","WEAPON_MARKSMANRIFLE",
            "WEAPON_MINIGUN", "WEAPON_SNSPISTOL_MK2", "WEAPON_REVOLVER_MK2","WEAPON_DOUBLEACTION","WEAPON_SPECIALCARBINE_MK2",
            "WEAPON_BULLPUPRIFLE_MK2","WEAPON_PUMPSHOTGUN_MK2","WEAPON_MARKSMANRIFLE_MK2",
            "WEAPON_RAYPISTOL","WEAPON_RAYCARBINE","WEAPON_RAYMINIGUN","WEAPON_NAVYREVOLVER",
            "WEAPON_CERAMICPISTOL"
            ]
            weaponNames.forEach(hash => native.setCurrentPedWeapon(alt.Player.local.scriptID, alt.hash(hash), false));
    } else if (name === "offset") {
        let localPlayer = alt.Player.local;
        let testObj = native.getClosestObjectOfType(localPlayer.pos.x, localPlayer.pos.y, localPlayer.pos.z, 3, alt.hash(args[0]), false, false, false)
        let offsetPos = native.getOffsetFromEntityGivenWorldCoords(testObj, localPlayer.pos.x, localPlayer.pos.y, localPlayer.pos.z);
        
        alt.log(offsetPos);
        alt.log(testObj);
    }
})

const melee = [
    0x92A27487,
    0x958A4A8F,
    0xF9E6AA4B,
    0x84BD7BFD,
    0x8BB05FD7,
    0x440E4788,
    0x4E875F73,
    0xF9DCBF2D,
    0xD8DF3C3C,
    0x99B507EA,
    0xDD5DF8D9,
    0xDFE37640,
    0x678B81B1,
    0x19044EE0,
    0xCD274149,
    0x94117305,
    0x3813FC08
]

alt.on("connectionComplete", _ => {
    //alt.log("Completed " + alt.Player.local.scriptID)
    // Skyline amina zick zack
    // native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", 93.077, -57.00, 332.78, -14.48, -3.30, 172.61, 51.799, true, 0)
    
    //Unter Wasser Aquarium von Jefferson
    native.setEntityCoords(alt.Player.local.scriptID, 3857.06, 3713.28, -23.15, 0,0,0, 1)
    native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", 3857.06, 3713.28, -23.15, 2.31, 5.34, 155.55, 90.79, true, 0)

    // native.setWeatherTypeNowPersist("XMAS");
    // native.setEntityCoords(alt.Player.local.scriptID, 141.69, -1037.93, 60.75, 0,0,0, 1)
    // native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", 141.69, -1037.93, 60.75, -15.44, -4.42, -30.16, 40, true, 0)
    
    
    native.setEntityVisible(alt.Player.local.scriptID, false, false)
    player.setInvincible(true)
    native.renderScriptCams(true, false, 0, true, false, 0)

    //Bringt nichts, weil muss in jedem Frame gecallt werden.
    native.setWeaponDamageModifierThisFrame(0xA2719263, 0.3)
    for (const hash of melee) {
        native.setWeaponDamageModifierThisFrame(hash, 0.2)
    }


    //Load blips
    // const blips = JSON.parse(alt.File.read("@SibauiRP_Assets/data/blips.json"))
    // for (const blipData of blips) {
    //     const blip = new alt.PointBlip(blipData.position.X, blipData.position.Y, blipData.position.Z)
    //     blip.sprite = blipData.sprite
    //     blip.color = blipData.color
    //     blip.name = blipData.name
    //     blip.shortRange = true

    //     player.blips.push(blip)
    // }

    const ipls = JSON.parse(alt.File.read("@SibauiRP_Assets/data/ipls.json"))
    for (const ipl of ipls) {
        if(ipl.active == 1)
        {
            alt.requestIpl(ipl.name)
        }
        else if (ipl.active == 0)
        {
            alt.removeIpl(ipl.name)
        }
    }



    //const blip = new alt.AreaBlip(0, 0 ,0 , 100, 1)
    // blip.color = 1
    // blip.alpha = 100
    // blip.heading = heading
    // blip.shortRange = true

    let interiorID = native.getInteriorAtCoords(2730.0, -380.0, -49.0, "ch_DLC_Arcade");

    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_ceiling_beams');
    native.activateInteriorEntitySet(interiorID, 'entity_set_big_screen');
    native.activateInteriorEntitySet(interiorID, 'entity_set_floor_option_01');
    native.activateInteriorEntitySet(interiorID, 'entity_set_constant_geometry');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_brawler');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_cabs');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_claw');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_gunner');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_king');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_love');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_monkey');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_patriot');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_racer');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_retro');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_strife');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_trophy_teller');
    native.activateInteriorEntitySet(interiorID, 'entity_set_arcade_set_streetx4');
    native.activateInteriorEntitySet(interiorID, 'entity_set_mural_neon_option_02');
    native.activateInteriorEntitySet(interiorID, 'entity_set_mural_option_06');
    native.activateInteriorEntitySet(interiorID, 'entity_set_floor_option_06');
    native.activateInteriorEntitySet(interiorID, 'entity_set_screens');
    native.refreshInterior(interiorID);



    const gabzmrpd = native.getInteriorAtCoords(451.0129, -993.3741, 29.1718);
    native.pinInteriorInMemory(gabzmrpd);
    if (native.isValidInterior(gabzmrpd)) {
        const data = [
        { name: "branded_style_set" },
        { name: "v_gabz_mrpd_rm1" },
        { name: "v_gabz_mrpd_rm2" },
        { name: "v_gabz_mrpd_rm3" },
        { name: "v_gabz_mrpd_rm4" },
        { name: "v_gabz_mrpd_rm5" },
        { name: "v_gabz_mrpd_rm6" },
        { name: "v_gabz_mrpd_rm7" },
        { name: "v_gabz_mrpd_rm8" },
        { name: "v_gabz_mrpd_rm9" },
        { name: "v_gabz_mrpd_rm10" },
        { name: "v_gabz_mrpd_rm11" },
        { name: "v_gabz_mrpd_rm12" },
        { name: "v_gabz_mrpd_rm13" },
        { name: "v_gabz_mrpd_rm14" },
        { name: "v_gabz_mrpd_rm15" },
        { name: "v_gabz_mrpd_rm16" },
        { name: "v_gabz_mrpd_rm17" },
        { name: "v_gabz_mrpd_rm18" },
        { name: "v_gabz_mrpd_rm19" },
        { name: "v_gabz_mrpd_rm20" },
        { name: "v_gabz_mrpd_rm21" },
        { name: "v_gabz_mrpd_rm22" },
        { name: "v_gabz_mrpd_rm23" },
        { name: "v_gabz_mrpd_rm24" },
        { name: "v_gabz_mrpd_rm25" },
        { name: "v_gabz_mrpd_rm26" },
        { name: "v_gabz_mrpd_rm27" },
        { name: "v_gabz_mrpd_rm28" },
        { name: "v_gabz_mrpd_rm29" },
        { name: "v_gabz_mrpd_rm30" },
        { name: "v_gabz_mrpd_rm31" },
        ];
        data.forEach((interior) => {
            if (!native.isInteriorEntitySetActive(gabzmrpd, interior.name)) {
            native.activateInteriorEntitySet(gabzmrpd, interior.name);
            if (interior.color) {
                native.setInteriorEntitySetColor(gabzmrpd, interior.name, interior.color);
            }
            }
        })
        native.refreshInterior(gabzmrpd)
    }

    const facility = native.getInteriorAtCoords(2514.6333007813,-422.57379150391,94.581428527832)
    native.activateInteriorEntitySet(facility, "fbi_logo")
    native.activateInteriorEntitySet(facility, "2fbi_logo")
    const facility2 = native.getInteriorAtCoords(2502.8713378906,-348.71630859375,94.09211730957)
    native.activateInteriorEntitySet(facility2, "fbi_logo")
    native.activateInteriorEntitySet(facility2, "2fbi_logo")

    
    alt.removeIpl('rc12b_destroyed');
    alt.removeIpl('rc12b_default');
    alt.removeIpl('rc12b_hospitalinterior_lod');
    alt.removeIpl('rc12b_hospitalinterior');
    alt.removeIpl('rc12b_fixed');

    let gabzpillbox= native.getInteriorAtCoords(311.2546, -592.4204, 42.32737);
    native.pinInteriorInMemory(gabzpillbox);
    native.refreshInterior(gabzpillbox);
})