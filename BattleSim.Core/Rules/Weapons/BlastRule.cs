namespace BattleSim.Core.Rules.Weapons;

public sealed class BlastRule(int maxWounds) : IAfterHitRule
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        
        if(context.Defender.ModelCount > 1)
        {
            // Deals at most as many wounds to a single model as Deadly
            context.Map(state => state with { TotalHits = state.TotalHits * Math.Min(context.Defender.ModelCount, maxWounds) });
        }
    }
}