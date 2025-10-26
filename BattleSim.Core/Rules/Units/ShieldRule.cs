namespace BattleSim.Core.Rules.Units;

public sealed class ShieldRule : IBeforeHitDefensiveRule
{
    public string Name => "Shield";
    
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ArmorPenetration -= 1;
    }
}