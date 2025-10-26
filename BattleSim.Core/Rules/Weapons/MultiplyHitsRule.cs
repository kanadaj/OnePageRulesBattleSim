namespace BattleSim.Core.Rules.Weapons;

public sealed class MultiplyHitsOffensiveRule(int hitMultiplier = 3) : IAfterHitOffensiveRule
{
    public string Name => "Multiply Hits";

    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state =>
        {
            if (state.TotalHits <= 0)
            {
                return state;
            }

            var multipliedHits = state.TotalHits * hitMultiplier;
            var multipliedSixes = state.NaturalSixes * hitMultiplier;
            return state with { TotalHits = multipliedHits, NaturalSixes = multipliedSixes };
        });
    }
}