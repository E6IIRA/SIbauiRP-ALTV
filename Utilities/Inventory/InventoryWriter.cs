using System;
using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Inventory
{
    public class InventoryWriter : IWritable
    {
        private readonly LocalInventory _playerInventory;
        private readonly LocalInventory _otherInventory;

        public InventoryWriter(LocalInventory playerInventory, LocalInventory otherInventory = null)
        {
            this._playerInventory = playerInventory;
            this._otherInventory = otherInventory;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.BeginArray();

            writer.BeginObject();
            writer.Name("i");
            writer.Value(_playerInventory.Id);
            writer.Name("n");
            writer.Value("Spieler");
            writer.Name("w");
            writer.Value(_playerInventory.InventoryTypeData.MaxWeight);
            writer.Name("s");
            writer.BeginArray();
            for (int i = 0; i < _playerInventory.InventoryTypeData.MaxSlots; i++)
            {
                writer.BeginObject();
                if (_playerInventory.InventoryItems.TryGetValue(i, out LocalItem localItem))
                {

                    writer.Name("id");
                    writer.Value(i);
                    writer.Name("itemId");
                    writer.Value(localItem.ItemId);
                    writer.Name("amount");
                    writer.Value(localItem.Amount);
                    if (localItem.CustomItemData != null)
                    {
                        writer.Name("customData");
                        writer.Value(localItem.GetCustomItemDataString());
                        Console.WriteLine($"custom {localItem.GetCustomItemDataString()}");
                    }
                }
                else
                {
                    writer.Name("id");
                    writer.Value(i);
                    writer.Name("itemId");
                    writer.Value(0);
                    writer.Name("amount");
                    writer.Value(0);

                }
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();

            if (_otherInventory != null)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(_otherInventory.Id);
                writer.Name("n");
                writer.Value(_otherInventory.InventoryTypeData.Name);
                writer.Name("w");
                writer.Value(_otherInventory.InventoryTypeData.MaxWeight);
                writer.Name("s");
                writer.BeginArray();
                for (int i = 0; i < _otherInventory.InventoryTypeData.MaxSlots; i++)
                {
                    writer.BeginObject();
                    if (_otherInventory.InventoryItems.TryGetValue(i, out LocalItem localItem))
                    {

                        writer.Name("id");
                        writer.Value(i);
                        writer.Name("itemId");
                        writer.Value(localItem.ItemId);
                        writer.Name("amount");
                        writer.Value(localItem.Amount);
                        if (localItem.CustomItemData != null)
                        {
                            writer.Name("customData");
                            writer.Value(localItem.GetCustomItemDataString());
                            Console.WriteLine($"custom {localItem.GetCustomItemDataString()}");
                        }
                    }
                    else
                    {
                        writer.Name("id");
                        writer.Value(i);
                        writer.Name("itemId");
                        writer.Value(0);
                        writer.Name("amount");
                        writer.Value(0);
                    }
                    writer.EndObject();
                }
                writer.EndArray();
                writer.EndObject();
            }


            writer.EndArray();
            writer.EndObject();

        }
    }
}
