using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Shop
{
    public class ShopWriter : IWritable
    {
        private readonly ShopData _shopData;
        private readonly int _playerMoney;

        public ShopWriter(ShopData shopData, int playerMoney)
        {
            _shopData = shopData;
            _playerMoney = playerMoney;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_shopData.Id);
            writer.Name("n");
            writer.Value(_shopData.Name);
            writer.Name("m");
            writer.Value(_playerMoney);
            writer.Name("data");
            writer.BeginArray();
            foreach (var shopItemData in _shopData.ShopItemData)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(shopItemData.ItemDataId);
                writer.Name("p");
                writer.Value(shopItemData.Price);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();

        }
    }
}
