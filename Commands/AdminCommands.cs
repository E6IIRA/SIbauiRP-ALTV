using System;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Elements.Pools;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Handlers.Interface;
using GangRP_Server.Models;
using GangRP_Server.Modules.Farming;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.VehicleKey;
using GangRP_Server.Modules.VehicleOverview;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Commands
{
    public class AdminCommands : IScript
    {

        [Command("veh")]
        public async Task Veh(IPlayer player, params string[] args)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            byte primaryColor = 1;
            byte secondaryColor = 1;
            if (args.Length == 3)
            {
                byte.TryParse(args[1], out primaryColor);
                byte.TryParse(args[2], out secondaryColor);
            }
            await using RPContext rpContext = new RPContext();

            VehicleData vehicleData = rpContext.VehicleData.FirstOrDefault(d => d.Name.ToLower().Contains(args[0].ToLower()));
            if (vehicleData == null) return;

            var vehicle = (RPVehicle) await  AltAsync.CreateVehicle(vehicleData.Hash, player.Position, player.Rotation);

            vehicle.PrimaryColor = primaryColor;
            vehicle.SecondaryColor = secondaryColor;
            vehicle.NumberplateText = "SIBAUI";
            await vehicle.SetEngineOnAsync(true);
            vehicle.OwnerId = rpPlayer.PlayerId;

            rpPlayer.WarpIntoVehicle(vehicle, -1);

        }

        [Command("v")]
        public async Task V(IPlayer player, params string[] args)
        {
            byte primaryColor = 1;
            byte secondaryColor = 1;

            if (args.Length == 3)
            {
                byte.TryParse(args[1], out primaryColor);
                byte.TryParse(args[2], out secondaryColor);
            }

            var vehicle = (RPVehicle)await AltAsync.CreateVehicle(args[0], player.Position, player.Rotation);
            vehicle.PrimaryColor = primaryColor;
            vehicle.SecondaryColor = secondaryColor;
            vehicle.NumberplateText = "SIBAUI";
            await vehicle.SetEngineOnAsync(true);

            RPPlayer rpPlayer = (RPPlayer)player;
            vehicle.OwnerId = rpPlayer.PlayerId;
            rpPlayer.WarpIntoVehicle(vehicle, -1);

        }


        [Command("model")]
        public void Model(IPlayer player, uint model)
        {
            player.Model = model;
        }

        [Command("afly")]
        public async Task Afly(IPlayer player)
        {
            byte primaryColor = 44;
            byte secondaryColor = 44;

            var vehicle = (RPVehicle)await AltAsync.CreateVehicle("Oppressor2", player.Position, player.Rotation);
            vehicle.PrimaryColor = primaryColor;
            vehicle.SecondaryColor = secondaryColor;
            vehicle.NumberplateText = "SIBAUI";
            await vehicle.SetEngineOnAsync(true);
            RPPlayer rpPlayer = (RPPlayer)player;
            vehicle.OwnerId = rpPlayer.PlayerId;
            rpPlayer.WarpIntoVehicle(vehicle, -1);

        }

        [Command("removeveh")]
        public async Task RemoveVeh(IPlayer player)
        {
            IVehicle vehicle = Alt.Server.GetVehicles().FirstOrDefault(v => v.Position.Distance(player.Position) < 3);
            if (vehicle == null) return;

            await vehicle.RemoveAsync();
        }

        [Command("additem")]
        public async Task AddItem(IPlayer player)
        {
            var rpPlayer = (RPPlayer)player;
            InventoryModule.Instance.AddItem(rpPlayer.Inventory, 1);
        }

        [Command("hp")]
        public async Task Hp(IPlayer player, String hpString)
        {
            ushort.TryParse(hpString, out ushort hp);
            await player.SetHealthAsync(hp);
        }

        [Command("armor")]
        public async Task Armor(IPlayer player, String armorString)
        {
            ushort.TryParse(armorString, out ushort armor);
            await player.SetArmorAsync(armor);
        }

        [Command("go")]
        public void GoTo(IPlayer player, String targetName)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            IPlayer targetPlayer = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(targetName.ToLower()));
            if (targetPlayer == null) return;
            RPPlayer targetRpPlayer = (RPPlayer) targetPlayer;
            Position targetPosition = targetPlayer.Position;
            if (player.IsInVehicle)
            {
                player.Vehicle.SetPositionAsync(targetPosition);
                player.Vehicle.SetDimensionAsync(targetPlayer.Dimension);
                
            }
            else
            {
                player.SetPositionAsync(targetPosition);
                player.SetDimensionAsync(targetPlayer.Dimension);
            }
            rpPlayer.DimensionType = targetRpPlayer.DimensionType;

        }
        [Command("gethere")]
        public void GetHere(IPlayer player, String targetName)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            Position targetPosition = player.Position;
            IPlayer targetPlayer = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(targetName.ToLower()));
            if (targetPlayer == null) return;
            RPPlayer targetRpPlayer = (RPPlayer)targetPlayer;
            if (targetPlayer.IsInVehicle)
            {
                targetPlayer.Vehicle.SetPositionAsync(targetPosition);
                targetPlayer.Vehicle.SetDimensionAsync(player.Dimension);
            }
            else
            {
                targetPlayer.SetPositionAsync(targetPosition);
                targetPlayer.SetDimensionAsync(player.Dimension);
            }
            targetRpPlayer.DimensionType = rpPlayer.DimensionType;
        }
        [Command("savecoords")]
        public async void SaveCoords(IPlayer player, String name)
        {
            Saveposition pos = new Saveposition
            {
                Name = name
            };

            if (player.IsInVehicle)
            {
                var vehicle = player.Vehicle;
                pos.PositionX = vehicle.Position.X;
                pos.PositionY = vehicle.Position.Y;
                pos.PositionZ = vehicle.Position.Z;

                pos.RotationX = vehicle.Rotation.Pitch;
                pos.RotationY = vehicle.Rotation.Roll;
                pos.RotationZ = vehicle.Rotation.Yaw;
            }
            else
            {
                pos.PositionX = player.Position.X;
                pos.PositionY = player.Position.Y;
                pos.PositionZ = player.Position.Z;

                pos.RotationX = player.Rotation.Pitch;
                pos.RotationY = player.Rotation.Roll;
                pos.RotationZ = player.Rotation.Yaw;
            }

            await using var rpContext = new RPContext();
            await rpContext.Saveposition.AddAsync(pos);
            await rpContext.SaveChangesAsync();
        }
        [Command("saveparcelposition")]
        public async void SaveParcelPosition(IPlayer player, String name)
        {
            if (!int.TryParse(name, out int areaId)) return;

            ParcelDeliveryPoints parcelDeliveryPoint = new ParcelDeliveryPoints
            {
                AreaId = areaId,
                PositionX = player.Position.X,
                PositionY = player.Position.Y,
                PositionZ = player.Position.Z - 1.0f
            };

            await using var rpContext = new RPContext();
            await rpContext.ParcelDeliveryPoints.AddAsync(parcelDeliveryPoint);
            await rpContext.SaveChangesAsync();
        }
        [Command("savegcoords")]
        public async void SaveGroundCoords(IPlayer player, String name)
        {
            //Irgendwo noch ein Fehler. Koordinaten werden gespeichert, aber Client freezed
            await player.EmitAsync("GetGroundZFrom3DCoord", player.Position, name);
        }
        [Command("weapon")]
        public async void Weapon(IPlayer player, string weaponName)
        {
            await using RPContext rpContext = new RPContext();
            WeaponData weaponData = rpContext.WeaponData.FirstOrDefault(d => d.Name.ToLower().Contains(weaponName.ToLower()));
            if (weaponData == null) return;
            player.GiveWeapon(weaponData.Hash, 500, true);
        }
        [Command("component")]
        public void Component(IPlayer player, uint component)
        {
            player.AddWeaponComponent(player.CurrentWeapon, component);
        }
        [Command("tint")]
        public void Tint(IPlayer player, string tintString)
        {
            byte.TryParse(tintString, out byte tint);

            player.SetWeaponTintIndex(player.CurrentWeapon, tint);
        }
        [Command("tune")]
        public async Task Tune(IPlayer player, params string[] args)
        {
            if (args.Length == 2)
            {
                if (player.IsInVehicle)
                {
                    byte.TryParse(args[0], out byte category);
                    byte.TryParse(args[1], out byte id);
                    IVehicle vehicle = player.Vehicle;
                    await vehicle.SetModKitAsync(1);
                    await vehicle.SetModAsync(category, id);
                }
            }

        }
        [Command("modkit")]
        public async Task Modkit(IPlayer player, String modKitString)
        {
            if (player.IsInVehicle)
            {
                byte.TryParse(modKitString, out byte modKit);
                IVehicle vehicle = player.Vehicle;
                await vehicle.SetModKitAsync(modKit);
            }

        }
        [Command("speed")]
        public void Speed(IPlayer player, float vehicleSpeed)
        {
            if (player.IsInVehicle)
            {
                player.Emit("VehicleSpeed", vehicleSpeed);
            }
        }
        [Command("cloth")]
        public void Cloth(IPlayer player, params string[] args)
        {
            if (args.Length == 3)
            {
                short.TryParse(args[0], out short component);
                short.TryParse(args[1], out short drawable);
                short.TryParse(args[2], out short texture);
                player.Emit("SetPlayerCloth", component, drawable, texture);
            }
        }

        [Command("addgirlfriendforchris")]
        public void Addgirlfriendforchris(IPlayer player, string playerName)
        {
            IPlayer targetPlayer = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(playerName.ToLower()));
            if (targetPlayer == null) return;
            RPPlayer targetRpPlayer = (RPPlayer)targetPlayer;

            //TODO add Sexy outfit 
        }
        [Command("prop")]
        public void Prop(IPlayer player, params string[] args)
        {
            if (args.Length == 3)
            {
                short.TryParse(args[0], out short component);
                short.TryParse(args[1], out short drawable);
                short.TryParse(args[2], out short texture);
                player.Emit("SetPlayerProp", component, drawable, texture);
            }
        }

        [Command("notify")]
        public void Notify(IPlayer player, params string[] args)
        {
            if (args.Length == 3)
            {
                RPPlayer rpPlayer = (RPPlayer) player;
                Int32.TryParse(args[2], out int duration);

                Enum.TryParse(args[1], out RPPlayer.NotificationType notficNotificationType);

                rpPlayer.SendNotification(args[0], notficNotificationType, "", duration);
            }
        }

        [Command("scenario")]
        public void PlayScenario(IPlayer player, String scenarioName)
        {
            player.Emit("PlayScenario", scenarioName);
        }

        [Command("object")]
        public void Object(IPlayer player, String objectName)
        {
            Position pos = player.Position;

            pos.Z -= 1.0f;


            PropStreamer.Create(objectName, pos, new Vector3(0, 0, 0), 0, visible:true, streamRange:1000, frozen: true);
        }

        [Command("arev")]
        public void Arev(IPlayer player, String playerName)
        {
            IPlayer targetPlayer = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(playerName.ToLower()));
            if (targetPlayer == null) return;
            RPPlayer targetRpPlayer = (RPPlayer) targetPlayer;
            targetRpPlayer.Revive();
        }

        [Command("coord")]
        public async void Coord(IPlayer player, params string[] args)
        {
            if (args.Length == 3)
            {
                short.TryParse(args[0], out short x);
                short.TryParse(args[1], out short y);
                short.TryParse(args[2], out short z);
                await player.SetPositionAsync(x, y, z);
            }
        }

        [Command("time")]
        public async void Time(IPlayer player, int hour, int minute)
        {
            await player.SetDateTimeAsync(1, 1, 1, hour, minute, 1);
        }

        [Command("cover")]
        public void Cover(IPlayer player, int toggle)
        {
            player.Emit("SetPlayerCanUseCover", toggle);
        }

        [Command("ragdoll")]
        public void Ragdoll(IPlayer player, int toggle)
        {
            //Event kommt am Client an, aber kp, wie man das testen kann
            player.Emit("GivePlayerRagdollControl", toggle);
        }

        [Command("aimmode")]
        public void Aimmode(IPlayer player, int toggle)
        {
            //0 = Traditional GTA
            //1 = Assisted Aiming
            //2 = Free Aim
            player.Emit("SetPlayerTargetingMode", toggle);
        }

        [Command("swimspeed")]
        public void Swimspeed(IPlayer player, float multiplier)
        {
            //Funktioniert
            //Werte zwischen 1 und 1,49 eingebem. Eingabe mit KOMMA notwendig (1,49 und nicht 1.49)
            player.Emit("SetSwimMultiplierForPlayer", multiplier);
        }

        [Command("runspeed")]
        public void Runspeed(IPlayer player, float multiplier)
        {
            //Funktioniert
            //Werte zwischen 1 und 1,49 eingebem. Eingabe mit KOMMA notwendig (1,49 und nicht 1.49)
            player.Emit("SetRunSprintMultiplierForPlayer", multiplier);
        }

        [Command("maxarmor")]
        public void Maxarmor(IPlayer player, int armor)
        {
            //Funktioniert!!! Werte bis 2000 Armor getestet
            player.Emit("SetPlayerMaxArmour", armor);
        }
        [Command("maxhealth")]
        public void Maxhealth(IPlayer player, int health)
        {
            player.Emit("SetPlayerMaxHealth", health);
        }

        [Command("nightvision")]
        public void Nightvision(IPlayer player, int toggle)
        {
            //Funktioniert. Bin danach aber gefreezed?
            player.Emit("SetNightvision", toggle);
        }

        [Command("flash")]
        public void Flash(IPlayer player)
        {
            //Unknown?? Macht irgendwie das Bild kurz etwas hell, aber kp
            player.Emit("SetFlash");
        }

        [Command("lights")]
        public void Lights(IPlayer player, int toggle)
        {
            //Nicht getestet
            player.Emit("SetArtificialLightsState", toggle);
        }

        [Command("heatvision")]
        public void Heatvision(IPlayer player, int toggle)
        {
            //Funktioniert. Bin danach aber gefreezed? oder auch nicht kp.
            player.Emit("SetSeethrough", toggle);
        }

        [Command("heatvisionscale")]
        public void Heatvisionscale(IPlayer player, params string[] args)
        {
            //NOCH OFFEN
            if (args.Length == 2)
            {
                int.TryParse(args[0], out int index);
                float.TryParse(args[1], out float heatScale);
                if (heatScale >= 0 && heatScale <= 0.75)
                {
                    player.Emit("SetSeethroughScale", index, heatScale);
                }
            }
        }

        [Command("blurin")]
        public void Blurin(IPlayer player, int transitionTime)
        {
            //Funktioniert. in Millisekunden!
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.SetBlurIn(transitionTime);
        }

        [Command("blurout")]
        public void Blurout(IPlayer player, int transitionTime)
        {
            //Funktioniert. in Millisekunden!
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.SetBlurOut(transitionTime);
        }

        [Command("clowneffect")]
        public void Clowneffect(IPlayer player, int toggle)
        {
            //ggf. nur Clownbluteffekt?
            player.Emit("EnableClownBloodVfx", toggle);
        }

        [Command("areablip")]
        public void Areablip(IPlayer player, params string[] args)
        {
            if (args.Length == 3)
            {
                int.TryParse(args[0], out int width);
                int.TryParse(args[1], out int height);
                int.TryParse(args[2], out int heading);
                player.Emit("CreateAreaBlip", player.Position, width, height, heading);
            }
        }

        [Command("maxdive")]
        public void MaxDiveTime(IPlayer player, float time)
        {
            //Funktioniert. in Millisekunden!
            player.Emit("SetPedMaxTimeUnderwater", time);
        }

        [Command("stop")]
        public async void Stop(IPlayer player)
        {
            Alt.Server.StopResource(Alt.Server.Resource.Name);
            await Task.Delay(5000);
            Environment.Exit(0);
        }

        [Command("weather")]
        public async void Weather(IPlayer player, int weatherId)
        {
            await player.SetWeatherAsync((uint) weatherId);
        }

        //[Command("anim")]
        //public void Anim(IPlayer player, string animDict, string animName, float speed, float speedMultiplier, int duration, int flag)
        //{
        //    ((RPPlayer)player).PlayAnimation(animDict, animName, speed, speedMultiplier, duration, flag, false);
        //}

        [Command("anim")]
        public void Anim(IPlayer player, string animDict, string animName, int flag)
        {
            ((RPPlayer)player).PlayAnimationDebug(animDict, animName, flag, false);
        }

        [Command("stopanim")]
        public void StopAnim(IPlayer player, string animDict, string animName, float p3)
        {
            ((RPPlayer)player).StopAnimation(animDict, animName, p3, false);
        }

        [Command("clear")]
        public void Clear(IPlayer player)
        {
            ((RPPlayer)player).StopAnimation(false);
        }

        [Command("zone")]
        public void Zone(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            player.Emit("GetZoneName", rpPlayer.Position);
        }

        [Command("banktype")]
        public async void banktype(IPlayer player, int bankType)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            await using var rpContext = new RPContext();
            var dbPlayer = await rpContext.Player.FindAsync(rpPlayer.PlayerId);
            dbPlayer.BankType = Convert.ToSByte(bankType);
            await rpContext.SaveChangesAsync();
        }

        [Command("index")]
        public void ChangeIndex(IPlayer player, int index, int indexAnim)
        {
            player.Emit("ChangeIndex", index, indexAnim);
        }

        [Command("flag")]
        public void ChangeFlag(IPlayer player, int flag)
        {
            player.Emit("ChangeFlag", flag);
        }

        [Command("gotogps")]
        public void GoToGPS(IPlayer player)
        {
            player.Emit("GoToGPS");
        }

        [Command("loadipl")]
        public void LoadIpl(IPlayer player, string iplname)
        {
            foreach (var rpPlayer in Alt.Server.GetPlayers())
            {
                player.Emit("LoadIpl", iplname);
            }
        }

        [Command("unloadipl")]
        public void UnloadIpl(IPlayer player, string iplname)
        {
            foreach (var rpPlayer in Alt.Server.GetPlayers())
            {
                player.Emit("UnloadIpl", iplname);
            }
        }

        [Command("prop")]
        public void LoadProp(IPlayer player, params string[] args)
        {
            if (args.Length >= 1)
            {
                int color = 0;
                string iplname = args[0];
                if (args.Length == 2)
                    if (!Int32.TryParse(args[1], out color))
                        return;
                RPPlayer rpPlayer = (RPPlayer) player;
                foreach (var p in Alt.Server.GetPlayers())
                {
                    p.Emit("LoadProp", rpPlayer.Position, iplname, color);
                }
            }
        }

        [Command("unprop")]
        public void UnloadProp(IPlayer player, string iplname)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            foreach (var p in Alt.Server.GetPlayers())
            {
                p.Emit("UnloadProp", rpPlayer.Position, iplname);
            }
        }

        [Command("boom")]
        public void Explode(IPlayer player, string playerName)
        {
            IPlayer targetPlayer = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(playerName.ToLower()));
            if (targetPlayer == null) return;
            RPPlayer rpPlayer = (RPPlayer) targetPlayer;
            foreach (var p in Alt.Server.GetPlayers())
            {
                p.Emit("Boom", rpPlayer.Position);
            }
        }

        [Command("startfire")]
        public void Startfire(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            foreach (var p in Alt.Server.GetPlayers())
            {
                p.Emit("StartFire", rpPlayer.Position);
            }
        }

        [Command("stopfire")]
        public void Stopfire(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            foreach (var p in Alt.Server.GetPlayers())
            {
                p.Emit("StopFire", rpPlayer.Position);
            }
        }

        [Command("starteffect")]
        public void StartEffect(IPlayer player, string animationName, int duration, int looped)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("StartEffect", animationName, duration, looped);
        }

        [Command("stopeffect")]
        public void StopEffect(IPlayer player, string animationName)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("StopEffect", animationName);
        }

        [Command("startspec")]
        public void StartEffect(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("StartSpectating", rpPlayer.Position + new Position(0, 0, 20));
        }

        [Command("stopspec")]
        public void StopEffect(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("StopSpectating");
        }

        [Command("loadstorage")]
        public void LoadStorage(IPlayer player, int typ, int number)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("LoadStorageroom", typ, number);
        }

        [Command("unloadstorage")]
        public void UnloadStorage(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.Emit("UnloadStorageroom");
        }

        [Command("allweapons")]
        public void GetAllWeapons(IPlayer player)
        {
            foreach (WeaponModel? weapon in Enum.GetValues(typeof(WeaponModel)))
            {
                player.GiveWeapon(weapon ?? 0, 600, false);
            } 
        }

        [Command("players")]
        public void Players(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            var onlinePlayers = $"Spieler Online ({Alt.GetAllPlayers().Count}): " + String.Join(", ", Alt.GetAllPlayers().Select(p => p.Name));
            rpPlayer.SendNotification(onlinePlayers, RPPlayer.NotificationType.INFO);
        }

        [Command("setragdoll")]
        public void SetRagdoll(IPlayer player, int time1, int time2, int ragdollType, bool p4, bool p5, bool p6)
        {
            player.Emit("SetRagdoll", time1, time2, ragdollType, p4, p5, p6);
        }

        [Command("spikestrip")]
        public void CreateSpikestrip(IPlayer player, int offset)
        {
            Position pos = player.Position;
            RPPlayer rpPlayer = (RPPlayer) player;
            pos.Z -= 0.9f;
            float yaw = Convert.ToSingle(player.Rotation.Yaw * 180 / Math.PI) - offset;
            float pitch = Convert.ToSingle(player.Rotation.Pitch * 180 / Math.PI);
            float roll = Convert.ToSingle(player.Rotation.Roll * 180 / Math.PI);
            Console.WriteLine("Rotation converted: roll: " + roll + ", pitch: " + pitch + ", yaw: " + yaw);
            PropStreamer.Create("p_ld_stinger_s", pos, new Rotation(roll, pitch, yaw), 0, visible:true, streamRange:100, frozen: true);
            //Alt.CreateColShapeCube()
            //PropStreamer.Create("p_ld_stinger_s", pos + new Position(0, 0, 0.5f), player.HeadRotation, 0, visible:true, streamRange:100, frozen: true);
            //PropStreamer.Create("p_ld_stinger_s", pos + new Position(0, 0, 1.0f), player.Rotation, 0, visible:true, streamRange:100, frozen: true);
            player.Emit("SpikestripEvent", pos);
        }

        [Command("vehover")]
        public void VehicleOverview(IPlayer player)
        {
            VehicleOverviewModule.Instance.ShowVehicleOverview(player).Wait();
        }


        [Command("keys")]
        public void VehicleKeys(IPlayer player)
        {
            VehicleKeyModule.Instance.ShowVehicleKeyOverview((RPPlayer) player).Wait();
        }

        [Command("aduty")]
        public void ADuty(IPlayer player)
        {
            if (player.HasData("ADuty"))
            {
                player.DeleteData("ADuty");
                player.Emit("ADuty", false);
            }
            else
            {
                player.SetData("ADuty", true);
                player.Emit("ADuty", true);
            }
        }
    }
}
