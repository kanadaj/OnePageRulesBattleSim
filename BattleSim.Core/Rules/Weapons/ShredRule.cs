namespace BattleSim.Core.Rules.Weapons;

public class ShredRule : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        context.Map(state => state with
        {
            UnsavedWounds = state.UnsavedWounds + state.NaturalOnes
        });
    }
}