namespace BattleSim.Core.Rules.Weapons;

public sealed class CavalryCapRule : IBeforeHitOffensiveRule, IWeaponKeyword
{
    private const int MaximumChargingModels = 5;

    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var cappedModels = Math.Min(MaximumChargingModels, context.AttackingModels);
        var baseAttacks = context.AttacksPerModel * context.AttackingModels;
        var cappedBase = context.AttacksPerModel * cappedModels;
        var bonusAttacks = Math.Max(0, context.TotalAttacks - baseAttacks);
        context.TotalAttacks = Math.Max(0, cappedBase + bonusAttacks);
    }
}