import alt from "alt"
import native from "natives"
import webview from "/api/modules/webview.mjs"
import speedlimiter from "/api/modules/speedlimiter.mjs"
import racing from "/api/modules/racing.mjs"
import animation from "/api/modules/animation.mjs"

class Vehicle {
    constructor() {
        this.fuelInterval = 0
        this.speedInterval = 0

        this.handbreakOn = false
        this.standing = false
        this.anchorOn = false
        this.km = 0
        this.distance = 0
        this.fuel = 0
        this.fuelConsumption = 0
        this.fuelConsumptionFactor = 0
        this.isEmpty = false
        this.vehicleData = []
        this.engine = true
        this.rpm = 0

        this.fuelCounter = 0
        
        this.isHeliOrPlane = false;

        this.vehicleTrunkDoors = [
            //-------------------------------Emergency--------------------------------
            {hash: 0x45D56ADA, trunkDoors: [2, 3]}, //Ambulance
            {hash: 0x1B38E955, trunkDoors: [2, 3]}, //Policet
            {hash: 0xB822A1AA, trunkDoors: [2, 3]}, //Riot
            //-------------------------------Commercials--------------------------------
            {hash: 0x7A61B330, trunkDoors: [5]}, //Benson
            {hash: 0x35ED670B, trunkDoors: [2, 3]}, //Mule
            {hash: 0xC1632BEB, trunkDoors: [5]}, //Mule2
            {hash: 0x85A5B471, trunkDoors: [2, 3]}, //Mule3
            {hash: 0x73F4110E, trunkDoors: [2, 3]}, //Mule4
            {hash: 0x35ED670B, trunkDoors: [2, 3]}, //Mule5
            {hash: 0x7DE35E7D, trunkDoors: [2, 3]}, //Pounder
            {hash: 0x6290F15B, trunkDoors: [2, 3]}, //Pounder2
            {hash: 0x6290F15B, trunkDoors: [2, 3]}, //Pounder2
            {hash: 0x6827CF72, trunkDoors: [2, 3]}, //Stockade
            {hash: 0xF337AB36, trunkDoors: [2, 3]}, //Stockade3
            {hash: 0x897AFC65, trunkDoors: []}, //Terbyte
            //-------------------------------Vans--------------------------------
            {hash: 0x898ECCEA, trunkDoors: [2, 3]}, //Boxville
            {hash: 0xF21B33BE, trunkDoors: [2, 3]}, //Boxville2
            {hash: 0x07405E08, trunkDoors: [2, 3]}, //Boxville3
            {hash: 0x1A79847A, trunkDoors: [2, 3]}, //Boxville4
            {hash: 0x28AD20E1, trunkDoors: [2, 3]}, //Boxville5
            {hash: 0xAFBB2CA4, trunkDoors: [2, 3]}, //Burrito
            {hash: 0xC9E8FF76, trunkDoors: [2, 3]}, //Burrito2
            {hash: 0x98171BD3, trunkDoors: [2, 3]}, //Burrito3
            {hash: 0x353B561D, trunkDoors: [2, 3]}, //Burrito4
            {hash: 0x437CF2A0, trunkDoors: [2, 3]}, //Burrito5
            {hash: 0x6FD95F68, trunkDoors: [3]}, //Camper
            {hash: 0x97FA4F36, trunkDoors: [2, 3]}, //Gburrito
            {hash: 0x11AA0E14, trunkDoors: [2, 3]}, //Gburrito2
            {hash: 0xF8D48E7A, trunkDoors: [2]}, //Journey
            {hash: 0x58B3979C, trunkDoors: [2, 3]}, //Paradise
            {hash: 0xF8DE29A8, trunkDoors: [2, 3]}, //Pony
            {hash: 0x38408341, trunkDoors: [2, 3]}, //Pony2
            {hash: 0x4543B74D, trunkDoors: [2, 3]}, //Rumpo
            {hash: 0x961AFEF7, trunkDoors: [2, 3]}, //Rumpo2
            {hash: 0xCFB3870C, trunkDoors: [2, 3]}, //Speedo
            {hash: 0x2B6DC64A, trunkDoors: [2, 3]}, //Speedo2
            {hash: 0xD17099D, trunkDoors: [2, 3]}, //Speedo4
            {hash: 0x03E5F6B8, trunkDoors: [2, 3]}, //Youga
            {hash: 0x3D29CD2B, trunkDoors: [2, 3]}, //Youga2
            {hash: 0x6B73A9BE, trunkDoors: [2, 3]}, //Youga3
            //-------------------------------Service--------------------------------
            {hash: 0x149BD32A, trunkDoors: [5]}, //Pbus2
            {hash: 0x72435A19, trunkDoors: [5]}, //Trash
            {hash: 0xB527915C, trunkDoors: [5]}, //Trash2
            //-------------------------------UTILITY--------------------------------
            //{hash: 0xCBB2BE0E, trunkDoors: [4, 5]}, //trailers funktioniert nicht - kein Interior beim Fahrzeug
            {hash: 0xA1DA3C91, trunkDoors: [2, 3]}, //trailers2 funktioniert nicht?
            {hash: 0x8548036D, trunkDoors: [5]}, //trailers3
            //{hash: 0xBE66F5AA, trunkDoors: [4, 5]}, //trailers4 funktioniert nicht - kein Interior beim Fahrzeug
        ]


        alt.onServer("VehEnter", (fuel, km, engineOn, multi) => {
            if (!racing.enabled)native.setVehicleEngineOn(alt.Player.local.vehicle.scriptID, engineOn, true, true)
            
            if (multi != undefined && multi != 0) native.modifyVehicleTopSpeed(alt.Player.local.vehicle.scriptID, multi)
            
            native.setVehicleMaxSpeed(alt.Player.local.vehicle.scriptID, -1)
            speedlimiter.init()

            this.engine = engineOn;

            km = parseFloat(km.toFixed(2));
            fuel =  parseFloat(fuel.toFixed(2));

            let vehicle = this.vehicleData.find(veh => veh.hash == alt.Player.local.vehicle.model || alt.hash(veh.hash) == alt.Player.local.vehicle.model);
            let maxFuel = vehicle.maxFuel

            webview.updateView("ShowVehHUD", [true, {
                doorOpen: true,
                engineOn,
                trunkOpen: alt.Player.local.vehicle.getStreamSyncedMeta("trunk"),
                speed: 0,
                km,
                fuel,
                maxFuel
            }])

            this.km = km
            this.fuel = fuel
            this.isEmpty = fuel == 0
            const isDriver = alt.Player.local.scriptID == native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false)
            if (isDriver && !this.isEmpty)
            {
                this.fuelInterval = alt.setInterval(this.calculateFuel.bind(this), 1000)
                let vehData = vehicle
                if(vehData == undefined)
                {
                    this.fuelConsumptionFactor = 1.0
                }
                else
                {
                    this.fuelConsumptionFactor = vehData.fuelConsumption
                }
            }
            
            this.isHeliOrPlane = native.isThisModelAHeli(alt.Player.local.vehicle.model) || native.isThisModelAPlane(alt.Player.local.vehicle.model);

            this.speedInterval = alt.setInterval(() => {
                if (alt.Player.local.vehicle == null) {
                    webview.updateView("ShowVehHUD", [false])
                    if (isDriver && !this.isEmpty) {
                        alt.emitServer("VehUpdate", parseInt(Number(this.fuelConsumption.toFixed(2)) * 100), parseInt(Number(this.distance.toFixed(2)) * 100))
                        this.fuelConsumption = 0
                        this.distance = 0
                        this.fuelCounter = 0
                        this.isEmpty = true
                    }
                    
                    if (this.handbreakOn) {
                        this.handbreakOn = false
                        native.setVehicleHandbrake(native.getLastDrivenVehicle(), this.handbreakOn)
                        webview.updateView("AddNotify", ["Handbremse gelöst", "#00a6cc"])
                    }
                    
                    if (this.fuelInterval != 0) {
                        alt.clearInterval(this.fuelInterval)
                        this.fuelInterval = 0;
                    }
                    if (this.speedInterval != 0) {
                        alt.clearInterval(this.speedInterval)
                        this.speedInterval = 0;
                    }
                    return
                }

                const speed = Math.floor(native.getEntitySpeed(alt.Player.local.vehicle.scriptID) * 3.6)
                //const speed = Math.floor(alt.Player.local.vehicle.speed * 3.6)
                if (speed == 0 && !this.standing) {
                    webview.updateView("UpdateVehHUD", ["speed", speed])
                    this.standing = true
                    return
                }
                
                if (speed != 0) {
                    webview.updateView("UpdateVehHUD", ["speed", speed])
                    this.standing = false
                }

                // RPM & GEAR
                const rpm = alt.Player.local.vehicle.rpm;
                webview.updateView("UpdateVehHUD", ["rpm", rpm.toFixed(2)])
                
                if (this.isHeliOrPlane) {
                    const feet = Math.round(alt.Player.local.vehicle.pos.z * 3.281);
                    webview.updateView("UpdateVehHUD", ["gear", feet])
                } else {
                    const gear = alt.Player.local.vehicle.gear;
                    webview.updateView("UpdateVehHUD", ["gear", gear])
                }
            }, 50)
        })

        alt.onServer("VehEngine", status => {
            webview.updateView("UpdateVehHUD", ["engineOn", status])
        })

        alt.onServer("VehDoor", status => {
            webview.updateView("UpdateVehHUD", ["doorOpen", !status])
        })

        alt.onServer("VehData", (fuel, km) => {
            webview.updateView("UpdateVehHUD", ["km", km.toFixed(2)])
            // webview.updateView("UpdateVehHUD", ["km", km.toFixed(2).replace(".", ",")])
        })

        alt.onServer("VehLock", async (state, veh, vehicleId, name) => {
            native.setVehicleDoorsLocked(veh, state ? 2 : 1)
            const title = "(" + vehicleId + ") - " + name;
            if(alt.Player.local.vehicle == null)
            {
                await animation.playAnim("anim@mp_player_intmenu@key_fob@", "fob_click_fp", 48)
            }
            if(state)
            {
                webview.updateView("AddNotifyWithTitle", ["Fahrzeug abgeschlossen", "#c72020", title])
                //native.playVehicleDoorCloseSound(veh, 1)
                native.setVehicleLights(veh, 2)
                alt.setTimeout(() => {
                    native.setVehicleLights(veh, 0)
                    alt.setTimeout(() => {
                        native.setVehicleLights(veh, 2)
                        alt.setTimeout(() => {
                            native.setVehicleLights(veh, 0)
                        }, 300)
                    }, 300)
                }, 450)
            }
            else
            {
                webview.updateView("AddNotifyWithTitle", ["Fahrzeug aufgeschlossen", "#0db600", title])
                //native.playVehicleDoorOpenSound(veh, 1)
                native.setVehicleLights(veh, 2)
                alt.setTimeout(() => {
                    native.setVehicleLights(veh, 0)
                    alt.setTimeout(() => {
                        native.setVehicleLights(veh, 2)
                        alt.setTimeout(() => {
                            native.setVehicleLights(veh, 0)
                        }, 300)
                    }, 300)
                }, 300)
            }
        })
    }

    openTrunk(entity, instant) {
        let doors = this.vehicleTrunkDoors.find(d => d.hash == entity.model);
        if(doors != undefined)
        {
            doors.trunkDoors.forEach(door => {
            native.setVehicleDoorOpen(entity.scriptID, door, false, instant)
            });
        }
        else
        {
            native.setVehicleDoorOpen(entity.scriptID, 5, false, instant)
        }
    }

    closeTrunk(entity, instant) {
        let doors = this.vehicleTrunkDoors.find(d => d.hash == entity.model);
        if(doors != undefined)
        {
            doors.trunkDoors.forEach(door => {
            native.setVehicleDoorShut(entity.scriptID, door, instant)
            });
        }
        else
        {
            native.setVehicleDoorShut(entity.scriptID, 5, instant)
        }
    }

    calculateFuel() {
        if (alt.Player.local.vehicle == null) return
        const rpm = alt.Player.local.vehicle.rpm.toFixed(2)
        if (rpm >= 0.200 && native.getIsVehicleEngineRunning(alt.Player.local.vehicle.scriptID)) {
            //alt.log("FuelCounter: " + this.fuelCounter)
            
            this.fuelCounter++
            let speed = native.getEntitySpeed(alt.Player.local.vehicle.scriptID)
            let rpmConsumption = 17 * rpm / 1024;
            let speedConsumption = (2 * speed) / 5120
            //alt.log("Verbrauch: " + (this.fuelConsumptionFactor * (speedConsumption + rpmConsumption)))
            this.fuelConsumption += (this.fuelConsumptionFactor * (speedConsumption + rpmConsumption))

            //alt.log("rpm: " + rpm + ", speed: " + speed + ", factor: " + this.fuelConsumptionFactor + ", fuelConsumption: " + this.fuelConsumption)

            this.distance += speed * 0.001
    
            if (this.fuelCounter >= 20) {
                this.updateFuelAndDistance()
            }
        }
        else
        {
            if(this.fuelCounter > 0)
            {
                this.updateFuelAndDistance()
            }
        }
    }

    updateFuelAndDistance() {
        this.fuel -= Number(this.fuelConsumption.toFixed(2))
        this.km += Number(this.distance.toFixed(2))
        if (this.fuel <= 0) {
            this.fuel = 0
            this.isEmpty = true
            this.fuelConsumption = 10;
            if (this.fuelInterval != 0) {
                alt.clearInterval(this.fuelInterval)
                this.fuelInterval = 0;
            }
        }
        this.sendFuelAndDistanceToServer()
    }

    sendFuelAndDistanceToServer() {
        let fuelConsumption = this.fuelConsumption.toFixed(2)
        this.fuelConsumption = 0
        let dist = this.distance.toFixed(2)
        this.distance = 0
        fuelConsumption = Number(fuelConsumption)
        dist = Number(dist)
        alt.emitServer("VehUpdate", parseInt(fuelConsumption * 100), parseInt(dist * 100))
        webview.updateView("UpdateVehHUD", ["km", this.km.toFixed(2)])
        webview.updateView("UpdateVehHUD", ["fuel", this.fuel.toFixed(2)])
        this.fuelCounter = 0
    }

    toggleHandbreak(vehicle) {
        if (alt.Player.local.scriptID != native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false)) {
            webview.updateView("AddNotify", ["Nur am Fahrersitz möglich", "#c72020"])
            return
        }

        this.handbreakOn = !this.handbreakOn
        native.setVehicleHandbrake(vehicle.scriptID, this.handbreakOn)
        if (this.handbreakOn)
            webview.updateView("AddNotify", ["Handbremse angezogen", "#00a6cc"])
        else
            webview.updateView("AddNotify", ["Handbremse gelöst", "#00a6cc"])
    }

    toggleAnchor(vehicle) {
        if (alt.Player.local.scriptID != native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false)) {
            webview.updateView("AddNotify", ["Nur am Fahrersitz möglich", "#c72020"])
            return
        }

        // if(!native.canAnchorBoatHere(vehicle.scriptID))
        // {
        //     webview.updateView("AddNotify", ["An dieser Stelle kann kein Anker gesetzt werden", "#c72020"])
        //     return
        // }

        if(alt.Player.local.vehicle.speed * 3.6 > 10)
        {
            webview.updateView("AddNotify", ["Anker kann nur bei geringer Geschwindigkeit gesetzt werden", "#c72020"])
            return
        }

        this.anchorOn = !this.anchorOn
        native.setBoatFrozenWhenAnchored(alt.Player.local.vehicle.scriptID, true);
        native.setBoatAnchor(vehicle.scriptID, this.anchorOn)
        if (this.anchorOn)
            webview.updateView("AddNotify", ["Anker gesetzt", "#00a6cc"])
        else
            webview.updateView("AddNotify", ["Anker eingeholt", "#00a6cc"])
    }

    shuffleVehicle(vehicle) {
        const driver = native.getPedInVehicleSeat(vehicle.scriptID, -1, false)
        if(alt.Player.local.vehicle != null)
            {
            // if(native.getEntitySpeed(alt.Player.local.vehicle.scriptID) * 3.6 > 5) 
            // {
            //     webview.updateView("AddNotify", ["Sitzplatz kann nicht während der Fahrt gewechselt werden", "#00a6cc"])
            //     return;
            // }
            if (driver == alt.Player.local.scriptID) {
                native.taskShuffleToNextVehicleSeat(alt.Player.local.scriptID, vehicle.scriptID, 1)
            } else {
                if(!native.isVehicleSeatFree(vehicle.scriptID, -1, false)) {
                    if (!native.isPedDeadOrDying(driver, 1)) return
                }

                native.taskShuffleToNextVehicleSeat(alt.Player.local.scriptID, vehicle.scriptID, 1)
            }
        }
    }
}

export default new Vehicle()