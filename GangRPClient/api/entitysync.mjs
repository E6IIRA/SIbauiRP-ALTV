import alt from "alt"
//import native from "natives"
import webview from "/api/modules/webview.mjs"

const entityData = {}

alt.onServer("entitySync:create", (id, type, position, data) => {
    if (type != 9) return
    if (data) {
       if (!entityData[type]) {
          entityData[type] = {}
       }
       if (!entityData[type][id]) {
            entityData[type][id] = {}
       }
        entityData[type][id] = data
    }
    //let currentData
    if (entityData[type] && entityData[type][id]) {
      //currentData = entityData[type][id]
      webview.updateView("AddNotify", [entityData[type][id].t, "#00a6cc"])
    }
    //alt.log(JSON.stringify(currentData))
})

// alt.onServer("entitySync:remove", (id, type) => {
//     if (type != 9) return
//     let currentData
//     if (entityData[type]) {
//          currentData = entityData[type][id]
//     } else {
//          currentData = null
//     }
//     //alt.log(currentData)
// })

// alt.onServer("entitySync:updatePosition", (id, type, position) => {
//     let currentData
//     if (entityData[type]) {
//          currentData = entityData[type][id]
//     } else {
//          currentData = null
//     }
//     //alt.log(currentData)
// })

// alt.onServer("entitySync:updateData", (id, type, newyData) => {
//     if (!entityData[type]) {
//        entityData[type] = {}
//     }
//     if (!entityData[type][id]) {
//          entityData[type][id] = {}
//     }
//     if (newyData) {
//         entityData[type][id] = newyData
//     }
//     //const currentData = entityData[type][id]
// })

// alt.onServer("entitySync:clearCache", (id, type) => {
//     if (type != 9) return
//     if (!entityData[type]) return
//     delete entityData[type][id]
// })