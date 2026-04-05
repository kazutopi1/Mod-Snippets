using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

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
            if (Game1.activeClickableMenu is ShopMenu shop && e.IsMultipleOf(2))
            {
                if (Helper.Input.IsDown(SButton.MouseLeft))
                {
                    Helper.Input.Press(SButton.MouseRight);
                }
            }
        }
    }
}
