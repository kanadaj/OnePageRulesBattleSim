namespace BattleSim.Core.Rules.Weapons;

public sealed class RelentlessRule : IAfterHitRule
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state => state with { TotalHits = state.TotalHits + state.NaturalSixes });
    }
}