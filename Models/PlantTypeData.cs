using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlantTypeData
    {
        public PlantTypeData()
        {
            Plant = new HashSet<Plant>();
            PlantTypeLootData = new HashSet<PlantTypeLootData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int SeedItemId { get; set; }
        public int TimeToGrow { get; set; }
        public string ObjectStageOne { get; set; }
        public string ObjectStageTwo { get; set; }
        public int MaximumWater { get; set; }
        public int MaximumFertilizer { get; set; }

        public virtual ICollection<Plant> Plant { get; set; }
        public virtual ICollection<PlantTypeLootData> PlantTypeLootData { get; set; }
    }
}
