using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using System.Reflection;

namespace HoldButtonBButYouCanNoLongerEat
{
    public class Main : Mod
    {
        public MethodInfo overrideButtonMethod = AccessTools.Method(Game1.input.GetType(), "OverrideButton");
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.pressActionButton)),
                prefix: new HarmonyMethod(typeof(Main), nameof(Main.Prefix)),
                postfix: new HarmonyMethod(typeof(Main), nameof(Main.Postfix))
            );

            helper.Events.GameLoop.UpdateTicked += ButtonB;
        }
        static bool Prefix(out (int originalEdibility, StardewValley.Object item)? __state)
        {
            __state = null;

            if (Game1.player.ActiveObject != null)
            {
                if (Game1.player.ActiveObject.Edibility > StardewValley.Object.inedible)
                {
                    __state = (Game1.player.ActiveObject.Edibility, Game1.player.ActiveObject);

                    Game1.player.ActiveObject.Edibility = StardewValley.Object.inedible;
                }
            }
            return true;
        }
        static void Postfix((int originalEdibility, StardewValley.Object item)? __state)
        {
            if (__state.HasValue)
            {
                var savedValue = __state.Value;

                if (savedValue.item != null)
                {
                    savedValue.item.Edibility = savedValue.originalEdibility;
                }
            }
        }
        void ButtonB(object s, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(4))
                return;

            if (overrideButtonMethod != null)
            {
                if (Game1.virtualJoypad.ButtonBPressed)
                {
                    Helper.Input.Suppress(SButton.MouseLeft);
                    overrideButtonMethod.Invoke(Game1.input, new object[] { SButton.MouseRight, true });
                }
            }
        }
    }
}
