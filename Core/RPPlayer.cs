using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Models;
using GangRP_Server.Modules.Door;
using GangRP_Server.Modules.Injury;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities.Cloth;
using GangRP_Server.Utilities.ClothNew;
using GangRP_Server.Utilities.ClothProp;
using GangRP_Server.Utilities.Player;
using Microsoft.EntityFrameworkCore;
using ClothTypeData = GangRP_Server.Utilities.ClothNew.ClothTypeData;
using ClothVariationData = GangRP_Server.Utilities.ClothNew.ClothVariationData;
using Player = AltV.Net.Elements.Entities.Player;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Core
{

    public enum DimensionType
    {
        WORLD,
        HOUSE,
        CAMPER,
        STORAGEROOM,
        PAINTBALL
    }

    public enum Animation
    {
        HANDCUFFED,
        TIED,
        KNEEL
    }

    public class RPPlayer : Player
    {
        public int Money { get; set; }
        public int BankMoney { get; set; }

        public int BankType { get; set; }

        public int PlayerId { get; set; }

        public IEnumerable<int> Vehicles { get; set; }

        public IEnumerable<int> VehicleKeys { get; set; }

        public IEnumerable<PlayerWeapon> Weapons { get; set; }

        public Dictionary<int, Cloth> EquippedClothes { get; set; }
        public Dictionary<int, ClothTypeData> OwnedClothes { get; set; }

        public Dictionary<int, PlayerCrime> Crimes { get; set; }

        public int TeamId { get; set; }

        public PlayerTeamPermission PlayerTeamPermission { get; set; }

        public PlayerLicence Licences { get; set; }

        public HashSet<int> OwnedHouses { get; set; }

        public HashSet<int> OwnedStoragerooms { get; set; }

        public Dictionary<int, int> RentHouses { get; set; }


        public bool DutyStatus { get; set; }

        public List<int> IsCreatingInventoryType { get; set; }

        public List<int> IsLoadingInventoryType { get; set; }


        public int InventoryId { get; set; }

        public LocalInventory? Inventory { get; set; }

        public LocalInventory? LockerInventory { get; set; }

        public LocalInventory? CamperInputInventory { get; set; }
        
        public LocalInventory? CamperOutputInventory { get; set; }

        public LocalInventory? WareExportInventory { get; set; }

        public int Gender { get; set; }

        public int TimePlayed { get; set; }

        public bool IsTied { get; set; }
        public bool IsCuffed { get; set; }

        public int JailTime { get; set; }

        public int ProbationTime { get; set; }


        public int Experience { get; set; }
        public int MaximumHealth { get; set; }
        public int MaximumArmor { get; set; }
        public int Level { get; set; }
        public int MaximumAttributes { get; set; }
        public int Strength { get; set; }
        public int Vitality { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }

        public InjuryStatus? InjuryStatus { get; set; }
        public HashSet<IColShape> ColShapes { get; set; }
        public DimensionType DimensionType { get; set; }
        public CancellationTokenSource? CancellationToken { get; set; }

        public bool HasSmartphone { get; set; }
        public bool HasRadio { get; set; }

        public int PhoneNumber { get; set; }


        internal RPPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            PlayerId = 0;
            Money = 0;
            BankMoney = 0;
            PlayerId = 0;
            Vehicles = new List<int>();
            VehicleKeys = new List<int>();
            Weapons = new List<PlayerWeapon>();
            Crimes = new Dictionary<int, PlayerCrime>();
            TeamId = 1;
            PlayerTeamPermission = new PlayerTeamPermission();
            OwnedClothes = new Dictionary<int, ClothTypeData>();
            EquippedClothes = new Dictionary<int, Cloth>();
            Licences = new PlayerLicence();
            DutyStatus = false;
            OwnedHouses = new HashSet<int>();
            OwnedStoragerooms = new HashSet<int>();
            RentHouses = new Dictionary<int, int>();
            InventoryId = 0;
            Inventory = null;
            LockerInventory = null;
            CamperInputInventory = null;
            CamperOutputInventory = null;
            WareExportInventory = null;
            Gender = 1;
            TimePlayed = 0;
            IsTied = false;
            IsCuffed = false;
            JailTime = 0;
            ProbationTime = 0;
            InjuryStatus = null;
            ColShapes = new HashSet<IColShape>();
            DimensionType = DimensionType.WORLD;
            CancellationToken = null;
            HasSmartphone = false;
            HasRadio = false;
            PhoneNumber = 0;
            IsCreatingInventoryType = new List<int>();
            IsLoadingInventoryType = new List<int>();
            Strength = 0;
            Vitality = 0;
            Dexterity = 0;
            Intelligence = 0;
            Experience = 0;
            Level = 1;
            MaximumAttributes = 0;
            MaximumHealth = 200;
            MaximumArmor = 100;
        }


        public async Task<bool> MoneyToBank(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            if (Money < valueMoney) return false;
            Money -= valueMoney;
            BankMoney += valueMoney;
            await SaveMoney(true);
            return true;
        }

        public async Task<bool> BankToMoney(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            if (BankMoney < valueMoney) return false;
            BankMoney -= valueMoney;
            Money += valueMoney;
            await SaveMoney(true);
            return true;
        }

        public async Task<bool> TakeMoney(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            if (Money < valueMoney) return false;
            Money -= valueMoney;
            await SaveMoney();
            return true;
        }
        public async Task<bool> TakeBankMoney(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            if (BankMoney < valueMoney) return false;
            BankMoney -= valueMoney;
            await SaveBankMoney();
            return true;
        }


        public async Task<bool> GiveMoney(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            Money += valueMoney;
            await SaveMoney();
            return true;
        }

        public async Task<bool> GiveBankMoney(int valueMoney)
        {
            if (valueMoney <= 0) return false;
            BankMoney += valueMoney;
            await SaveBankMoney();
            return true;
        }

        private async Task SaveMoney(bool saveBankMoney = false)
        {
            await using var rpContext = new RPContext();
            var player = await rpContext.Player.FindAsync(PlayerId);
            player.Money = Money;
            if (saveBankMoney) player.BankMoney = BankMoney;
            await rpContext.SaveChangesAsync();
        }

        private async Task SaveBankMoney()
        {
            await using var rpContext = new RPContext();
            var player = await rpContext.Player.FindAsync(PlayerId);
            player.BankMoney = BankMoney;
            await rpContext.SaveChangesAsync();
        }

        public async Task SavePosition()
        {
            await using var rpContext = new RPContext();
            var player = await rpContext.Player.FindAsync(PlayerId);
            player.PositionX = this.Position.X;
            player.PositionY = this.Position.Y;
            player.PositionZ = this.Position.Z;
            await rpContext.SaveChangesAsync();
        }

        public async Task SavePosition(Position position)
        {
            await using var rpContext = new RPContext();
            var player = await rpContext.Player.FindAsync(PlayerId);
            player.PositionX = position.X;
            player.PositionY = position.Y;
            player.PositionZ = position.Z;
            await rpContext.SaveChangesAsync();
        }

        public async Task SaveHasSmartphone()
        {
            await using var rpContext = new RPContext();
            var player = await rpContext.Player.FindAsync(PlayerId);
            player.HasSmartphone = HasSmartphone;
            await rpContext.SaveChangesAsync();

        }

        public void SetPosition(Position position)
        {   
            this.SetPosition(Position.X, Position.Y, Position.Z);
        }


        public void UpdateView(string eventName, string arg)
        {
            Emit("UpdateView", eventName, arg);
        }

        public enum NotificationType
        {
            INFO,
            SUCCESS,
            ERROR,
            OOC
        }

        private String GetNotificationString(NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.INFO => "blue",
                NotificationType.SUCCESS => "green",
                NotificationType.ERROR => "red",
                NotificationType.OOC => "orange",
                _ => "",
            };
        }

        public void SendNotification(String message, NotificationType notificationType, string title = "", int duration = 5000)
        {
            Emit("UpdateView", "AddNotify", message, GetNotificationString(notificationType), title, duration);
        }


        public ClothInformationData? GetOwnCloth(int component, int drawable, int texture)
        {
            if (OwnedClothes.TryGetValue(component, out ClothTypeData? clothTypeData))
            {
                if (clothTypeData.ClothDataData.TryGetValue(drawable, out ClothDataData? clothDataData))
                {
                    if (clothDataData.ClothVariationData.TryGetValue(texture, out ClothVariationData? clothVariationData))
                    {
                        return new ClothInformationData(component, clothDataData.clothDataValue, clothVariationData.clothVariationValue, clothTypeData.clothTypeName, clothDataData.clothDataName, clothVariationData.clothVariationName, clothDataData.clothDataPrice, clothVariationData.clothVariationDbId);
                    }
                }
            }
            return null;
        }

        public void AddCloth(ClothInformationData cloth)
        {
            if (OwnedClothes.TryGetValue(cloth.clothTypeValue, out ClothTypeData? typeData))
            {
                if (typeData.ClothDataData.TryGetValue(cloth.clothDataValue, out ClothDataData? clothdata))
                {
                    clothdata.ClothVariationData.Add(cloth.clothVariationValue, new Utilities.ClothNew.ClothVariationData(cloth.clothVariationValue, cloth.clothVariationName, cloth.clothVariationDbId));
                    OwnedClothes[cloth.clothTypeValue].ClothDataData[cloth.clothDataValue] = clothdata;
                }
                else
                {
                    ClothDataData clothDataData = new ClothDataData(cloth.clothDataValue, cloth.clothDataName, cloth.clothDataPrice);
                    clothDataData.ClothVariationData.Add(cloth.clothVariationValue, new ClothVariationData(cloth.clothVariationValue, cloth.clothVariationName, cloth.clothVariationDbId));
                    OwnedClothes[cloth.clothTypeValue].ClothDataData.Add(cloth.clothDataValue, clothDataData);
                }
            }
            else
            {
                ClothTypeData clothTypeData = new ClothTypeData(cloth.clothTypeValue, cloth.clothTypeName);
                clothTypeData.ClothDataData.Add(cloth.clothDataValue, new ClothDataData(cloth.clothDataValue, cloth.clothDataName, cloth.clothDataPrice));
                clothTypeData.ClothDataData[cloth.clothDataValue].ClothVariationData.Add(cloth.clothVariationValue, new ClothVariationData(cloth.clothVariationValue, cloth.clothVariationName, cloth.clothVariationDbId));
                OwnedClothes.Add(cloth.clothTypeValue, clothTypeData);
            }
        }

        public void PlayerLoaded()
        {
            Emit("PlayerLoaded", new PlayerLoadedWriter(this));
        }

        public void SetEquippedClothes()
        {
            Emit("SetClothes", new ClothPropDataWriter(EquippedClothes));
        }

        public void SetJailClothes()
        {
            Emit("SetJailClothes");
        }

        public bool CanControlVehicle(RPVehicle rpVehicle)
        {
            //Player is owner of Vehicle
            if (rpVehicle.OwnerId == PlayerId) return true;
            //Player has key of Vehicle
            if (VehicleKeys.Any(d => d == rpVehicle.VehicleId)) return true;
            //vehicle is team vehicle and player is in same team as vehicle
            if (rpVehicle.TeamId != 1 && TeamId == rpVehicle.TeamId) return true;

            return false;
        }

        public bool CanControlHouse(House house)
        {
            if (this.OwnedHouses.Contains(house.HouseDataId) || this.RentHouses.ContainsKey(house.HouseDataId)) return true;
            return false;
        }

        public bool CanControlStorageroom(Storageroom storageroom)
        {
            if (this.OwnedStoragerooms.Contains(storageroom.Id)) return true;
            return false;
        }

        public bool CanControlDoor(Door door)
        {
            if (door.TeamHashSet.Contains(TeamId)) return true;
            return false;
        }



        public int GetHouseControlLevel(House house)
        {
            //check if player is owner of house
            if (OwnedHouses.Contains(house.HouseDataId)) return 2;
            //check if player rents this house
            else if (RentHouses.TryGetValue(house.HouseDataId, out int cost)) return 1;
            return 0;
        }

        public int GetStorageroomControlLevel(Storageroom storageroom)
        {
            //check if player is owner of house
            if (OwnedStoragerooms.Contains(storageroom.StorageroomDataId)) return 2;
            return 0;
        }


        public async void Revive(/*bool atPosition = true, */Position? position = null)
        {
            //if (position == null) position = this.Position;
            await this.SetHealthAsync(Convert.ToUInt16(MaximumHealth));
            await this.SetArmorAsync(0);
            await this.SpawnAsync(position ?? Position);
            this.InjuryStatus = null;
            this.SetBlurOut(5000);
        }

        public async Task SetMaximumHealth(int health)
        {
            this.Emit("SetPlayerMaxHealth", health);
            //await Task.Delay(5000);
        }

        public async Task SetHealth(int health)
        {
            await this.SetHealthAsync(Convert.ToUInt16(health));
        }

        public void CalculateMaximumHealth()
        {
            this.MaximumHealth = Convert.ToUInt16(200 + 3 * Math.Sqrt(Convert.ToDouble(this.Vitality)));
        }

        public async Task SetMaximumArmor(int armor)
        {
            this.Emit("SetPlayerMaxArmor", armor);
            //await Task.Delay(5000);
        }

        public async Task SetArmor(int armor)
        {
            await this.SetArmorAsync(Convert.ToUInt16(armor));
        }

        public void CalculateMaximumArmor()
        {

        }

        public void SetRunSpeed(float multiplier)
        {
            if (multiplier > 1.49f)
                multiplier = 1.49f;
            else if (multiplier < 1.0f)
                multiplier = 1.49f;
            this.Emit("SetRunSprintMultiplierForPlayer", multiplier);
        }

        public void SetSwimSpeed(float multiplier)
        {
            if (multiplier > 1.49f)
                multiplier = 1.49f;
            else if (multiplier < 1.0f)
                multiplier = 1.49f;
            this.Emit("SetSwimMultiplierForPlayer", multiplier);
        }

        public void PlayAnimationDebug(string animDict, string animName, float speed, float speedMultiplier, int duration, int flag, bool freeze)
        {
            Emit("PlayAnimDebug2", animDict, animName, speed, speedMultiplier, duration, flag, freeze);
        }

        public void PlayAnimationDebug(string animDict, string animName, int flag, bool freeze)
        {
            Emit("PlayAnimDebug", animDict, animName, flag, freeze);
        }

        public void PlayAnimation(Animation animation)
        {
            Emit("PlayAnim", (int)animation);
        }

        public void StopAnimation(string animDict, string animName, float p3, bool unfreeze)
        {
            Emit("StopAnim", animDict, animName, p3, unfreeze);
        }

        public void StopAnimation(bool unfreeze)
        {
            Emit("ClearTasks", unfreeze);
        }

        public void PlayScenario(string scenarioName)
        {
            Emit("PlayScenario", scenarioName);
        }

        public void SetBlurIn(int timeInMs)
        {
            this.Emit("BlurIn", timeInMs);
        }

        public void SetBlurOut(int timeInMs)
        {
            this.Emit("BlurOut", timeInMs);
        }

        public void OpenWarderobe()
        {
            Emit("ShowIF", "ClothShop", new WarderobeOpenDataWriter("Kleiderschrank", OwnedClothes));
        }

        public bool CanInteract()
        {
            if (IsCuffed || IsTied || InjuryStatus != null) return false;

            return true;
        }

        public bool IsAlive()
        {
            return InjuryStatus == null;
        }

        public async Task<bool> StartTask(int delay)
        {
            CancellationToken = new CancellationTokenSource();
            return await Task.Delay(delay, CancellationToken.Token).ContinueWith(task => !task.IsCanceled);
        }

        public bool CancelTask()
        {
            if (CancellationToken != null)
            {
                CancellationToken.Cancel();
                CancellationToken = null;
                return true;
            }

            return false;
        }

        public void WarpIntoVehicle(IVehicle vehicle, sbyte seatId)
        {
            
            Emit("WarpIntoVeh", vehicle, (int)seatId);
        }
    }
}
