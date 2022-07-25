using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            DrugCamper = new HashSet<DrugCamper>();
            PlayerVehicleKey = new HashSet<PlayerVehicleKey>();
            TeamKeyStorage = new HashSet<TeamKeyStorage>();
            VehicleTuning = new HashSet<VehicleTuning>();
        }

        public int Id { get; set; }
        public int VehicleDataId { get; set; }
        public int PlayerId { get; set; }
        public int GarageDataId { get; set; }
        public int TeamDataId { get; set; }
        public float Fuel { get; set; }
        public uint BodyHealth { get; set; }
        public int EngineHealth { get; set; }
        public int PetrolTankHealth { get; set; }
        public bool InGarage { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationRoll { get; set; }
        public float RotationPitch { get; set; }
        public float RotationYaw { get; set; }
        public byte ColorPrimary { get; set; }
        public byte ColorSecondary { get; set; }
        public byte ColorPearl { get; set; }
        public byte ColorNeonR { get; set; }
        public byte ColorNeonG { get; set; }
        public byte ColorNeonB { get; set; }
        public byte ColorNeonA { get; set; }
        public string NumberPlate { get; set; }
        public int Distance { get; set; }
        public DateTime CreationDate { get; set; }
        public int KeyGeneration { get; set; }
        public int InventoryId { get; set; }

        public virtual GarageData GarageData { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual Player Player { get; set; }
        public virtual TeamData TeamData { get; set; }
        public virtual VehicleData VehicleData { get; set; }
        public virtual ICollection<DrugCamper> DrugCamper { get; set; }
        public virtual ICollection<PlayerVehicleKey> PlayerVehicleKey { get; set; }
        public virtual ICollection<TeamKeyStorage> TeamKeyStorage { get; set; }
        public virtual ICollection<VehicleTuning> VehicleTuning { get; set; }
    }
}
