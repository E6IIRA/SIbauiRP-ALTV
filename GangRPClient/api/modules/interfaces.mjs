import alt from "alt"

class Interfaces {
    constructor() {
        this.openInterface = ""
        this.phoneActive = false
        this.funkActive = false
    }

    open(interfaceName) {
        this.openInterface = interfaceName
    }

    isOpen(interfaceName) {
        return this.openInterface == interfaceName
    }

    isAnyOpen() {
        if (this.openInterface == "") return false
        if (this.openInterface == "Inventory") return false
        if (this.openInterface == "Self") return false
        if (this.openInterface == "PaintballStatistic") return false
        return true
    }

    hide() {
        this.openInterface = ""
    }
}

export default new Interfaces()