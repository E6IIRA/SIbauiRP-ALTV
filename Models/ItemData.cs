using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ItemData
    {
        public ItemData()
        {
            DrugCamperTypeItemData = new HashSet<DrugCamperTypeItemData>();
            DrugExportContainer = new HashSet<DrugExportContainer>();
            FarmObjectLootData = new HashSet<FarmObjectLootData>();
            Item = new HashSet<Item>();
            PlantTypeLootData = new HashSet<PlantTypeLootData>();
            ServerScenarioLootData = new HashSet<ServerScenarioLootData>();
            ShopItemData = new HashSet<ShopItemData>();
            WareExportData = new HashSet<WareExportData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Stacksize { get; set; }
        public bool RemoveOnUse { get; set; }
        public bool Illegal { get; set; }

        public virtual ICollection<DrugCamperTypeItemData> DrugCamperTypeItemData { get; set; }
        public virtual ICollection<DrugExportContainer> DrugExportContainer { get; set; }
        public virtual ICollection<FarmObjectLootData> FarmObjectLootData { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<PlantTypeLootData> PlantTypeLootData { get; set; }
        public virtual ICollection<ServerScenarioLootData> ServerScenarioLootData { get; set; }
        public virtual ICollection<ShopItemData> ShopItemData { get; set; }
        public virtual ICollection<WareExportData> WareExportData { get; set; }
    }
}
