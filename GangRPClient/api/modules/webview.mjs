import alt from "alt"
import native from "natives"
import cursor from "/api/modules/cursor.mjs"
import xmenu from "/api/modules/xmenu.mjs"
import interfaces from "/api/modules/interfaces.mjs"
import player from "/api/modules/player.mjs"
import cam from "/api/modules/cam.mjs"
import voice from "/api/modules/voice.mjs"
import move from "/api/modules/move.mjs"
import animation from "/api/modules/animation.mjs"
import paintball from "./paintball.mjs";
import sibsib from "./sibsib.mjs"

class WebView {
    constructor() {
        this.mainView = new alt.WebView("http://resource/index.html")
        this.camera = null
        this.chatCommands = []
        this.freezeInterval = null
        

        alt.onServer("ShowIF", (...args) => {
            //alt.log("ShowIF " + JSON.stringify(args[1]))
            
            this.showInterface(args)
        })
        
        alt.onServer("UpdateView", (eventName, ...args) => {
            //alt.log("UpdateView " + eventName + " " + JSON.stringify(...args))
            
            this.mainView.emit("UpdateView", eventName, args)
        })

        alt.onServer("Phone", () => {
            // alt.log("Phone show")
            
            if (player.cantCancelAnimation)return

            this.mainView.emit("UpdateView", "Phone", [true])
            this.mainView.focus()
            cursor.show()
            // if (alt.Player.local.vehicle == null) {
            //     if (!native.isPedClimbing(alt.Player.local.scriptID))
            //         if (native.getVehiclePedIsEntering(alt.Player.local.scriptID) == 0)
            //             native.taskStartScenarioInPlace(alt.Player.local.scriptID, "WORLD_HUMAN_STAND_MOBILE", 0, true)
            // }
            alt.toggleGameControls(false)
            interfaces.open("Phone")
            interfaces.phoneActive = true
        })

        alt.onServer("CloseIF", () => {
            this.closeInterface()
        })

        alt.onServer("Funk", async () => {
            // alt.log("Funk show")
            
            this.mainView.emit("UpdateView", "Funk", [true])
            //native.taskPlayAnim(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", 8, -4, -1, 50, 0, false, false, false)
            animation.playAnim( "random@arrests", "generic_radio_chatter", 50);
            this.mainView.focus()
            cursor.show()
            alt.toggleGameControls(false)
            interfaces.open("Funk")
            interfaces.funkActive = true
        })
        
        this.mainView.on("EmitServer", (eventName, ...args) => {
            alt.emitServer(eventName, ...args)
        })
        
        this.mainView.on("HideInterface", () => {
            // alt.log("Hideinterface")
        
            this.mainView.unfocus()
            cursor.hide()
            alt.toggleGameControls(true)
            interfaces.hide()
        })

        this.mainView.on("ShowInterface", name => {
            this.showInterface([name])
        })

        this.mainView.on("UpdateView", (eventName, ...args) => {
            // alt.log("UpdateView " + eventName + " " + JSON.stringify(...args))
            this.mainView.emit("UpdateView", eventName, args)
        })
        
        // this.mainView.on("VehColor", (color) => {
        //     if (alt.Player.local.vehicle != null) {
        //         let vehicle = alt.Player.local.vehicle
        //         alt.log(color[0])
        //         alt.log(color[1])
        //         alt.log(color[2])
        //         native.setVehicleCustomPrimaryColour(vehicle, color[0], color[1], color[2])
        //     }
        // })

        this.mainView.on("SetClothShopCloth", (component, texture, variation) => {
            if (component > 0) native.setPedComponentVariation(alt.Player.local.scriptID, component, texture, variation, 0)
            else native.setPedPropIndex(alt.Player.local.scriptID, component * -1, texture, variation, true)
        })
        
        this.mainView.on("StartCam", (data, x, y, z, r) => {
            cam.start(data, x, y, z, r)
        })
        
        this.mainView.on("NextCam", () => {
            cam.nextCam()
        })
        
        this.mainView.on("PreviousCam", () => {
            cam.previousCam()
        })
        
        this.mainView.on("CloseCam", () => {
            cam.stop()
        })

        this.mainView.on("GetSex", () => {
            this.mainView.emit("UpdateView", "ReturnSex", [player.gender])
        });

        this.mainView.on("ToggleVehicleHud", (bool) => {
            this.mainView.emit("UpdateView", "ToggleVehicleHud", [bool])
        });
        
        this.mainView.on("CatchFish", () => {
            var pos = alt.Player.local.pos
            var zoneNameId = native.getNameOfZone(pos.x, pos.y, pos.z)
            alt.emitServer("ReturnCatchFish", zoneNameId)
        })


        this.mainView.on("CreateTattoCamera", (categoryId) => {
            let pos = alt.Player.local.pos;
            let heading = native.getEntityHeading(alt.Player.local.scriptID);

            let targetPositionFly, targetPositionPoint, offset;
            

            switch(categoryId) {
                case -1: //Tattoladen
                    offset = this.offsetPosition(pos.x, pos.y, heading, 3)
                    this.camera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", offset.x, offset.y, pos.z, 0, 0, 0, 40, true, 0)
                    native.pointCamAtCoord(this.camera, pos.x, pos.y, pos.z)
                    native.renderScriptCams(true, false, 0, true, false, 0)
                    return;
                case 1: //Torso
                    offset = this.offsetPosition(pos.x, pos.y, heading, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 0.5))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z + 0.2))
                    break;
                case 2: //Kopf
                    offset = this.offsetPosition(pos.x, pos.y, heading, 1.1);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 0.7))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z + 0.7))
                    break;
                case 3: //Linkes Bein
                    offset = this.offsetPosition(pos.x, pos.y, heading + 90, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z - 0.6))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z - 0.6))
                    break;
                case 4: //Rechtes Bein
                    offset = this.offsetPosition(pos.x, pos.y, heading - 90, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z - 0.6))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z - 0.6))
                    break;
                case 5: //Linker Arm
                    offset = this.offsetPosition(pos.x, pos.y, + 90, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 0.5))
                    targetPositionPoint = pos
                    break;
                case 6: //Rechter Arm
                    offset = this.offsetPosition(pos.x, pos.y, - 90, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 0.5))
                    targetPositionPoint = pos
                    break;
                case 7: //Rücken
                    offset = this.offsetPosition(pos.x, pos.y, + 180, 1.5);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 0.5))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z + 0.2))
                    break;
                case 8: //Kopf
                    offset = this.offsetPosition(pos.x, pos.y, heading, 1.1);
                    targetPositionFly   = new alt.Vector3(offset.x, offset.y, (pos.z + 1.2))
                    targetPositionPoint = new alt.Vector3(pos.x, pos.y, (pos.z + 0.7))
                    break;
            }


            // native.setCamActive(this.camera, false)
            // native.renderScriptCams(false, false, 0, true, false)

            // let camPos = native.getCamCoord(this.camera)
            // let rot = native.getCamRot(this.camera, 2)
            // let fov = native.getCamFov(this.camera)
            // let interpCamera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", camPos.x, camPos.y, camPos.z, rot.x, rot.y, rot.z, fov, true, 0)

            // native.setCamCoord(this.camera, targetPositionFly.x, targetPositionFly.y, targetPositionFly.z)
            // native.stopCamPointing(this.camera)
            // native.setCamActiveWithInterp(interpCamera, this.camera, 500, 0, 0)
            // native.renderScriptCams(true, false, 0, true, false)
            
            // let rot = native.getCamRot(this.camera, 2)
            // let fov = native.getCamFov(this.camera)
            native.setCamCoord(this.camera, targetPositionFly.x, targetPositionFly.y, targetPositionFly.z)
            //this.camera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", targetPositionFly.x, targetPositionFly.y, targetPositionFly.z, rot.x, rot.y, rot.z, fov, true, 0)
            native.pointCamAtCoord(this.camera, targetPositionPoint.x, targetPositionPoint.y, targetPositionPoint.z)
            // this.camera = interpCamera;
    
        })


        this.mainView.on("CreateCameraLookAtMe", (x, y, z) => {
            const pos = alt.Player.local.pos
            const test = native.getOffsetFromEntityInWorldCoords(alt.Player.local.scriptID, x, y, z)
            this.camera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", test.x, test.y, test.z, 0, 0, 0, 45, true, 0)
            native.pointCamAtEntity(this.camera, alt.Player.local.scriptID, 1.2, 0, 0, 1)
            //native.pointCamAtCoord(this.camera, pos.x, pos.y, pos.z)
            native.renderScriptCams(true, false, 0, true, false, 0)
        })

        this.mainView.on("PointCameraAtFace", () => {
            const rot = alt.Player.local.rot
            const cords = native.getOffsetFromEntityInWorldCoords(alt.Player.local.scriptID, 0, -1, 0.7)
            this.camera = native.createCamWithParams("DEFAULT_SCRIPTED_CAMERA", cords.x, cords.y, cords.z, rot.x, rot.y, rot.z, 40, true, 0)
            const head = native.getPedBoneCoords(alt.Player.local.scriptID, 31086, 0, 0, 0)
            native.pointCamAtCoord(this.camera, head.x, head.y, head.z)
            native.renderScriptCams(true, false, 0, true, false, 0)
            this.freezeInterval = alt.setInterval(() => native.clearPedTasksImmediately(alt.Player.local.scriptID), 0)
        })

        this.mainView.on("DestroyCamera", () => {
            if(this.camera != null)
            {
                native.setCamActive(this.camera, false)
                native.destroyCam(this.camera, false)
            }
            native.renderScriptCams(false, false, 0, true, false, 0)
            if (this.freezeInterval != null) alt.clearInterval(this.freezeInterval)
            native.freezeEntityPosition(alt.Player.local.scriptID, false)
            this.camera = null
        })

        this.mainView.on("XMenuSelected", item => {
            xmenu.onItemSelected(item)
        })

        this.mainView.on("XMenuClearMarker", () => {
            xmenu.clearMarker()
        })

        this.mainView.on("HideHud", () => {
            this.hideHud()
        })

        this.mainView.on("LastMessage", (msg) => {
            if(msg.startsWith('/spawn') ||
            msg.startsWith('/noclip') ||
            msg.startsWith('/spectate') ||
            msg.startsWith('/coord') ||
            msg.startsWith('/go')) {
                player.lastCommand = Date.now()
            }
        })

        this.mainView.on("SetGPS", (x, y) => {
            native.setNewWaypoint(x, y)
        })

        this.mainView.on("SetComa", (status) => {
            if(status)
            {
                player.isComa = true;
                native.animpostfxPlay("DeathFailOut", 0, true);
            }
            else
            {
                player.isComa = false;
                native.animpostfxStop("DeathFailOut");
            }
        })

        this.mainView.on("SetPart", (method, ...args) => {
            native[method](alt.Player.local.scriptID, ...args)
        })

        this.mainView.on("GetPlayer", prop => {            
            this.mainView.emit("UpdateView", prop, [player[prop]])
        })

        this.mainView.on("GetPlayerPosForSMS", () => {
            this.mainView.emit("UpdateView", "SendPlayerPosForSMS", [player.getPos()])
        }) 

        this.mainView.on("SetPlayer", (prop, value) => {
            // alt.log("SetPlayer " + prop + " " + value)
            player[prop] = value
        })

        this.mainView.on("ResetAppearance", () => {
            player.setAppearance(true)
        })

        this.mainView.on("ResetClothes", () => {
            player.setClothes()
        })

        this.mainView.on("ClearProps", () => {
            this.clearProps()
        })

        this.mainView.on("SaltyChat_OnConnected", voice.OnPluginConnected)
        this.mainView.on("SaltyChat_OnDisconnected", voice.OnPluginDisconnected)
        this.mainView.on("SaltyChat_OnMessage", voice.OnPluginMessage)
        this.mainView.on("SaltyChat_OnError", voice.OnPluginError)

        this.mainView.on("ToggleCloth", async id => {
            // alt.log("Cloth " + id + " " + alt.Player.local.model)

            //if(!player.canToggleClothes) return;

            if (id != 1) {
                if (native.isPedMale(alt.Player.local.scriptID)) this.toggleMenCloth(id)
                else this.toggleWomenCloth(id)
                return
            }

            animation.playAnim("mp_masks@on_foot", "put_on_mask", 48)
            alt.setTimeout(() => {
                if (native.isPedMale(alt.Player.local.scriptID)) this.toggleMenCloth(1)
                else this.toggleWomenCloth(1)
            }, 200)
        })

        this.mainView.on("ClearProp", id => {
            // alt.log("ClearProp " + id)
            native.clearPedProp(alt.Player.local.scriptID, id)
        })

        this.mainView.on("PlaySet", set => {
            if (player.isTied || player.isCuffed || player.isInjured || player.isDrunk) return
            if (set) move.setClipset(set, 1.0)
            else native.resetPedMovementClipset(alt.Player.local.scriptID, 0.25)
        })

        this.mainView.on("PlayAnim", (dict, anim, flag) => {
            if (player.isTied || player.isCuffed || player.isInjured) return
            if (anim) animation.playAnim(dict, anim, flag)
            else native.clearPedTasks(alt.Player.local.scriptID)
        })

        this.mainView.on("AddNotify", (message, color) => {
            this.mainView.emit("UpdateView", "AddNotify", [message, color])
        })

        this.mainView.on("AddNotifyWithTitle", (message, color, title) => {
            this.mainView.emit("UpdateView", "AddNotify", [message, color, title])
        })

        this.mainView.on("SetFunkStatus", (status) => {
            this.mainView.emit("UpdateView", "SetFunkStatus", [status]);
        });

        this.mainView.on("VoiceSound", (fileName, loop, handle) => {
            voice.PlaySound(fileName, loop, handle)
        })

        this.mainView.on("VoiceStop", handle => {
            voice.StopSound(handle)
        })

        this.mainView.on("SendLocation", number => {
            alt.emitServer("SendPos", alt.Player.local.pos, number)
        })
        
        this.mainView.on("PackGun", inventoryId => {
            let weapon = native.getSelectedPedWeapon(alt.Player.local.scriptID)
            //Hand
            if(weapon != 2725352035)
            {
                alt.emitServer("PackGun", player.weapons[weapon])
            }
            else
            {
                this.updateView("AddNotify", ["Du musst die Waffe in der Hand halten!", "#c72020"])
            }
        })

        this.mainView.on("PlaySound", (id, name, ref, idk) => {
            let beepMuted = alt.LocalStorage.get("beepMute")
            if (beepMuted == null) {
                beepMuted = false
                alt.LocalStorage.set("beepMute", beepMuted)
                alt.LocalStorage.save()
            }
            else if (beepMuted == true)return;
            native.playSoundFrontend(id, name, ref, idk)
        })

        this.mainView.on("SaveAnim", (slot, anim) => {
            this.updateView("AddNotify", ["Animation " + anim[0] + " erfolgreich auf Slot " + slot + " gespeichert", "#0db600"])
            let anims = alt.LocalStorage.get("animsnew")

            if (anims == null) {
                anims = {}
            }
            anims[slot] = anim
            alt.LocalStorage.set("animsnew", anims)
            alt.LocalStorage.save()
        })


        this.mainView.on("ChangeSmsMute", status => {
            alt.LocalStorage.set("smsMute", status)
            alt.LocalStorage.save();
        })

        this.mainView.on("ChangeCallMute", status => {
            alt.LocalStorage.set("callMute", status)
            alt.LocalStorage.save();
        })

        this.mainView.on("ChangeBeepMute", status => {
            alt.LocalStorage.set("beepMute", status)
            alt.LocalStorage.save();
        })


        this.mainView.on("ChangeWallpaperId", id => {
            alt.LocalStorage.set("wallpaperId", id)
            alt.LocalStorage.save();
            this.mainView.emit("UpdateView", "UpdateWallpaper", [id])

        })

        this.mainView.on("ChangeRingtone", ringtone => {
            alt.LocalStorage.set("ringtoneId", ringtone)
            alt.LocalStorage.save();
            this.mainView.emit("UpdateView", "UpdateRingtone", [ringtone])
        })

        this.mainView.on("ChangeRingtoneVolume", volume => {
            alt.LocalStorage.set("volume", volume)
            alt.LocalStorage.save();
            this.mainView.emit("UpdateView", "UpdateRingtoneVolume", [volume])
        })


        let testObj;

        this.mainView.on("StopPaintball", () => {
            paintball.stopPaintball();
        })


        let casinoHash = 0
        let casinoPos = new alt.Vector3(0,0,0)
        this.mainView.on("EnterSlotMachineChair", (hash, x, y, z) => {
            casinoHash = hash
            casinoPos = new alt.Vector3(x, y, z)
            let player = alt.Player.local
            let offset = new alt.Vector3(0, -0.75, 0)
            let dict = "anim_casino_a@amb@casino@games@slots@ped_male@regular@01b@idles"
            let anim = "idle_c"
            if (player.gender == 0) 
            {
                dict = "anim_casino_a@amb@casino@games@slots@ped_female@no_heels@regular@02a@idles"
                anim = "idle_a"
                offset = new alt.Vector3(0, -0.75, 0.03)
            }
            
            testObj = native.getClosestObjectOfType(x, y, z, 0.5, hash, false, false, false)
            var offsetPos = native.getOffsetFromEntityInWorldCoords(testObj, offset.x, offset.y, offset.z);
            native.setEntityNoCollisionEntity(player.scriptID, testObj, false);
            native.setEntityCoords(player.scriptID, offsetPos.x, offsetPos.y, offsetPos.z, false, false, false, false);
            native.setEntityHeading(player.scriptID, native.getEntityHeading(testObj));
            native.freezeEntityPosition(player.scriptID, true);
            animation.playAnim(dict, anim, 1)
        })
        
        this.mainView.on("LeaveSlotMachineChair", () => {
            let player = alt.Player.local

            var obj = native.getClosestObjectOfType(casinoPos.x, casinoPos.y, casinoPos.z, 0.5 , casinoHash, false, false, true);
            var heading = native.getEntityHeading(obj);
            var offsetPos = native.getOffsetFromEntityInWorldCoords(obj, -0.5, -1.5, 0);
        
            native.clearPedTasksImmediately(player.scriptID);
            native.setEntityNoCollisionEntity(player.scriptID, obj, true);
            native.freezeEntityPosition(player.scriptID, false);
        
            //native.taskGoStraightToCoord(localPlayer.scriptID, offsetPos.x, offsetPos.y, offsetPos.z, 1.0, 5000, heading - 180, 100.0);
        })

        this.mainView.on("GetPlayerTeam", responseEventName => {
            this.mainView.emit("UpdateView", responseEventName, [player.team])
        })

        this.mainView.on("HidePlayerHud", (bool) => {
            this.mainView.emit("UpdateView", "HidePlayerHud", [bool])
        })

        this.mainView.on("GetPlayerStreetName", responseEventName => {
            let pos = alt.Player.local.pos;
            let streetHash = native.getStreetNameAtCoord(pos.x, pos.y, pos.z);
            let streetName = native.getStreetNameFromHashKey(streetHash[1]);
            // console.log(streetName);
            this.mainView.emit("UpdateView", responseEventName, [streetName]);
        })


        this.mainView.on("GetPhoneSettings", () => {
            let smsMute = alt.LocalStorage.get("smsMute")
            if (smsMute == null) {
              smsMute = false
            }
            alt.LocalStorage.set("smsMute", smsMute)
      
            let callMute = alt.LocalStorage.get("callMute")
            if (callMute == null) {
              callMute = false
            }
            alt.LocalStorage.set("callMute", callMute);

            let wallpaperId = alt.LocalStorage.get("wallpaperId")
            if (wallpaperId == null) {
                wallpaperId = 1
            }
            alt.LocalStorage.set("wallpaperId", wallpaperId);

            let ringtone = alt.LocalStorage.get("ringtoneId")
            if (ringtone == null) {
                ringtone = "0"
            }
            alt.LocalStorage.set("ringtoneId", ringtone);

            let volume = alt.LocalStorage.get("volume")
            if (volume == null) {
                volume = 25
            }
            alt.LocalStorage.set("volume", volume);


            alt.LocalStorage.save();
            this.mainView.emit("UpdateView", "SendPhoneSettings", [smsMute, callMute, ringtone, volume])
            this.mainView.emit("UpdateView", "UpdateWallpaper", [wallpaperId])
        })





        this.mainView.on("GetFunkSettings", () => {
            let funkFavorites = alt.LocalStorage.get("funkFavorites")
            if (funkFavorites == null) {
                funkFavorites = []
            }
            alt.LocalStorage.set("funkFavorites", funkFavorites)


            alt.LocalStorage.save();
            this.mainView.emit("UpdateView", "SendFunkSettings", [funkFavorites])
        })

        this.mainView.on("AddFunkSettings", frequenz => {
            let funkFavorites = alt.LocalStorage.get("funkFavorites")
            funkFavorites.push(frequenz)
            alt.LocalStorage.set("funkFavorites", funkFavorites)
            alt.LocalStorage.save();
        })

        this.mainView.on("RemoveFunkSettings", slot => {
            let funkFavorites = alt.LocalStorage.get("funkFavorites")
            funkFavorites.splice(slot, 1)
            alt.LocalStorage.set("funkFavorites", funkFavorites)
            alt.LocalStorage.save();
        })

        this.mainView.on("SetRotation", rot => {
            native.setEntityHeading(alt.Player.local.scriptID, parseFloat(rot))
        })

        this.mainView.on("SetTattoo", (overlay, collection) => {
            let overlayHash = alt.hash(overlay);
            let collectionHash = alt.hash(collection);
            player.setTattoos(player.tattoos)
            native.addPedDecorationFromHashes(alt.Player.local.scriptID, overlayHash, collectionHash)
        })

        this.mainView.on("ActualizeTattoos", () => {
            player.setTattoos(player.tattoos);
        })

        this.mainView.on("RemoveCloth", id => {
            if (id < 1) {
                id *= -1
                const w = player.props[id].w
                player.props[id] = undefined
                player.props[id].w = w
              } else {
                const w = player.cloth[id].w
                player.cloth[id] = undefined
                player.cloth[id].w = w
              }
        })

        this.mainView.on("ClearLocalWeapons", () => {
            player.weapons = {};
            player.oldWeapons = {};
            native.removeAllPedWeapons(alt.Player.local.scriptID, true);
        })

        this.mainView.on("SelectGangwarWeaponPack", (selection) => {
            let hash = 0;
            if (selection == 0) hash = 1627465347
            else if (selection == 1) hash = 3231910285
            else if (selection == 2) hash = 2937143193
            else if (selection == 3) hash = 3220176749
            else if (selection == 4) hash = 2210333304
            else if (selection == 5) hash = 2132975508

            player.weapons[hash] = 5000
            player.oldWeapons[hash] = 5000;
            native.giveWeaponToPed(alt.Player.local.scriptID, hash, 5000, 0, 1)

            player.weapons[3523564046] = 5000
            player.oldWeapons[3523564046] = 5000;
            native.giveWeaponToPed(alt.Player.local.scriptID, 3523564046, 5000, 0, 1)

            player.weapons[3441901897] = 5000
            player.oldWeapons[3441901897] = 5000;
            native.giveWeaponToPed(alt.Player.local.scriptID, 3441901897, 893, 0, 1)
        })

        this.mainView.on("SetAtmState", toggle => {
            player.isInAtm = toggle;
        })
    }

    clearProps() {
        player.cloth = {}
        player.props = {}
    }


    offsetPosition (x, y, rot, distance) {
        return {
            x: x + Math.sin(-rot * Math.PI / 180) * distance,
            y: y + Math.cos(-rot * Math.PI / 180) * distance,
        }
    }

    updateView(eventName, args = []) {
        this.mainView.emit("UpdateView", eventName, args)
    }

    showInterface(args) {
        this.mainView.emit("ShowIF", args)
        this.mainView.focus()
        cursor.show()
        alt.toggleGameControls(false)
        interfaces.open(args[0])
    }

    showInteractionMenu(items) {
        // alt.log("items " + JSON.stringify(items))
        this.mainView.focus()
        this.mainView.emit("UpdateView", "ShowIM", [items])
    }

    closeInterface() {
        this.mainView.emit("ShowIF", "")
        this.mainView.emit("UpdateView", "CloseIF");
        this.mainView.unfocus()
        cursor.hide()
        alt.toggleGameControls(true)
        interfaces.hide()

        if (this.camera == null) return
        native.setCamActive(this.camera, false)
        native.destroyCam(this.camera, false)
        native.renderScriptCams(false, false, 0, true, false, 0)
        if (this.freezeInterval != null) alt.clearInterval(this.freezeInterval)
        native.freezeEntityPosition(alt.Player.local.scriptID, false)
        this.camera = null
    }

    showChat() {
        this.mainView.focus()
        this.mainView.emit("UpdateView", "ShowChat")
        alt.toggleGameControls(false)
        interfaces.open("Chat")
    }

    hideHud() {
        alt.toggleGameControls(true)
        interfaces.hide()
    }

    hidePhone() {
        this.mainView.emit("UpdateView", "Phone", [false])
        this.mainView.unfocus()
        cursor.hide()
        // if (native.isPedUsingScenario(alt.Player.local.scriptID, "WORLD_HUMAN_STAND_MOBILE")) {
        //     native.clearPedTasksImmediately(alt.Player.local.scriptID)
        //     native.clearPedTasks(alt.Player.local.scriptID)
        // }
        native.clearPedTasks(alt.Player.local.scriptID)
        alt.emitServer("StopPropSync")
        alt.toggleGameControls(true)
        interfaces.hide()
        interfaces.phoneActive = false
    }

    hideFunk() {
        this.mainView.emit("UpdateView", "Funk", [false])
        native.stopAnimTask(alt.Player.local.scriptID, "random@arrests", "generic_radio_chatter", -4)
        this.mainView.unfocus()
        cursor.hide()
        
        alt.toggleGameControls(true)
        interfaces.hide()
        interfaces.funkActive = false
    }

    toggleMenCloth(id) {
        if (id > 0) {
            if (player.cloth[id] == undefined) {
                player.cloth[id] = { w: true, d: 0, t: 0 }
            }

            if (player.cloth[id].w) {
                if (id == 11) native.setPedComponentVariation(alt.Player.local.scriptID, 3, 15, 0, 0)
                native.setPedComponentVariation(alt.Player.local.scriptID, id, player.noClothMen[id][0], player.noClothMen[id][1], 0)
            } else {
                if (id == 11) {
                    if (player.cloth[3] == undefined) native.setPedComponentVariation(alt.Player.local.scriptID, 3, 0, 0, 0)
                    else native.setPedComponentVariation(alt.Player.local.scriptID, 3, player.cloth[3].d, player.cloth[3].t, 0)
                }
                native.setPedComponentVariation(alt.Player.local.scriptID, id, player.cloth[id].d, player.cloth[id].t, 0)
            }
            player.cloth[id].w = !player.cloth[id].w
        } else {
            id *= -1
            if (player.props[id] == undefined) return

            if (player.props[id].w) {
                native.clearPedProp(alt.Player.local.scriptID, id)
            } else {
                native.setPedPropIndex(alt.Player.local.scriptID, id, player.props[id].d, player.props[id].t, 1)
            }
            player.props[id].w = !player.props[id].w
        }
    }

    toggleWomenCloth(id) {
        if (id > 0) {
            if (player.cloth[id] == undefined) {
                player.cloth[id] = { w: true, d: 0, t: 0 }
            }

            if (player.cloth[id].w) {
                if (id == 11) native.setPedComponentVariation(alt.Player.local.scriptID, 3, 15, 0, 0)
                native.setPedComponentVariation(alt.Player.local.scriptID, id, player.noClothWomen[id][0], player.noClothWomen[id][1], 0)
            } else {
                if (id == 11) {
                    if (player.cloth[3] == undefined) native.setPedComponentVariation(alt.Player.local.scriptID, 3, 0, 0, 0)
                    else native.setPedComponentVariation(alt.Player.local.scriptID, 3, player.cloth[3].d, player.cloth[3].t, 0)
                }
                native.setPedComponentVariation(alt.Player.local.scriptID, id, player.cloth[id].d, player.cloth[id].t, 0)
            }
            player.cloth[id].w = !player.cloth[id].w
        } else {
            id *= -1
            if (player.props[id] == undefined) return

            if (player.props[id].w) {
                native.clearPedProp(alt.Player.local.scriptID, id)
            } else {
                native.setPedPropIndex(alt.Player.local.scriptID, id, player.props[id].d, player.props[id].t, 1)
            }
            player.props[id].w = !player.props[id].w
        }
    
    }
}

export default new WebView()