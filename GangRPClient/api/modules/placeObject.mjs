import alt from "alt-client";
import native from "natives";
import player from "./player.mjs"
import instructionalKeys from "./instructionalKeys.mjs";
import webview from "./webview.mjs";
import noclip from "./noclip.mjs";
import raycast from "./raycast.mjs"

class PlaceObject {
    constructor() {

        this.placeObjectDatas = JSON.parse(alt.File.read("@SibauiRP_Assets/data/placeObjectData.json"));
        this.active = false;
        
        let headingOffset = 0;
        let pitchOffset = 0;
        let rollOffset = 0;
        let zOffset = 0;

        alt.onServer("RqPlaceObject", async (id, eventObject) => {
            let obj = this.placeObjectDatas.find(data => data.id == id);
            let isCustomObject = false;
            if(obj == null) {
                obj = {
                    "id": 0,
                    "model": id,
                    "offsetPosition": {
                        "X": 1,
                        "Y": 1,
                        "Z": 0
                    },
                    "offsetRotation": {
                        "Roll": 0,
                        "Pitch": 0,
                        "Yaw": 0
                    },
                    "deployDuration": 0
                }
                isCustomObject = true;
            }
            this.active = true;

            let position = alt.Player.local.pos;
            let prop = native.createObject(alt.hash(obj.model), position.x + obj.offsetPosition.X, position.y + obj.offsetPosition.Y, position.z + obj.offsetPosition.Z, false, false, false);
            native.setEntityAlpha(prop, 102, true);
            native.setEntityCollision(prop, false, false)

            await instructionalKeys.init();
            instructionalKeys.addKey(175, "Nach rechts drehen");
            instructionalKeys.addKey(174, "Nach links drehen");
            if(noclip.enabled)
            {
                instructionalKeys.addKey(172, "Nach oben drehen");
                instructionalKeys.addKey(173, "Nach unten drehen");
                instructionalKeys.addKey(10, "Nach vorne drehen");
                instructionalKeys.addKey(11, "Nach hinten drehen");
                instructionalKeys.addKey(101, "Nach oben bewegen");
                instructionalKeys.addKey(113, "Nach unten bewegen");
                instructionalKeys.addKey(45, "Reset");
            }
            instructionalKeys.addKey(25, "Abbrechen");
            instructionalKeys.addKey(24, "Objekt Platzieren");
            instructionalKeys.finish();

            let counter = 0;

            let slideEveryTick = alt.everyTick(() => {
                native.disableControlAction(0, 22, true);
                native.disableControlAction(0, 24, true);
                native.disableControlAction(0, 25, true);
                native.disableControlAction(0, 257, true);
                native.disableControlAction(0, 142, true);

                let pos = alt.Player.local.pos;
                native.setEntityRotation(prop, obj.offsetRotation.Roll, obj.offsetRotation.Pitch, obj.offsetRotation.Yaw + native.getEntityHeading(alt.Player.local) + headingOffset, 2, true)

                counter += 1;

                if(!noclip.enabled)
                {
                    let forward = native.getEntityForwardVector(alt.Player.local.scriptID);
                    let pos3 = new alt.Vector3(pos.x+forward.x*obj.offsetPosition.X, pos.y+forward.y*obj.offsetPosition.Y, pos.z + obj.offsetPosition.Z);
                    native.setEntityCoordsNoOffset(prop, pos3.x, pos3.y, pos3.z, false, false, false);
                    native.placeObjectOnGroundProperly(prop);
                }
                else
                {
                    let [_,_hit,_endCoords,_surfaceNormal,_materialHash,_entityHit] = raycast.getRaycastResult(250);
                    if(_hit)
                    {
                        native.setEntityRotation(prop, obj.offsetRotation.Roll + rollOffset, obj.offsetRotation.Pitch + pitchOffset, obj.offsetRotation.Yaw + native.getEntityHeading(alt.Player.local) + headingOffset, 2, true)
                        native.setEntityCoordsNoOffset(prop, _endCoords.x, _endCoords.y, _endCoords.z + zOffset, false, false, false);
                    }
                }

                if(noclip.enabled)
                {
                    //g
                    if(native.isControlPressed(0, 113)){
                        zOffset-=0.005;
                    }
                    //h
                    if(native.isControlPressed(0, 101)){
                        zOffset+=0.005;
                    }
                    //reset
                    if(native.isControlPressed(0, 45)){
                        pitchOffset = 0;
                        headingOffset = 0;
                        rollOffset = 0;
                        zOffset = 0;
                    }
                    //pfeil oben
                    if(native.isControlPressed(0, 172)){
                        pitchOffset+=0.5;
                    }
                    //pfeil unten
                    if(native.isControlPressed(0, 173)){
                        pitchOffset-=0.5;
                    }
                    //page up
                    if(native.isControlPressed(0, 10)){
                        rollOffset+=0.5;
                    }
                    //page down
                    if(native.isControlPressed(0, 11)){
                        rollOffset-=0.5;
                    }
                }
                //pfeil links
                if(native.isControlPressed(0, 174)){
                    headingOffset+=0.5;
                }
                //pfeil rechts
                if(native.isControlPressed(0, 175)){
                    headingOffset-=0.5;
                }
                // Confirm
                if(native.isDisabledControlJustPressed(0, 24)){
                    if (eventObject || (!native.isEntityInAir(prop) && !native.isEntityInWater(prop))) {
                        alt.clearEveryTick(slideEveryTick);
                        slideEveryTick = null;
                        native.disableControlAction(0, 22, false);
                        native.disableControlAction(0, 24, false);
                        native.disableControlAction(0, 257, false);
                        native.disableControlAction(0, 142, false);
                        let entityPos = native.getEntityCoords(prop, false);
                        let entityPosModified = new alt.Vector3(entityPos.x, entityPos.y, entityPos.z + obj.offsetPosition.Z);
                        let entityRot = native.getEntityRotation(prop, 2);
                        if(isCustomObject)
                        {
                            alt.emitServer("RsPlaceCustomObject", obj.model, entityPosModified, entityRot);
                        }
                        else
                        {
                            alt.emitServer("RsPlaceObject", id, entityPosModified, entityRot);
                        }
                        this.active = false;
                        if(eventObject)
                        {
                            native.deleteObject(prop);
                        }
                        else
                        {
                            alt.setTimeout(() => native.deleteObject(prop), obj.deployDuration + 300);
                        }
                    }
                    else
                    {
                        webview.updateView("AddNotify", ["Ungültige Position!", "#c72020"]);
                    }
                }
                // Abbrechen
                if(native.isDisabledControlJustPressed(0, 25) || alt.Player.local.vehicle != null){
                    alt.clearEveryTick(slideEveryTick);
                    slideEveryTick = null;
                    native.deleteObject(prop);
                    native.disableControlAction(0, 22, false);
                    native.disableControlAction(0, 24, false);
                    native.disableControlAction(0, 257, false);
                    native.disableControlAction(0, 142, false);
                    this.active = false;
                }

                instructionalKeys.draw();
            });
        });
    }
}

export default new PlaceObject()