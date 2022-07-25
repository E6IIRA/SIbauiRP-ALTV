import * as native from "natives";
import alt from "alt"

class Perico {
    constructor() {
        this.nearIsland = false;
        this.everyTick = null;
        this.interval = alt.setInterval(this.positionCheck.bind(this), 5000)
        let blip = native.addBlipForCoord(5943.5679611650485, -6272.114833599767,2); // a invisible blip to make the map clickable at the island
        native.setBlipSprite(blip, 407);
        native.setBlipScale(blip, 0);
        native.setBlipAsShortRange(blip, false);
    }

    toggleIsland(state) {
        this.nearIsland = state;
        native.setIslandHopperEnabled('HeistIsland', state);
        native.setScenarioGroupEnabled('Heist_Island_Peds', state);
        native.setAudioFlag("PlayerOnDLCHeist4Island", state);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Zones", state, state);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Disabled_Zones", false, state);
        if(state)
        {
            this.everyTick = alt.everyTick(this.activateIslandRadar.bind(this));
        }
        else
        {
            alt.clearEveryTick(this.everyTick);
        }
    }

    activateIslandRadar() {
        native.setRadarAsExteriorThisFrame();
        native.setRadarAsInteriorThisFrame(alt.hash("h4_fake_islandx"), 4700.0, -5145.0, 0, 0);
    }

    positionCheck() {
        if(alt.Player.local.pos.distanceTo(new alt.Vector3(4840.571, -5174.425, 2.0)) < 2000)
        {
            if(!this.nearIsland)
            {
                this.toggleIsland(true)
            }
        }
        else
        {
            if(this.nearIsland)
            {
                this.toggleIsland(false)
            }
        }
    }
}

export default new Perico()