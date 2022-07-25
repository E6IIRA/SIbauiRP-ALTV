import alt from "alt-client";
import native from "natives";

class InstructionalKeys {
    constructor() {
        this.enabled = false
        this.scaleform = null
        this.keyAmount = 6;
    }

    async init() {
        this.scaleform = await this.requestScaleform();
        
        native.drawScaleformMovieFullscreen(this.scaleform, 255, 255, 255, 0, 0);
        
        native.beginScaleformMovieMethod(this.scaleform, "CLEAR_ALL");
        native.endScaleformMovieMethod();

        native.beginScaleformMovieMethod(this.scaleform, "SET_CLEAR_SPACE");
        native.scaleformMovieMethodAddParamInt(200);
        native.endScaleformMovieMethod();
    }
    
    async requestScaleform() {
        return new Promise((resolve, reject) => {
            let scaleform = native.requestScaleformMovie("instructional_buttons");
            
            if (native.hasScaleformMovieLoaded(scaleform)) {
                return resolve(scaleform);
            }
            
            let iterations = 0;
            
            const check = alt.setInterval(() => {
                if (iterations === 50) {
                    alt.clearInterval(check);
                    return reject("Not loaded in time");
                }
                
                if (native.hasScaleformMovieLoaded(scaleform)) {
                    alt.clearInterval(check);
                    return resolve(scaleform);
                }
                
                iterations++;
            }, 50)
        });
    }
    
    addKey(key, message) {
        native.beginScaleformMovieMethod(this.scaleform, "SET_DATA_SLOT");
        native.scaleformMovieMethodAddParamInt(this.keyAmount++);
        native.scaleformMovieMethodAddParamPlayerNameString(native.getControlInstructionalButton(2, key, true));
        native.beginTextCommandScaleformString("STRING");
        native.addTextComponentSubstringPlayerName(message);
        native.endTextCommandScaleformString();
        native.endScaleformMovieMethod();
    }
    
    finish() {
        native.beginScaleformMovieMethod(this.scaleform, "DRAW_INSTRUCTIONAL_BUTTONS");
        native.endScaleformMovieMethod();

        native.beginScaleformMovieMethod(this.scaleform, "SET_BACKGROUND_COLOUR");
        native.scaleformMovieMethodAddParamInt(0);
        native.scaleformMovieMethodAddParamInt(0);
        native.scaleformMovieMethodAddParamInt(0);
        native.scaleformMovieMethodAddParamInt(80);
        native.endScaleformMovieMethod();
        
        this.keyAmount = 6;
    }
    
    draw() {
        native.drawScaleformMovieFullscreen(this.scaleform, 255, 255, 255, 255, 0);
    }
}
export default new InstructionalKeys()