using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MonoGame.Framework;
using System;
using Microsoft.Xna.Framework;

namespace Testy
{
    public class Net6 : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonReleased += this.MouseRight;
        }
        public void MouseRight(object s, ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft && Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                Vector2 playerTile = Game1.player.Tile;
                Vector2 cursorTile = e.Cursor.Tile;

                float xx = Math.Abs(playerTile.X - cursorTile.X);
                float yy = Math.Abs(playerTile.Y - cursorTile.Y);

                if (xx <= 1 && yy <= 1)
                {
                    this.Helper.Input.Press(SButton.MouseRight);
                }
            }
        }
    }
}
