namespace BattleSim.Core.Rules.Units;

public sealed class FortifiedOffensiveRule : IAfterHitDefensiveRule
{
    public string Name => "Fortified";
    
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

            return state with
            {
                ArmorPenetrationBonus = state.ArmorPenetrationBonus >= 1 ? state.ArmorPenetrationBonus - 1 : 0,
            };
        });
    }
}