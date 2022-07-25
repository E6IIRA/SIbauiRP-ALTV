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
    public class ClothTypeData
    {
        public int clothTypeValue { get; set; }
        public String clothTypeName { get; set; }
        
        public Dictionary<int, ClothDataData> ClothDataData { get; set; }
        public ClothTypeData(int clothTypeValue, string clothTypeName)
        {
            this.clothTypeValue = clothTypeValue;
            this.clothTypeName = clothTypeName;
            this.ClothDataData = new Dictionary<int, ClothDataData>();
        }



    }
}
