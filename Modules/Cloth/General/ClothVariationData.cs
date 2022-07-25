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
    public class ClothVariationData
    {
        public int clothVariationValue { get; set; }
        public string clothVariationName { get; set; }

        public int clothVariationDbId { get; set; }


        public ClothVariationData(int clothVariationValue, string clothVariationName, int clothVariationDbId)
        {
            this.clothVariationValue = clothVariationValue;
            this.clothVariationName = clothVariationName;
            this.clothVariationDbId = clothVariationDbId;
        }

    }
}
