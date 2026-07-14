using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Reflection;
using HarmonyLib;

namespace SmapiWayButtonA
{
    public class Main : Mod
    {
        MethodInfo overrideButton = AccessTools.Method(Game1.input.GetType(), "OverrideButton");

        public override void Entry(IModHelper helper)
        {
            if (Constants.TargetPlatform != GamePlatform.Android)
            {
                Monitor.Log("This only works for the mobile version of the game.", LogLevel.Error);
                return;
            }
            helper.Events.GameLoop.UpdateTicked += ButtonA;
        }
        void ButtonA(object s, UpdateTickedEventArgs e)
        {
            if (Game1.virtualJoypad == null || Game1.player.CurrentItem is StardewValley.Tool)
                return;

            if (Game1.virtualJoypad.ButtonAPressed)
            {
                if (overrideButton != null)
                {
                    overrideButton.Invoke(Game1.input, new object[] { SButton.MouseRight, true });
                }
            }
        }
    }
}
