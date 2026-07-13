using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mobile;
using Microsoft.Xna.Framework;
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
                    if (location.Objects.TryGetValue(tile, out var machineTile))
                    {
                        if (machineTile.readyForHarvest.Value)
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
                    if (Utility.tryToPlaceItem(location, currentItem, x, y))
                    {
                        //
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
                        if (hoeDirt.crop != null && hoeDirt.readyForHarvest())
                        {
                            if (hoeDirt.crop.GetData() != null)
                            {
                                if (hoeDirt.crop.GetData().HarvestMethod == StardewValley.GameData.Crops.HarvestMethod.Scythe)
                                {
                                    return;
                                }
                            }
                            if (hoeDirt.crop.harvest((int)grabTile.X, (int)grabTile.Y, hoeDirt))
                            {
                                if (hoeDirt.crop.GetData()?.RegrowDays == -1)
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
