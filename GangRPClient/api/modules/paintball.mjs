import alt from "alt-client";
import native from "natives";
import webview from "/api/modules/webview.mjs"
import player from "/api/modules/player.mjs"
import sibsib from "/api/modules/sibsib.mjs"

class Paintball {
    constructor() {
        this.active = false;
        this.stats = {};
        this.positionCheckerInterval = null;
        this.centerPosition = null;
        this.range = null;
        this.isPlayerOutside = false;
        this.activeIpl = null;

        alt.onServer("RespawnPaintball", (hp, armor) => {
            sibsib.hpAndArmorBlockCounter = 5;
            native.setEntityInvincible(alt.Player.local.scriptID, true);
            alt.setTimeout(() => {
                sibsib.setHP(hp);
                sibsib.setArmor(armor);
                let weaponHash = native.getBestPedWeapon(alt.Player.local.scriptID, 0);
                native.setCurrentPedWeapon(alt.Player.local.scriptID, weaponHash, true);
                native.setEntityInvincible(alt.Player.local.scriptID, false);
            }, 1500);
        });

        alt.onServer("StartPaintball", (team, type, map, pos, range) => {
            webview.closeInterface();
            player.weapons = {};
            player.oldWeapons = {};
            native.removeAllPedWeapons(alt.Player.local.scriptID, true);
            this.active = true;
            this.centerPosition = pos;
            this.range = range;

            switch(map)
            {
                case 2:
                    this.activeIpl = "p_containerhafen"
                    break;
                case 10:
                    this.activeIpl = "p_geewarehouse"
                    break;
                default:
                    this.activeIpl = null
                    break;
            }

            if(this.activeIpl != null)
            {
                alt.requestIpl(this.activeIpl)
            }
            
            this.positionCheckerInterval = alt.setInterval(this.checkPosition.bind(this), 5000);

            let weaponHashes = []
            switch(type)
            {
                case 1: //Deathmatch
                    sibsib.setHP(200);
                    sibsib.setArmor(100);
                    weaponHashes.push(1593441988); //Combat Pistol
                    weaponHashes.push(1627465347); //Gusenberg
                    weaponHashes.push(3220176749); //Assaultrifle
                    weaponHashes.push(2937143193); //AdvancedRifle
                    weaponHashes.push(2132975508); //Bullpuprifle
                    weaponHashes.push(2210333304); //Carbinerifle
                    weaponHashes.push(3231910285); //Special Carbine
                    break;
                case 2: //TeamDeathmatch
                    sibsib.setHP(200);
                    sibsib.setArmor(100);
                    weaponHashes.push(1593441988); //Combat Pistol
                    weaponHashes.push(3220176749); //Assaultrifle
                    weaponHashes.push(2937143193); //AdvancedRifle
                    weaponHashes.push(2132975508); //Bullpuprifle
                    weaponHashes.push(2210333304); //Carbinerifle
                    weaponHashes.push(3231910285); //Special Carbine
            
                    switch(team)
                    {
                        case 1: //Team 1 (Fuchs)
                            if (native.isPedMale(alt.Player.local.scriptID))
                            {
                                native.setPedComponentVariation(alt.Player.local.scriptID, 1, 171, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 3, 1, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 4, 31, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 6, 24, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 8, 15, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 11, 352, 0, 0)
                            }
                            else
                            {
                                native.setPedComponentVariation(alt.Player.local.scriptID, 1, 171, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 3, 3, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 4, 30, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 6, 24, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 8, 2, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 11, 370, 0, 0)
                            }
                            break;
                        case 2: //Team 2 (Schwein)
                            if (native.isPedMale(alt.Player.local.scriptID))
                            {
                                native.setPedComponentVariation(alt.Player.local.scriptID, 1, 173, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 3, 1, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 4, 31, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 6, 24, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 8, 15, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 11, 262, 14, 0)
                            }
                            else
                            {
                                native.setPedComponentVariation(alt.Player.local.scriptID, 1, 173, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 3, 3, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 4, 30, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 6, 24, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 8, 2, 0, 0)
                                native.setPedComponentVariation(alt.Player.local.scriptID, 11, 271, 14, 0)
                            }
                            break;
                    }
                    break;
                case 3: //GunGame
                    sibsib.setHP(200);
                    sibsib.setArmor(100);
                    weaponHashes.push(4024951519); //AssaultSMG
                    break;
                case 4: //Revolver
                    sibsib.setHP(140);
                    sibsib.setArmor(0);
                    weaponHashes.push(3249783761); //Revolver
                    break;
                case 5: //Sniper
                    sibsib.setHP(130);
                    sibsib.setArmor(0);
                    weaponHashes.push(177293209); //Sniper
                    break;
            }

            weaponHashes.forEach(hash => {
                player.weapons[hash] = 5000;
                player.oldWeapons[hash] = 5000;
                native.giveWeaponToPed(alt.Player.local.scriptID, hash, 5000, false, true);
            });
        })
            
        alt.onServer("StopPaintball", () => {
            this.stopPaintball()
        })
            
        alt.onServer("StopPaintballWithStats", (data) => {
            this.stopPaintball()
            alt.setTimeout(() => {
                this.stats = data
                this.stats.end = true;
                this.showPaintballStats();
            }, 3000)
        })
            
        alt.onServer("NextPaintballWeapon", (weaponHash) => {
            webview.updateView("AddNotify", ["Nächste Waffe!", "#c72020"])
            player.weapons = {};
            player.oldWeapons = {};
            native.removeAllPedWeapons(alt.Player.local.scriptID, true);
            player.weapons[weaponHash] = 5000
            player.oldWeapons[weaponHash] = 5000;
            native.giveWeaponToPed(alt.Player.local.scriptID, weaponHash, 5000, 0, 1);
        })

        alt.onServer("RsPaintballStats", (data) => {
            this.stats = data;
            this.showPaintballStats();
        })

    }
    stopPaintball() {
        player.weapons = {};
        player.oldWeapons = {};
        native.removeAllPedWeapons(alt.Player.local.scriptID, true);
        alt.clearInterval(this.positionCheckerInterval)
        this.active = false;
        this.centerPosition = null
        this.range = null

        if(this.activeIpl != null)
        {
            alt.removeIpl(this.activeIpl)
        }
        webview.updateView("ShowPaintballOutOfRange", [false]);
    }
    showPaintballStats() {
        webview.showInterface(["PaintballStatistic", this.stats]);
    }

    checkPosition() {
        let distance = this.getDistance(alt.Player.local.pos, this.centerPosition);
        if(distance > this.range)
        {
            webview.updateView("ShowPaintballOutOfRange", [true]);
            if(this.isPlayerOutside)
            {
                let actualHealth = native.getEntityHealth(alt.Player.local.scriptID);
                if(actualHealth >= 100)
                {
                    let newHealth = actualHealth - 15;
                    sibsib.setHP(newHealth);
                }
            }
            else
            {
                this.isPlayerOutside = true;
            }
        } else {
            if(this.isPlayerOutside)
            {
                webview.updateView("ShowPaintballOutOfRange", [false]);
                this.isPlayerOutside = false;
            }
        }
    }

    getDistance(vector1, vector2) {
        return Math.sqrt(Math.pow(vector1.x - vector2.x, 2) + Math.pow(vector1.y - vector2.y, 2));
    }
}

export default new Paintball()