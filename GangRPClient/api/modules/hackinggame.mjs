import * as alt from 'alt-client';
import * as natives from 'natives';
import webview from "./webview.mjs";

class HackingGame {
    /**
     * Init The Hacking Game
     * @param {string} word 8 Char string that will be the Solution 
     * @param {number} lives [OPTIONAL] Number of lives. Default 3 
     * @param {number} minSpeed [OPTIONAL] minimum Rotation Speed of column - Default 10
     * @param {number} maxSpeed [OPTIONAL] maximum Rotation Speed of column. Default 100
     */
    
    constructor() {
        this.active = false;
        this.setup = false;
        this.timerAction = {
            None: 0,
            Reset: 1,
            Remove: 2,
            Kill: 3
        }
        this.livesStart = 3;
        this.scaleForm = 0;
        this.timer = 0;
        this.action = this.timerAction.None;
        this.inputReturn = 0;
        this.finished = false;
        this.setup = true;
        this.lives = 0;
        this.everyTick = undefined;
    }

    init(word, lives = 3, minSpeed = 10, maxSpeed = 100, event = "") {
        this.livesStart = lives;
        this.word = word.toUpperCase();
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
        this.event = event;
    }

    start() {
        if(!this.setup) {
            this._logError("Not Setup! Check class constructor");
            return;
        }

        if(this.word === null) {
            this._logError("Word was null - Did you Setup Correct?");
            return;
        }

        if(this.everyTick !== undefined) {
            this._logError("Game is already running!");
            return;
        }

        natives.setPlayerControl(alt.Player.local.scriptID, false, 0);

        this.scaleForm = natives.requestScaleformMovieInteractive("Hacking_PC");

        this.active = true;
        webview.updateView("ShowHud", [false]);
        
        this._startInternal();
        this.lives = this.livesStart;
    }

    /**
    * 
    * @param {boolean} outcome 
    */
    _stop(outcome) {
        this._scaleformRemove();
        natives.setPlayerControl(alt.Player.local.scriptID, true, 0);
        alt.clearEveryTick(this.everyTick);
        this.everyTick = undefined;
        alt.emitServer(this.event, outcome);
        this.active = false;
        webview.updateView("ShowHud", [true]);
    }

    _startInternal(tryNumber = 0) {
        if(!natives.hasScaleformMovieLoaded(this.scaleForm)) {
            // 2.5 Seconds should be enough?
            if(tryNumber > 100) { 
                this._logError("Could Not Load Scaleform. Aborting");
                return;
            }

            alt.setTimeout(() => {
                this._startInternal(++tryNumber);
            },25)
            return;
        }

        natives.beginScaleformMovieMethod(this.scaleForm, "SET_BACKGROUND");
        natives.scaleformMovieMethodAddParamInt(0);
        natives.endScaleformMovieMethod();

        this._scaleformRunProgram(4);
        this._scaleformRunProgram(83);

        this._scaleformUpdateLives();

        natives.beginScaleformMovieMethod(this.scaleForm, "SET_ROULETTE_WORD");
        this._scaleformPushString(this.word);
        natives.endScaleformMovieMethod();

        for(let i = 0; i < 8; i++) {
            natives.beginScaleformMovieMethod(this.scaleForm, "SET_COLUMN_SPEED");
            natives.scaleformMovieMethodAddParamInt(i);
            natives.scaleformMovieMethodAddParamFloat(Math.random() * (this.maxSpeed - this.minSpeed) + this.minSpeed);
            natives.endScaleformMovieMethod();
        }

        this.everyTick = alt.everyTick(() => {
            this._updateGame();
        })
    }

    _updateGame() {
        natives.drawScaleformMovieFullscreen(this.scaleForm, 255, 255, 255, 255, 0);
        if(this.action === this.timerAction.None) {
            this._scaleformCheckInput(32, 172, 8);
            this._scaleformCheckInput(33, 173, 9);
            this._scaleformCheckInput(34, 174, 10);
            this._scaleformCheckInput(35, 175, 11);

            if(natives.isControlJustPressed(2, 201)) {
                natives.beginScaleformMovieMethod(this.scaleForm, "SET_INPUT_EVENT_SELECT");
                this.inputReturn = natives.endScaleformMovieMethodReturnValue();
            }
        }

        if(this.inputReturn !== 0) {
            if(natives.isScaleformMovieMethodReturnValueReady(this.inputReturn)) {
                switch(natives.getScaleformMovieMethodReturnValueInt(this.inputReturn)) {
                    
                    // Player succeeded in hack
                    case 86: 
                        this.timer = natives.getGameTimer() + 2000;
                        this.action = this.timerAction.Remove;
                        natives.playSoundFrontend(-1, "HACKING_SUCCESS", 0, 1);
                        natives.beginScaleformMovieMethod(this.scaleForm, "SET_ROULETTE_OUTCOME");
                        natives.scaleformMovieMethodAddParamBool(true);
                        this._scaleformPushString("Successful Hacked");
                        natives.endScaleformMovieMethod();
                    break;

                    // Player failed one of the columns (our job to find if they completely failed)
                    case 87: 
                        natives.playSoundFrontend(-1, "HACKING_CLICK_BAD", 0, 1);
                        this.lives--;
                        if(this.lives <= 0) {
                            this.timer = natives.getGameTimer() + 2000;
                            this.action = this.timerAction.Kill;
                            this._scaleformRemove();
                            natives.playPedAmbientSpeechNative(alt.Player.local.scriptID, "GENERIC_CURSE_HIGH", "SPEECH_PARAMS_FORCE_FRONTEND", 1);
                        } else {
                            this.timer = natives.getGameTimer() + 500;
                            this.action = this.timerAction.Reset;
                            natives.callScaleformMovieMethod(this.scaleForm, "STOP_ROULETTE");
                            this._scaleformUpdateLives();
                        }
                    break;

                    // Properly hit character
                    case 92:
                        natives.playSoundFrontend(-1, "HACKING_CLICK", 0, 1);
                    break;

                }
                this.inputReturn = 0;
            }
        }

        if(this.action !== this.timerAction.None && natives.getGameTimer() >= this.timer) {
            switch(this.action) {
                
                case this.timerAction.Remove:
                    this._stop(true);
                break;

                case this.timerAction.Reset:
                    this._scaleformReset();
                break;

                case this.timerAction.Kill:
                    this._stop(false);
                break;
            }

            this.timer = 0;
            this.action = this.timerAction.None;
        }
    }

    /**
     * 
     * @param {string} msg 
     */
    _logError(msg) {
        alt.logError(`[HackingGame] ${msg}`);
    }

    /**
     * 
     * @param {string} text 
     */
    _scaleformPushString(text) {
        natives.beginTextCommandScaleformString("STRING");
        natives.addTextComponentSubstringPlayerName(text);
        natives.endTextCommandScaleformString();
    }

    _scaleformUpdateLives() {
        natives.beginScaleformMovieMethod(this.scaleForm, "SET_LIVES");
        natives.scaleformMovieMethodAddParamInt(this.lives);
        natives.scaleformMovieMethodAddParamInt(2);
        natives.endScaleformMovieMethod();
    }

    /**
     * @param {number} program
     */
    _scaleformRunProgram(program) {
        natives.beginScaleformMovieMethod(this.scaleForm, "RUN_PROGRAM");
        natives.scaleformMovieMethodAddParamInt(program);
        natives.endScaleformMovieMethod();
    }

    /**
     * 
     * @param {number} first 
     * @param {number} second 
     * @param {number} input 
     */
    _scaleformCheckInput(first, second, input) {
        if(natives.isControlJustPressed(2, first) || natives.isControlJustPressed(2, second)) {
            natives.playSoundFrontend(-1, "HACKING_MOVE_CURSOR", 0, 1);
            natives.beginScaleformMovieMethod(this.scaleForm, "SET_INPUT_EVENT");
            natives.scaleformMovieMethodAddParamInt(input);
            natives.endScaleformMovieMethod();
        }
    }

    _scaleformRemove() {
        natives.setScaleformMovieAsNoLongerNeeded(this.scaleForm);
        this.scaleForm = 0;
        this.finished = true;
    }

    _scaleformReset() {
        natives.callScaleformMovieMethod(this.scaleForm, "RESET_ROULETTE");
    }
}


export default new HackingGame()