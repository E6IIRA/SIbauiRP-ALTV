using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Cloth;
using GangRP_Server.Utilities.ClothNew;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ClothTypeData = GangRP_Server.Utilities.ClothNew.ClothTypeData;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Cloth
{
    public sealed class ClothModule : ModuleBase, IPressedEEvent, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        public IEnumerable<ClothShopData> _clothShops = Enumerable.Empty<ClothShopData>();
        public IEnumerable<ClothData> _clothDatas = Enumerable.Empty<ClothData>();

        public Dictionary<int, Utilities.ClothNew.ClothTypeData> AllManClothes { get; set; } = new Dictionary<int, ClothTypeData>();
        public Dictionary<int, Utilities.ClothNew.ClothTypeData> AllWomanClothes { get; set; } = new Dictionary<int, ClothTypeData>();

        public static ClothModule Instance { get; private set; }

        public ClothModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
            Instance = this;
        }

        public void OnLoad()
        {
            _clothShops = AddTableLoadEvent<ClothShopData>(_rpContext.ClothShopData, OnItemLoad);
            _clothDatas = AddTableLoadEvent<ClothData>(_rpContext.ClothData.Include(d => d.ClothVariationData).Include(d => d.ClothTypeData));

            var temp = _rpContext.ClothTypeData.Include(d => d.ClothData).ThenInclude(d => d.ClothVariationData);
            AllManClothes = new Dictionary<int, ClothTypeData>();
            AllWomanClothes = new Dictionary<int, ClothTypeData>();
            int i = 1;
            _logger.Info("STARTED Loading all Clothes");
            //IF SERVER CRASHES -> THIS MEANS THERE ARE SOME CLOTHES NOT ONLY ONCE IN THE DB!!!! CHECK IT

            //MAN
            foreach (var typedata in temp)
            {
                Utilities.ClothNew.ClothTypeData clothTypeData = new Utilities.ClothNew.ClothTypeData(typedata.Value, typedata.Name);
                foreach (var clothdata in typedata.ClothData.Where(d => (d.Gender == 2) || (d.Gender == 0)))
                {
                    Utilities.ClothNew.ClothDataData clothDataData = new Utilities.ClothNew.ClothDataData(clothdata.Value, clothdata.Name, clothdata.Price);
                    foreach (var variation in clothdata.ClothVariationData)
                    {
                        // UNCOMMENT THIS TO CHECK WHERE SERVER IS CRASHING -> AT WHAT VALUE
                        //_logger.Debug(variation.ClothDataId.ToString());
                        Utilities.ClothNew.ClothVariationData clothVariationData = new Utilities.ClothNew.ClothVariationData(variation.Value, variation.Name, variation.Id);
                        //_logger.Debug($"{variation.Value} {clothVariationData.clothVariationName} {clothVariationData.clothVariationDbId}");
                        clothDataData.ClothVariationData.Add(variation.Value, clothVariationData);
                        i++;
                    }
                    //clothamk.Add(clothdata.Value, list);
                    clothTypeData.ClothDataData.Add(clothdata.Value, clothDataData);
                }

                //AllClothes.Add(typedata.Value, clothamk);
                AllManClothes.Add(typedata.Value, clothTypeData);
            }

            //WOMAN
            foreach (var typedata in temp)
            {
                Utilities.ClothNew.ClothTypeData clothTypeData = new Utilities.ClothNew.ClothTypeData(typedata.Value, typedata.Name);
                foreach (var clothdata in typedata.ClothData.Where(d => (d.Gender == 2) || (d.Gender == 1)))
                {
                    Utilities.ClothNew.ClothDataData clothDataData = new Utilities.ClothNew.ClothDataData(clothdata.Value, clothdata.Name, clothdata.Price);
                    foreach (var variation in clothdata.ClothVariationData)
                    {
                        // UNCOMMENT THIS TO CHECK WHERE SERVER IS CRASHING -> AT WHAT VALUE
                        //_logger.Debug(variation.ClothDataId.ToString());
                        Utilities.ClothNew.ClothVariationData clothVariationData = new Utilities.ClothNew.ClothVariationData(variation.Value, variation.Name, variation.Id);
                        //_logger.Debug($"{variation.Value} {clothVariationData.clothVariationName} {clothVariationData.clothVariationDbId}");
                        clothDataData.ClothVariationData.Add(variation.Value, clothVariationData);
                        i++;
                    }
                    //clothamk.Add(clothdata.Value, list);
                    clothTypeData.ClothDataData.Add(clothdata.Value, clothDataData);
                }

                //AllClothes.Add(typedata.Value, clothamk);
                AllWomanClothes.Add(typedata.Value, clothTypeData);
            }
            _logger.Info($"FINISHED Loading all Clothes - {i-1} variations");


            AddClientEvent<int, int>("GetClothesByTypeId", GetClothesByTypeId);
            AddClientEvent<int, object>("BuyClothes", BuyClothes);
            AddClientEvent("CloseCloth", CloseCloth);
        }


        private void OnItemLoad(ClothShopData clothShopData)
        {
#if DEBUG
            MarkerStreamer.Create(MarkerTypes.MarkerTypeReplayIcon, clothShopData.Position, new Vector3(1), color: new Rgba(255, 255, 0, 255));
            TextLabelStreamer.Create($"ClothShop Id: {clothShopData.Id}", clothShopData.Position, color: new Rgba(255, 255, 0, 255));

            MarkerStreamer.Create(MarkerTypes.MarkerTypeReplayIcon, clothShopData.WarderobePosition, new Vector3(1), color: new Rgba(255, 255, 0, 255));
            TextLabelStreamer.Create($"Warderobe ClothShop Id: {clothShopData.Id}", clothShopData.WarderobePosition, color: new Rgba(255, 255, 0, 255));
#endif

        }

        public ClothInformationData? GetClothByParams(int gender, int component, int drawable, int texture)
        {
            var temp = gender == 0 ? AllManClothes : AllWomanClothes;

            if (temp.TryGetValue(component, out Utilities.ClothNew.ClothTypeData? clothTypeData))
            {
                if (clothTypeData.ClothDataData.TryGetValue(drawable, out ClothDataData? clothDataData))
                {
                    if (clothDataData.ClothVariationData.TryGetValue(texture, out Utilities.ClothNew.ClothVariationData? clothVariationData))
                    {
                        return new ClothInformationData(component, drawable, texture, clothTypeData.clothTypeName, clothDataData.clothDataName, clothVariationData.clothVariationName, clothDataData.clothDataPrice, clothVariationData.clothVariationDbId);
                    }
                }
            }

            return null;
        }

        public void CloseCloth(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.SetEquippedClothes();
        }


        public async void BuyClothes(IPlayer player, int shopId, object obj)
        {
            List<Utilities.Cloth.Cloth> deserializeObject = JsonConvert.DeserializeObject<List<Utilities.Cloth.Cloth>>(JsonConvert.SerializeObject(obj));
            if (deserializeObject == null) return;
            RPPlayer rpPlayer = (RPPlayer)player;
            if (shopId == -1)
            {
                await using RPContext rpContext = new RPContext();
                foreach (var cloth in deserializeObject)
                {
                    var ownCloth = rpPlayer.GetOwnCloth(cloth.component, cloth.drawable, cloth.texture);
                    if (ownCloth != null)
                    {
                        if (rpPlayer.EquippedClothes.TryGetValue(ownCloth.clothTypeValue, out Utilities.Cloth.Cloth? clothInfo))
                        {
                            //already has this cloth type on him
                            var toBeUpdatedCloth = await rpContext.PlayerClothEquipped.FirstOrDefaultAsync(d => (d.PlayerId == rpPlayer.PlayerId) && (d.ClothVariationDataId == clothInfo.dbId));
                            if (toBeUpdatedCloth != null)
                            {
                                toBeUpdatedCloth.ClothVariationDataId = ownCloth.clothVariationDbId;
                                await rpContext.SaveChangesAsync();
                                rpPlayer.EquippedClothes[ownCloth.clothTypeValue] = new Utilities.Cloth.Cloth(ownCloth.clothVariationDbId, cloth.component, cloth.drawable, cloth.texture, ownCloth.clothDataPrice);
                            }

                        }
                        else
                        {
                            var temp = new PlayerClothEquipped
                            {
                                ClothVariationDataId = ownCloth.clothVariationDbId,
                                PlayerId = rpPlayer.PlayerId
                            };

                            await rpContext.PlayerClothEquipped.AddAsync(temp);
                            await rpContext.SaveChangesAsync();
                            //has no clothes on this type
                            rpPlayer.EquippedClothes.Add(ownCloth.clothTypeValue, new Utilities.Cloth.Cloth(temp.Id, ownCloth.clothTypeValue, ownCloth.clothDataValue, ownCloth.clothVariationValue, ownCloth.clothDataPrice));
                        }
                    }
                }
                rpPlayer.SetEquippedClothes();
                player.Emit("UpdateView", "FinishChangeCloth");
            }
            else
            {
                int cost = 0;


                List<ClothInformationData> toBeBoughtClothes = new List<ClothInformationData>();
                foreach (var cloth in deserializeObject)
                {
                    var ownCloth = rpPlayer.GetOwnCloth(cloth.component, cloth.drawable, cloth.texture);
                    if (ownCloth == null)
                    {
                        var clothInformationData = GetClothByParams(rpPlayer.Gender, cloth.component, cloth.drawable, cloth.texture);
                        if (clothInformationData == null) continue;
                        toBeBoughtClothes.Add(clothInformationData);
                        cost += cloth.price;
                    }
                }
                //not enough money amk
                if (cost > rpPlayer.Money) return;
                await using RPContext rpContext = new RPContext();
                await rpPlayer.TakeMoney(cost);
                foreach (var cloth in toBeBoughtClothes)
                {
                    PlayerClothOwned playerClothOwned = new PlayerClothOwned
                    {
                        PlayerId = rpPlayer.PlayerId,
                        ClothVariationDataId = cloth.clothVariationDbId,
                    };
                    await rpContext.PlayerClothOwned.AddAsync(playerClothOwned);
                    rpPlayer.AddCloth(cloth);
                }
                await rpContext.SaveChangesAsync();
                player.Emit("UpdateView", "FinishClothBuy", toBeBoughtClothes.Count, (deserializeObject.Count - toBeBoughtClothes.Count), cost);
            }
        }

        public void GetClothesByTypeId(IPlayer player, int shopId, int clothTypeId)
        {
            _logger.Debug("GET CLOTHES BY TYPE ID");

            RPPlayer rpPlayer = (RPPlayer) player;
            if (shopId == -1)
            {
                player.Emit("UpdateView", "SendClothesToClient", new WarderobeDataWriter(-1, clothTypeId, rpPlayer.OwnedClothes));
            }
            else
            {
                player.Emit("UpdateView", "SendClothesToClient", new ClothDataWriter(shopId, clothTypeId, _clothDatas, (byte)rpPlayer.Gender));
            }

        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            var clothShop = _clothShops.FirstOrDefault(c => c.Position.Distance(player.Position) < 3);
            if (clothShop == null)
            {
                var warderobe = _clothShops.FirstOrDefault(c => c.WarderobePosition.Distance(player.Position) < 3);
                if (warderobe != null)
                {
                    rpPlayer.OpenWarderobe();
                    _logger.Info(player.Name + " opened Warderobe of ClothShop " + warderobe.Id);
                    return Task.FromResult(true);
                }
                

            }
            else
            {
                _logger.Info(player.Name + " opened ClothShop " + clothShop.Id);
                //ClothShop
                player.Emit("ShowIF", "ClothShop", new ClothShopDataWriter(clothShop.Id, clothShop.Name, rpPlayer.Money, _clothDatas));
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
