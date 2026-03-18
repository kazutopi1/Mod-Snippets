using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mobile;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Microsoft.Xna.Framework;

namespace GlobalSeeder
{
    internal class ModEntry : Mod
    {
        private static bool wasATapped = false;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonAPressed)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Seed))
            );
        }
        public static void Seed(ref bool __result)
        {
            if (__result && !wasATapped)
            {
                if (Game1.player.ActiveObject is StardewValley.Object heldItem && heldItem.Category == StardewValley.Object.SeedsCategory)
                {
                    foreach (var feature in Game1.currentLocation.terrainFeatures.Pairs)
                    {
                        if (feature.Value is HoeDirt dirt)
                        {
                            if (dirt.crop == null)
                            {
                                dirt.plant(Game1.player.ActiveObject.ItemId, Game1.player, false);

                                Game1.player.reduceActiveItemByOne();

                                if (Game1.player.ActiveObject == null) { break; }
                            }
                        }
                    }
                }
            }
            if (__result && !wasATapped)
            {
                if (Game1.player.CurrentTool is StardewValley.Tools.WateringCan can)
                {
                    foreach (var feature in Game1.currentLocation.terrainFeatures.Pairs)
                    {
                        if (feature.Value is HoeDirt dirt)
                        {
                            dirt.state.Value = HoeDirt.watered;
                        }
                    }
                }
            }
            if (__result && wasATapped)
            {
                if (Game1.player.CurrentTool is StardewValley.Tools.MeleeWeapon weapon && weapon.isScythe())
                {
                    MeleeWeapon iridiumS = new MeleeWeapon("(W)66");

                    foreach (var feature in Game1.currentLocation.terrainFeatures.Pairs)
                    {
                        if (feature.Value is HoeDirt dirt && dirt.crop != null)
                        {
                            if (dirt.crop.currentPhase.Value >= dirt.crop.phaseDays.Count - 1)
                            {
                                Vector2 tile = feature.Key;

                                dirt.performToolAction(iridiumS, 0, tile);
                            }
                        }
                    }
                }
            }
            wasATapped = __result;
        }
    }
}
