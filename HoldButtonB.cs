using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using HarmonyLib;
using StardewValley.Mobile;
using System;
using Microsoft.Xna.Framework;

namespace SmapiWayButtonB
{
    public class Main : Mod
    {
        private static int ticks = 0;
        private const int tickThreshhold = 2;
        private static bool wasBDown = false;
        static IMonitor _monitor;

        public override void Entry(IModHelper helper)
        {
            _monitor = this.Monitor;

            if (Constants.TargetPlatform != GamePlatform.Android)
            {
                Monitor.Log("This only works for the mobile version of the game.", LogLevel.Error);
                return;
            }
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(VirtualJoypad), nameof(VirtualJoypad.CheckForTapJoystickAndButtons)),
                postfix: new HarmonyMethod(typeof(Main), nameof(Main.CheckForTapJoystickAndButtons_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.pressActionButton)),
                prefix: new HarmonyMethod(typeof(Main), nameof(Main.pressActionButton_Prefix)),
                postfix: new HarmonyMethod(typeof(Main), nameof(Main.pressActionButton_Postfix))
            );
        }
        static void CheckForTapJoystickAndButtons_Postfix(VirtualJoypad __instance)
        {
            try
            {
                if (!Context.IsPlayerFree)
                {
                    return;
                }
                if (__instance.buttonBHeld)
                {
                    if (!wasBDown)
                    {
                        Game1.player.currentLocation.tapToMove.mobileKeyStates.actionButtonPressed = true;
                        ticks = 0;
                    }
                    else
                    {
                        ticks++;
                        if (ticks >= tickThreshhold)
                        {
                            Game1.player.currentLocation.tapToMove.mobileKeyStates.actionButtonPressed = true;
                            ticks = 0;
                        }
                    }
                    wasBDown = true;
                }
                else
                {
                    ticks = 0;
                    wasBDown = false;
                    Game1.player.currentLocation.tapToMove.mobileKeyStates.actionButtonPressed = false;
                }
            }
            catch (Exception ex)
            {
                _monitor.LogOnce($"{ex}", LogLevel.Error);
            }
        }
        private static bool pressActionButton_Prefix(out (int originalEdibility, StardewValley.Object item)? __state)
        {
            __state = null;
            try
            {
                if (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Edibility == StardewValley.Object.inedible)
                    return true;

                var surroundingTile = Utility.getSurroundingTileLocationsArray(Game1.player.Tile);

                foreach (Vector2 tile in surroundingTile)
                {
                    if (Game1.player.currentLocation.Objects.TryGetValue(tile, out var obj))
                    {
                        if (obj.GetMachineData() != null)
                        {
                            __state = (Game1.player.ActiveObject.Edibility, Game1.player.ActiveObject);

                            Game1.player.ActiveObject.Edibility = StardewValley.Object.inedible;

                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _monitor.LogOnce($"{ex}", LogLevel.Error);
                return true;
            }
        }
        private static void pressActionButton_Postfix((int originalEdibility, StardewValley.Object item)? __state)
        {
            try
            {
                if (__state.HasValue)
                {
                    var savedValue = __state.Value;

                    if (savedValue.item != null)
                    {
                        savedValue.item.Edibility = savedValue.originalEdibility;
                    }
                }
            }
            catch (Exception ex)
            {
                _monitor.LogOnce($"{ex}", LogLevel.Error);
            }
        }
    }
}
