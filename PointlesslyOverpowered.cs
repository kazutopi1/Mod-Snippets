using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using HarmonyLib;
using System;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace PointlesslyOverpowered
{
    public class Main : Mod
    {
        static IMonitor _monitor;

        public override void Entry(IModHelper helper)
        {
            _monitor = this.Monitor;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] {
                    typeof(Rectangle),
                    typeof(int),
                    typeof(int),
                    typeof(bool),
                    typeof(float),
                    typeof(int),
                    typeof(float),
                    typeof(float),
                    typeof(bool),
                    typeof(Farmer),
                    typeof(bool)
                }),
                prefix: new HarmonyMethod(typeof(Main), nameof(Main.damageMonsterPrefix))
            );
        }
        public static void damageMonsterPrefix(GameLocation __instance, Rectangle areaOfEffect, Farmer who, ref int minDamage, ref int maxDamage)
        {
            try
            {
                foreach (var character in __instance.characters)
                {
                    if (character is Monster monster && monster.GetBoundingBox().Intersects(areaOfEffect))
                    {
                        if (who.CurrentItem is MeleeWeapon m)
                        {
                            if (Random.Shared.Next(1, 10001) == 1)
                            {
                                minDamage = 99999;
                                maxDamage = 99999;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _monitor.LogOnce($"{ex}", LogLevel.Error);
            }
        }
    }
}
