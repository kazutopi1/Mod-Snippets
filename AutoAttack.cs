using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using System;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

namespace AutoAttack
{
    public class Main : Mod
    {
        Rectangle grabTile_Rect = new Rectangle(0, 0, 0, 0);

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += CheckForCollision;
        }
        private void CheckForCollision(object s, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }
            if (Game1.player == null || Game1.currentLocation == null)
            {
                return;
            }

            var mobileKeyStates = Game1.player.currentLocation.tapToMove.mobileKeyStates;

            grabTile_Rect.X = (int)Game1.player.GetGrabTile().X * Game1.tileSize;
            grabTile_Rect.Y = (int)Game1.player.GetGrabTile().Y * Game1.tileSize;
            grabTile_Rect.Width = Game1.tileSize;
            grabTile_Rect.Height = Game1.tileSize;

            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (npc is Monster m && m.GetBoundingBox().Intersects(grabTile_Rect))
                {
                    if (Game1.player.CurrentItem is MeleeWeapon)
                    {
                        if (!Game1.player.IsBusyDoingSomething())
                        {
                            mobileKeyStates.useToolButtonPressed = true;
                            break;
                        }
                    }
                }
                else
                {
                    mobileKeyStates.useToolButtonPressed = false;
                }
            }
        }
    }
}
