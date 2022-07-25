import alt from "alt"
import native from "natives"
import player from "/api/modules/player.mjs"

class Animation {
    constructor() {
        //Verletzung untersuchen, Jemanden aufhelfen, Stabilisieren, Weste/Verbandskasten benutzen, Farmen (Feld)
        this.list = [
            ["mp_arresting", "arrested_spin_l_0", 50], //Handschellen festziehen
            ["mp_arresting", "sprint", 50], //Fesseln am Rücken
            ["missheistdockssetup1ig_3@base", "welding_base_dockworker", 1], //Hinknien
            ["missmechanic", "work2_base", 1], //Fahrzeug reparieren
            ["mini@cpr@char_a@cpr_str", "cpr_pumpchest", 1], //Herzdruckmassage
            ["anim@amb@prop_human_seat_computer@male@idle_a","idle_a", 30], //Computer tippen im stehen
            ["anim@heists@fleeca_bank@drilling","outro", 1], //Heist Loot nehmen
            ["anim@heists@ornate_bank@thermal_charge", "thermal_charge", 0], //Thermalpaste platzieren
            ["anim@heists@fleeca_bank@drilling", "drill_right_end", 1], //Heist Bohren
            ["anim@move_m@trash", "pickup", 1], //Etwas Aufheben
            ["anim@heists@ornate_bank@grab_cash", "intro", 2], //HEISTCASH INTRO
            ["anim@heists@ornate_bank@grab_cash", "grab", 1], //HEISTCASH GRAB
            ["cover@weapon@grenade", "hi_r_throw_90", 0], //Wegwerfen
            ["missfbi4prepp1", "_bag_pickup_garbage_man", 0], //Müll aufheben
            ["missfbi4prepp1", "_idle_action_garbage_man", 50], //Müll in der Hand
            ["missfbi4prepp1", "_bag_throw_garbage_man", 0], //Müll wegwerfen
            ["rcmepsilonism8", "bag_handler_grab_walk_left", 0], //Packet abladen
            ["missarmenian2", "corpse_search_exit_ped", 1], //17 Verletzt am Boden liegen
            ["mp_arresting", "a_uncuff", 48], //Dietrich
            ["mini@cpr@char_a@cpr_def", "cpr_intro", 1], //Stabilisieren identifizieren
            ["amb@medic@standing@tendtodead@idle_a", "idle_a", 1], //Transportbereit machen
            ["anim@mp_radio@garage@high", "action_a", 1], //Camper aufbrechen innen
            ["anim@heists@money_grab@duffel", "enter", 2], //Weste packen
            ["anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 1], //Weste ziehen
            ["mp_uncuff_paired", "crook_01_p2_fwd", 0], //Handschellen abnehmen
            ["mp_common", "givetake2_a", 48], //Item geben / Lizenz zeigen
            ["timetable@gardener@filling_can", "gar_ig_5_filling_can", 1], //Tanken
            ["anim@heists@load_box", "idle", 49], //Riechen
            ["amb@medic@standing@tendtodead@base", "base", 0], //ins Fahrzeug ziehen
            ["mp_player_int_upperwank", "mp_player_int_wank_01", 49], // Würfeln
            ["switch@michael@sitting", "idle", 1], //Sitzen für Interactions
            ["mini@strip_club@pole_dance@pole_dance1", "pd_dance_01", 1], //Tanzen Stripclub
            ["mini@strip_club@pole_dance@pole_dance2", "pd_dance_02", 1], //Tanzen Stripclub
            ["mini@strip_club@pole_dance@pole_dance3", "pd_dance_03", 1], //Tanzen Stripclub
            ["timetable@gardener@smoking_joint", "idle_cough", 0], //Husten
            ["mp_arresting", "arrested_spin_l_0", 50], //Fußfesseln
            ["anim@mp_player_intcelebrationmale@wank", "wank", 52], //Würfeltisch
            ["missarmenian3_gardener", "idle_a", 0], //Pflanze messen/untersuchen
            ["amb@prop_human_atm@male@enter", "enter", 0] //ATM Karte reinschieben

        ]

        //anim@amb@casino@valet_scenario@pose_c@ base_a_m_y_vinewood_01 Hände hinter den Rücken verschränken
        //anim@heists@ornate_bank@grab_cash intro 2 in Position bringen, um dann Geldgraben zu machen
        // anim@heists@ornate_bank@grab_cash grab 1 letzendlich das Geld grabben

        //this.loadAnimations(this.list)
    }

    async play(id) {
        const [animDict, animName, flag] = this.list[id]
        //await this.loadAnim(animDict)
        //native.taskPlayAnim(alt.Player.local.scriptID, animDict, animName, 8, -4, -1, flag, 0, false, false, false)
        this.playAnim(animDict, animName, flag)
        //native.removeAnimDict(animDict)
        // player.disableActions = true
        // disable inventory
    }

    async playAtCoords(id, pos, rot) {
        const [animDict, animName, flag] = this.list[id]
        //await this.loadAnim(animDict)
        //native.taskPlayAnimAdvanced(alt.Player.local.scriptID, animDict, animName, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, 8, -4, -1, flag, 0, 0, 0)
        this.playAnimAtCoords(animDict, animName, flag, pos, rot)
        //native.removeAnimDict(animDict)
        // player.disableActions = true
        // disable inventory
    }

    // loadAnimations(list) {
    //     for (const animation of list) {
    //         native.requestAnimDict(animation[0])
    //     }
    // }

    playAnim(animDict, animName, flag) {
        this.loadAnim(animDict).then(val=> {
            native.taskPlayAnim(alt.Player.local.scriptID, animDict, animName, 8, -4, -1, parseInt(flag), 0, false, false, false)
            //native.removeAnimDict(animDict)
        })
    }

    playAnimAtCoords(animDict, animName, flag, pos, rot) {
        this.loadAnim(animDict).then(val=> {
            native.taskPlayAnimAdvanced(alt.Player.local.scriptID, animDict, animName, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, 8, -4, -1, flag, 0, 0, 0)
            //native.removeAnimDict(animDict)
        })
    }

    playAnimAtCoordsFullParams(animDict, animName, flag, pos, rot, enterSpeed, exitSpeed, duration, animTime) {
        this.loadAnim(animDict).then(val=> {
            native.taskPlayAnimAdvanced(alt.Player.local.scriptID, animDict, animName, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, enterSpeed, exitSpeed, duration, flag, animTime, 0, 0)
            //native.removeAnimDict(animDict)
        })
    }
    
    async playAnimAndWait(animDict, animName, flag, maxDuration = 5000) {
        this.playAnim(animDict, animName, flag)
        await this.waitAnimPlaying(animDict, animName, flag, maxDuration)
    }
    
    async playAnimAtCoordsAndWait(animDict, animName, flag, pos, rot, maxDuration = 5000) {
        this.playAnimAtCoords(animDict, animName, flag, pos, rot)
        await this.waitAnimPlaying(animDict, animName, flag, maxDuration)
    }
    
    async playAnimAtCoordsFullParamsAndWait(animDict, animName, flag, pos, rot, enterSpeed, exitSpeed, duration, animTime) {
        this.playAnimAtCoordsFullParams(animDict, animName, flag, pos, rot, enterSpeed, exitSpeed, duration, animTime)
        await this.waitAnimPlaying(animDict, animName, flag, duration)
    }

    async loadAnim(animDict) {
        return new Promise((resolve, reject) => {
            if (!native.hasAnimDictLoaded(animDict)) {
                native.requestAnimDict(animDict)
                let tryAmount = 0
                const interval = alt.setInterval(() => {
                    if (native.hasAnimDictLoaded(animDict)) {
                        alt.clearInterval(interval)
                        return resolve(true)
                    }
                    tryAmount++;
                    if (tryAmount > 100) {
                        alt.clearInterval(interval)
                        return reject(false)
                    }
                }, 50)
            } 
            else return resolve(true)
            
        })
    }

    async waitAnimPlaying(animDict, animName, flag, maxDuration) {
        return new Promise((resolve, reject) => {
            let tryAmount = 0
            const interval = alt.setInterval(() => {
                if (!native.isEntityPlayingAnim(alt.Player.local.scriptID, animDict, animName, flag)) {
                    alt.clearInterval(interval)
                    return resolve(true)
                }
                tryAmount++;
                if (tryAmount * 50 > maxDuration) {
                    alt.clearInterval(interval)
                    return reject(false)
                }
            }, 50)
        })
    }
}

export default new Animation()