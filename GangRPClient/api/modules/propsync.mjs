import * as alt from "alt-client";
import * as native from "natives";
import webview from "/api/modules/webview.mjs"
import player from "/api/modules/player.mjs"
import animation from "/api/modules/animation.mjs"

class Propsync {
    constructor() {
        this.props = JSON.parse(alt.File.read("@SibauiRP_Assets/data/propsyncdata.json"))
        this.active = []


        this.temp = null;

        alt.onServer("PlayPropSync", async (type, posX, posY, posZ, rotX, rotY, rotZ) => {

            if (this.temp != null) {
                native.detachEntity(this.temp, true, true);
                native.deleteObject(this.temp);
            }

            let entityId = alt.Player.local.scriptID
            let p = alt.Player.local;
            let pos = p.pos;
            let propInfo = this.props[type];
            let obj = native.createObject(alt.hash(propInfo.prop), pos.x, pos.y, pos.z, false, true, false);
            native.attachEntityToEntity(obj, entityId, native.getPedBoneIndex(entityId, propInfo.bone), posX, posY, posZ, rotX, rotY, rotZ, false, false, false, true, 2, true);
            if (propInfo.anim != undefined) {
                await animation.playAnim(propInfo.animDict, propInfo.anim, propInfo.flag)
            }
            this.temp = obj
        })
    }

    async start(entity, type) {
        let p = alt.Player.local;
        let propInfo = this.props.find(prop => prop.id == type);
        let counter = 0;
        const interval = alt.setInterval(()  => {
            // const playerScriptId = entity.scriptID
            if (entity.valid) {
                let entityId = entity.scriptID
                let pos = entity.pos;

                if (entityId == p.scriptID) {
                    animation.playAnim(propInfo.animDict, propInfo.anim, propInfo.flag)
                }

                let obj = native.createObject(alt.hash(propInfo.prop), pos.x, pos.y, pos.z, false, true, false);
                native.attachEntityToEntity(obj, entityId, native.getPedBoneIndex(entityId, propInfo.bone), propInfo.posOffset.x, propInfo.posOffset.y, propInfo.posOffset.z, propInfo.rotOffset.x, propInfo.rotOffset.y, propInfo.rotOffset.z, false, false, false, true, 2, true);
                this.active[entityId] = obj;
                alt.clearInterval(interval)
                return
            } else {
                counter++
                if (counter >= 100) {
                    alt.clearInterval(interval)
                    return
                }
            }
        }, 10)

    }
    stop(entity) {
        if (this.active[entity.scriptID] != undefined) {
            let p = alt.Player.local;
            let obj = this.active[entity.scriptID]
            if (entity.scriptID == p.scriptID) native.clearPedTasks(p.scriptID);

            native.detachEntity(obj, true, true);
            native.deleteObject(obj);
            this.active[entity.scriptID] = undefined
        }
    }

    cancel() {

        let propSyncId = alt.Player.local.getStreamSyncedMeta("propSync")
        if (propSyncId) {
            let prop = this.props.find(prop => prop.id == propSyncId-1);
            if (prop.IsCancelAble) {
                alt.emitServer("StopPropSync");
                native.clearPedTasks(alt.Player.local.scriptID)
                player.disableActions = false
            }
        }
        else {
            native.clearPedTasks(alt.Player.local.scriptID)
            player.disableActions = false
        }
    }
}

export default new Propsync()