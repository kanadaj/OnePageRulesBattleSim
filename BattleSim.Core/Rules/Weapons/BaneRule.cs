namespace BattleSim.Core.Rules.Weapons;

public sealed class BaneRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.SuppressRegeneration = true;
        context.DefenseModifiers.ForceDefenseSixReroll = true;
    }
}

// Hit rolls of 6 get 2 AP

// Deals wounds to own unit on hit rolls of 1