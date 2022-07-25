import alt from "alt"
import native from "natives"

class Enter {
    constructor() {
        this.model = null
        this.distance = 0
        this.bones = [ // count 17
            "seat_pside_f",//0
            "seat_dside_r",//1
            "seat_pside_r",//2
            "seat_dside_r1",//3
            "seat_pside_r1",//4
            "seat_dside_r2",//5
            "seat_pside_r2",//6
            "seat_dside_r3",//7
            "seat_pside_r3",//8
            "seat_dside_r4",//9
            "seat_pside_r4",//10
            "seat_dside_r5",//11
            "seat_pside_r5",//12
            "seat_dside_r6",//13
            "seat_pside_r6",//14
            "seat_dside_r7",//15
            "seat_pside_r7"//16
        ]
    }
    // native.isSeatWarpOnly()
    getClosestVehicle() {
        this.distance = 5
        let closest = null
        for (const vehicle of alt.Vehicle.streamedIn) {
            if (vehicle.scriptID == 0) continue
            const dist = this.getDistance(vehicle.pos)

            if (dist > this.distance) {
                continue
            }

            if (closest == null) {
                closest = vehicle
            }

            if (dist < this.getDistance(closest.pos)) {
                this.model = vehicle.model
                closest = vehicle
            }
        }
        
        return closest
    }

    getClosestVehicleSeat(vehicle) {
        let closestSeat = 0
        let closestDistance = this.distance

        for (let i = 0; i < this.bones.length; i++) {
            const boneName = this.bones[i]
            const seat = this.bones.indexOf(boneName)
            const bonePosition = native.getWorldPositionOfEntityBone(vehicle.scriptID, native.getEntityBoneIndexByName(vehicle.scriptID, boneName))
            const distance = this.getDistance(bonePosition)
            if (distance < closestDistance && native.isVehicleSeatFree(vehicle.scriptID, parseInt(seat), false)) {
                closestSeat = seat
                // alt.log("closestSeat " + closestSeat)
                closestDistance = distance
                // alt.log("closestDistance " + closestDistance)
            }
        }
        
        // if (closestSeat == 0) {
        //     if (native.isThisModelABike(vehicle.model)) closestSeat = 1
        // }
        //closestSeat = this.getSpecialSeatByModel(vehicle, closestSeat, this.model)

        return closestSeat
    }

    getSpecialSeatByModel(vehicle, closestSeat, model) {
        //Granger, Fbi2, Sheriff2, Pranger
        if (model == 0x9628879C || model == 0x9DC66994 || model == 0x72935408 || model == 0x2C33B46E) {
            if (closestSeat == 5) {
                if (!native.isVehicleSeatFree(vehicle.scriptID, 1, false)) {
                    return 5
                }
                return 1
            }
            if (closestSeat == 4) {
                if (!native.isVehicleSeatFree(vehicle.scriptID, 0, false)) {
                    return 4
                }
                return 0
            }
            if (closestSeat == 6) {
                if (!native.isVehicleSeatFree(vehicle.scriptID, 2, false)) {
                    return 6
                }
                return 2
            }
        }
        // else if (this.getVehicleTeleportLimitSeat() != 99 && closestSeat >= this.getVehicleTeleportLimitSeat()) {
        //     for (let i = this.getVehicleTeleportLimitSeat(); i < 15; i++) {
        //         if (native.isVehicleSeatFree(vehicle.scriptID, i)) {
        //             this.teleport = true
        //             return i
        //         }
        //     }
        // }

        return model
    }

    enterVehicle(vehicle, seat) {
        // if (this.teleport) {
        //     if (native.getVehicleDoorsLockedForPlayer(vehicle.scriptID, alt.Player.local.scriptID)) return
        //     native.setPedIntoVehicle(alt.Player.local.scriptID, vehicle.scriptID, seat)
        //     return
        // }

        if (native.getVehicleDoorLockStatus(vehicle.scriptID) === 2) {
            let i = 0
            const interval = alt.setInterval(() => {
              if (i === 15) {
                alt.clearInterval(interval)
                return;
              }
              if (native.getVehicleDoorLockStatus(vehicle.scriptID) === 1) {
                native.taskEnterVehicle(alt.Player.local.scriptID, vehicle.scriptID, -1, parseInt(seat), 2, 0, 0)
                alt.clearInterval(interval)
                return
              }
              i++
            }, 200)
        }

        native.taskEnterVehicle(alt.Player.local.scriptID, vehicle.scriptID, -1, parseInt(seat), 2, 0, 0)
    }

    getVehicleTeleportLimitSeat() {

        switch(this.model) {
            // Einsteigen ab 3 Sitze
            case 0x3412AE2D: //Sentinel2
            case 0xFCFCB68B: //Cargobob
            case 0x60A7EA10: //Cargobob
            case 0x53174EEF: //Cargobob
            case 0x78BC1A3C: //Cargobob
            case 0x250B0C5E: //Luxor
            case 0xB79C1BF5: //Shamal
            case 0x9C429B6A: //Velum
            case 0x9D80F93:  //Miljet
            case 0xB2CF7250: //Nimbus
            case 0x761E2AD3: //Titan
            case 0xF8D48E7A: //Journey
                return 1
            // Einsteigen ab 2 Sitz
            case 0xC1CE1183: //Marquis
            case 0xD577C962: //Bus
            case 0x4C80EB0E: //Airbus
            case 0x84718D34: //Coach
            case 0x885F3671: //Pbus
            case 0x73B1C3CB: //Tourbus
            case 0xBE819C63: //Rental
            case 0x56590FE9: //Tropic2
            case 0x1149422F: //Tropic
            case 0x362CAC6D: //Toro2
            case 0x3FD5AA2F: //Toro
            case 0xEF2295C9: //Suntrap
            case 0x72435A19: //Trash
            case 0xB527915C: //Trash2
                return 0
            // Einsteigen ab 5 Sitz
            case 0x8B13F083: //Stretch
            case 0xE6E967F8: //Patriot 2
                return 3
            default:
                return 99
        }
    }

    getDistance(position) {
        const playerPosition = alt.Player.local.pos
        return Math.sqrt(
            (playerPosition.x - position.x) * (playerPosition.x - position.x) +
            (playerPosition.y - position.y) * (playerPosition.y - position.y) +
            (playerPosition.z - position.z) * (playerPosition.z - position.z)
        )
    }
    // Math.pow(playerPosition.x - position.x, 2) +
    //         Math.pow(playerPosition.y - position.y, 2) +
    //         Math.pow(playerPosition.z - position.z, 2)
}

export default new Enter()