﻿using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerClothEquipped
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ClothVariationDataId { get; set; }

        public virtual ClothVariationData ClothVariationData { get; set; }
        public virtual Player Player { get; set; }
    }
}
