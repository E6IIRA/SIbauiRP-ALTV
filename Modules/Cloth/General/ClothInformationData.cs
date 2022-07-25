using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.ClothNew
{
    public class ClothInformationData
    {

        public int clothTypeValue { get; set; }
        public int clothDataValue { get; set; }
        public int clothVariationValue { get; set; }

        public String clothTypeName { get; set; }
        public String clothDataName { get; set; }
        public String clothVariationName { get; set; }

        public int clothDataPrice { get; set; }


        public int clothVariationDbId { get; set; }


        public ClothInformationData(int clothTypeValue, int clothDataValue, int clothVariationValue, String clothTypeName, String clothDataName, String clothVariationName, int clothDataPrice, int clothVariationDbId)
        {
            this.clothTypeValue = clothTypeValue;
            this.clothDataValue = clothDataValue;
            this.clothVariationValue = clothVariationValue;
            this.clothTypeName = clothTypeName;
            this.clothDataName = clothDataName;
            this.clothVariationName = clothVariationName;
            this.clothDataPrice = clothDataPrice;
            this.clothVariationDbId = clothVariationDbId;
        }



    }
}
