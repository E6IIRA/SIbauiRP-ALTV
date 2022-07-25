import alt from "alt"
import native from "natives"
import webview from "/api/modules/webview.mjs"
import cursor from "/api/modules/cursor.mjs"
import xmenu from "/api/modules/xmenu.mjs"
import enter from "/api/modules/enter.mjs"
import interfaces from "/api/modules/interfaces.mjs"
import finger from "/api/modules/finger.mjs"
import move from "/api/modules/move.mjs"
import player from "/api/modules/player.mjs"
import voice from "/api/modules/voice.mjs"
import animation from "/api/modules/animation.mjs"
import sibsib from "/api/modules/sibsib.mjs"
import helicam from "/api/modules/helicam.mjs"
import binoculars from "/api/modules/binoculars.mjs"
import speedlimiter from "/api/modules/speedlimiter.mjs"
import paintball from "/api/modules/paintball.mjs"
import prophunt from "./modules/prophunt.mjs";
import blackscreen from "./modules/blackscreen.mjs";
import placeObject from "./modules/placeObject.mjs";

const keyDownHandlers = {}
const keyDownBeforeHandlers = {}
const keyUpHandlers = {}
const keyUpBeforeHandlers = {}

const onKeyDown = (key, handler) => {
    keyDownHandlers[key] = handler
}

const onKeyDownBefore = (key, handler) => {
    keyDownBeforeHandlers[key] = handler
}

const onKeyUpBefore = (key, handler) => {
    keyUpBeforeHandlers[key] = handler
}

const onKeyUp = (key, handler) => {
    keyUpHandlers[key] = handler
}

alt.on("keydown", key => {
    if (keyDownBeforeHandlers[key] != undefined) {
        keyDownBeforeHandlers[key]()
        return
    }

    if (alt.isConsoleOpen() || interfaces.isAnyOpen()) return
    if (keyDownHandlers[key] != undefined) {
        keyDownHandlers[key]()
    }
})

alt.on("keyup", key => {
    if (key == 0x1B) { // ESC
        keyUpHandlers[0x1B]()
        return
    }

    if (keyUpBeforeHandlers[key] != undefined) {
        keyUpBeforeHandlers[key]()
        return
    }

    if (alt.isConsoleOpen() || interfaces.isAnyOpen()) return
    if (keyUpHandlers[key] != undefined) {
        keyUpHandlers[key]()
    }
})

let lastPressedE = 0
let lastPressedY = 0
let lastInteractKey = 0
let lastInteractKeyY = 250
let lastInteractCooldown = 500

let lastUpdatePaintballStats = 0
let lastUpdatePaintballStatsCooldown = 5000



//Down
onKeyDown(0x45, _ => { // E
    if (alt.Player.local.vehicle != null || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || player.isInjured) return
    
    if (binoculars.enabled || helicam.enabled) return;
    const now = Date.now();


    if (now - lastPressedE > 1500) {
        lastPressedE = now;
        player.lastInteraction = now;
        if (prophunt.enabled && !prophunt.isSeeker) {
            prophunt.changeFrozenStatus();
            return;
        }
        alt.emitServer("PressE");
    }
    else webview.updateView("AddNotify", ["[Anti-Spam] Nur jede Sekunde möglich", "#c72020"]);
})

onKeyDown(0x4C, _ => { // L
    if (placeObject.active || binoculars.enabled || helicam.enabled || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || player.isInjured || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || native.getIsTaskActive(alt.Player.local.scriptID, 165)) return;

    const now = Date.now()
    if (now - lastInteractKey > lastInteractCooldown) {
        lastInteractKey = now
        alt.emitServer("PressL")
    }
})

// onKeyDown(0x4D, _ => { // M
//     if (alt.Player.local.vehicle == null) return
//     alt.emitServer("PressM")
// })

onKeyDown(0x49, _ => { // I
    if (interfaces.isOpen("Inventory")) {
        webview.closeInterface()
    } else {
        if (placeObject.active || prophunt.enabled || paintball.active || player.isTied || player.isCuffed || player.isInjured || player.cantCancelAnimation || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled) return        
        const now = Date.now()
        if (now - lastInteractKey > lastInteractCooldown) {
            lastInteractKey = now
            alt.emitServer("PressI")
        }
    }
})

onKeyDown(0x4B, _ => { // K
    if (player.isTied || player.isCuffed || player.isInjured || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled) return

    const now = Date.now()
    if (now - lastInteractKey > lastInteractCooldown) {
        lastInteractKey = now
        alt.emitServer("PressK")
    }
})

onKeyDown(0x78, _ => { // F9
    cursor.toggle()
})

onKeyDown(0x58, _ => { // X
    if (placeObject.active || player.isTied || player.cantCancelAnimation || player.playAnim || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled || paintball.active || prophunt.enabled || player.isInHacking || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || native.getIsTaskActive(alt.Player.local.scriptID, 165)) return
    const items = xmenu.getItems()
    xmenu.show(items)
    if (items != null) {
        cursor.show()
        alt.toggleGameControls(false)
    }
})

onKeyDown(0x47, _ => { // G
    if (placeObject.active || alt.Player.local.vehicle != null || player.isInjured || player.cantCancelAnimation || playAnim || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled) return
    const vehicle = enter.getClosestVehicle()
    if (vehicle == null) return
    const seat = enter.getClosestVehicleSeat(vehicle)
    enter.enterVehicle(vehicle, seat)
})

onKeyDown(0x42, _ => { // B
    if (player.isTied || player.isCuffed || player.isInjured ||
        native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || native.isPedReloading(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled) return
    finger.start()
})

onKeyDown(0x51, _ => { // Q
    if (prophunt.enabled || alt.Player.local.vehicle != null || player.isTied || player.isCuffed || player.isInjured || player.playAnim || player.isDrunk || player.cantCancelAnimation ||
        native.isPedReloading(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return

    if (playAnim) move.setStance("Standing")
    else move.setStance("Crouching")
    playAnim = !playAnim
    //if (!playAnim) player.disableActions = false
})

onKeyDown(0x5A, _ => { // Z
    if (prophunt.enabled || !native.isControlPressed(0, 21) || alt.Player.local.vehicle != null || player.isTied || player.isCuffed || player.isInjured || player.playAnim || player.isDrunk || player.cantCancelAnimation || 
        native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || native.isPedReloading(alt.Player.local.scriptID) ||
        native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || native.isPedSwimming(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
    if (move.currentStance == "Prone") move.setStance("Standup")
    else if (playAnim && move.currentStance != "Jump") move.setStance("Standing")
    else if (move.currentStance != "Jump") move.setStance("Prone")
    else return
    playAnim = !playAnim
    player.disableActions = playAnim
})


onKeyDown(0xBC, _ => { // ,
    if (placeObject.active || prophunt.enabled || paintball.active || alt.Player.local.vehicle != null || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || player.cantCancelAnimation ||
        player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") ||
        player.playAnim || native.isPedSwimming(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) ||
        native.isPedRagdoll(alt.Player.local.scriptID) || binoculars.enabled) return
    
    const now = Date.now()
    if (now - lastInteractKey > lastInteractCooldown) {
        lastInteractKey = now
        alt.emitServer("Comma")
    }
})

onKeyDown(0xBE, _ => { // .
    if (placeObject.active || prophunt.enabled || paintball.active || alt.Player.local.vehicle != null || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || player.cantCancelAnimation ||
        player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") ||
        player.playAnim || native.isPedRagdoll(alt.Player.local.scriptID) || native.isPedSwimming(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled) return

    const now = Date.now()
    if (now - lastInteractKey > lastInteractCooldown) {
        lastInteractKey = now
        alt.emitServer("Dot")
    }
})

let mapState = 0
onKeyDown(0x75, _ => { // F6
    if(helicam.enabled || binoculars.enabled) return;
    if(mapState == 0)
    {
        native.setBigmapActive(true, false)
        mapState++
    }
    else if(mapState == 1)
    {
        native.setBigmapActive(true, true)
        mapState++
    }
    else if(mapState == 2)
    {
        native.setBigmapActive(false, false)
        mapState = 0
    }
    native.flashMinimapDisplay()
})

let playAnim = false

onKeyDown(0x48, async _ => { // H
    if (alt.Player.local.vehicle != null || player.playAnim || player.isTied || player.isCuffed || player.isInjured || player.cantCancelAnimation ||
        native.isPedReloading(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0) return
    if (playAnim) native.clearPedTasks(alt.Player.local.scriptID)
    //else native.taskPlayAnim(alt.Player.local.scriptID, "ped", "handsup_enter", 8, -4, -1, 50, 0, false, false, false)
    else animation.playAnim("ped", "handsup_enter", 50);
    playAnim = !playAnim
    player.disableActions = playAnim
})

onKeyDown(0x4F, _ => { // O
    if (alt.Player.local.vehicle != null || player.playAnim || player.isTied || player.isCuffed || player.isInjured || player.cantCancelAnimation || 
        native.isPedReloading(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0) return
    if (playAnim) native.clearPedTasks(alt.Player.local.scriptID)
    //else native.taskPlayAnim(alt.Player.local.scriptID, "random@arrests@busted", "idle_a", 8, -4, -1, 50, 0, false, false, false)
    else animation.playAnim("random@arrests@busted", "idle_a", 50);
    playAnim = !playAnim
    player.disableActions = playAnim
})

onKeyDown(0xBB, _ => { // +
    if(speedlimiter.enabled)
    {
        speedlimiter.increaseSpeed()
    }
})

onKeyDown(0x6B, _ => { // +
    if(speedlimiter.enabled)
    {
        speedlimiter.increaseSpeed()
    }
})

onKeyDown(0xBD, _ => { // -
    if(speedlimiter.enabled)
    {
        speedlimiter.decreaseSpeed()
    }
})

onKeyDown(0x6D, _ => { // -
    if(speedlimiter.enabled)
    {
        speedlimiter.decreaseSpeed()
    }
})


onKeyDown(0xDC, _ => { // ^
    if(interfaces.isAnyOpen()) return;
    const now = Date.now()

    if (paintball.active) {
        if (now - lastUpdatePaintballStats > lastUpdatePaintballStatsCooldown) {
            lastUpdatePaintballStats = now
            alt.emitServer("RqPaintballStats") 
        } else {
            paintball.showPaintballStats();
        }
    }
})

onKeyUp(0xDC, _ => { // ^

    if (paintball.active) { 
        if(interfaces.isOpen("PaintballStatistic")) {
            webview.closeInterface()
        }
    }

})


// onKeyDown(0x6A, _ => { // *
//     if (animViewer.active) animViewer.stop()
//     else animViewer.start()
// })

// onKeyDown(0x6B, _ => { // +
//     animViewer.next()
// })

// onKeyDown(0x6D, _ => { // -
//     animViewer.previous()
// })

onKeyDownBefore(0x71, _ => { // F2
    if (!player.hasSmartphone) return;
    if(interfaces.isOpen("Barber") || interfaces.isOpen("ClothShop") || interfaces.isOpen("Tattoo") || interfaces.isOpen("SlotMachine") || interfaces.isOpen("GangwarWeaponSelect")) return

    if (native.isEntityInAir(alt.Player.local.scriptID) || native.isPedInParachuteFreeFall(alt.Player.local.scriptID) || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 ||
    native.getIsTaskActive(alt.Player.local.scriptID, 165) ||
    native.isPedJumping(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled || placeObject.active) return
    if (interfaces.phoneActive) webview.hidePhone()
    else {
        if (alt.Player.local.vehicle == null) {
            if (player.cantCancelAnimation || native.getSelectedPedWeapon(alt.Player.local.scriptID) != 0xA2719263) return
        }
        else if (player.cantCancelAnimation) return;
        const now = Date.now()
        if (now - lastInteractKey > lastInteractCooldown) {
            lastInteractKey = now
            alt.emitServer("Phone") 
        }

    }
})

onKeyDown(0x59, _ => { // Y
    let yNow = Date.now();
    if (yNow - lastPressedY > lastInteractKeyY) {
        lastPressedY = yNow
        voice.ToggleVoiceRange()
    } else {
        webview.updateView("AddNotify", ["[Anti-Spam] Nur jede Sekunde möglich", "#c72020"])
    }

})

onKeyDown(0x4D, async _ => { // M
    if(interfaces.isOpen("Barber") || interfaces.isOpen("ClothShop") || interfaces.isOpen("Tattoo") || interfaces.isOpen("SlotMachine") || interfaces.isOpen("GangwarWeaponSelect")) return
    if(placeObject.active || helicam.enabled || binoculars.enabled || paintball.active || prophunt.enabled || player.hasDiveClothes) return;
    if (native.isControlPressed(0, 21)) {
        if (native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || player.isTied || player.isCuffed || player.isInjured || player.cantCancelAnimation || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
        animation.playAnim("mp_masks@on_foot", "put_on_mask", 48)
        alt.setTimeout(() => {
            if (native.isPedMale(alt.Player.local.scriptID)) webview.toggleMenCloth(1)
            else webview.toggleWomenCloth(1)
        }, 200)
        return
    }

    if (interfaces.isOpen("Self")) {
        webview.closeInterface()
    } else {
        if (player.isTied || player.isCuffed || player.isInjured || player.cantCancelAnimation ||
            native.isEntityDead(alt.Player.local.scriptID, 1) || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
        //webview.showInterface(["Self", {'s': player.canToggleClothes}])
        webview.showInterface(["Self"])
    }
})

onKeyDownBefore(0x72, _ => { // F3
    if (!player.hasRadio) return;
    if(interfaces.isOpen("Barber") || interfaces.isOpen("ClothShop") || interfaces.isOpen("Tattoo") || interfaces.isOpen("SlotMachine") || interfaces.isOpen("GangwarWeaponSelect")) return;
    if (placeObject.active || helicam.enabled || binoculars.enabled) return;
    if (interfaces.funkActive) webview.hideFunk()
    else {
        if (player.isTied || player.isCuffed || player.isInjured ||player.cantCancelAnimation || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return

        const now = Date.now()
        if (now - lastInteractKey > lastInteractCooldown) {
            lastInteractKey = now
            alt.emitServer("Funk") 
        }
    }
})

onKeyDown(0x74, _ => { // F5
    if(placeObject.active || helicam.enabled || binoculars.enabled) return;
    if (interfaces.isOpen("AnimMenu")) {
        webview.closeInterface()
    } else {
        if (prophunt.enabled || alt.Player.local.vehicle != null || player.isTied || player.isCuffed || player.isInjured || player.playAnim || player.cantCancelAnimation ||
            native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
        webview.showInterface(["AnimMenu"])
    }
})

onKeyDownBefore(0x28, _ => { // ArrowDown
    if (voice.radio == 0 || interfaces.isOpen("ClothShop") || interfaces.isOpen("Chat") ||
        player.isTied || player.isCuffed || player.isInjured || native.isPauseMenuActive() || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || native.isPedRagdoll(alt.Player.local.scriptID) || interfaces.isOpen("Funk") || interfaces.isOpen("Phone")) return
    //native.taskPlayAnim(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", 8, -4, -1, 50, 0, false, false, false)
    animation.playAnim("random@arrests", "generic_radio_chatter", 50)
    alt.emitServer("Talk", true)
    //alt.log("Talk " + voice.radio)
})

onKeyUpBefore(0x54, _ => { // T
    if(helicam.enabled) return;

    if (alt.isConsoleOpen() || interfaces.isAnyOpen() && !interfaces.isOpen("Coma")) return;

    webview.showChat()
})

let hudOff = false
onKeyDown(0x76, _ => { // F7
    if(helicam.enabled || binoculars.enabled || blackscreen.enabled) return;
    native.displayRadar(hudOff)
    webview.updateView("ShowHud", [hudOff])
    hudOff = !hudOff
})

onKeyDown(0x52, _ => { // R
    if (prophunt.enabled || paintball.active || alt.Player.local.vehicle != null || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || player.cantCancelAnimation ||
        player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") ||
        player.playAnim || native.isPedSwimming(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled || native.getIsTaskActive(alt.Player.local, 134) /* is in anim */) return
    
    if (binoculars.enabled || helicam.enabled) return;
    let group = native.getWeapontypeGroup(player.weapon);
    if(native.isEntityInAir(alt.Player.local.scriptID) == true){
        native.setPedToRagdoll(alt.Player.local.scriptID, 2000, 2000, 0, true, true, true);
    }
    else if (group == 2685387236)
    {
        native.setPedToRagdoll(alt.Player.local.scriptID, 2500, 2500, 0, true, true, true);    
    }
})

onKeyDown(0x26, _ => { // ArrowUp
    if (interfaces.isOpen("ClothShop") || interfaces.isOpen("Chat") ||
    player.isTied || player.isCuffed || player.isInjured || native.isPauseMenuActive() || native.isPedRagdoll(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return

    const now = Date.now()
    if (now - lastInteractKey > lastInteractCooldown) {
        lastInteractKey = now
        //native.taskPlayAnim(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", 8, -4, -1, 48, 0, false, false, false)
        animation.playAnim("random@arrests", "generic_radio_chatter", 48)
        alt.emitServer("ChFunk")
    }


})

onKeyDown(0x4E, _ => { // N
    if (prophunt.enabled || alt.Player.local.vehicle != null || player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") || player.playAnim || player.cantCancelAnimation ||
        native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || native.getVehiclePedIsEntering(alt.Player.local.scriptID) != 0 || native.isPedRagdoll(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled) return
    const items = xmenu.getAnimations()
    xmenu.show(items)
    cursor.show()
    alt.toggleGameControls(false)
})

onKeyDown(0x55, async _ => { // U
    if (player.togetherAnimData == null || alt.Player.local.vehicle != null || player.isTied || player.isCuffed || player.isInjured || player.playAnim ||
        native.isPedSwimming(alt.Player.local.scriptID) || player.cantCancelAnimation || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled) return
    const { target, anim, startTime } = player.togetherAnimData
    // if(Date.now() - startTime > 15000)
    // {
        //await animation.playAnimFor(target, anim[1], anim[2], anim[3])
        alt.emitServer("Anim", target, anim[1], anim[2], anim[3])
        animation.playAnim(anim[1], anim[2], anim[3])
        player.togetherAnimData = null
    // } else {
    //     webview.updateView("AddNotify", ["Die Anfrage ist zu alt!", "#c72020"])
    // }
})

onKeyDown(0x70, _ => { // F1
    webview.updateView("TogglePoint")
})

onKeyDown(0x2D, _ => { // INSERT
    sibsib.insertAlert()
})

onKeyDown(0x23, _ => { // INSERT
    sibsib.endAlert()
})

onKeyDown(0x73, _ => { // F4
    if(Date.now() - lastAltPressed < 2500)
    {
        sibsib.AltF4Alert()
    }
    if(alt.Player.local.vehicle != null && native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false) == alt.Player.local.scriptID)
    {
        alt.emitServer("VehicleSiren")
    }
})

let lastAltPressed = 0
onKeyDown(0x12, _ => { // Alt
    lastAltPressed = Date.now();
})

onKeyDown(0x56, _ => { // V
    if(helicam.enabled)
    {
        native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
        helicam.changeVision()
        return;
    }
    if(binoculars.enabled)
    {
        native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
        binoculars.changeVision()
        return;
    }
})

onKeyDown(0x20, _ => { // SPACEBAR
    if(helicam.enabled)
    {
        helicam.triggerEMP()
        return;
    }
})

//Up

onKeyUp(0x58, _ => { // X
    if (placeObject.active || player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled || binoculars.enabled || paintball.active) return


    // webview.mainView.unfocus()
    cursor.hide()
    alt.toggleGameControls(true)
})

onKeyUp(0x42, _ => { // B
    if (player.isTied || player.isCuffed || player.isInjured ||
        native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1) || native.isPedReloading(alt.Player.local.scriptID) || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || helicam.enabled) return
    finger.stop()
})

onKeyUp(0x28, _ => { // ArrowDown
    if (voice.radio == 0 || interfaces.isOpen("ClothShop") || interfaces.isOpen("Chat") ||
        player.isTied || player.isCuffed || player.isInjured || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
    native.stopAnimTask(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", -4)
    alt.emitServer("Talk", false)
    //alt.log("TalkStop " + voice.radio)
})

onKeyUp(0x1B, _ => { // ESC
    if (helicam.enabled)
    {
        native.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false)
        native.clearPedTasks(alt.Player.local.scriptID)
        helicam.stop()
        return
    }
    else if (binoculars.enabled)
    {
        binoculars.stop()
        return
    }

    if (interfaces.isOpen("Inventory")) {
        webview.closeInterface()
        return;
    }

    if (interfaces.isOpen("Char") || interfaces.isOpen("CCTVComputer") || interfaces.isOpen("ClothShop") || interfaces.isOpen("Coma") || interfaces.phoneActive || interfaces.funkActive) return
    // else if (interfaces.isOpen("Fishing"))
    // {
    //     alt.emitServer("StopFishing")
    // }
    // else if (interfaces.isOpen("GoldFarming"))
    // {
        
    //     alt.emitServer("StopGoldFarming")
    // }
    webview.updateView("CloseIF")
})

onKeyUp(0x26, _ => { // ArrowUp
    if (interfaces.isOpen("ClothShop") || interfaces.isOpen("Chat") ||
    player.isTied || player.isCuffed || player.isInjured || native.isPedBeingStunned(alt.Player.local.scriptID, 0)) return
    native.stopAnimTask(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", -4)
})

onKeyUp(0x4E, _ => { // N
    if (alt.Player.local.vehicle != null || interfaces.isAnyOpen() || player.isTied || player.isCuffed || player.isInjured || interfaces.isOpen("Inventory") || native.isPedBeingStunned(alt.Player.local.scriptID, 0) || binoculars.enabled) return
    cursor.hide()
    alt.toggleGameControls(true)
    // webview.mainView.unfocus()
})