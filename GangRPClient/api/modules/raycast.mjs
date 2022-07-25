import alt from "alt"
import native from "natives"

class Raycast {
    // constructor() {
    //     alt.everyTick(() => {
    //         if (this.position == null) return
    //         native.drawLine(this.position.x, this.position.y, this.position.z, this.away.x, this.away.y, this.away.z, 255, 0, 0, 255)
    //     })

    //     this.position = null
    //     this.away = null
    // }

    getRaycastResult(range = 8) {
        const position = native.getGameplayCamCoord()
        const direction = this.getDirection(native.getGameplayCamRot(2))
        const away = new alt.Vector3(
            direction.x * range + position.x,
            direction.y * range + position.y,
            direction.z * range + position.z
        )
        // this.position = position
        // this.away = away

        //alt.log("position " + JSON.stringify(this.position))
        //alt.log("away " + JSON.stringify(this.away))
        //alt.log("coords " + JSON.stringify(alt.Player.local.pos))

        const raycast = native.startExpensiveSynchronousShapeTestLosProbe(
            position.x,
            position.y,
            position.z,
            away.x,
            away.y,
            away.z,
            4294967295, //vehicles 2, peds 8, object 16
            alt.Player.local.scriptID,
            4
        )
        
        return native.getShapeTestResult(raycast)
    }

    getEntity(range = 8) {
        const position = native.getGameplayCamCoord()
        const direction = this.getDirection(native.getGameplayCamRot(2))
        const away = new alt.Vector3(
            direction.x * range + position.x,
            direction.y * range + position.y,
            direction.z * range + position.z
        )
        // this.position = position
        // this.away = away

        //alt.log("position " + JSON.stringify(this.position))
        //alt.log("away " + JSON.stringify(this.away))
        //alt.log("coords " + JSON.stringify(alt.Player.local.pos))

        const raycast = native.startExpensiveSynchronousShapeTestLosProbe(
            position.x,
            position.y,
            position.z,
            away.x,
            away.y,
            away.z,
            4294967295, //vehicles 2, peds 8, object 16
            alt.Player.local.scriptID,
            4
        )
        
        const [_, hasHit, endCoords, surfaceNormal, entity] = native.getShapeTestResult(raycast)

        // alt.log("HasHit: " + hasHit)
        // alt.log("entity: " + entity)
        if (hasHit) {
            return entity
        }

        return null
    }

    getDirection(rotation) {
        const z = rotation.z * (Math.PI / 180.0)
        const x = rotation.x * (Math.PI / 180.0)
        const num = Math.abs(Math.cos(x))
    
        return new alt.Vector3(
            (-Math.sin(z) * num),
            (Math.cos(z) * num),
            Math.sin(x)
        )
    }
}

export default new Raycast()