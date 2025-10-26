namespace BattleSim.Core.Rules.Weapons;

public class ShredRule : IAfterDefenseOffensiveRule
{
    public string Name => "Shred";
    
    public void Apply(AfterDefenseContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        context.Map(state => state with
        {
            //UnsavedWounds = state.UnsavedWounds + state.NaturalOnes
        });
    }
}