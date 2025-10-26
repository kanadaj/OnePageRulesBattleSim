namespace BattleSim.Core.Rules.Weapons;

public sealed class CounterRule : IBeforeHitOffensiveRule, IWeaponKeyword
{
    public void Apply(BeforeHitContext context)
    {
        // Counter modifies attack order, handled by the simulator; no per-attack adjustment required.
    }
}