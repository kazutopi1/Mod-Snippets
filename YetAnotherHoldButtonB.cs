using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mobile;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Test
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += HoldButtonB;
        }
        public void HoldButtonB(object s, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(4) || !Context.IsPlayerFree)
                return;

            var v = Game1.virtualJoypad;

            if (v.ButtonBPressed)
            {
                var f = Game1.player;
                var grabTile = f.GetGrabTile();
                var location = f.currentLocation;
                int x = (int)grabTile.X * Game1.tileSize;
                int y = (int)grabTile.Y * Game1.tileSize;

                List<Vector2> adjacentTiles = Utility.getAdjacentTileLocations(f.Tile);

                if (Game1.player.CurrentItem is StardewValley.Object currentItem)
                {
                    if (currentItem.canBePlacedHere(location, grabTile))
                    {
                        if (currentItem.isPlaceable() && currentItem.placementAction(Game1.player.currentLocation, x, y, f))
                        {
                            f.reduceActiveItemByOne();
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
            }
        }
    }
}
