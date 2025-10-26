namespace BattleSim.Core.Rules.Weapons;

public sealed class ReapRule : IBeforeHitOffensiveRule
{
    public string Name => "Reap";
    
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Defender.Defense is 2 or 3)
        {
            context.ArmorPenetration += 2;
        }
    }
}