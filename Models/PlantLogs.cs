using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlantLogs
    {
        public int Id { get; set; }
        public int PlantTypeDataId { get; set; }
        public int GrowState { get; set; }
        public float LootFactor { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int PlanterPlayerId { get; set; }
        public int ActualWater { get; set; }
        public int ActualFertilizer { get; set; }
        public DateTime PlantDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public int? HarvestPlayerId { get; set; }
    }
}
