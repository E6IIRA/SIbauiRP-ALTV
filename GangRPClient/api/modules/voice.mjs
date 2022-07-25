import alt from "alt"
import native from "natives"
import webview from "/api/modules/webview.mjs"
import player from "/api/modules/player.mjs"

var Command
(function (Command) {
    Command[Command["PluginState"] = 0] = "PluginState"
    Command[Command["Initiate"] = 1] = "Initiate"
    Command[Command["Reset"] = 2] = "Reset"
    Command[Command["Ping"] = 3] = "Ping"
    Command[Command["Pong"] = 4] = "Pong"
    Command[Command["InstanceState"] = 5] = "InstanceState"
    Command[Command["SoundState"] = 6] = "SoundState"
    Command[Command["SelfStateUpdate"] = 7] = "SelfStateUpdate"
    Command[Command["PlayerStateUpdate"] = 8] = "PlayerStateUpdate"
    Command[Command["BulkUpdate"] = 9] = "BulkUpdate"
    Command[Command["RemovePlayer"] = 10] = "RemovePlayer"
    Command[Command["TalkState"] = 11] = "TalkState"
    Command[Command["PlaySound"] = 18] = "PlaySound"
    Command[Command["StopSound"] = 19] = "StopSound"
    Command[Command["PhoneCommunicationUpdate"] = 20] = "PhoneCommunicationUpdate"
    Command[Command["StopPhoneCommunication"] = 21] = "StopPhoneCommunication"
    Command[Command["RadioCommunicationUpdate"] = 30] = "RadioCommunicationUpdate"
    Command[Command["StopRadioCommunication"] = 31] = "StopRadioCommunication"
    Command[Command["RadioTowerUpdate"] = 32] = "RadioTowerUpdate"
    Command[Command["MegaphoneCommunicationUpdate"] = 40] = "MegaphoneCommunicationUpdate"
    Command[Command["StopMegaphoneCommunication"] = 41] = "StopMegaphoneCommunication"
})(Command || (Command = {}))


var PluginError
(function (PluginError) {
    PluginError[PluginError["OK"] = 0] = "OK"
    PluginError[PluginError["InvalidJson"] = 1] = "InvalidJson"
    PluginError[PluginError["NotConnectedToServer"] = 2] = "NotConnectedToServer"
    PluginError[PluginError["AlreadyInGame"] = 3] = "AlreadyInGame"
    PluginError[PluginError["ChannelNotAvailable"] = 4] = "ChannelNotAvailable"
    PluginError[PluginError["NameNotAvailable"] = 5] = "NameNotAvailable"
    PluginError[PluginError["InvalidValue"] = 6] = "InvalidValue"
})(PluginError || (PluginError = {}))

class PluginCommand {
    constructor(command, serverIdentifier, parameter) {
        this.Command = command
        this.ServerUniqueIdentifier = serverIdentifier
        this.Parameter = parameter
    }
}

class GameInstance {
    constructor(serverIdentifier, name, channelId, channelPassword, soundPack, SwissChannelIds) {
        this.ServerUniqueIdentifier = serverIdentifier
        this.Name = name
        this.ChannelId = channelId
        this.ChannelPassword = channelPassword
        this.SoundPack = soundPack
        this.SwissChannelIds = SwissChannelIds
    }
}

class PlayerState {
    constructor(name, position, voiceRange, isAlive, volumeOverride, distanceCulled) {
        this.Name = name
        this.Position = position
        this.VoiceRange = voiceRange
        this.IsAlive = isAlive
        this.VolumeOverride = volumeOverride
        this.DistanceCulled = distanceCulled
        this.Muffle = null
    }
}

class SelfState {
    constructor(position, rotation, voiceRange) {
        this.Position = position
        this.Rotation = rotation
        this.VoiceRange = voiceRange
        this.IsAlive = true
    }
}

class PhoneCommunication {
    constructor(name, signalStrength, volume, direct, relayedBy) {
        this.Name = name
        this.SignalStrength = signalStrength
        this.Volume = volume
        this.Direct = direct
        this.RelayedBy = relayedBy
    }
}

class RadioCommunication {
    constructor(name, senderRadioType, ownRadioType, playerMicClick, direct, isSecondary, relayedBy) {
        this.Name = name
        this.SenderRadioType = senderRadioType
        this.OwnRadioType = ownRadioType
        this.PlayMicClick = playerMicClick
        this.Secondary = isSecondary
        this.Direct = direct
        this.RelayedBy = relayedBy
        this.Volume = 1.2
    }
}

class MegaphoneCommunication {
    constructor(name, range, volume) {
        this.Name = name
        this.Range  = range
        this.Volume = volume
    }
}

class Sound {
    constructor(filename, isLoop, handle) {
        this.Filename = filename
        this.IsLoop = isLoop
        this.Handle = handle
    }
}
class VoiceClient {
    constructor(player, tsName, voiceRange, alive) {
        this.Player = player
        this.TeamSpeakName = tsName
        this.VoiceRange = voiceRange
        this.IsAlive = alive
    }
}
class VoiceManager {
    constructor() {
        this.IsEnabled = false
        this.ServerUniqueIdentifier = null
        this.SoundPack = null
        this.IngameChannel = null
        this.IngameChannelPassword = null
        this.SwissChannels = null
        this.TeamSpeakName = null
        this.VoiceRange = null
        this.IsTalking = false
        this.IsMicrophoneMuted = false
        this.IsSoundMuted = false
        this.IsConnected = false
        this.IsReady = false
        this.IsInGame = false
        this.NextUpdate = Date.now()
        this.VoiceClients = new Map()
        this.ClientIdMap = new Map()
        this.radio = 0
        this.RadioTowers = {}
        this.VoiceRanges = [2, 8, 15, 64]
        this.toggleVoiceTick = null;
        this.toggleVoiceTimeout = null;
        this.VoiceRangeIndex = 1
        this.teamsWithMegaphone = new Set([3, 5, 7])

        alt.onServer("VCInit", this.OnInitialize)
        alt.onServer("VCUpdate", this.OnUpdateVoiceClient)
        alt.onServer("VCDc", this.OnPlayerDisconnect)
        alt.onServer("VCDied", this.OnPlayerDied)
        alt.onServer("VCRev", this.OnPlayerRevived)
        alt.onServer("StartCall", this.OnEstablishCall)
        //alt.onServer("SaltyChat_EstablishedCallRelayed", this.OnEstablishCallRelayed)
        alt.onServer("EndCall", this.OnEndCall)
        alt.onServer("VCRadio", this.OnSetRadioChannel)
        //alt.onServer("SaltyChat_LeaveRadioChannel", this.OnLeaveRadioChannel)
        //alt.onServer("SaltyChat_LeaveAllRadioChannels", this.OnLeaveAllRadioChannels)
        alt.onServer("Send", this.OnPlayerIsSending)
        //alt.onServer("SaltyChat_IsSendingRelayed", this.OnPlayerIsSendingRelayed)
        //alt.onServer("SaltyChat_UpdateRadioTowers", this.OnUpdateRadioTowers)
        alt.onServer("Megaphone", this.OnUseMegaphone)
        //alt.onServer("SaltyChat_OnDisconnected", this.OnPluginDisconnected)
        //alt.onServer("SaltyChat_OnMessage", this.OnPluginMessage)
        //alt.onServer("SaltyChat_OnError", this.OnPluginError)
        //alt.onServer("SaltyChat_OnConnected", this.OnPluginConnected)
    }
    OnInitialize = (tsName, serverIdentifier, soundPack, ingameChannel, ingameChannelPassword, swissChannels, radioTowers) => {
        this.TeamSpeakName = tsName
        this.ServerUniqueIdentifier = serverIdentifier
        this.SoundPack = soundPack
        this.IngameChannel = parseInt(ingameChannel)
        this.IngameChannelPassword = ingameChannelPassword
        this.IsEnabled = true
        this.SwissChannels = swissChannels
        this.RadioTowers = radioTowers
    }
    OnUpdateVoiceClient = (playerId, voiceRange, tsName) => {
        if (playerId == null)
            return

        if (playerId == alt.Player.local.id) {
            this.VoiceRange = voiceRange
        }
        else {
            if (this.VoiceClients.has(playerId)) {
                const voiceClient = this.VoiceClients.get(playerId)
                //voiceClient.TeamSpeakName = tsName
                voiceClient.VoiceRange = voiceRange
            }
            else {
                const player = alt.Player.all.find(p => {
                    return (p.id == playerId)
                })
                if (player != undefined) {
                    this.VoiceClients.set(playerId, new VoiceClient(player, tsName, voiceRange, true))
                    this.ClientIdMap.set(tsName, playerId)
                }
            }
        }
    }
    OnStreamIn = player => {
        //alt.log("ID " + player.id)
        let voiceClient = this.VoiceClients.get(player.id)
        const range = player.getStreamSyncedMeta("range")
        if (voiceClient == undefined) {
            //alt.log("In voiceClient undefined")
            voiceClient = new VoiceClient(player, player.id, range ? range : 8, !player.getStreamSyncedMeta("dead"))
            this.VoiceClients.set(player.id, voiceClient)
            this.ClientIdMap.set(player.id, player.id)
        } else {
            //alt.log("voiceClient changed " + JSON.stringify(voiceClient))
            voiceClient.Player = player
            voiceClient.VoiceRange = range
            voiceClient.TeamSpeakName = player.id
            voiceClient.IsAlive = !player.getStreamSyncedMeta("dead")
        }

        //alt.log("Range " + range)
        //alt.log("Alive " + !player.getStreamSyncedMeta("dead"))
        //alt.log("Ts " + player.getStreamSyncedMeta("ts"))
    }
    OnStreamOut = player => {
        const voiceClient = this.VoiceClients.get(player.id)
        if (voiceClient != undefined) {
            //alt.log("Out voiceClient changed")
            if (!voiceClient.onPhone && !voiceClient.onRadio) {
                //alt.log("voiceClient delete")
                this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName)))
                this.VoiceClients.delete(player.id)
                this.ClientIdMap.delete(voiceClient.TeamSpeakName)
            } else {
                //alt.log("voiceClient null")
                voiceClient.Player = null
            }
        } else {
            //alt.logError("[VOICE][StreamOut] " + player.id)
        }
    }
    OnPlayerDisconnect = (playerId) => {
        if (this.VoiceClients.has(playerId)) {
            const voiceClient = this.VoiceClients.get(playerId)
            this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName)))
            this.VoiceClients.delete(playerId)
            this.ClientIdMap.delete(voiceClient.TeamSpeakName)
        }
    }
    OnPlayerTalking = (teamSpeakName, isTalking) => {
        var player = null
        var voiceClient = this.VoiceClients.get(parseInt(teamSpeakName))

        if (voiceClient != null) {
            player = voiceClient.Player
        }
        else if (teamSpeakName == this.TeamSpeakName) {
            player = alt.Player.local
        }

        if (player != null) {
            if (isTalking) native.playFacialAnim(player.scriptID, "mic_chatter", "mp_facial");
            else native.playFacialAnim(player.scriptID, "mood_normal_1", "facials@gen_male@variations@normal");
        }

    }
    OnPlayerDied = (playerId) => {
        if (this.VoiceClients.has(playerId)) {
            const voiceClient = this.VoiceClients.get(playerId)
            voiceClient.IsAlive = false
            //this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName, voiceClient.Player.pos, voiceClient.VoiceRange, false, 0)))
        }
    }
    OnPlayerRevived = (playerId) => {
        if (this.VoiceClients.has(playerId)) {
            const voiceClient = this.VoiceClients.get(playerId)
            voiceClient.IsAlive = true
            //this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName, voiceClient.Player.pos, voiceClient.VoiceRange, false, 1.0)))
        }
    }
    OnEstablishCall = (playerId, tsName) => {
        let voiceClient = this.VoiceClients.get(playerId)
        if (voiceClient == undefined) {
            voiceClient = new VoiceClient(null, playerId, 8, true)
            voiceClient.onPhone = true
            this.VoiceClients.set(playerId, voiceClient)
            this.ClientIdMap.set(tsName, playerId)
        } else {
            voiceClient.onPhone = true
        }
        // alt.log("Call " + playerId)
        // if (this.VoiceClients.has(playerId)) {
        //     const voiceClient = this.VoiceClients.get(playerId)
            // const ownPosition = alt.Player.local.pos
            // const playerPosition = voiceClient.Player.pos
            // alt.log("Pos " + JSON.stringify(ownPosition) + " " + JSON.stringify(playerPosition))
            // const signal = native.getZoneScumminess(native.getZoneAtCoords(ownPosition.x, ownPosition.y, ownPosition.z)) +
            // native.getZoneScumminess(native.getZoneAtCoords(playerPosition.x, playerPosition.y, playerPosition.z))
            // alt.log("Signal " + signal)
            //voiceClient.onPhone = true
            //this.ExecuteCommand(new PluginCommand(Command.PhoneCommunicationUpdate, this.ServerUniqueIdentifier, new PhoneCommunication(voiceClient.TeamSpeakName, 0, 1.2, true, null)))
        //}
    }
    OnEstablishCallRelayed = (playerId, direct, relayJson) => {
        const relays = relayJson
        if (this.VoiceClients.has(playerId)) {
            const voiceClient = this.VoiceClients.get(playerId)
            const ownPosition = alt.Player.local.pos
            const playerPosition = voiceClient.Player.pos
            this.ExecuteCommand(new PluginCommand(Command.PhoneCommunicationUpdate, this.ServerUniqueIdentifier, new PhoneCommunication(voiceClient.TeamSpeakName, native.getZoneScumminess(native.getZoneAtCoords(ownPosition.x, ownPosition.y, ownPosition.z)) +
                native.getZoneScumminess(native.getZoneAtCoords(playerPosition.x, playerPosition.y, playerPosition.z)), 1.2, direct, relays)))
        }
    }
    OnEndCall = playerId => {
        const voiceClient = this.VoiceClients.get(playerId)
        if (voiceClient != undefined) {
            if (voiceClient.Player == null && !voiceClient.onRadio) {
                this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName)))
                this.VoiceClients.delete(playerId)
                this.ClientIdMap.delete(voiceClient.TeamSpeakName)
            } else {
                voiceClient.onPhone = false
            }
        } else {
            alt.logError("[VOICE][CallEnd] " + playerId)
        }
        //alt.log("CallEnd " + playerId)
        //if (this.VoiceClients.has(playerId)) {
            //const voiceClient = this.VoiceClients.get(playerId)
            //voiceClient.onPhone = false
            //this.ExecuteCommand(new PluginCommand(Command.StopPhoneCommunication, this.ServerUniqueIdentifier, new PhoneCommunication(voiceClient.TeamSpeakName)))
        //}
    }
    OnSetRadioChannel = radioChannel => {
        if (radioChannel == 0)
            this.PlaySound("leaveRadioChannel", false, "radio")
        else
            this.PlaySound("enterRadioChannel", false, "radio")
        
        //webview.
        this.radio = radioChannel
    }
    OnLeaveRadioChannel = (radioChannel) => {
        if (radioChannel != null && radioChannel != "") {
            this.radioChannels = this.radioChannels.filter(x => x != radioChannel)
            //alt.emitServer("leaveradio", radioChannel)
        }
    }
    OnLeaveAllRadioChannels = () => {
        for (let index = 0; index < radioChannels.length; index++) {
            const channel = radioChannels[index]
            //alt.emitServer("leaveradio", channel)
        }
        this.radioChannels = []
    }
    OnPlayerIsSending = (playerId, isOnRadio) => {
        //alt.log("Send " + playerId + " " + isOnRadio)
        //let playerId = parseInt(playerHandle.id)
        //let player = playerHandle
        if (alt.Player.local.id == playerId) {
            if (isOnRadio)
                this.PlaySound("selfOnMicClick", false, "MicClick")
            else
                this.PlaySound("selfOffMicClick", false, "MicClick")

            return
        }
        // alt.log(`Sending ${playerId} ${isOnRadio}`)
        //alt.log("Has Id")
        //const voiceClient = this.VoiceClients.get(playerId)
        //voiceClient.onRadio = isOnRadio ? true : undefined
        //alt.log("Get Id " + JSON.stringify(voiceClient))
        if (isOnRadio) {
            //this.ExecuteCommand(new PluginCommand(Command.RadioCommunicationUpdate, this.ServerUniqueIdentifier, new RadioCommunication(voiceClient.TeamSpeakName, 4, 4, true, true, false, [])))
            //alt.log("RadioCommunicationUpdate")
            let voiceClient = this.VoiceClients.get(playerId)
            if (voiceClient == undefined) {
                voiceClient = new VoiceClient(null, playerId, 8, true)
                voiceClient.onRadio = true
                this.VoiceClients.set(playerId, voiceClient)
                this.ClientIdMap.set(playerId, playerId)
            } else {
                voiceClient.onRadio = true
            }
        } else {
            //this.ExecuteCommand(new PluginCommand(Command.StopRadioCommunication, this.ServerUniqueIdentifier, new RadioCommunication(voiceClient.TeamSpeakName, 0, 0, true)))
            //alt.log("StopRadioCommunication")
            const voiceClient = this.VoiceClients.get(playerId)
            if (voiceClient != undefined) {
                if (voiceClient.Player == null && !voiceClient.onPhone) {
                    this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName)))
                    this.VoiceClients.delete(playerId)
                    this.ClientIdMap.delete(voiceClient.TeamSpeakName)
                } else {
                    voiceClient.onRadio = false
                }
            } else {
                // alt.logError("[VOICE][Mute] " + playerId)
            }
        }
    }
    OnPlayerIsSendingRelayed = (playerHandle, channel, isOnRadio, relayJson) => {
        let playerId = parseInt(playerHandle.id)
        let relays = JSON.parse(relayJson)
        let player = playerHandle
        if (this.radioChannels.indexOf(channel) !== -1) {
            if (player != alt.Player.local && this.VoiceClients.has(playerId)) {
                let voiceClient = this.VoiceClients.get(playerId)
                this.ExecuteCommand(new PluginCommand(isOnRadio ? Command.RadioCommunicationUpdate : Command.StopRadioCommunication, this.ServerUniqueIdentifier, new RadioCommunication(voiceClient.TeamSpeakName, 4, 4, true, 1, true, false, relays)))
            }
        }
    }
    OnUpdateRadioTowers = radioTowers => {
        this.RadioTowers = radioTowers
        this.ExecuteCommand(new PluginCommand(Command.RadioTowerUpdate, this.ServerUniqueIdentifier, this.RadioTowers))
    }
    OnUseMegaphone = (playerId, toggle) => {
        if (this.VoiceClients.has(playerId)) {
            const voiceClient = this.VoiceClients.get(playerId)
            this.ExecuteCommand(new PluginCommand(toggle ? Command.MegaphoneCommunicationUpdate : Command.StopMegaphoneCommunication, this.ServerUniqueIdentifier, new MegaphoneCommunication(voiceClient.TeamSpeakName, 32, 1.2)))
        }
    }
    OnPluginConnected = () => {
        alt.log("OnPluginConnected")
        this.IsConnected = true
        this.IsInGame = true
        this.IsReady = true
        this.VoiceRange = this.VoiceRanges[1]
        this.Initiate()
    }
    OnPluginDisconnected = () => {
        alt.log("OnPluginDisconnected")
        this.IsInGame = false
    }
    OnPluginMessage = (messageJson) => {
        const message = JSON.parse(messageJson)

        if (message.ServerUniqueIdentifier != this.ServerUniqueIdentifier)
            return

        if (message.Command == Command.Ping) {
            this.ExecuteCommand(new PluginCommand(Command.Pong, this.ServerUniqueIdentifier, null))
            return
        }

        if (message.Parameter === typeof ('undefined') || message.Parameter == null)
            return

        const parameter = message.Parameter

        switch (message.Command) {
            case Command.PluginState:
                alt.emitServer("VCCheck", parameter.Version)
                //alt.log("PluginState")
                //this.ExecuteCommand(new PluginCommand(Command.RadioTowerUpdate, this.ServerUniqueIdentifier, this.RadioTowers))
                break
            case Command.Reset:
                if (this.NextUpdate + 1000 > Date.now()) {
                    this.IsInGame = false
                    this.Initiate()
                }
                break
            case Command.InstanceState:
                this.IsConnected = parameter.IsConnectedToServer
                this.IsInGame = true
                break
            case Command.SoundState:
                if (parameter.IsMicrophoneMuted != this.IsMicrophoneMuted) {
                    this.IsMicrophoneMuted = parameter.IsMicrophoneMuted
                    if (this.IsMicrophoneMuted) webview.updateView("VoiceRange", [0])
                    else webview.updateView("VoiceRange", [this.VoiceRanges[this.VoiceRangeIndex]])
                    //alt.emitServer("SaltyChat_MicStateChanged", this.IsMicrophoneMuted)
                }

                if (parameter.IsMicrophoneEnabled != this.IsMicrophoneEnabled) {
                    this.IsMicrophoneEnabled = parameter.IsMicrophoneEnabled
                    //alt.emitServer("SaltyChat_MicEnabledChanged", this.IsMicrophoneEnabled)
                }

                if (parameter.IsSoundMuted != this.IsSoundMuted) {
                    this.IsSoundMuted = parameter.IsSoundMuted
                    //alt.emitServer("SaltyChat_SoundStateChanged", this.IsSoundMuted)
                }

                if (parameter.IsSoundEnabled != this.IsSoundEnabled) {
                    this.IsSoundEnabled = parameter.IsSoundEnabled
                    //alt.emitServer("SaltyChat_SoundEnabledChanged", this.IsSoundEnabled)
                }

                break
            case Command.TalkState:
                this.OnPlayerTalking(parameter.Name, parameter.IsTalking)
                break
        }
    }
    OnPluginError = (errorJson) => {
        try {
            const error = JSON.parse(errorJson)
            if (error.Error == PluginError.AlreadyInGame) {
                this.Initiate()
            }
            else {
                alt.logError("[Salty Chat] Error: " + error.Error + " | Message: " + error.Message)
            }
        }
        catch {
            alt.logError("[Salty Chat] We got an error, but couldn't deserialize it...")
        }
    }
    PlaySound = (fileName, loop, handle) => {
        this.ExecuteCommand(new PluginCommand(Command.PlaySound, this.ServerUniqueIdentifier, new Sound(fileName, loop, handle)))
    }
    StopSound = (handle) => {
        this.ExecuteCommand(new PluginCommand(Command.StopSound, this.ServerUniqueIdentifier, new Sound(handle, false, handle)))
    }
    Initiate = () => {
        if (this.IsEnabled) {
            alt.log("ExecuteCommand Initiate")
            this.ExecuteCommand(new PluginCommand(Command.Initiate, this.ServerUniqueIdentifier, new GameInstance(this.ServerUniqueIdentifier, this.TeamSpeakName, this.IngameChannel, this.IngameChannelPassword, this.SoundPack, this.SwissChannels)))
        } else {
            alt.setTimeout(() => {
                this.Initiate()
            }, 500)
        }
    }
    PlayerStateUpdate = () => {
        //alt.log("Size " + this.VoiceClients.size)
        // for (const voiceClient of this.VoiceClients) {
        // }

        // this.VoiceClients.forEach((voiceClient, playerId) => {
        //     if (voiceClient.onPhone || voiceClient.onRadio) {
        //         this.ExecuteCommand(new PluginCommand(Command.PlayerStateUpdate, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName, alt.Player.local.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false)))
        //         return
        //     }
        //     this.ExecuteCommand(new PluginCommand(Command.PlayerStateUpdate, this.ServerUniqueIdentifier, new PlayerState(voiceClient.TeamSpeakName, voiceClient.Player.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, voiceClient.Player.scriptID == 0)))
        // })
        // this.ExecuteCommand(new PluginCommand(Command.SelfStateUpdate, this.ServerUniqueIdentifier, new SelfState(alt.Player.local.pos, native.getGameplayCamRot(0).z)))
    
        const PlayerStates = []
        //let client
        //try {
            // this.VoiceClients.forEach(voiceClient => {
            //     //client = voiceClient
            //     if (voiceClient.onPhone || voiceClient.onRadio) {
            //         PlayerStates.push(new PlayerState(voiceClient.TeamSpeakName, alt.Player.local.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false))
            //     } else {
            //         if (voiceClient.Player.valid) {
            //             PlayerStates.push(new PlayerState(voiceClient.TeamSpeakName, voiceClient.Player.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, voiceClient.Player.scriptID == 0))
            //         } else {
            //             this.VoiceClients.delete(voiceClient.TeamSpeakName)
            //         }
            //     }
            // })
        //} catch (error) {
            // alt.logError("[VOICE][Valid] " + client.Player.valid)
            // alt.logError("[VOICE][Client] " + JSON.stringify(client))
            //alt.logError("[VOICE][Error] " + JSON.stringify(error))
        //}

        for (const pl of alt.Player.all) { //todo optimize
            if (pl.id == alt.Player.local.id)
                continue

            // Das einkommentieren, damit der nicht stabilisierte Spieler am Boden nichts mehr hört?
            // if (player.isInjured && !player.isStabilized)
            // {
            //     PlayerStates.push(new PlayerState(pl.id, pl.pos, 0, false, 1.0, true))
            //     continue;
            // }
            // alt.log(`Update ${pl.id} ${pl.name}`)
            const voiceClient = this.VoiceClients.get(pl.id)
            if (voiceClient != undefined) {
                if (voiceClient.onPhone || voiceClient.onRadio) {
                    // alt.log(`onRadio ${pl.id} ${pl.name} ${voiceClient.onRadio}`)
                    PlayerStates.push(new PlayerState(voiceClient.TeamSpeakName, alt.Player.local.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false))
                } else if (pl.scriptID != 0) { 
                    PlayerStates.push(new PlayerState(pl.id, pl.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false))
                } else if (pl.scriptID == 0) {
                    PlayerStates.push(new PlayerState(pl.id, pl.pos, 0, false, 1.0, true))
                }
            } else if (pl.scriptID == 0) {
                PlayerStates.push(new PlayerState(pl.id, pl.pos, 0, false, 1.0, true))
            } else {
                const range = pl.getStreamSyncedMeta("range")
                PlayerStates.push(new PlayerState(pl.id, pl.pos, range == undefined ? 8 : range, !pl.getStreamSyncedMeta("dead"), 1.0, false))
                // if (voiceClient == undefined) {
                //     OnStreamIn(pl)
                // } else if (voiceClient.onPhone || voiceClient.onRadio) {
                //     PlayerStates.push(new PlayerState(voiceClient.TeamSpeakName, alt.Player.local.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false))
                // } else if (voiceClient.Player.valid) {
                //     PlayerStates.push(new PlayerState(voiceClient.TeamSpeakName, voiceClient.Player.pos, voiceClient.VoiceRange, voiceClient.IsAlive, 1.0, false))
                // }
            }
        }


        if (player.arcade == 1) {
            player.arcade = -1;
            PlayerStates.push(new PlayerState("Arcade Ambient Sound", alt.Player.local.pos, 30, true, 1.0, false))
        }
        else if (player.arcade == 0) {
            player.arcade = -1;
            this.ExecuteCommand(new PluginCommand(Command.RemovePlayer, this.ServerUniqueIdentifier, new PlayerState("Arcade Ambient Sound")))
        }


        this.ExecuteCommand(new PluginCommand(Command.BulkUpdate, this.ServerUniqueIdentifier,
            { PlayerStates, SelfState: new SelfState(alt.Player.local.pos, native.getGameplayCamRot(0).z, this.VoiceRange) }))
    }
    VoicePlayerDead = () => {
        this.VoiceRangeIndex = 0
        this.ExecuteCommand(new PluginCommand(Command.SelfStateUpdate, this.ServerUniqueIdentifier, new SelfState(alt.Player.local.pos, native.getGameplayCamRot(0).z, this.VoiceRange)))
        alt.emitServer("VCRange", this.VoiceRanges[this.VoiceRangeIndex])
    }
    ToggleVoiceRange = () => {
        if (this.IsMicrophoneMuted) return;

        this.VoiceRangeIndex++
  
        if (player.isInjured) {
            if (this.VoiceRangeIndex >= 2) {
                this.VoiceRangeIndex = 0;
            }
        } else if (this.VoiceRangeIndex >= (this.teamsWithMegaphone.has(player.team) ? 4 : 3)) {
            this.VoiceRangeIndex = 0
        }

        //if (this.VoiceRangeIndex == 3)
        //{
            //MEGAPHON?? ggf wenn man länger Y drückt, dann erst? und den 3er State rausnehmen
            // let dict = "arrest"
            // let name = "radio_chatter"

            // let prop = "prop_megaphone_01"
            // let bone = 57005
            // let attachPos = (0.1, 0.03, 0.0)
            // let attachRot = (-71, 0, 0)

            // native.requestAnimDict(dict)
            // native.taskPlayAnim(alt.Player.local.scriptID, dict, name, 8, 2, -1, 48, 0.0, false, false, false)
        //}

        let range = this.VoiceRanges[this.VoiceRangeIndex]
        if(this.toggleVoiceTick != null)
        {
            alt.clearEveryTick(this.toggleVoiceTick);
            if(this.toggleVoiceTimeout != null)
            {
                alt.clearTimeout(this.toggleVoiceTimeout);
            }
        }
        this.toggleVoiceTick = alt.everyTick(() => {
            let pos = alt.Player.local.pos
            native.drawMarker(
                1, pos.x, pos.y, pos.z - 1,
                0, 0, 0,
                0, 0, 0,
                parseFloat(1.9 * range), parseFloat(1.9 * range), 1,
                57, 192, 216, 255,
                !!false, !!false, 2, !!false, null, null, !!false
            );
        });
        this.toggleVoiceTimeout = alt.setTimeout(() => {
            alt.clearEveryTick(this.toggleVoiceTick)
        }, 2500);

        //this.VoiceRange = this.VoiceRanges[this.VoiceRangeIndex]
        ///this.PlayerStateUpdate()
        //this.ExecuteCommand(new PluginCommand(Command.SelfStateUpdate, this.ServerUniqueIdentifier, new SelfState(alt.Player.local.pos, native.getGameplayCamRot(0).z, this.VoiceRange)))
        alt.emitServer("VCRange", this.VoiceRanges[this.VoiceRangeIndex])
    }
    ExecuteCommand = (command) => {
        //alt.log("ExecuteCommand " + JSON.stringify(command) + " " + this.IsEnabled + " " + this.IsConnected)
        if (this.IsEnabled && this.IsConnected) {
            webview.updateView("VoiceCommand", [JSON.stringify(command)])
        }
    }
}

export default new VoiceManager()