using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BuildMenu
{
    internal class Main : Mod
    {
        public Response[] building;
        public StardewValley.GameLocation.afterQuestionBehavior buildingLogic;
        public const string KTShop = "(O)kt.shop";
        public Config config;

        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += this.InitializeQuestionDialogue;
            helper.Events.Input.ButtonReleased += this.Key;
        }
        public void InitializeQuestionDialogue(object s, GameLaunchedEventArgs e)
        {
            building = new Response[]
            {
                new Response("buildBuildings", "Robin"),
                new Response("wizard", "Wizard"),
                new Response("return", "Return")
            };
            buildingLogic = (Farmer who, string buildinganswers) =>
            {
                switch (buildinganswers)
                {
                    case "wizard": Game1.currentLocation.ShowConstructOptions("Wizard"); break;
                    case "buildBuildings": Game1.currentLocation.ShowConstructOptions("Robin"); break;
                }
            };

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) { return; }

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new Config(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Left Click Key",
                getValue: () => config.KeyBind,
                setValue: value => config.KeyBind = value
            );
        }
        public void Key(object s, ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.MouseRight || e.Button == SButton.C || e.Button == SButton.ControllerA)
            {
                if (Context.IsWorldReady && Game1.activeClickableMenu == null)
                {
                    if (Game1.player.CurrentItem?.QualifiedItemId == KTShop)
                    {
                        Game1.currentLocation.createQuestionDialogue(
                            question: "Shop Menu",
                            answerChoices: this.building,
                            afterDialogueBehavior: this.buildingLogic
                        );
                    }
                }
            }
            else if (e.Button == config.KeyBind && Game1.activeClickableMenu == null)
            {
                Game1.currentLocation.createQuestionDialogue(
                    question: "Shop Menu",
                    answerChoices: this.building,
                    afterDialogueBehavior: this.buildingLogic
                );
            }
        }
    }
    public class Config
    {
        public SButton KeyBind { get; set; } = SButton.S;
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

        /// <summary>Add a keybind at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
