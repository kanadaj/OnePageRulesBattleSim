namespace BattleSim.Core.Rules.Weapons;

public sealed class PiercingTagRule(int armorPenetration = 1) : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ArmorPenetration += armorPenetration;
    }
}