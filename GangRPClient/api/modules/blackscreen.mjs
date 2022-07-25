import webview from "/api/modules/webview.mjs";
import * as native from "natives";
import alt from "alt";

class Blackscreen {
  constructor() {
    this.enabled = false;
    this.everyTick = null;
  }

  start() {
    this.enabled = true;
    webview.closeInterface();
    webview.updateView("SetBlackscreen", [true]);
    this.everyTick = alt.everyTick(this.everyTickHandler.bind(this));
  }
  stop() {
    this.enabled = false;
    webview.closeInterface();
    webview.updateView("SetBlackscreen", [false]);
    alt.clearEveryTick(this.everyTick);
  }

  everyTickHandler() {
    native.disableControlAction(0, 199, true);
    native.disableControlAction(0, 200, false);
  }
}

export default new Blackscreen();
