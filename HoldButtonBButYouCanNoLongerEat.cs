using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HoldButtonB
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.pressActionButton)),
                prefix: new HarmonyMethod(typeof(Main), nameof(Main.Prefix)),
                postfix: new HarmonyMethod(typeof(Main), nameof(Main.Postfix))
            );

            helper.Events.GameLoop.UpdateTicked += ButtonB;
        }
        static bool Prefix(out (int originalEdibility, StardewValley.Object item)? __state)
        {
            __state = null;
            List<Vector2> adjacentTiles = Utility.getAdjacentTileLocations(Game1.player.Tile);

            if (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Edibility == StardewValley.Object.inedible)
                return true;

            foreach (Vector2 tile in adjacentTiles)
            {
                if (Game1.player.currentLocation.Objects.TryGetValue(tile, out StardewValley.Object obj))
                {
                    __state = (Game1.player.ActiveObject.Edibility, Game1.player.ActiveObject);

                    Game1.player.ActiveObject.Edibility = StardewValley.Object.inedible;

                    break;
                }
            }
            return true;
        }
        static void Postfix((int originalEdibility, StardewValley.Object item)? __state)
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
        static void ButtonB(object s, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(4) || !Context.IsPlayerFree)
                return;

            if (Game1.virtualJoypad.ButtonBPressed)
            {
                Game1.pressActionButton(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));
            }
        }
    }
}
