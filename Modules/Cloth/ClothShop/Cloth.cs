using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Cloth
{
    public class Cloth
    {
        [JsonIgnore]
        public int dbId { get; set; }
        [JsonProperty("c")]
        public int component { get; set; }
        [JsonProperty("d")]
        public int drawable { get; set; }
        [JsonProperty("t")]
        public int texture { get; set; }

        [JsonProperty("p")]
        public int price { get; set; }

        public Cloth(int dbId, int component, int drawable, int texture, int price)
        {
            this.dbId = dbId;
            this.component = component;
            this.drawable = drawable;
            this.texture = texture;
            this.price = price;
        }

    }
}
