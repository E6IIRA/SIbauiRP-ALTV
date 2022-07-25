using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.ClothNew
{
    public class ClothDataData
    {

        public int clothDataValue { get; set; }
        public string clothDataName { get; set; }

        public int clothDataPrice { get; set; }

        public Dictionary<int, ClothVariationData> ClothVariationData { get; set; }


        public ClothDataData(int clothDataValue, string clothDataName, int clothDataPrice)
        {
            this.clothDataValue = clothDataValue;
            this.clothDataName = clothDataName;
            this.clothDataPrice = clothDataPrice;
            this.ClothVariationData = new Dictionary<int, ClothVariationData>();
        }

    }
}
