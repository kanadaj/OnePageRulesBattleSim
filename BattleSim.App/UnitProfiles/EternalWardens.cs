using BattleSim.Core.Rules;

namespace BattleSim.App.UnitProfiles;

public static class EternalWardens
{
    
}

public class WardedRule : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        
    }
}