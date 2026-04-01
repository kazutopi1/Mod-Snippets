using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MonoGame.Framework;

namespace HoldToBuy
{
    internal class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.IDK;
        }
        public void IDK(object s, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu)
            {
                Helper.Events.GameLoop.UpdateTicked -= this.Shopp;
                Helper.Events.GameLoop.UpdateTicked += this.Shopp;
            }
            else
            {
                Helper.Events.GameLoop.UpdateTicked -= this.Shopp;
            }
        }
        public void Shopp(object s, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu shop)
            {
                if (Helper.Input.IsDown(SButton.MouseLeft))
                {
                    Helper.Input.Press(SButton.LeftShift);
                    Helper.Input.Press(SButton.MouseRight);

                    if (shop.heldItem != null)
                    {
                        if (shop.heldItem is Item item)
                        {
                            Game1.player.addItemToInventory(item);
                            shop.heldItem = null;
                        }
                    }
                }
            }
        }
    }
}
