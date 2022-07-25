import alt from "alt";
import native from "natives";
import webview from "/api/modules/webview.mjs";
import animation from "/api/modules/animation.mjs";
import voice from "/api/modules/voice.mjs";
import sibsib from "/api/modules/sibsib.mjs";
import vehicle from "/api/modules/vehicle.mjs";
import move from "/api/modules/move.mjs"
import interfaces from "./interfaces.mjs";

class Player {
    constructor() {
        this.adutyInterval = null;

        this.name = "";
        this.team = 1;
        this.isTied = false;
        this.isCuffed = false;
        this.isInjured = false;
        this.isFreezed = false;
        this.isInvincible = false;
        this.isStabilized = false;
        this.cloth = {};
        this.props = {};
        this.tattoos = {};
        this.jailTime = 0;
        this.maxArmor = 100;
        this.maxHealth = 100;
        this.oldWeapons = {};
        this.weapons = {};
        this.sibsib = 20;
        this.blips = [];
        this.gender = 0;
        this.arcade = -1;
        this.hunger = 0;
        this.thirst = 0;

        this.hasSmartphone = false;
        this.hasRadio = false;
        
        this.alcohol = 0;
        this.isDrunk = false;
        
        this.drugFitness = false;
        this.fitness = 0;

        this.disableActions = false;
        this.disableMovement = false;
        this.playAnim = false;
        this.weapon = 0;
        this.ammo = 0;
        this.togetherAnimData = null;
        this.cantCancelAnimation = false;
        this.isSpectating = false;
        this.isInAdminDuty = false;
        this.hasDiveClothes = false;
        this.isComa = false;
        this.isInAtm = false;
        const now = Date.now()
        this.lastCommand = now;
        this.lastInteraction = now;
        this.loadedIpls = []
        this.isInHacking = false;

        this.noClothMen = {
            11: [0, 16],
            4: [61, 1],
            8: [0, 16],
            7: [0, 0],
            6: [34, 0],
            1: [0, 0],
            10: [0, 0]
        };
        this.noClothWomen = {
            11: [18, 0],
            4: [17, 0],
            8: [0, 16],
            7: [0, 0],
            6: [35, 0],
            1: [0, 0],
            10: [0, 0]
        };
        this.jailMan = [
            {c: 3, d: 5, t: 0},
            {c: 4, d: 7, t: 15},
            {c: 6, d: 7, t: 0},
            {c: 8, d: 15, t: 0},
            {c: 11, d: 5, t: 0}
        ];
        this.jailWoman = [
            {c: 3, d: 0, t: 0},
            {c: 4, d: 3, t: 15},
            {c: 6, d: 3, t: 1},
            {c: 8, d: 2, t: 0},
            {c: 11, d: 23, t: 0}
        ];
        this.diveMan = [
            {c: 1, d: 122, t: 0},
            {c: 4, d: 94, t: -1},
            {c: 6, d: 67, t: -1},
            {c: 8, d: 151, t: 0},
            {c: 11, d: 243, t: -1},
        ];
        this.diveWoman = [
            {c: 1, d: 122, t: 0},
            {c: 4, d: 97, t: -1},
            {c: 6, d: 70, t: -1},
            {c: 8, d: 187, t: 0},
            {c: 11, d: 251, t: -1},
        ];

        alt.onServer("PlayerLoaded", async player => {
            this.isTied = player.isTied;
            this.isCuffed = player.isCuffed;
            this.isFreezed = player.isFreezed;
            this.isInjured = player.isInjured;
            this.jailTime = player.j;
            this.sibsib = player.s;
            this.gender = player.g;
            this.hunger = player.hunger / 100;
            this.thirst = player.thirst / 100;
            this.alcohol = player.alcohol;
            this.hasSmartphone = player.hs;
            this.hasRadio = player.hr;

            vehicle.vehicleData = JSON.parse(alt.File.read("@SibauiRP_Assets/data/vehicleData.json"))

            native.setWeatherTypeNowPersist(player.w);

            let snowWeathers = ["XMAS"]

            if(snowWeathers.includes(player.w))
            {
                native.setForceVehicleTrails(true)
                native.setForcePedFootstepsTracks(true)
                native.requestScriptAudioBank("ICE_FOOTSTEPS", false, 0)
                native.requestScriptAudioBank("SNOW_FOOTSTEPS", false, 0)

                native.requestNamedPtfxAsset("core_snow");

                let timer = alt.setInterval(() => {
                    if(native.hasNamedPtfxAssetLoaded("core_snow")){
                        native.useParticleFxAsset("core_snow");
                        alt.clearInterval(timer);
                    }
                }, 1);
            }

            // native.setClockTime(player.hour, player.minute, player.second);
            // native.setClockTime(1, 0, 0)
            alt.setMsPerGameMinute(1000 * 60);
            // alt.setMsPerGameMinute(9999999999);
            native.setClockTime(player.hour, player.minute, player.second);
            // alt.setMsPerGameMinute(1000 * 60);

            if (player.isTied) {
                await animation.play(1);
                this.disableActions = true;
            } else if (player.isCuffed) {
                await animation.play(0);
                this.disableActions = true;
            }

            if (player.isInjured) {
                native.triggerScreenblurFadeIn(0);
                this.disableMovement = true;
                this.playAnim = true;
                await animation.play(17);
                native.setEntityInvincible(alt.Player.local.scriptID, true);
            }

            for (const obj of player.cloth) {
                this.cloth[obj.c] = obj;
                if (obj.c == 1) continue;
                this.cloth[obj.c].w = true;
            }

            for (const obj of player.props) {
                this.props[obj.c] = obj;
                this.props[obj.c].w = true;
            }

            native.requestModel(alt.hash("mp_f_freemode_01"));
            native.requestModel(alt.hash("mp_m_freemode_01"));
            //alt.loadModel("mp_f_freemode_01")
            //alt.loadModel("mp_m_freemode_01")

            native.setPedHeadBlendData(
                alt.Player.local.scriptID,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false
            );
            if (player.a != "") {
                this.appearance = JSON.parse(player.a);
                this.setAppearance();
            }
            this.tattoos = player.tattoos;
            this.setTattoos(player.tattoos);
            this.setSpawnClothes(player.cloth);
            this.setSpawnProps(player.props);
            this.setDrunkStatus();

            webview.updateView("SetHunger", [this.hunger]);
            webview.updateView("SetThirst", [this.thirst]);

            webview.updateView("SetMoney", [player.m]);

            webview.updateView("HidePlayerHud", [false]);

            this.name = player.name;
            this.team = player.team;
            this.maxArmor = player.maxArmor;
            this.maxHealth = player.maxHealth;

            this.loadMapBlips()

            //FreeAim
            native.setPlayerTargetingMode(2);

            for (const weapon of player.weapons) {
                const hash = Object.keys(weapon)[0];
                this.oldWeapons[hash] = weapon[hash];
                this.weapons[hash] = weapon[hash];
            }

            for (const ipl of player.ipls) {
                const hash = Object.keys(ipl)[0];
                this.loadedIpls.push(hash)
                alt.requestIpl(hash)
            }
            
            native.setPedConfigFlag(alt.Player.local.scriptID, 35, false); //PutOnMotorcycleHelmet
            native.setPedConfigFlag(alt.Player.local.scriptID, 184, true); //DisableShufflingToDriverSeat
            native.setPedConfigFlag(alt.Player.local.scriptID, 241, true); //DisableStoppingEngine
            //native.setPedConfigFlag(alt.Player.local.scriptID, 423, true) //DisablePropKnockOff
            native.setPedConfigFlag(alt.Player.local.scriptID, 429, true); //DisableStartingVehEngine
            native.setPlayerCanUseCover(alt.Player.local.scriptID, false);
            native.setPlayerMaxArmour(alt.Player.local, parseInt(this.maxArmor));
            native.setPedMaxHealth(
                alt.Player.local.scriptID,
                parseInt(this.maxHealth)
            );
            native.setEntityVisible(alt.Player.local.scriptID, true, false);
            native.setPedSuffersCriticalHits(alt.Player.local.scriptID, false);
			this.setInvincible(true)

            if (player.a != "") {
                alt.setTimeout(() => {
                    let pos = alt.Player.local.pos;
                    let rot = alt.Player.local.rot;
                    let cameraUp = native.createCamWithParams(
                        "DEFAULT_SCRIPTED_CAMERA",
                        pos.x,
                        pos.y,
                        pos.z + 1240,
                        -90,
                        rot.y,
                        rot.z,
                        65,
                        true,
                        0
                    );
                    let cameraDown = native.createCamWithParams(
                        "DEFAULT_SCRIPTED_CAMERA",
                        pos.x,
                        pos.y,
                        pos.z + 4,
                        -90,
                        rot.y,
                        rot.z,
                        65,
                        true,
                        0
                    );

                    native.setCamActive(cameraUp, true);
                    native.renderScriptCams(true, false, 0, true, false, 0)
                    alt.setTimeout(() => {
                        native.setCamActiveWithInterp(cameraDown, cameraUp, 5000, 1, 1);
                        alt.setTimeout(() => {
                            native.renderScriptCams(false, true, 1000, true, false, 0)
                            native.destroyCam(cameraUp, false);
                            native.destroyCam(cameraDown, false);
                            
                            // Unfreeze after is at ground & camera finished
                            native.freezeEntityPosition(alt.Player.local.scriptID, false);
                            this.setInvincible(false);
                        }, 5500);
                    }, 1000);
                }, 1000);
            }

            //test
            alt.setTimeout(() => {
                sibsib.start(this.sibsib);
            }, 3000);

            // Set player to ground if is in air
            alt.setTimeout(() => {
                if (native.isEntityInAir(alt.Player.local.scriptID)) {
                    native.freezeEntityPosition(alt.Player.local.scriptID, true);
                    alt.setTimeout(() => {
                        let pos = alt.Player.local.pos;
                        let [toggle, groundZ] = native.getGroundZFor3dCoord(
                            pos.x,
                            pos.y,
                            pos.z,
                            null,
                            false,
                            false
                        );
                        if (groundZ == 0) {
                            [toggle, groundZ] = native.getGroundZFor3dCoord(
                                pos.x,
                                pos.y,
                                pos.z + 100,
                                null,
                                false,
                                false
                            );
                        }
                        native.setEntityCoords(
                            alt.Player.local.scriptID,
                            pos.x,
                            pos.y,
                            groundZ + 1,
                            false,
                            false,
                            false,
                            false
                        );
                        // Freeze so you cant move while camera is moving
                        native.freezeEntityPosition(alt.Player.local.scriptID, true);
                    }, 2000);
                } else {
                    // Freeze so you cant move while camera is moving
                    native.freezeEntityPosition(alt.Player.local.scriptID, true);
                }
            }, 2000);
        });
        
        alt.onServer("ToggleTempIpl", (iplName) => {
            if (!this.loadedIpls.includes(iplName)) {
                this.loadedIpls.push(iplName)
                alt.requestIpl(iplName);
            } else {
                this.loadedIpls = this.loadedIpls.filter(function (e) {return e !== iplName})
                alt.removeIpl(iplName);
            }
        })

        alt.onServer("SetHunger", (hunger) => {
            hunger = hunger / 100;
            this.hunger = hunger;
            webview.updateView("SetHunger", [hunger]);
        })

        alt.onServer("DeadReset", () => {
            this.hasSmartphone = false;
            this.hasRadio = false;
        })

        alt.onServer("EquipSmartphone", (toggle) => {
            this.hasSmartphone = toggle;
        })

        alt.onServer("EquipRadio", (toggle) => {
            this.hasRadio = toggle;
        })

        alt.onServer("SetThirst", (thirst) => {
            thirst = thirst / 100;
            this.thirst = thirst;
            webview.updateView("SetThirst", [thirst]);
        })

        alt.onServer("SetAlcohol", (alcohol) => {
            
            this.updateDrunkStatus(alcohol)
        })

        alt.onServer("SetNutriment", (hunger, thirst, alcohol) => {
            hunger = hunger / 100;
            thirst = thirst / 100;
            this.hunger = hunger;
            this.thirst = thirst;
            if (alcohol > 0 || alcohol < this.alcohol) {
                this.updateDrunkStatus(alcohol)
            }
            
            webview.updateView("SetHungerAndThirst", [hunger, thirst]);
        })


        alt.onServer("SetTied", tied => {
            if (tied) {
                webview.hidePhone();
                webview.hideFunk();
                webview.closeInterface();
            }
            this.isTied = tied;
            this.disableActions = tied;
            native.setPlayerCanDoDriveBy(alt.Player.local.scriptID, !tied);
        });

        alt.onServer("SetCuffed", cuffed => {
            if (cuffed) {
                native.clearPedTasksImmediately(alt.Player.local.scriptID);
                webview.hidePhone();
                webview.hideFunk();
                webview.closeInterface();
            }
            this.isCuffed = cuffed;
            this.disableActions = cuffed;
            native.setPlayerCanDoDriveBy(alt.Player.local.scriptID, !cuffed);
        });

        alt.onServer("SetFreezed", freezed => {
            this.isFreezed = freezed;
            this.disableActions = freezed;
            this.disableMovement = freezed;

            // if (alt.Player.local.vehicle == null)
            //     native.freezeEntityPosition(alt.Player.local.vehicle.scriptID, freezed)
        });

        alt.onServer("Arcade", status => {
            this.arcade = status
        })

        alt.onServer("SetFakeInjured", async () => {
            await animation.play(17);
        })

        alt.onServer("SetInjured", async injured => {
            this.isInjured = injured;
            this.disableMovement = injured;
            this.playAnim = injured;
            native.setEntityInvincible(alt.Player.local.scriptID, injured);
            if (injured) {
                //native.animpostfxPlay("DeathFailOut", 0, true);
                sibsib.stop();
                this.isStabilized = false;
                voice.VoicePlayerDead();
                await animation.play(17);
            } else {
                native.clearPedTasks(alt.Player.local.scriptID);
                if(this.isComa)
                {
                    this.isComa = false;
                    native.animpostfxStop("DeathFailOut");
                }
                native.animpostfxStop("DeathFailOut");
                //native.animpostfxStopAll();
                this.cuffed = false;
                this.isStabilized = false;
                sibsib.start(this.sibsib);
            }
            native.setPlayerCanDoDriveBy(alt.Player.local.scriptID, !injured);
        });

        alt.onServer("SetTeam", team => {
            this.team = team;
            this.unloadMapBlips();
            this.loadMapBlips();
        });

        alt.onServer("SetAmmo", (weapon, ammo) => {
            this.ammo = native.getAmmoInPedWeapon(alt.Player.local.scriptID, weapon);
            this.weapons[weapon] = ammo;
            this.oldWeapons[weapon] = ammo;
        });
        alt.onServer("AddTattoo", (o, c) => {
            let obj = new Object();
            obj.o = o;
            obj.c = c;
            this.tattoos.push(obj);
            this.setTattoos(this.tattoos);
            
        });

        alt.onServer("RemoveTattoo", (o, c) => {
            this.tattoos = this.tattoos.filter(item => item.o != o && item.c != c)
            this.setTattoos(this.tattoos)
        });

        alt.onServer("AddWpn", weapon => {
            this.weapons[weapon] = 0;
            this.oldWeapons[weapon] = 0;
        });

    alt.onServer("AddWpns", weapons => {
        weapons.forEach(weapon => {
            this.weapons[weapon.h] = weapon.m
            this.oldWeapons[weapon.h] = weapon.m;
            native.giveWeaponToPed(alt.Player.local.scriptID, weapon.h, weapon.m, 0, 1)
            native.setPedWeaponTintIndex(alt.Player.local.scriptID, weapon.h, weapon.t)
        });
    });

    alt.onServer("AddWpnsNotHand", weapons => {
        weapons.forEach(weapon => {
            this.weapons[weapon.h] = weapon.m
            this.oldWeapons[weapon.h] = weapon.m;
            native.giveWeaponToPed(alt.Player.local.scriptID, weapon.h, weapon.m, 0, 0)
            native.setPedWeaponTintIndex(alt.Player.local.scriptID, weapon.h, weapon.t)
        });
    });

    alt.onServer("RmWpn", weapon => {
      const ammo = native.getAmmoInPedWeapon(alt.Player.local.scriptID, weapon);
      //alt.log("Remove " + ammo + " " + ammo - this.weapons[weapon])
      native.setPedAmmo(
        alt.Player.local.scriptID,
        weapon,
        ammo - this.weapons[weapon],
        true
      ); //setAmmoInClip
      this.weapons[weapon] = undefined;
      this.oldWeapons[weapon] = undefined;
    });

        alt.onServer("RmWpns", () => {
            this.weapons = {};
            this.oldWeapons = {};
            native.removeAllPedWeapons(alt.Player.local.scriptID, true);
            //TODO MAYBE REMOVE AMMO? THIS SUCKS DAMN
        });

        alt.onServer("Names", toggle => {
            if (toggle == true && this.adutyInterval == null) {
                this.adutyInterval = alt.setInterval(this.drawNames.bind(this), 0);
            } else if (toggle == false && this.adutyInterval != null) {
                alt.clearInterval(this.adutyInterval);
                this.adutyInterval = null;
            }
        });

        alt.onServer("TogAnim", (target, anim) => {
            webview.updateView("AddNotify", [
                target.name +
                " hat dir eine Anfrage für " +
                anim[0] +
                " geschickt. Drücke U, um sie anzunehmen",
                "#00a6cc"
            ]);
            this.togetherAnimData = {target, anim, startTime: Date.now()};
        });

        alt.onServer("SetAppearance", appearanceData => {
            this.appearance = JSON.parse(appearanceData);
            this.setAppearance();
        });

        alt.setInterval(() => {
            // if (native.getPedConfigFlag(alt.Player.local.scriptID, 78, 1)) { }
            native.setPedUsingActionMode(alt.Player.local.scriptID, false, -1, 0);

            // if (native.isPedPerformingStealthKill(alt.Player.local.scriptID))
            //     native.clearPedTasksImmediately(alt.Player.local.scriptID)

            const weapon = native.getSelectedPedWeapon(alt.Player.local.scriptID);
            const group = native.getWeapontypeGroup(weapon);
            if (group == 2685387236 || group == 3566412244) {
                native.enableAllControlActions(1);
            } else {
                native.disableControlAction(0, 140, true);
                native.disableControlAction(0, 141, true);
                native.disableControlAction(0, 142, true);
            }

            // if (group != 2685387236 || group != 3566412244) {
            //     native.disableControlAction(0, 140, true)
            //     native.disableControlAction(0, 142, true)
            // }

            // if (group != 2685387236 || group != 3566412244) { //Melee
            //     native.disableControlAction(0, 140, true)
            //     native.disableControlAction(0, 141, true)
            //     //native.disableControlAction(0, 142, true)
            // }
            if (this.weapon != weapon) {
                //WeaponSwitch Disablen. Bis auf weiteres kommt das nicht rein.
                // if(alt.Player.local.vehicle == null)
                // {
                //     native.setPedCanSwitchWeapon(alt.Player.local.scriptID, false);
                //     alt.setTimeout(() => {
                //         native.setPedCanSwitchWeapon(alt.Player.local.scriptID, true);
                //     }, 1500)
                // }
                //sibsib.checkDamage(weapon)
                if (
                    this.weapons[weapon] == undefined &&
                    weapon != 966099553 &&
                    weapon != 0 &&
                    weapon != 2725352035 && //unarmed
                    weapon != 4194021054 && //animal
                    group != 2685387236
                ) {
                    alt.emitServer("SibsibWeapon", weapon);
                    native.removeWeaponFromPed(alt.Player.local.scriptID, weapon);
                } else {
                    this.ammo = native.getAmmoInPedWeapon(
                        alt.Player.local.scriptID,
                        weapon
                    );
                    this.weapon = weapon;
                }
                //alt.log("Switch " + weapon + " " + this.ammo)
            } else if (native.isControlJustReleased(0, 24)) {
                if (group == 2685387236) return; //Melee
                const ammo = native.getAmmoInPedWeapon(
                    alt.Player.local.scriptID,
                    weapon
                );
                const diff = this.ammo - ammo;
                //alt.log("Diff " + this.ammo + " " + ammo + " " + diff)
                if (diff < 1) return;
                if (ammo == this.weapons[weapon]) return;
                //if (this.weapons[weapon] != undefined) this.weapons[weapon] -= diff
                this.weapons[weapon] -= diff;
                this.ammo = ammo;
                //alt.log("Old " + this.ammo + " Ammo " + ammo + " Current " + this.weapons[weapon] + " Weapon " + weapon + " Group " + group)
                //alt.emitServer("SaveAmmo", weapon+"", this.weapons[weapon])
            }

            // const now = Date.now()

            if (this.disableMovement) {
                native.disableControlAction(0, 30, true); //Move LR
                native.disableControlAction(0, 31, true); //Move UD
                this.disableMovementControls();
            } else if (this.disableActions) {
                this.disableMovementControls();
            }

            // if (this.isInjured) {
            //     native.setPedToRagdoll(alt.Player.local.scriptID, -1, -1, 0, 0, 0, 0)
            // }

            // if (now > voice.NextUpdate) {
            //     if (voice.IsConnected && voice.IsInGame) {
            //         voice.PlayerStateUpdate()
            //         voice.NextUpdate = now + 666
            //     }
            // }

            native.disableControlAction(0, 36, true);
            native.disableControlAction(0, 345, true);
        }, 1);

        alt.setInterval(() => {
            if (voice.IsConnected && voice.IsInGame) {
                voice.PlayerStateUpdate();
                //voice.NextUpdate = Date.now() + 666
            }
            if (this.isInjured && alt.Player.local.vehicle == null) {
                if (
                    !native.isEntityPlayingAnim(
                        alt.Player.local.scriptID,
                        "missarmenian2",
                        "corpse_search_exit_ped",
                        1
                    )
                ) {
                    // native.taskPlayAnim(
                    //     alt.Player.local.scriptID,
                    //     "missarmenian2",
                    //     "corpse_search_exit_ped",
                    //     8,
                    //     -4,
                    //     -1,
                    //     1,
                    //     0,
                    //     false,
                    //     false,
                    //     false
                    // );
                    animation.playAnim("missarmenian2","corpse_search_exit_ped",1)
                }
            }
        }, 666);

        alt.setInterval(() => {
            native.invalidateIdleCam();
            native.invalidateVehicleIdleCam();
        }, 29000);

        alt.setInterval(() => {
            if (!this.isInAdminDuty) {
                this.updateDrunkStatus(this.alcohol-1);
                this.hunger -= (0.32);
                this.thirst -= (0.45);

                if (this.hunger <= 0)
                {
                    this.hunger = 0;
                    sibsib.setDamage(10, 1)
                }
                if (this.thirst <= 0)
                {
                    this.thirst = 0;
                    sibsib.setDamage(10, 2)
                }
                webview.updateView("SetHungerAndThirst", [this.hunger, this.thirst]);
            }
            
            let weapons = null;
            for (const weapon in this.weapons) {
                if (this.weapons[weapon] != this.oldWeapons[weapon]) {
                    if (weapons == null) weapons = {};
                    weapons[weapon] = this.weapons[weapon];
                    this.oldWeapons[weapon] = this.weapons[weapon];
                }
            }
            //alt.log("SaveAmmo " + JSON.stringify(weapons))
            if (weapons != null) alt.emitServer("SaveAmmo", weapons);
        }, 60 * 1000);
    }

    setClothes() {
        native.setEnableScuba(alt.Player.local.scriptID, false)
        this.hasDiveClothes = false;
        this.resetDiveTime();
        for (let i = 1; i <= 11; i++) {
            if (i == 2 || i == 9) continue;
            //alt.log(i + " " + JSON.stringify(this.cloth[i]))
            if (this.cloth[i] != undefined) {
                // alt.log("FOUND " + player.cloth[i].c)
                //alt.log("w " + this.cloth[i].w)
                if (this.cloth[i].w) {
                    if (this.cloth[i].c == undefined)
                        native.setPedComponentVariation(
                            alt.Player.local.scriptID,
                            i,
                            0,
                            0,
                            0
                        );
                    else
                        native.setPedComponentVariation(
                            alt.Player.local.scriptID,
                            this.cloth[i].c,
                            this.cloth[i].d,
                            this.cloth[i].t,
                            0
                        );
                } else {
                    if (i == 11)
                        native.setPedComponentVariation(
                            alt.Player.local.scriptID,
                            3,
                            15,
                            0,
                            0
                        );
                    if (native.isPedMale(alt.Player.local.scriptID))
                        native.setPedComponentVariation(
                            alt.Player.local.scriptID,
                            i,
                            this.noClothMen[i][0],
                            this.noClothMen[i][1],
                            0
                        );
                    else
                        native.setPedComponentVariation(
                            alt.Player.local.scriptID,
                            i,
                            this.noClothWomen[i][0],
                            this.noClothWomen[i][1],
                            0
                        );
                }
            } else {
                // alt.log("NOT FOUND " + i)
                if (i == 8)
                    native.setPedComponentVariation(
                        alt.Player.local.scriptID,
                        8,
                        0,
                        16,
                        0
                    );
                else
                    native.setPedComponentVariation(
                        alt.Player.local.scriptID,
                        i,
                        0,
                        0,
                        0
                    );
            }
        }
    }

    setProps() {
        for (let i = 0; i <= 7; i++) {
            if (i >= 3 && i <= 5) continue;
            if (this.props[i] != undefined) {
                // alt.log("FOUND " + player.cloth[i].c)
                if (this.props[i].w)
                    native.setPedPropIndex(
                        alt.Player.local.scriptID,
                        this.props[i].c,
                        this.props[i].d,
                        this.props[i].t,
                        0
                    );
                else native.clearPedProp(alt.Player.local.scriptID, i);
            } else {
                // alt.log("NOT FOUND " + i)
                native.clearPedProp(alt.Player.local.scriptID, i);
            }
        }
    }

    setSpawnClothes(clothes) {
        if (this.jailTime < 1) {
            native.setPedComponentVariation(alt.Player.local.scriptID, 8, 0, 16, 0);
            for (const obj of clothes) {
                if (obj.c == 1) continue;
                native.setPedComponentVariation(
                    alt.Player.local.scriptID,
                    obj.c,
                    obj.d,
                    obj.t,
                    0
                );
            }
        } else {
            this.setJailClothes();
        }
    }

    setFitness(fitness) {
        this.fitness = fitness;
        webview.updateView("SetFitness", [fitness]);
        if(this.drugFitness) return;
        let multiplier = 1.0
        let maxSpeed = 1.13
        let threshold = 80
        let stamina = 100
        if(fitness > threshold)
        {
            if(fitness > 100)
            {
                fitness = 100
            }
            multiplier += (fitness - threshold) * (maxSpeed - 1.0) / (100 - threshold)
        }
        native.setRunSprintMultiplierForPlayer(alt.Player.local.scriptID, multiplier)
        native.setSwimMultiplierForPlayer(alt.Player.local.scriptID, multiplier)
        if(fitness <= 50)
        {
            stamina = 2 * fitness
        }
        alt.setStat("stamina", stamina)
    }

    setSpawnProps(props) {
        if (this.jailTime < 1) {
            for (const obj of props) {
                native.setPedPropIndex(
                    alt.Player.local.scriptID,
                    obj.c,
                    obj.d,
                    obj.t,
                    0
                );
            }
        }
    }

    applyDamageEffect()
    {
        if (this.isInjured)
        {
            native.setTimecycleModifier("damage")
            native.setTimecycleModifierStrength(1.1)
        }
        else if(sibsib.allowedHealth <= 165 && sibsib.allowedHealth >= 101)
        {
            native.setTimecycleModifier("damage")
            //maximum 1.05 //minimum 0.65 //180hp - 101hp
            //allowedHealth - 100 between 0 and 80
            native.setTimecycleModifierStrength(1.05 - 0.006 * (sibsib.allowedHealth - 100)) 
        }
        else if(sibsib.allowedHealth >= 165)
        {
            native.clearTimecycleModifier()
        }
    }

    setDrunkStatus() {
        if (this.alcohol >= 20) {
            move.setClipset("move_m@drunk@verydrunk", 0.25)
            native.shakeGameplayCam("DRUNK_SHAKE", 0.8)
        } else if (this.alcohol >= 10) {
            move.setClipset("move_m@drunk@moderatedrunk", 0.25)
            native.shakeGameplayCam("DRUNK_SHAKE", 1.4)
        } else if (this.alcohol >= 5) {
            move.setClipset("move_m@drunk@slightlydrunk", 0.25)
            native.shakeGameplayCam("DRUNK_SHAKE", 2)
        }
        if (this.alcohol >= 5) {
            this.isDrunk = true;
        }
    }
    
    updateDrunkStatus(newValue) {
        if (newValue < 0) return;
        if (newValue == 0) {
            native.resetPedMovementClipset(alt.Player.local.scriptID, 0.25)
            native.shakeGameplayCam("DRUNK_SHAKE", 0.0)
            this.alcohol = 0;
            this.isDrunk = false;
        }
        this.alcohol = newValue;
        if (newValue >= 20 && !native.hasAnimSetLoaded("move_m@drunk@verydrunk")) {
            this.setDrunkStatus()
        }
        else if (newValue >= 10 && !native.hasAnimSetLoaded("move_m@drunk@moderatedrunk")) {
            this.setDrunkStatus()
        }
        else if (newValue >= 5 && !native.hasAnimSetLoaded("move_m@drunk@slightlydrunk")) {
            this.setDrunkStatus()
        }
    }
    
    setTattoos(tattoos) {
        native.clearPedDecorations(alt.Player.local.scriptID);
        if (tattoos.length) {
            for (const obj of tattoos) {
                let overlayHash = alt.hash(obj.o);
                let collectionHash = alt.hash(obj.c);
                native.addPedDecorationFromHashes(
                    alt.Player.local.scriptID,
                    overlayHash,
                    collectionHash
                );
            }
        } else {
            if (this.gender == 0) {
                native.addPedDecorationFromHashes(
                    alt.Player.local.scriptID,
                    alt.hash("customtattoos_overlays"),
                    alt.hash("mp_customtattoos_tattoo_000_m")
                );
            } else {
                native.addPedDecorationFromHashes(
                    alt.Player.local.scriptID,
                    alt.hash("customtattoos_overlays"),
                    alt.hash("mp_customtattoos_tattoo_000_f")
                );
            }
        }
    }

    setAppearance(reset = false) {
        const appearance = this.appearance;

        native.setPedHeadBlendData(
            alt.Player.local.scriptID,
            parseInt(appearance.Parents.FatherShape),
            parseInt(appearance.Parents.MotherShape),
            0,
            parseInt(appearance.Parents.FatherSkin),
            parseInt(appearance.Parents.MotherSkin),
            0,
            parseFloat(appearance.Parents.Similarity),
            parseFloat(appearance.Parents.SkinSimilarity),
            0,
            false
        );

        native.setPedComponentVariation(
            alt.Player.local.scriptID,
            2,
            parseInt(appearance.Hair.Hair),
            0,
            0
        );
        native.setPedHairColor(
            alt.Player.local.scriptID,
            parseInt(appearance.Hair.Color),
            parseInt(appearance.Hair.HighlightColor)
        );
        native.setPedEyeColor(alt.Player.local.scriptID, parseInt(appearance.EyeColor));

        for (const index in appearance.Features) {
            native.setPedFaceFeature(
                alt.Player.local.scriptID,
                parseInt(index),
                parseFloat(appearance.Features[index])
            );
        }

        for (const index in appearance.Appearance) {
            const part = appearance.Appearance[index];
            if (part.Value == 255 && !reset) continue;
            native.setPedHeadOverlay(
                alt.Player.local.scriptID,
                parseInt(index),
                parseInt(part.Value),
                parseFloat(part.Opacity)
            );
        }

        native.setPedHeadOverlayColor(
            alt.Player.local.scriptID,
            2,
            1,
            parseInt(appearance.EyebrowColor),
            0
        );
        native.setPedHeadOverlayColor(
            alt.Player.local.scriptID,
            1,
            1,
            parseInt(appearance.BeardColor),
            0
        );
        native.setPedHeadOverlayColor(
            alt.Player.local.scriptID,
            10,
            1,
            parseInt(appearance.ChestHairColor),
            0
        );

        native.setPedHeadOverlayColor(
            alt.Player.local.scriptID,
            5,
            2,
            parseInt(appearance.BlushColor),
            0
        );
        native.setPedHeadOverlayColor(
            alt.Player.local.scriptID,
            8,
            2,
            parseInt(appearance.LipstickColor),
            0
        );
    }

    distance(vector1, vector2) {
        return Math.sqrt(
            Math.pow(vector1.x - vector2.x, 2) +
            Math.pow(vector1.y - vector2.y, 2) +
            Math.pow(vector1.z - vector2.z, 2)
        );
    }

    inVehicle() {
        return alt.Player.local.vehicle != null;
    }

    getPos() {
        return alt.Player.local.pos;
    }

    setCuffed(cuffed) {
        native.setEnableHandcuffs(alt.Player.local.scriptID, cuffed);
    }

    disableMovementControls() {
        native.disablePlayerFiring(alt.Player.local.scriptID, true);
        native.disableControlAction(0, 22, true); //Space
        native.disableControlAction(0, 23, true); //Veh Enter
        native.disableControlAction(0, 25, true); //Right Mouse
        native.disableControlAction(0, 44, true); //Q
        native.disableControlAction(2, 75, true); //Exit Vehicle
        native.disableControlAction(2, 140, true); //R
        native.disableControlAction(2, 141, true); //Left Mouse
    }

    setAdminDuty(status) {
        this.setInvincible(status)
        this.isInAdminDuty = status;
        native.setPedCanRagdoll(alt.Player.local.scriptID, !status);

        if (status) {
            native.setPedComponentVariation(alt.Player.local.scriptID, 7, 0, 0, 0);
            native.setPedComponentVariation(alt.Player.local.scriptID, 8, 0, 16, 0);
            native.setPedComponentVariation(alt.Player.local.scriptID, 9, 0, 0, 0);
            native.clearAllPedProps(alt.Player.local.scriptID);
            if (this.adutyInterval == null)
                this.adutyInterval = alt.setInterval(this.drawNames.bind(this), 0);
        } else {
            if (this.cloth[1] != undefined && !this.cloth[1].w)
                native.setPedComponentVariation(alt.Player.local.scriptID, 1, 0, 0, 0);
            this.setClothes();
            this.setProps();
            if (this.adutyInterval == null) return;
            alt.clearInterval(this.adutyInterval);
            this.adutyInterval = null;
        }
    }

    setInvincible(status)
    {
        this.isInvincible = status
        native.setPlayerInvincible(alt.Player.local.scriptID, status)
    }

    drawNames() {
        for (const player of alt.Player.streamedIn) {
            if (player == alt.Player.local || player.scriptID == 0) continue;
            if (this.distance(player.pos, alt.Player.local.pos) > 75) continue;
            if (!native.isEntityOnScreen(player.scriptID)) continue;

            const playerPos = player.pos;
            const entity = player.vehicle ? player.vehicle.scriptID : player.scriptID;
            const vector = native.getEntityVelocity(entity);
            const frameTime = native.getFrameTime();
            native.setDrawOrigin(
                playerPos.x + vector.x * frameTime,
                playerPos.y + vector.y * frameTime,
                playerPos.z + vector.z * frameTime + 1.1,
                0
            );
            native.beginTextCommandDisplayText("STRING");
            native.setTextFont(4);
            native.setTextCentre(true);
            native.setTextOutline();

            const fontSize =
                (1 - (0.8 * this.distance(player.pos, alt.Player.local.pos)) / 100) *
                0.4;
            native.setTextScale(fontSize, fontSize);
            native.setTextProportional(true);
            //native.setTextColour(255, 255, 255, 255)
            native.addTextComponentSubstringPlayerName(
                `~g~${native.getEntityHealth(player.scriptID)} ~w~${
                    player.name
                } ~b~${native.getPedArmour(player.scriptID)}`
            );
            native.endTextCommandDisplayText(0, 0, 0);
            native.clearDrawOrigin();
        }
    }

    playScenario(scenarioName) {
        native.taskStartScenarioInPlace(
            alt.Player.local.scriptID,
            scenarioName,
            0,
            true
        );
    }

    clearTasks() {
        this.cantCancelAnimation = false
        native.clearPedTasks(alt.Player.local.scriptID)
        webview.updateView("Bar", [0])
        //if (unfreeze) player.isFreezed = false
        this.playAnim = false
        this.disableMovement = false
    }

    setJailClothes() {
        let cloth;
        if (native.isPedMale(alt.Player.local.scriptID)) cloth = this.jailMan;
        else cloth = this.jailWoman;

        for (const obj of cloth) {
            if (this.cloth[obj.c] == undefined) this.cloth[obj.c] = {w: obj.c != 1};

            this.cloth[obj.c].c = obj.c;
            this.cloth[obj.c].d = obj.d;
            this.cloth[obj.c].t = obj.t;
        }

        this.setClothes();
    }

    setDiveClothes(color) {
        let cloth;
        if (native.isPedMale(alt.Player.local.scriptID)) cloth = this.diveMan;
        else cloth = this.diveWoman;

        for (const obj of cloth) {
            if (this.cloth[obj.c] == undefined) this.cloth[obj.c] = {w: obj.c != 1};

            this.cloth[obj.c].c = obj.c;
            this.cloth[obj.c].d = obj.d;
            this.cloth[obj.c].w = true;
            if(obj.t == -1)
            {
                this.cloth[obj.c].t = color
            }
            else
            {
                this.cloth[obj.c].t = obj.t;
            }
        }
        this.setClothes();
        this.setDiveTime(15 * 60);
        this.hasDiveClothes = true;

        native.setEnableScuba(alt.Player.local.scriptID, true)
    }

    setDiveTime(time)
    {
        native.setPedMaxTimeUnderwater(alt.Player.local.scriptID, parseFloat(time))
    }

    resetDiveTime()
    {
        native.setPedMaxTimeUnderwater(alt.Player.local.scriptID, parseFloat(40))
    }

    unloadMapBlips() {
        for (const obj of this.blips) {
            obj.destroy();
        }
        this.blips = [];
    }

    loadMapBlips() {
        let blips = JSON.parse(alt.File.read("@SibauiRP_Assets/data/blips.json"))
        for (const blipData of blips) {
            if(blipData.teamIds.includes(this.team) || blipData.teamIds.includes(1) || blipData.teamIds.length == 0)
            {
                const blip = new alt.PointBlip(blipData.position.X, blipData.position.Y, blipData.position.Z)
                blip.sprite = blipData.sprite
                blip.color = blipData.color
                blip.name = blipData.name
                blip.shortRange = true
                
                this.blips.push(blip)
            }
        }
    }
}

export default new Player();
