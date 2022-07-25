import alt from "alt-client";
import native from "natives";
import webview from "/api/modules/webview.mjs"
import player from "/api/modules/player.mjs"
import blackscreen from "/api/modules/blackscreen.mjs"
import animation from "./animation.mjs";

class PropHunt {
    constructor() {
        this.enabled = false;
        this.isSeeker = false;
        this.frozen = false;
        this.propObjectAmount = 0;
        this.activeIpl = null;

        alt.onServer("StartPropHunt", (isSeeker, propObjectAmount, mapId) => {
            this.enabled = true;
            this.propObjectAmount = propObjectAmount
            this.isSeeker = isSeeker
            webview.closeInterface()
            native.removeAllPedWeapons(alt.Player.local.scriptID, true);
            
            switch(mapId) {
                case 1:
                    this.activeIpl = "filmstudios_ph";
                    break;
                case 3:
                    this.activeIpl = "altabaustelle_ph";
                    break;
                case 4:
                    this.activeIpl = "geewarehouse_ph";
                    break;
                case 5:
                    this.activeIpl = "humane_ph";
                    break;
                case 6:
                    this.activeIpl = "lighthouse_ph";
                    break;
                default:
                    this.activeIpl = null
                    break;
            }

            if (this.activeIpl != null) {
                alt.requestIpl(this.activeIpl)
            }
            
            if (isSeeker) {
                player.weapons = {};
                player.oldWeapons = {};

                let amount = 300 + 60 * propObjectAmount;
                
                player.weapons[2132975508] = amount;
                player.oldWeapons[2132975508] = amount;
                native.giveWeaponToPed(alt.Player.local.scriptID, 2132975508, amount, false, true);
                player.isFreezed = true;
                player.disableActions = true;
                player.disableMovement = true;
                blackscreen.start()

                webview.updateView("StartCountdownHud", [30]);
                //webview.updateView("Bar", [30 * 1000])
                alt.setTimeout(() => {
                    blackscreen.stop()
                    player.isFreezed = false;
                    player.disableActions = false;
                    player.disableMovement = false;
                }, 30 * 1000)
            }

            webview.updateView("ShowProphunt", [this.isSeeker, this.propObjectAmount]);
        })

        alt.onServer("StopPropHunt", () => {
            // webview.updateView("StopCountdownHud", []);
            this.enabled = false;
            this.frozen = false;
            this.propObjectAmount = 0;
            native.clearPedTasks(alt.Player.local.scriptID)
            player.isFreezed = this.frozen;
            player.disableActions = this.frozen;
            player.disableMovement = this.frozen;

            if(this.activeIpl != null)
            {
                alt.removeIpl(this.activeIpl)
            }

            webview.updateView("HideProphunt");
        })

        alt.onServer("UpdatePropHuntAmount", () => {
            this.propObjectAmount--;
            webview.updateView("UpdateProphuntPropCount", [this.propObjectAmount]);
        })
    }
    
    changeFrozenStatus() {
        if (this.frozen) {
            webview.updateView("AddNotify", ["Du kannst dich nun wieder bewegen", "green"])
            native.clearPedTasks(alt.Player.local.scriptID)
            this.frozen = false;
        } else {
            animation.playAnim("hs3f_int1-1", "mp_m_freemode_01_dual-1", 1)
            webview.updateView("AddNotify", ["Du stehst nun fest", "#c72020"])
            this.frozen = true;
        }
        player.isFreezed = this.frozen;
        player.disableActions = this.frozen;
        player.disableMovement = this.frozen;
    }

    stop() {
        this.enabled = false
    }
}

export default new PropHunt()