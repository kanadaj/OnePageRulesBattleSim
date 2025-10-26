namespace BattleSim.Core.Rules.Weapons;

public sealed class RendingRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.SuppressRegeneration = true;
        context.DefenseModifiers.AdditionalArmorPenetrationOnNaturalSix += 4;
    }
}