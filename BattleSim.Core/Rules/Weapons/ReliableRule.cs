namespace BattleSim.Core.Rules.Weapons;

public sealed class ReliableRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Min(context.Quality, 2);
    }
}