using StardewValley;
using StardewModdingAPI;
using System;
using StardewModdingAPI.Events;

namespace LifeSteal
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += Roll;
        }
        public void Roll(object s, DayStartedEventArgs e)
        {
            int minHeath = Math.Max(1, (int)(Game1.player.maxHealth * 0.01));
            int maxHealth = Game1.player.maxHealth + 1;
            int randomHealth = Random.Shared.Next(minHeath, maxHealth);

            Game1.player.health = randomHealth;
        }
    }
}
