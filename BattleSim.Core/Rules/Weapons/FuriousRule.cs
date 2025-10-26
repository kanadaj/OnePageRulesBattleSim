namespace BattleSim.Core.Rules.Weapons;

public sealed class FuriousOffensiveRule : IAfterHitOffensiveRule
{
    public string Name => "Furious";
    
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsCharging)
        {
            return;
        }

        context.Map(state => state with { TotalHits = state.TotalHits + state.NaturalSixes });
    }
}