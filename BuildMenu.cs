using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BuildMenu
{
    internal class Main : Mod
    {
        public Response[] building;
        public StardewValley.GameLocation.afterQuestionBehavior buildingLogic;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.InitializeQuestionDialogue;
            helper.Events.Input.ButtonPressed += this.Key;
        }
        public void InitializeQuestionDialogue(object s, GameLaunchedEventArgs e)
        {
            building = new Response[]
            {
                new Response("buildBuildings", "Robin"),
                new Response("wizard", "Wizard")
            };
            buildingLogic = (Farmer who, string buildinganswers) =>
            {
                switch (buildinganswers)
                {
                    case "wizard": Game1.currentLocation.ShowConstructOptions("Wizard"); break;
                    case "buildBuildings": Game1.currentLocation.ShowConstructOptions("Robin"); break;
                }
            };
        }
        public void Key(object s, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F1 && Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                if (Helper.Input.IsDown(SButton.LeftShift) && Helper.Input.IsDown(SButton.LeftControl))
                {
                    Game1.currentLocation.createQuestionDialogue(
                        question: "Build",
                        answerChoices: this.building,
                        afterDialogueBehavior: this.buildingLogic
                    );
                }
            }
        }
    }
}
