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
            if (Game1.player.CurrentItem is MeleeWeapon melee)
            {
                int x = (int)Game1.player.Position.X;
                int y = (int)Game1.player.Position.Y;
                int facingDirection = Game1.player.FacingDirection;
                Vector2 tileLocation1 = Vector2.Zero;
                Vector2 tileLocation2 = Vector2.Zero;
                Rectangle wielderBoundingBox = Game1.player.GetBoundingBox();
                int indexInCurrentAnimation = Game1.player.FarmerSprite.currentAnimationIndex;

                Rectangle weaponAoE = melee.getAreaOfEffect(
                    x,
                    y,
                    facingDirection,
                    ref tileLocation1,
                    ref tileLocation2,
                    wielderBoundingBox,
                    indexInCurrentAnimation
                );

                var mobileKeyStates = Game1.player.currentLocation.tapToMove.mobileKeyStates;

                foreach (NPC npc in Game1.player.currentLocation.characters)
                {
                    if (npc is Monster m && m.GetBoundingBox().Intersects(weaponAoE))
                    {
                        if (!Game1.player.IsBusyDoingSomething())
                        {
                            mobileKeyStates.useToolButtonPressed = true;
                            break;
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
}
