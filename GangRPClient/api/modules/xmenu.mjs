import alt from "alt"
import native from "natives"
import webview from "/api/modules/webview.mjs"
import raycast from "/api/modules/raycast.mjs"
import player from "/api/modules/player.mjs"
import vehicle from "/api/modules/vehicle.mjs"
import animation from "/api/modules/animation.mjs"
import sibsib from "/api/modules/sibsib.mjs"
import helicam from "/api/modules/helicam.mjs"
import speedlimiter from "/api/modules/speedlimiter.mjs"
import propsync from "/api/modules/propsync.mjs"

class XMenu {
    constructor() {
        this.playerOnPlayerItems = [
            { text: "Geld geben", event: "GiveMoney", server: true },
            { text: "Dokument zeigen", event: "ShowDoc", server: true },
            { text: "Dokument nehmen", event: "TakeDoc", server: true }, //Person macht animation?
            { text: "Durchsuchen", event: "InvSearch", server: true },
            { text: "Erste Hilfe", event: "FirstAid", server: true },
            { text: "Gegenstand geben", event: "Give", server: true },
            { text: "Ins Fahrzeug ziehen", event: "GrabPlayer", server: true },
            { text: "Fesseln", event: "Tie", server: true }
        ]

        this.playerInVehicleItems = [
            { text: "Motor an/aus", event: "ToggleEngine", server: true },
            { text: "Türen auf/zu", event: "ToggleDoor", server: true },
            { text: "Kofferraum auf/zu", event: "ToggleTrunk", server: true },
            { text: "Handbremse", event: "Handbreak", server: false },
            { text: "Rauswerfen", event: "Eject", server: true }
        ]

        this.playerOutVehicleItems = [
            { text: "Kofferraum auf/zu", event: "ToggleTrunk", server: true },
            { text: "Türen auf/zu", event: "ToggleDoor", server: true },
            { text: "Reparieren", event: "Repair", server: true },
            { text: "Tanken", event: "Refuel", server: true },
            { text: "Einparken", event: "Park", server: true }
        ]

        this.playerOnObjectItems = [
            { text: "Objekt entfernen", event: "RemoveObject", server: true },
        ]

        this.placeObjectData = JSON.parse(alt.File.read("@SibauiRP_Assets/data/placeObjectData.json"))

        

        this.vehiclesWithComputer = new Set([
            /* PD */
            0x432EA949,
            0x9DC66994,
            0x79FBB0C5,
            0x9F05F101,
            0x71FA16EA,
            0x8A63C7B9,
            0xFDEFAEC3,
            0x1517D4D9,
            0xA46462F7,
            0x95F4C618,
            0x1B38E955,
            0xE2E7D4AB,
            0xB822A1AA,
            0x9B16A3B4,
            0x9BAA707C,
            0x72935408,
            0xFE457F00,
            0xC80F1294,
            0x5714B7DA,
            0x243133AB,
            0x295306FA,
            0xC4BAC3AC,
            0x6090706C,
            0x35AC5ADF,
            0xAC384A90,
            0x9745D4D9,
            0x8F3BC3CD,
            0xCC66AF42,
            0x43CCAF27,
            0x3A0E34C1,
            0xB1C5826,
            0xE69FECAD,
            0xAB7C8500,
            0xFE457F00,
            0xC80F1294,
            0xA02CD259,
            0x63E9AE91,
            0x7EE7CE5F,
            0xC2D44B02,
            0x55707124,
            0x97426529,
            /* FIB */
            0xAE2BFE94,
            0xC94C6317,
            0x16C5BC6E,
            0xC2EFE913,
            0xE7AF8816,
            0x8DC6408D,
            0x5E92D1C6,
            0x742E9AC0,
            0x9E3F6D01,
            0x9657E3DB,
            0xF6226B9E,
            0x2862EE28,
            0x607FCAF8,
            0x9721F53F,
            0xB75DF8B7,
            0x3082EB0F,
            0xFEC48613,
            0x62C96F57,
            0x0616B538,
            0xEE4B07C5
        ])

        this.vehiclesWithServicelist = new Set([
            /* Medics */
            0x45D56ADA,
            0x1517D4D9,
            0x432EA949,
            0x9DC66994,
            0x33B47F96,
            0x5E92D1C6,
            0xC2EFE913,
            0x9E3F6D01,
            0xCEC2828D,
            0x2862EE28, //UMK Schlagen
            0x121A43CE, //lsmcambulance
            0x9D707F3C, //lsmcbus
            0x8021963C, //lsmcscout
            
            /* DPOS */
            0xB12314E0,
            0xE5A2D6C6,
            0xCFB3870C,
            0x50B0215A,
            0x60A7EA10,
            0xAF966F3C,
            0xFCFCB68B,
            0xF9A16657, //dposcaracara2
            0xE42D439F, //dposgranger2
            0x9A474B5E, //avisa

            /* TAXI */
            0xC703DB5F,
            0x65B5B1C0, //OracleTaxi

            /* WN */
            0x4543B74D, //Rumpo
            0x2DB8D1AA, //alpha
            0xCE0B9F22, //landstalker2
            0xC3DDFDCE, //tailgater
            0xB5D306A4, //tailgater2
            0x56CDEE7D, //vstr
            0x742E9AC0, //frogger2
            0x2A54C47D, //supervolito
            0x90A28E77, //weazellandstalker2
            0x1052149C, //weazelmaverick
            0x7263666, //weazeltailgater2
            0x4A0E4116, //weazelvstr

            /* LSC */
            0xAE0124A4, //lsccaracara2
        ])

        this.truckerVehicles = new Set([
            0x7A61B330, //Benson
            //0x32B91AE8, //Biff
            0x35ED670B, //Mule
            0xC1632BEB, //Mule2
            0x85A5B471, //Mule3
            0x73F4110E, //Mule4
            0x7DE35E7D, //Pounder
            0x6290F15B, //Pounder2
            //0x9A5B1DCC, //Rubble
            //0x2E19879, //Tiptruck
            //0xC7824E5E, //Tiptruck2
            //0x898ECCEA, //Boxville
            0xF21B33BE, //Boxville2
            //0x7405E08, //Boxville3
            //0x1A79847A, //Boxville4
            0xCBB2BE0E, //trailers
            0xA1DA3C91, //trailers2
            0x8548036D, //trailers3
            0xBE66F5AA, //trailers4
            0x5A82F9AE, //hauler
            0x21EEE87D, //packer
            0x809AA4CB, //phantom
            0xA90ED5C, //phantom3
            0xD17099D, //speedo4
            0x5EC94862, //t670
            0xD94C5AA7, //vnl780
        ])

        this.items = null
        this.inVehicle = false
        this.entity = null
        this.lastAnimationRequest = Date.now()
        this.markerTick = null
    }

    getItems() {
        this.inVehicle = player.inVehicle()
        this.items = null
        this.type = undefined

        if (this.inVehicle) {
            this.entity = alt.Player.local.vehicle
            this.getPlayerInVehicleItems()
        } else {
            const entity = raycast.getEntity()

            if (entity != null) {
                if (native.isEntityAPed(entity)) {
                    for (const player of alt.Player.streamedIn) {
                        if (player.scriptID == entity) {
                            this.entity = player
                            break
                        }
                    }
                    if(this.entity != null)
                    {
                        this.markerTick = alt.everyTick(() => {
                            if (this.entity == undefined) {
                                this.clearMarker()
                            } else {
                                native.drawMarker(
                                    25, this.entity.pos.x, this.entity.pos.y, this.entity.pos.z-1.0,
                                    0, 0, 0,
                                    0, 0, 0,
                                    1, 1, 1,
                                    52, 192, 216, 255,
                                    !!false, !!false, 2, !!false, null, null, !!false
                                );
                            }

                        });
                        this.getPlayerOnPlayerItems()
                    }

                } else if (native.isEntityAVehicle(entity)) {
                    for (const vehicle of alt.Vehicle.streamedIn) {
                        if (vehicle.scriptID == entity) {
                            this.entity = vehicle
                            break
                        }
                    }
                    // const [_, min, max] = native.getModelDimensions(this.entity.model, null, null)
                    // let width = max.x - min.x
                    // if(native.isThisModelAHeli(this.entity.model) || native.isThisModelAPlane(this.entity.model))
                    // {
                    //     width = 3
                    // }
                    // this.markerTick = alt.everyTick(() => {
                    //     native.drawMarker(
                    //         25, this.entity.pos.x, this.entity.pos.y, this.entity.pos.z,
                    //         0, 0, 0,
                    //         0, 0, 0,
                    //         width, width, 1,
                    //         52, 154, 228, 204,
                    //         !!false, !!false, 2, !!false, null, null, !!false
                    //     );
                    // });
                    this.getPlayerOnVehicleItems()
                } else if (native.isEntityAnObject(entity)) {
                    let model = native.getEntityModel(entity);
                    if(this.placeObjectData.some(obj => alt.hash(obj.model) == model))
                    {
                        this.entity = native.getEntityCoords(entity, false)
                        this.getPlayerOnObjectItems()
                    }
                }
            }
        }

        return this.items
    }

    getPlayerOnPlayerItems() {
        this.items = [...this.playerOnPlayerItems]
        if (player.team == 3 || player.team == 7) { //Pd, Fib
            this.items.push({ text: "Festnehmen", event: "Cuff", server: true })
            this.items.push({ text: "Fußfesseln", event: "FootCuff", server: true })
        }
    }

    getPlayerOnObjectItems() {
        this.items = [...this.playerOnObjectItems]
    }

    getPlayerInVehicleItems() {
        this.items = [...this.playerInVehicleItems]
        if (native.getPedInVehicleSeat(this.entity.scriptID, -1, false) == alt.Player.local.scriptID) {
            if (native.isThisModelACar(this.entity.model) || native.isThisModelABike(this.entity.model)) {
                this.items.push({ text: "Speedlimiter", event: "Speedlimiter", server: false })
            }
            if (native.isThisModelABoat(this.entity.model)) {
                this.items.push({ text: "Anker", event: "Anchor", server: false })
            }
        }
        if (player.team == 3 || player.team == 7) { //Pd, Fib
            if (this.vehiclesWithComputer.has(this.entity.model)) {
                this.items.push({ text: "Polizeicomputer", event: "PolComputer", server: true })
            }
            if (player.team == 7 && this.entity.model == 0x55707124) {
                if (native.getPedInVehicleSeat(this.entity.scriptID, 0, false) == alt.Player.local.scriptID)
                {
                    this.items.push({ text: "Luftanalyse durchführen", event: "AirAnalysis", server: true })
                }
            }
        } else if (player.team == 5) {
            if (this.vehiclesWithServicelist.has(this.entity.model)) {
                this.items.push({ text: "Serviceliste", event: "Services", server: true })
            }
        } else if (player.team == 6) {
            if (this.vehiclesWithServicelist.has(this.entity.model)) {
                this.items.push({ text: "Serviceliste", event: "Services", server: true })
            }
        } else if (player.team == 23) {
            if (this.vehiclesWithServicelist.has(this.entity.model)) {
                this.items.push({ text: "Serviceliste", event: "Services", server: true })
            }
        } else if (player.team == 22) {
            if (this.truckerVehicles.has(this.entity.model)) {
                this.items.push({ text: "Entladepunkte", event: "DeliveryMarker", server: true })
            }
        } else if (player.team == 25) {
            if (this.vehiclesWithServicelist.has(this.entity.model)) {
                this.items.push({ text: "Serviceliste", event: "Services", server: true })
            }
        }

        if (this.entity.model == 0x9D0450CA || this.entity.model == 0x1517D4D9) {
            if (native.getPedInVehicleSeat(this.entity.scriptID, 1, false) == alt.Player.local.scriptID ||
                native.getPedInVehicleSeat(this.entity.scriptID, 2, false) == alt.Player.local.scriptID)
                this.items.push({ text: "Abseilen", event: "Rappel", server: false })
        }

        if (this.entity.model == 0xB12314E0 || this.entity.model == 0xE5A2D6C6 || this.entity.model == 0xA02CD259) {
            this.items.push({ text: "Speedlimiter zurücksetzen", event: "ResetSpeedlimiter", server: false })
        }

        if ((this.entity.model == 0x1517D4D9 && native.getVehicleLivery(this.entity.scriptID) == 0) || this.entity.model == 0x742E9AC0) {
            if (native.getPedInVehicleSeat(this.entity.scriptID, 0, false) == alt.Player.local.scriptID) {
                this.items.push({ text: "Kamera", event: "Helicam", server: false })
            }
        }

        if (native.canShuffleSeat(this.entity.scriptID, 0)) {
            this.items.push({ text: "Sitz wechseln", event: "Shuffle", server: false })
        }
    }

    getPlayerOnVehicleItems() {
        this.items = [...this.playerOutVehicleItems]

        if (player.team == 3 || player.team == 7) { //Pd, Fib
            this.items.push({ text: "Fahrzeug durchsuchen", event: "VehSearch", server: true })
        }
        else if (player.team == 5) { //Medic
            this.items.push({ text: "Patient entladen", event: "UnloadPatient", server: true })
        }
        else if (player.team == 6) { //Dpos
            this.items.push({ text: "Fahrzeug aufladen", event: "AttachVeh", server: true })
            this.items.push({ text: "Fahrzeug beschlagnahmen", event: "DepositoryVeh", server: true })
        }
        else if (player.team == 22) {
            if (this.truckerVehicles.has(this.entity.model)) {
                let boneIndex = native.getEntityBoneIndexByName(this.entity.scriptID, "reversinglight_l")
                let doorPosition = native.getWorldPositionOfEntityBone(this.entity.scriptID, boneIndex);
                let distance = this.get2dDistance(doorPosition)
                if(distance < 2)
                {
                    this.items.push({ text: "Fahrzeug beladen", event: "LoadPackageToVeh", server: true })
                }
                else
                {
                    boneIndex = native.getEntityBoneIndexByName(this.entity.scriptID, "reversinglight_r")
                    doorPosition = native.getWorldPositionOfEntityBone(this.entity.scriptID, boneIndex);
                    distance = this.get2dDistance(doorPosition)
                    if(distance < 2)
                    {
                        this.items.push({ text: "Fahrzeug beladen", event: "LoadPackageToVeh", server: true })
                    }
                }
            }
        }

        if (this.entity.model == 0x1A79847A) {
            this.items.push({ text: "Pakete abladen", event: "UnloadPackage", server: true })
        }

        if (player.team == 7) {
            if (this.entity.model == 0x6FD95F68 || this.entity.model == 0xF8D48E7A) {
                this.items.push({ text: "Riechen", event: "VehSmell", server: true })
            }
        }
    }

    get2dDistance(position) {
        const playerPosition = alt.Player.local.pos
        return Math.sqrt(
            (playerPosition.x - position.x) * (playerPosition.x - position.x) +
            (playerPosition.y - position.y) * (playerPosition.y - position.y) + 
            (playerPosition.z - position.z) * (playerPosition.z - position.z)
        )
    }

    show(items) {
        // webview.showInteractionMenu(items == null ? null : items.map(i => i.text))
        webview.mainView.focus()
        webview.updateView("XMenu", [items])
        const [_, x, y] = native.getActiveScreenResolution()
        alt.setCursorPos({ x: x / 2, y: y / 2 })
    }

    clearMarker() {
        if(this.markerTick != null)
        {
            alt.clearEveryTick(this.markerTick)
            this.markerTick = null;
        }
    }

    onItemSelected(item) {
        
        webview.updateView("CloseIF")
        if (this.type != undefined) {
            if (player.cantCancelAnimation) return
            if (item.event == "StopAnim") {
                propsync.cancel();
            } else {
                if (this.type == 1) animation.playAnim(item.anim[1], item.anim[2], item.anim[3])
                else 
                {
                    let now = Date.now();
                    if(now - this.lastAnimationRequest > 1500)
                    {
                        alt.emitServer("TogAnim", this.entity, item.anim);
                        this.lastAnimationRequest = now;
                    }
                    else
                    {
                        webview.updateView("AddNotify", ["[Anti-Spam] Nur jede Sekunde möglich", "#c72020"]);
                    }
                }
            }
            return
        }

        if (item.server) {
            if (this.entity == null) alt.log("entity null")
            //alt.log("XMenu emit to server " + item.event)
            alt.emitServer(item.event, this.entity)
        } else {
            //alt.log("XMenu client " + item.event)

            switch (item.event) {
                case "Handbreak":
                    vehicle.toggleHandbreak(this.entity)
                    break
                case "Rappel":
                    native.taskRappelFromHeli(alt.Player.local.scriptID, 10.0)
                    sibsib.blockFlyCheck = true
                    alt.setTimeout(() => {
                        sibsib.blockFlyCheck = false
                    }, 25000)
                    break
                case "Shuffle":
                    vehicle.shuffleVehicle(this.entity)
                    break
                case "Helicam":
                    helicam.start()
                    break
                case "Speedlimiter":
                    speedlimiter.toggle()
                    break
                case "Anchor":
                    vehicle.toggleAnchor(this.entity)
                    break
                case "ResetSpeedlimiter":
                    if (alt.Player.local.vehicle != null) {
                        let attachedVeh = native.getEntityAttachedToTowTruck(alt.Player.local.vehicle.scriptID)
                        if (attachedVeh != 0) {
                            //alt.log(attachedVeh)
                            native.setVehicleMaxSpeed(attachedVeh, -1)
                            webview.updateView("AddNotify", ["Speedlimiter deaktiviert!", "#00a6cc"])
                        }
                        else {
                            webview.updateView("AddNotify", ["Kein Fahrzeug am Haken!", "#00a6cc"])
                        }
                    }
                    break
            }
        }

        this.items = null
        this.entity = null
        webview.mainView.focus()
    }

    getAnimations() {
        //this.inVehicle = player.inVehicle()
        this.items = null
        this.type = 1

        const entity = raycast.getEntity()

        if (entity != null) {
            if (native.isEntityAPed(entity)) {
                for (const player of alt.Player.streamedIn) {
                    if (player.scriptID == entity) {
                        this.entity = player
                        break
                    }
                }
    
                this.type = 2
            }
        }

        this.getPlayerAnimations()

        return this.items
    }

    getPlayerAnimations() {
        let anims = alt.LocalStorage.get("animsnew")

        this.items = [{ text: "Animation beenden", event: "StopAnim" }]

        if (anims == null) {
            anims = {}
        }

        for (const anim in anims) {
            this.items.push({ text: anims[anim][0], event: "PlayAnim", anim: anims[anim] })
        }

        return this.items
    }
}

export default new XMenu()