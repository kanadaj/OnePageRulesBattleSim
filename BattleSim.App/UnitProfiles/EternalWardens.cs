using BattleSim.Core.Models;
using BattleSim.Core.Rules;

namespace BattleSim.App.UnitProfiles;

public static class EternalWardens
{
    public static class Destroyers
    {
        public static readonly UnitProfile MeteorHammer = new(
            name: "EW Destroyers",
            quality: 3,
            defense: 2,
            toughness: 3,
            fear: 0,
            modelCount:3,
            modelRules: [new WardedRule()],
            weapons:[WeaponProfiles.MeteorHammers]
        );
    }
}

public class WardedRule : IAfterDefenseDefensiveRule
{
    public void Apply(AfterDefenseContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        
    }

    public string Name => "Warded";
}