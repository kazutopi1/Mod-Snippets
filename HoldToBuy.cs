using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace HoldToBuy
{
    internal class Main : Mod
    {
        private int delay = 0;
        public Config config;

        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += this.InitializeConfig;
            helper.Events.Display.MenuChanged += this.IDK;
        }
        public void IDK(object s, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu)
            {
                Helper.Events.GameLoop.UpdateTicked -= this.Shopp;
                Helper.Events.GameLoop.UpdateTicked += this.Shopp;
            }
            else if (e.OldMenu is ShopMenu)
            {
                Helper.Events.GameLoop.UpdateTicked -= this.Shopp;
            }
        }
        public void Shopp(object s, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu shop)
            {
                if (Helper.Input.IsDown(SButton.MouseLeft) || Helper.Input.IsDown(SButton.ControllerA))
                {
                    delay++;
                    if (delay >= config.GetDelayFrames())
                    {
                        if (delay % config.GetBuyFrames() == 0)
                        {
                            shop.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                        }
                    }

                    if (config.PutItemsInInventory)
                    {
                        if (shop.heldItem != null)
                        {
                            if (shop.heldItem is Item i)
                            {
                                Game1.player.addItemToInventory(i);
                                shop.heldItem = null;
                            }
                        }
                    }
                }
                else
                {
                    delay = 0;
                }
            }
        }
        public void InitializeConfig(object s, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) { return; }

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new Config(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Delay in Seconds",
                getValue: () => this.config.DelayInSeconds,
                setValue: value => this.config.DelayInSeconds = value,
                allowedValues: new string[] { "1", "2", "3" }
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Buy Speed",
                getValue: () => this.config.BuySpeed,
                setValue: value => this.config.BuySpeed = value,
                allowedValues: new string[] { "60 Items/sec", "30 Items/sec", "15 Items/sec" }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Put Items in Inventory",
                getValue: () => this.config.PutItemsInInventory,
                setValue: value => this.config.PutItemsInInventory = value
            );
        }
    }
    public class Config
    {
        public string DelayInSeconds { get; set; } = "2";
        public bool PutItemsInInventory { get; set; } = false;
        public string BuySpeed { get; set; } = "30 Items/sec";

        public int GetDelayFrames()
        {
            return this.DelayInSeconds switch
            {
                "1" => 60,
                "2" => 120,
                "3" => 180,
                _ => 120
            };
        }
        public int GetBuyFrames()
        {
            return this.BuySpeed switch
            {
                "60 Items/sec" => 1,
                "30 Items/sec" => 2,
                "15 Items/sec" => 4,
                _ => 2
            };
        }
    }
    public interface IGenericModConfigMenuApi
    {
        /*********
        ** Methods
        *********/
        /****
        ** Must be called first
        ****/
        /// <summary>Register a mod whose config can be edited through the UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="reset">Reset the mod's config to its default values.</param>
        /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        /// <summary>Add a string option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
        /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        /// <summary>Add a boolean option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
