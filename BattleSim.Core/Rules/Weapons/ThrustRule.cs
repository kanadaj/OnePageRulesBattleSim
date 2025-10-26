namespace BattleSim.Core.Rules.Weapons;

public sealed class ThrustRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsCharging)
        {
            return;
        }

        context.Quality = Math.Max(2, context.Quality - 1);
        context.ArmorPenetration += 1;
    }
}