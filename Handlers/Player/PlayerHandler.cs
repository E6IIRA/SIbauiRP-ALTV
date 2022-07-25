using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Modules;
using GangRP_Server.Modules.Crime;
using GangRP_Server.Modules.Injury;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities.Cloth;
using GangRP_Server.Utilities.ClothNew;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Player
{
    public class PlayerHandler : IPlayerHandler, IPlayerConnectEvent, IPlayerDisconnectEvent, IMinuteUpdateEvent, IPlayerEnterVehicleEvent, IPlayerLeaveVehicleEvent
    {
        private readonly ILogger _logger;
        private readonly IInventoryHandler _inventoryHandler;
        private readonly CrimeModule _crimeModule;
        private readonly InjuryDataModule _injuryDataModule;
        public Dictionary<int, RPPlayer> RPPlayers = new Dictionary<int, RPPlayer>();

        public PlayerHandler(ILogger logger, IInventoryHandler inventoryHandler, CrimeModule crimeModule, InjuryDataModule injuryDataModule)
        {
            _logger = logger;
            _inventoryHandler = inventoryHandler;
            _injuryDataModule = injuryDataModule;
            _crimeModule = crimeModule;
        }

        public enum InventoryType
        {
            SPIELER = 1,
            KLEINER_RUCKSACK = 5,
            GROSSER_RUCKSACK = 6,
            SPIND = 4,
            CAMPERINPUT = 11,
            CAMPEROUTPUT = 12,
            WAREEXPORT = 77
        }



        public async Task<RPPlayer?> LoadPlayerFromDb(IPlayer player)
        {
            try
            {
                await using var rpContext = new RPContext();

                //TODO split in data modules
                Models.Player firstOrDefault = await rpContext.Player
                    .Include(w => w.PlayerWeapon).ThenInclude(p => p.WeaponData)
                    .Include(w => w.PlayerWeapon).ThenInclude(d => d.WeaponTintData)
                    .Include(w => w.PlayerWeaponComponent).ThenInclude(d => d.WeaponComponentData)
                    .Include(w => w.PlayerClothEquipped).ThenInclude(d => d.ClothVariationData).ThenInclude(d => d.ClothData).ThenInclude(d => d.ClothTypeData)
                    .Include(w => w.PlayerClothOwned).ThenInclude(d => d.ClothVariationData).ThenInclude(d => d.ClothData).ThenInclude(d => d.ClothTypeData)
                    .Include(w => w.PlayerVehicleKey).ThenInclude(w => w.Vehicle)
                    .Include(w => w.Team)
                    .Include(w => w.PlayerTeamPermission)
                    .Include(w => w.PlayerCrimePlayer)
                    .Include(w => w.PlayerLicence)
                    .Include(w => w.PlayerHouseOwned)
                    .Include(w => w.PlayerStorageroomOwned)
                    .Include(w => w.PlayerHouseRent)
                    .Include(w => w.PlayerAttributes)
                    .FirstOrDefaultAsync(p => p.Name.Equals(player.Name));

                if (firstOrDefault == null) return null;


                RPPlayer rpPlayer = (RPPlayer)player;
                rpPlayer.PlayerId = firstOrDefault.Id;

                rpPlayer.Vehicles = firstOrDefault.Vehicle.Select(d => d.Id);
                rpPlayer.VehicleKeys = firstOrDefault.PlayerVehicleKey.Select(d => d.VehicleId);
                rpPlayer.Weapons = firstOrDefault.PlayerWeapon;

                rpPlayer.TeamId = firstOrDefault.Team.Id;
                rpPlayer.PlayerTeamPermission = firstOrDefault.PlayerTeamPermission.FirstOrDefault();


                rpPlayer.Money = firstOrDefault.Money;
                rpPlayer.BankMoney = firstOrDefault.BankMoney;
                rpPlayer.BankType = firstOrDefault.BankType;

                rpPlayer.DutyStatus = firstOrDefault.Duty;
                rpPlayer.Gender = firstOrDefault.Gender;
                rpPlayer.TimePlayed = firstOrDefault.TimePlayed;
                rpPlayer.IsTied = firstOrDefault.IsTied;
                rpPlayer.IsCuffed = firstOrDefault.IsCuffed;
                rpPlayer.JailTime = firstOrDefault.JailTime;
                rpPlayer.HasSmartphone = firstOrDefault.HasSmartphone;
                rpPlayer.HasRadio = firstOrDefault.HasRadio;
                rpPlayer.PhoneNumber = firstOrDefault.PhoneNumber;


                PlayerAttributes playerAttributes = firstOrDefault.PlayerAttributes.FirstOrDefault();

                //New Player, generate new playerAttributes
                if (playerAttributes == null)
                {
                    var insert = new PlayerAttributes()
                    {
                        PlayerId = rpPlayer.PlayerId
                    };

                    await rpContext.PlayerAttributes.AddAsync(insert);
                    await rpContext.SaveChangesAsync();

                    rpPlayer.Strength = insert.Strength;
                    rpPlayer.Vitality = insert.Vitality;
                    rpPlayer.Dexterity = insert.Dexterity;
                    rpPlayer.Intelligence = insert.Intelligence;
                    rpPlayer.Experience = insert.Experience;
                    rpPlayer.MaximumAttributes = insert.MaximumAttributes;
                    rpPlayer.Level = insert.Level;
                }
                else
                {
                    rpPlayer.Strength = playerAttributes.Strength;
                    rpPlayer.Vitality = playerAttributes.Vitality;
                    rpPlayer.Dexterity = playerAttributes.Dexterity;
                    rpPlayer.Intelligence = playerAttributes.Intelligence;
                    rpPlayer.Experience = playerAttributes.Experience;
                    rpPlayer.MaximumAttributes = playerAttributes.MaximumAttributes;
                    rpPlayer.Level = playerAttributes.Level;
                }


                //Mögliche Natives:
                //setSwimMultiplierForPlayer
                //setRunSprintMultiplierForPlayer
                //setPlayerMaxHealth
                //setPlayerMaxArmour
                //setPlayerWeaponDefenseModifier
                //setPlayerMeleeWeaponDefenseModifier
                //Eventuell weitere Effekt beim Farmen, Ausrauben etc.
                //Stärke -> Mehr tragen können?


                //InventoryId sollte sich eigentlich aus der Tabelle "player_inventories" geholt werden!!
                //rpPlayer.InventoryId = firstOrDefault.InventoryId;
                

                rpPlayer.ProbationTime = firstOrDefault.ProbationTime;

                (await rpContext.Player.FindAsync(firstOrDefault.Id)).IsOnline = true;
                await  rpContext.SaveChangesAsync();

                //LATER LOADUP

                await rpPlayer.SpawnAsync(firstOrDefault.Position);

                rpPlayer.Model = firstOrDefault.Gender == 0 ? (uint)0x705E61F2 : 0x9C9EFFD8;

                await rpPlayer.SetRotationAsync(firstOrDefault.Rotation);

                rpPlayer.CalculateMaximumHealth();

                await rpPlayer.SetHealthAsync(firstOrDefault.Health);
                await rpPlayer.SetArmorAsync(firstOrDefault.Armor);

                //if player is Alive, injurystatus = null, otherwise create new InjuryStatus
                if (firstOrDefault.InjuryTypeDataId != 1)
                {
                    rpPlayer.InjuryStatus = new InjuryStatus(firstOrDefault.InjuryTypeDataId, _injuryDataModule.GetById(firstOrDefault.InjuryTypeDataId).TreatmentType, firstOrDefault.InjuryTimeLeft);
                }


                /*
                 * Player Equipped Weapons
                 */

                foreach (var weapon in firstOrDefault.PlayerWeapon)
                {
                    await player.GiveWeaponAsync(weapon.WeaponData.Hash, weapon.Ammo, false);
                    player.SetWeaponTintIndex(weapon.WeaponData.Hash, weapon.WeaponTintData.Value);
                }

                /*
                 * Player Equipped Weapon Components
                 */

                foreach (var component in firstOrDefault.PlayerWeaponComponent)
                {
                    player.AddWeaponComponent(component.WeaponComponentData.WeaponData.Hash, component.WeaponComponentData.Value);
                }


                /*
                 * Player Equipped Clothes
                 */

                Dictionary<int, Cloth> equippedClothes = new Dictionary<int, Cloth>();

                foreach (PlayerClothEquipped playerClothEquipped in firstOrDefault.PlayerClothEquipped)
                {
                    var component = playerClothEquipped.ClothVariationData.ClothData.ClothTypeData.Value;
                    var drawable = playerClothEquipped.ClothVariationData.ClothData.Value;
                    var texture = playerClothEquipped.ClothVariationData.Value;
                    var price = playerClothEquipped.ClothVariationData.ClothData.Price;
                    equippedClothes.Add(component, new Cloth(playerClothEquipped.ClothVariationDataId, component, drawable, texture, price));
                    //_logger.Debug($"component {component} drawable {drawable} texture {texture}");
                }
                rpPlayer.EquippedClothes = equippedClothes;



                /*
                 * Player Owned Clothes
                 */

                Dictionary<int, Dictionary<int, Dictionary<int, Cloth>>> ownedClothes = new Dictionary<int, Dictionary<int, Dictionary<int, Cloth>>>();
                foreach (PlayerClothOwned playerClothOwned in firstOrDefault.PlayerClothOwned)
                {
                    var clothVariation = playerClothOwned.ClothVariationData;
                    var clothData = clothVariation.ClothData;
                    var clothType = clothData.ClothTypeData;
                    var component = clothType.Value;
                    var drawable = clothData.Value;
                    var texture = clothVariation.Value;
                    var dbId = clothVariation.Id;
                    var price = clothData.Price;
                    var clothTypeName = clothType.Name;
                    var clothDataName = clothData.Name;
                    var clothVariationName = clothVariation.Name;
                    rpPlayer.AddCloth(new ClothInformationData(component, drawable, texture, clothTypeName, clothDataName, clothVariationName, price, dbId));
                    //_logger.Debug($"component {component} drawable {drawable} texture {texture}");
                }

                rpPlayer.Crimes = firstOrDefault.PlayerCrimePlayer.ToDictionary(crime => crime.Id);

                PlayerLicence playerLicence = firstOrDefault.PlayerLicence.FirstOrDefault();


                //New Player, generate playerLicence
                if (playerLicence == null)
                {
                    var insert = new PlayerLicence()
                    {
                        PlayerId = rpPlayer.PlayerId

                    };

                    await rpContext.PlayerLicence.AddAsync(insert);
                    await rpContext.SaveChangesAsync();

                    rpPlayer.Licences = insert;
                }
                else
                {
                    rpPlayer.Licences = playerLicence;
                }


                //Owned Houses
                rpPlayer.OwnedHouses = firstOrDefault.PlayerHouseOwned.Select(d => d.HouseId).ToHashSet();

                //Owned Storagerooms
                rpPlayer.OwnedStoragerooms = firstOrDefault.PlayerStorageroomOwned.Select(d => d.StorageRoomId).ToHashSet();

                //Rent Houses
                foreach (var playerHouseRent in firstOrDefault.PlayerHouseRent)
                {
                    rpPlayer.RentHouses.Add(playerHouseRent.HouseId, playerHouseRent.Cost);
                }

                //Load Players Inventory (zum reaktivieren, oben Inventory ID setzen lassen
                //rpPlayer.Inventory = await _inventoryHandler.LoadInventory(rpPlayer.InventoryId);

                rpPlayer.Inventory =
                    await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer, InventoryType.SPIELER);
                rpPlayer.InventoryId = rpPlayer.Inventory.Id;

                /* Beide folgenden Inventare werden erst geladen, wenn der Spieler darauf zugreift!
                 rpPlayer.LockerInventory =
                    await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer, InventoryType.SPIND);
                
                rpPlayer.WareExportInventory =
                    await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer, InventoryType.WAREEXPORT);*/

                rpPlayer.PlayerLoaded();

                return rpPlayer;
            }
            catch (Exception e)
            {
                _logger.Error("[Exception] " + e.InnerException);
                return null;
            }
        }

        public async Task SavePlayerToDb(IPlayer player, bool disconnect = false)
        {
            await using var rpContext = new RPContext();

            RPPlayer rpPlayer = (RPPlayer) player;

            Models.Player dbPlayer = await rpContext.Player.FindAsync(rpPlayer.PlayerId);


            if (player.Dimension == 0)
            {
                dbPlayer.PositionX = player.Position.X;
                dbPlayer.PositionY = player.Position.Y;
                dbPlayer.PositionZ = player.Position.Z;
            }

            dbPlayer.RotationRoll = player.Rotation.Roll;
            dbPlayer.RotationPitch = player.Rotation.Pitch;
            dbPlayer.RotationYaw = player.Rotation.Yaw;

            dbPlayer.Health = player.Health;
            dbPlayer.Armor = player.Armor;
            dbPlayer.TimePlayed = rpPlayer.TimePlayed;
            dbPlayer.JailTime = rpPlayer.JailTime;
            dbPlayer.ProbationTime = rpPlayer.ProbationTime;
            dbPlayer.InjuryTypeDataId = rpPlayer.InjuryStatus?.InjuryTypeDataId ?? 1;
            dbPlayer.InjuryTimeLeft = rpPlayer.InjuryStatus?.TimeLeft ?? 0;

            if (disconnect)
            {
                dbPlayer.IsOnline = false;
            }

            await rpContext.SaveChangesAsync();
        }

        public async Task SaveAllPlayersToDb()
        {
            foreach (var rpPlayer in RPPlayers.Values)
            {
                await SavePlayerToDb(rpPlayer);
            }
        }

        public async void OnPlayerConnect(IPlayer player, string reason)
        {
            //if (!player.Exists) return;
            _logger.Info($"#LOGIN {player.Name} IP {player.Ip} HWID {player.SocialClubId} HWIDEX {player.HardwareIdExHash};");
           RPPlayer? rpPlayer = await LoadPlayerFromDb(player);
           if (rpPlayer == null)
           { 
               player.Kick("Player not Loaded correctly"); 
               return;
           }
           this.RPPlayers.Add(rpPlayer.PlayerId, rpPlayer);
        }

#if DEBUG
        public void OnPlayerDisconnect(IPlayer player, string reason)
        {
            _logger.Info($"#LOGOUT {player.Name}");
            RPPlayer rpPlayer = (RPPlayer)player;

            if (RPPlayers.ContainsKey(rpPlayer.PlayerId))
            {
                RPPlayers.Remove(rpPlayer.PlayerId);
                SavePlayerToDb(player, true).Wait();
                _inventoryHandler.UnloadInventory(rpPlayer.InventoryId);
                if (rpPlayer.LockerInventory != null) _inventoryHandler.UnloadInventory(rpPlayer.LockerInventory.Id);
                if (rpPlayer.CamperInputInventory != null) _inventoryHandler.UnloadInventory(rpPlayer.CamperInputInventory.Id);
                if (rpPlayer.CamperOutputInventory != null) _inventoryHandler.UnloadInventory(rpPlayer.CamperOutputInventory.Id);
                if (rpPlayer.WareExportInventory != null) _inventoryHandler.UnloadInventory(rpPlayer.WareExportInventory.Id);
            }



        }
#else
        public async void OnPlayerDisconnect(IPlayer player, string reason)
        {
            _logger.Info($"#LOGOUT {player.Name}");
            RPPlayer rpPlayer = (RPPlayer)player;
            RPPlayers.Remove(rpPlayer.PlayerId);
            await SavePlayerToDb(player, true);
            _inventoryHandler.UnloadInventory(rpPlayer.InventoryId);
        }
#endif

        public RPPlayer? GetOnlineRPPlayerByPlayerId(int playerId)
        {
            if (RPPlayers.TryGetValue(playerId, out RPPlayer? rpPlayer))
            {
                return rpPlayer;
            }

            return null;
        }


        public async void OnMinuteUpdate()
        {
            foreach (var rpPlayer in RPPlayers.Values)
            {
                if (!rpPlayer.Exists) continue;

                rpPlayer.TimePlayed++;
                //check if player is in jail
                if (rpPlayer.JailTime > 0)
                {
                    rpPlayer.JailTime--;

                    //check if player will be released
                    if (rpPlayer.JailTime == 0)
                    {
                        rpPlayer.SendNotification("Deine Haftzeit ist abgelaufen.", RPPlayer.NotificationType.INFO, "Staatsgefängnis");
                    }
                    else
                    {
                        //if time is 5, 10, 15 etc. send message
                        if (rpPlayer.JailTime % 5 == 0)
                        {
                            rpPlayer.SendNotification($"Du bist noch für {rpPlayer.JailTime} Einheiten inhaftiert", RPPlayer.NotificationType.INFO, "Staatsgefängnis");
                        }
                    }

                }
                //check if player has probation
                if (rpPlayer.ProbationTime > 0)
                {
                    //if player has crime again, probation is not going down!
                    if (_crimeModule.GetJailtimeAndCost(rpPlayer).Time == 0)
                    {
                        rpPlayer.ProbationTime--;

                        if (rpPlayer.ProbationTime == 0)
                        {
                            rpPlayer.SendNotification("Deine Bewährung ist abgelaufen.", RPPlayer.NotificationType.INFO, "Staatsgefängnis");
                        }
                        else
                        {
                            //if time is 5, 10, 15 etc. send message
                            if (rpPlayer.ProbationTime % 5 == 0)
                            {
                                rpPlayer.SendNotification($"Du bist noch für {rpPlayer.ProbationTime} Einheiten auf Bewährung", RPPlayer.NotificationType.INFO, "Staatsgefängnis");
                            }
                        }
                    }
                }

                if (rpPlayer.InjuryStatus != null)
                {
                    _logger.Info($"{rpPlayer.InjuryStatus.InjuryTypeDataId}/ {rpPlayer.InjuryStatus.TimeLeft}");
                    rpPlayer.InjuryStatus.TimeLeft--;
                    if (rpPlayer.InjuryStatus.TimeLeft == 0)
                    {
                        //Player has injury which heals himself lel
                        if (rpPlayer.InjuryStatus.TreatmentType == TreatmentType.SELF)
                        {
                            rpPlayer.Revive();
                        }
                        else
                        {
                            rpPlayer.SendNotification("Du bist verstorben oder iwie sowas lel", RPPlayer.NotificationType.INFO);
                            rpPlayer.Revive();
                        }


                    }
                }

                await SavePlayerToDb(rpPlayer);
            }
            
            //_logger.Info("Players Saved");
        }

        public void OnPlayerEnterVehicle(IVehicle vehicle, IPlayer player, sbyte seat)
        {
            RPVehicle rpVehicle = (RPVehicle) vehicle;
            RPPlayer rpPlayer = (RPPlayer) player;
            _logger.Info($"SEAT {seat}");
            if (!rpVehicle.Passengers.ContainsKey(seat)) rpVehicle.Passengers.Add(seat, rpPlayer);
        }

        public void OnPlayerLeaveVehicle(IVehicle vehicle, IPlayer player, sbyte seat)
        {
            RPVehicle rpVehicle = (RPVehicle)vehicle;
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpVehicle.Passengers.ContainsKey(seat)) rpVehicle.Passengers.Remove(seat);
        }

        public HashSet<RPPlayer> GetRpPlayers()
        {
            return RPPlayers.Values.ToHashSet();
        }
    }
}
