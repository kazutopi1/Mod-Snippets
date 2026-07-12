using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mobile;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;

namespace YetAnotherHoldButtonA
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += HoldButtonA;
        }
        public void HoldButtonA(object s, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(4) || !Context.IsPlayerFree || Game1.player.CurrentItem is StardewValley.Tool)
                return;

            var v = Game1.virtualJoypad;

            if (v.ButtonAPressed)
            {
                var f = Game1.player;
                var grabTile = f.GetGrabTile();
                var location = f.currentLocation;
                int x = (int)grabTile.X * Game1.tileSize;
                int y = (int)grabTile.Y * Game1.tileSize;

                var adjacentTiles = Utility.getSurroundingTileLocationsArray(f.Tile);

                foreach (Vector2 tile in adjacentTiles)
                {
                    if (location.Objects.TryGetValue(tile, out StardewValley.Object machineTile))
                    {
                        if (machineTile.readyForHarvest.Value && machineTile.heldObject.Value != null)
                        {
                            if (machineTile.checkForAction(f, justCheckingForActivity: false))
                            {
                                break;
                            }
                        }
                    }
                }
                if (f.CurrentItem is StardewValley.Object currentItem)
                {
                    if (currentItem.canBePlacedHere(location, grabTile))
                    {
                        if (currentItem.isPlaceable())
                        {
                            if (currentItem.placementAction(location, x, y, f))
                            {
                                f.reduceActiveItemByOne();
                            }
                        }
                    }
                    else
                    {
                        foreach (Vector2 tile in adjacentTiles)
                        {
                            if (location.Objects.TryGetValue(tile, out StardewValley.Object machineTile))
                            {
                                if (machineTile.performObjectDropInAction(currentItem, probe: true, f))
                                {
                                    machineTile.performObjectDropInAction(currentItem, probe: false, f);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (location.terrainFeatures.TryGetValue(grabTile, out var feature))
                {
                    if (feature is HoeDirt hoeDirt)
                    {
                        if (hoeDirt.crop != null)
                        {
                            if (hoeDirt.readyForHarvest())
                            {
                                if (hoeDirt.crop.harvest((int)grabTile.X, (int)grabTile.Y, hoeDirt))
                                {
                                    if (hoeDirt.crop.GetData() == null || hoeDirt.crop.GetData().RegrowDays == -1)
                                    {
                                        hoeDirt.destroyCrop(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
