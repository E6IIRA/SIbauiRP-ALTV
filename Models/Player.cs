using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Player
    {
        public Player()
        {
            PlayerAttributes = new HashSet<PlayerAttributes>();
            PlayerClothEquipped = new HashSet<PlayerClothEquipped>();
            PlayerClothOwned = new HashSet<PlayerClothOwned>();
            PlayerCrimeOfficer = new HashSet<PlayerCrime>();
            PlayerCrimePlayer = new HashSet<PlayerCrime>();
            PlayerHouseOwned = new HashSet<PlayerHouseOwned>();
            PlayerHouseRent = new HashSet<PlayerHouseRent>();
            PlayerInventories = new HashSet<PlayerInventories>();
            PlayerLicence = new HashSet<PlayerLicence>();
            PlayerPhoneContact = new HashSet<PlayerPhoneContact>();
            PlayerStorageroomOwned = new HashSet<PlayerStorageroomOwned>();
            PlayerTeamPermission = new HashSet<PlayerTeamPermission>();
            PlayerVehicleKey = new HashSet<PlayerVehicleKey>();
            PlayerWeapon = new HashSet<PlayerWeapon>();
            PlayerWeaponComponent = new HashSet<PlayerWeaponComponent>();
            Vehicle = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int BankMoney { get; set; }
        public sbyte BankType { get; set; }
        public ushort Health { get; set; }
        public ushort Armor { get; set; }
        public byte Gender { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationRoll { get; set; }
        public float RotationPitch { get; set; }
        public float RotationYaw { get; set; }
        public int PhoneNumber { get; set; }
        public int PhoneBalance { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
        public bool Duty { get; set; }
        public bool IsTied { get; set; }
        public bool IsCuffed { get; set; }
        public int InventoryId { get; set; }
        public int TimePlayed { get; set; }
        public int JailTime { get; set; }
        public int ProbationTime { get; set; }
        public int InjuryTypeDataId { get; set; }
        public int InjuryTimeLeft { get; set; }
        public bool HasSmartphone { get; set; }
        public bool HasRadio { get; set; }

        public virtual Account Account { get; set; }
        public virtual InjuryTypeData InjuryTypeData { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual TeamData Team { get; set; }
        public virtual ICollection<PlayerAttributes> PlayerAttributes { get; set; }
        public virtual ICollection<PlayerClothEquipped> PlayerClothEquipped { get; set; }
        public virtual ICollection<PlayerClothOwned> PlayerClothOwned { get; set; }
        public virtual ICollection<PlayerCrime> PlayerCrimeOfficer { get; set; }
        public virtual ICollection<PlayerCrime> PlayerCrimePlayer { get; set; }
        public virtual ICollection<PlayerHouseOwned> PlayerHouseOwned { get; set; }
        public virtual ICollection<PlayerHouseRent> PlayerHouseRent { get; set; }
        public virtual ICollection<PlayerInventories> PlayerInventories { get; set; }
        public virtual ICollection<PlayerLicence> PlayerLicence { get; set; }
        public virtual ICollection<PlayerPhoneContact> PlayerPhoneContact { get; set; }
        public virtual ICollection<PlayerStorageroomOwned> PlayerStorageroomOwned { get; set; }
        public virtual ICollection<PlayerTeamPermission> PlayerTeamPermission { get; set; }
        public virtual ICollection<PlayerVehicleKey> PlayerVehicleKey { get; set; }
        public virtual ICollection<PlayerWeapon> PlayerWeapon { get; set; }
        public virtual ICollection<PlayerWeaponComponent> PlayerWeaponComponent { get; set; }
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}
