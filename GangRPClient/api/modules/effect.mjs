import * as alt from "alt-client";
import * as native from "natives";
import player from "/api/modules/player.mjs";
import webview from "/api/modules/webview.mjs"

class Effect {
    constructor() {
        let hasEphedrinEffect = false;
        let hasOpiumEffect = false;
        let hasMarihuanaEffect = false;
        let hasMushroomEffect = false;
        let hasLsdEffect = false;
        let ephedrinEffectName = "DeathFailMPDark"
        let opiumEffectName = "MP_job_load"
        let marihuanaEffectName = "InchPurple"
        let mushroomEffectName = "SurvivalAlien"
        let lsdEffectName = "DMT_flight_intro"
        this.interval = null;

        alt.onServer("StartEffect", (type, duration, usedItem) => {
            if(this.interval == null)
            {
                this.interval = alt.setInterval(this.intervalHandler.bind(this), 60000);
            }
            let scenarioName = null;
            let scenarioDuration = null;
            let displayName = null
            let effectName = null;
            let looped = true;
            switch(type)
            {
                case 1: //Ephedrin
                    displayName = "Ephedrin";
                    scenarioName = "PROP_HUMAN_BUM_BIN";
                    scenarioDuration = 20000;
                    effectName = ephedrinEffectName;
                    looped = true;
                    this.hasEphedrinEffect = true;
                    break;
                case 2: //Opium
                    displayName = "Opium";
                    scenarioName = "WORLD_HUMAN_DRINKING";
                    scenarioDuration = 20000;
                    effectName = opiumEffectName;
                    looped = true;
                    this.hasOpiumEffect = true;
                    break;
                case 3: //Marihuana
                    displayName = "Marihuana";
                    scenarioName = "WORLD_HUMAN_DRUG_DEALER";
                    scenarioDuration = 20000;
                    effectName = marihuanaEffectName;
                    looped = true;
                    this.hasMarihuanaEffect = true;
                    break;
                case 4: //Mushroom
                    displayName = "Magic Mushrooms";
                    scenarioName = "WORLD_HUMAN_AA_COFFEE";
                    scenarioDuration = 20000;
                    effectName = mushroomEffectName;
                    looped = true;
                    this.hasMushroomEffect = true;
                    break;
                case 5: //LSD
                    displayName = "LSD";
                    scenarioName = "WORLD_HUMAN_DRUG_DEALER_HARD";
                    scenarioDuration = 20000;
                    effectName = lsdEffectName;
                    looped = true;
                    this.hasLsdEffect = true;
                    native.setRunSprintMultiplierForPlayer(alt.Player.local.scriptID, 1.3)
                    player.drugFitness = true;
                    break;
            }
            if(usedItem)
            {
                let visualDuration = 2;
                player.cantCancelAnimation = true;
                native.taskStartScenarioInPlace(alt.Player.local.scriptID, scenarioName, 0, true);
                if (scenarioDuration > 0) webview.updateView("Bar", [scenarioDuration]);
                alt.setTimeout(() => {
                    player.clearTasks();
                    webview.updateView("AddNotify", ["Du stehst nun unter dem Einfluss von " + displayName, "#c72020"]);
                    native.animpostfxPlay(effectName, duration * 60 * 1000, looped);
                    alt.setTimeout(() => {
                        native.animpostfxStop(effectName);
                    }, visualDuration * 60 * 1000)
                }, scenarioDuration);
            }
            else
            {
                webview.updateView("AddNotify", ["Du stehst unter dem Einfluss von " + displayName, "#c72020"]);

                // if(duration == 45)
                // {
                //     let visualDuration = 2;
                //     native.animpostfxPlay(effectName, duration * 60 * 1000, looped);
                //     alt.setTimeout(() => {
                //         native.animpostfxStop(effectName);
                //     }, visualDuration * 60 * 1000)
                // }
            }
        });

        alt.onServer("StopEffect", (type) => {
            switch(type)
            {
                case 1: //Ephedrin
                    webview.updateView("AddNotify", ["Die Wirkung vom Ephedrin hat nachgelassen", "#c72020"]);
                    native.animpostfxStop(this.ephedrinEffectName);
                    this.hasEphedrinEffect = false;
                    break;
                case 2: //Opium
                    webview.updateView("AddNotify", ["Die Wirkung vom Opium hat nachgelassen", "#c72020"]);
                    native.animpostfxStop(this.opiumEffectName);
                    this.hasOpiumEffect = false;
                    break;
                case 3: //Marihuana
                    webview.updateView("AddNotify", ["Die Wirkung vom Marihuana hat nachgelassen", "#c72020"]);
                    native.animpostfxStop(this.marihuanaEffectName);
                    this.hasMarihuanaEffect = false;
                    break;
                case 4: //Mushroom
                    webview.updateView("AddNotify", ["Die Wirkung vom Mushroom hat nachgelassen", "#c72020"]);
                    native.animpostfxStop(this.mushroomEffectName);
                    this.hasMushroomEffect = false;
                    break;
                case 5: //LSD
                    webview.updateView("AddNotify", ["Die Wirkung vom LSD hat nachgelassen", "#c72020"]);
                    native.animpostfxStop(this.lsdEffectName);
                    this.hasLsdEffect = false;
                    player.drugFitness = false;
                    player.setFitness(player.fitness)
                    break;
            }

            if (!this.hasEphedrinEffect && !this.hasOpiumEffect && !this.hasMarihuanaEffect && !this.hasMushroomEffect && !this.hasLsdEffect) {
                alt.clearInterval(this.interval)
                this.interval = null
            }
        });
    }

    intervalHandler() {
        let rnd = Math.random();
        if(player.isInjured) return;
        if(this.hasEphedrinEffect)
        {
            if(alt.Player.local.vehicle != null && alt.Player.local.scriptID == native.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1, false))
            {
                if (rnd < (1 / 2))
                {
                    let rnd = 2 * Math.random() - 1;
                    let everyTick = alt.everyTick(() => {
                        if(alt.Player.local.vehicle != null)
                        {
                            native.setVehicleSteerBias(alt.Player.local.vehicle, rnd);
                            player.cantCancelAnimation = true;
                        }
                    })

                    alt.setTimeout(() => {
                        player.cantCancelAnimation = false;
                        alt.clearEveryTick(everyTick);
                    }, 2500);
                }
            }
            rnd = Math.random();
            if (rnd < (1 / 10))
            {
                native.setPedToRagdoll(alt.Player.local.scriptID, 2500, 2500, 0, true, true, true);
            }
        }
        // if(this.hasOpiumEffect)
        // {
        //     if (rnd < (1 / 10))
        //     {
        //         native.setPedToRagdoll(alt.Player.local.scriptID, 2500, 2500, 0, true, true, true);
        //     }
        // }
    }
}

export default new Effect()