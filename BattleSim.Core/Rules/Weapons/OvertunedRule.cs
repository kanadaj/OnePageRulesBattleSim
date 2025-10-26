namespace BattleSim.Core.Rules.Weapons;

public sealed class OvertunedRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state => state.AddSelfWounds(state.NaturalOnes));
    }
}