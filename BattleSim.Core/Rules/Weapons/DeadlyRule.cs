namespace BattleSim.Core.Rules.Weapons;

public sealed class DeadlyRule(int maxWounds) : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        
        if(context.Defender.Toughness > 1)
        {
            // Deals at most as many wounds to a single model as Deadly
            context.Map(state => state with { UnsavedWounds = state.UnsavedWounds * Math.Min(context.Defender.Toughness, maxWounds) });
        }
    }
}