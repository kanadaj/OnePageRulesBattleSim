namespace BattleSim.Core.Rules.Weapons;

public sealed class CrackOffensiveRule : IAfterHitOffensiveRule, IWeaponKeyword
{
    public string Name => "Crack";
        
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.AdditionalArmorPenetrationOnNaturalSix += 2;
    }
}