using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using System;
using StardewValley.Tools;
using Microsoft.Xna.Framework;

namespace LifeSteal
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var h = new Harmony(this.ModManifest.UniqueID);

            h.Patch(
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
                postfix: new HarmonyMethod(typeof(Main), nameof(Main.damageMonsterPostfix))
            );
        }
        static void damageMonsterPostfix(ref bool __result, Farmer who)
        {
            if (__result)
            {
                if (who.CurrentItem is MeleeWeapon m)
                {
                    int averageDamage = (int)(m.minDamage.Value + m.maxDamage.Value) / 2;

                    int lifeSteal = Math.Min(5, (int)(averageDamage * 0.01) + 1);

                    if (Random.Shared.Next(1, 4) == 1)
                    {
                        if (lifeSteal > 0)
                        {
                            who.health = Math.Min(who.maxHealth, who.health + lifeSteal);
                            who.currentLocation.debris.Add(new Debris(lifeSteal, who.getStandingPosition(), Color.Lime, 1f, who));
                        }
                    }
                }
            }
        }
    }
}
