namespace BattleSim.Core.Rules.Weapons;

public sealed class PreciseRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Max(2, context.Quality - 1);
    }
}