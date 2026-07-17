using StardewValley;
using StardewValley.Monsters;
using StardewModdingAPI;
using HarmonyLib;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

namespace testmob
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += Check;
        }
        public void Check(object s, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree
            || Game1.player == null || Game1.player.currentLocation?.characters == null
            )
            {
                return;
            }
            var grabTile_Rect = new Rectangle(
                (int)Game1.player.GetGrabTile().X * Game1.tileSize,
                (int)Game1.player.GetGrabTile().Y * Game1.tileSize,
                Game1.tileSize,
                Game1.tileSize
            );
            foreach (var character in Game1.player?.currentLocation?.characters)
            {
                if (character is Monster m && m.GetBoundingBox().Intersects(grabTile_Rect))
                {
                    if (Game1.player?.CurrentItem is MeleeWeapon melee)
                    {
                        Game1.player.currentLocation.tapToMove.mobileKeyStates.useToolButtonPressed = true;
                        break;
                    }
                }
                else
                {
                    Game1.player.currentLocation.tapToMove.mobileKeyStates.useToolButtonPressed = false;
                }
            }
        }
    }
}
