using System.Collections.Generic;
using System.Text.Json;
using AltV.Net;
using GangRP_Server.Modules.VehicleOverview;
using GangRP_Server.Utilities.ClothProp;
using GangRP_Server.Utilities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.VehicleOverview
{
    public class VehicleOverviewWriter : IWritable
    {
        private readonly List<OverviewVehicle> _overviewVehicles;
        private readonly bool _hasTeam;

        public VehicleOverviewWriter(List<OverviewVehicle> overviewVehicles, bool hasTeam)
        {
            this._overviewVehicles = overviewVehicles;
            this._hasTeam = hasTeam;

        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("ht");
            writer.Value(_hasTeam);
            writer.Name("data");
            writer.BeginArray();
                foreach (var value in _overviewVehicles)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.VehicleId);
                        writer.Name("n");
                        writer.Value(value.VehicleHash);
                        writer.Name("g");
                        writer.Value(value.GarageName);
                        if (!value.InGarage)
                        {
                            writer.Name("in");
                            writer.Value(false);
                        }
                        writer.Name("x");
                        writer.Value(value.Position.X);
                        writer.Name("y");
                        writer.Value(value.Position.Y);
                        writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}
