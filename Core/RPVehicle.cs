using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.VehicleData;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Core
{
    public class RPVehicle : Vehicle
    {
        public int VehicleId { get; set; }

        public int OwnerId { get; set; }

        public int VehicleDataId { get; set; }

        public int TeamId { get; set; }

        public float Fuel { get; set; }

        public IEnumerable<VehicleTuning> VehicleTunings { get; set; }

        public int InventoryId { get; set; }

        public LocalInventory Inventory { get; set; }

        public Dictionary<sbyte, RPPlayer> Passengers { get; set; }

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set
            {
                this.SetLockStateAsync(value ? VehicleLockState.Locked : VehicleLockState.Unlocked);
                _locked = value;
            }
        }

        private bool _engine;
        public bool Engine
        {
            get => _engine;
            set
            {
                this.SetEngineOnAsync(value);
                _engine = value;
            }
        }

        private bool _trunkStatus;

        public bool TrunkStatus
        {
            get => _trunkStatus;
            set
            {
                //TODO : Make the trunk open and close, currently broke!
                //SetDoorState(4, value ? (byte)1 : (byte)0);
                _trunkStatus = value;
            }
        }
        internal RPVehicle(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            VehicleId = 0;
            OwnerId = 0;
            VehicleDataId = 0;
            TeamId = 1;
            Locked = true;
            Engine = false;
            TrunkStatus = false;
            VehicleTunings = new List<VehicleTuning>();
            InventoryId = 0;
            Inventory = new LocalInventory();
            this.SetManualEngineControlAsync(true);
            Passengers = new Dictionary<sbyte, RPPlayer>();
        }

        public async Task SetIntoGarage(GarageData garageData)
        {
            await using RPContext rpContext = new RPContext();
            var vehicle = await rpContext.Vehicle.FindAsync(this.VehicleId);
            if (vehicle == null) return;
            vehicle.InGarage = true;
            vehicle.GarageDataId = garageData.Id;
            await rpContext.SaveChangesAsync();
        }

        public bool IsEngineBurning()
        {
            return base.EngineHealth <= 0;
        }

        public bool IsEnterable()
        {
            return base.EngineHealth >= -3500;
        }

        public bool TryPutPlayerIntoVehicle(RPPlayer rpPlayer)
        {
            VehicleData vehicleData = VehicleDataModule.Instance.GetVehicleDataById(VehicleDataId);

            if (vehicleData.Seats > 2)
            {
                for (sbyte i = 1; i < vehicleData.Seats-1; i++)
                {
                    if (!Passengers.ContainsKey(i))
                    {
                        Passengers.Add(i, rpPlayer);
                        rpPlayer.WarpIntoVehicle(this, i);
                        return true;
                    }
                }
            }
            else
            {
                if (!Passengers.ContainsKey(0))
                {
                    Passengers.Add(0, rpPlayer);
                    rpPlayer.WarpIntoVehicle(this, 0);
                    return true;
                }
            }
            return false;
        }
    }
}
