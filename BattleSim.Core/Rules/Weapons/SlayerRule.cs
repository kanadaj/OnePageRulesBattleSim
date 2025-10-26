namespace BattleSim.Core.Rules.Weapons;

public sealed class SlayerRule : IBeforeHitOffensiveRule
{
    public string Name => "Slayer";
    
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Defender.Toughness >= 3)
        {
            context.ArmorPenetration += 2;
        }
    }
}