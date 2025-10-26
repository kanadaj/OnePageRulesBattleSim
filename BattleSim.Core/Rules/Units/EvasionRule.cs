namespace BattleSim.Core.Rules.Units;

public sealed class EvasionRule : IBeforeHitDefensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Min(6, context.Quality + 1);
    }
}

// Implements a shield that increases armor by one