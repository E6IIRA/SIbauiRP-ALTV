import alt from "alt"

class Cursor {
    constructor() {
        this.active = false
    }

    show() {
        if (!this.active) {
            alt.showCursor(true)
        }

        this.active = true
    }

    hide() {
        if (this.active) {
            alt.showCursor(false)
        }
        
        this.active = false
    }

    toggle() {
        this.active = !this.active
        alt.showCursor(this.active)
    }
}

export default new Cursor()