using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Enums;
using GangRP_Server.Core;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Vehicle
{
    public interface IVehicleHandler
    {
        Task<RPVehicle> CreateVehicle(string model, Position position, Rotation rotation);
        Task<RPVehicle> CreateVehicle(uint model, Position position, Rotation rotation);
        Task<RPVehicle> CreateVehicle(VehicleModel model, Position position, Rotation rotation);
        Task<RPVehicle> CreateVehicleFromDatabase(Models.Vehicle vehicle);
        Task<RPVehicle> CreateVehicleFromDatabaseAtPosition(Models.Vehicle vehicle, Position position, Rotation rotation);
        Task<RPVehicle> VehicleSetup(RPVehicle rpVehicle, Models.Vehicle vehicle);
        Task SaveAllVehiclesToDb();
        Task<List<Models.Vehicle>> GetVehiclesInGarage(RPPlayer rpPlayer, GarageData garageData);

        Task TakeVehicleOutOfGarage(RPPlayer rpPlayer, int vehicleId, GarageData garageData);
        Task<Models.Vehicle> AddVehicleToDatabase(int rpPlayerId, int vehicleDataId, Position position);

        void ParkVehicleIntoGarage(RPPlayer rpPlayer, int vehicleId, GarageData garageData);
        RPVehicle? GetRpVehicle(int vehicleId);
        void RemoveRpVehicle(int vehicleId);

        Dictionary<int, RPVehicle> GetVehicles(); 
        RPVehicle GetClosestTeamRpVehicle(Position position, int teamId, int distance = 2);

        RPVehicle GetClosestRpVehicle(Position position, int distance = 2);

        IEnumerable<RPVehicle> GetRpVehiclesInRange(Position position, int range = 2);
    }
}
