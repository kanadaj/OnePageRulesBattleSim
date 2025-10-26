namespace BattleSim.Core.Rules.Units;

public sealed class StealthRule : IBeforeHitDefensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if  (context.Weapon.IsRanged){
            context.Quality = Math.Min(6, context.Quality + 1);
        }
    }
}