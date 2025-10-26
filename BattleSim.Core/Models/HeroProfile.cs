using BattleSim.Core.Rules;

namespace BattleSim.Core.Models;

/// <summary>
/// Represents an individual hero that can be attached to a unit.
/// </summary>
public sealed record HeroProfile : UnitProfile
{
    public HeroProfile(
        string name,
        int quality,
        int defense,
        int toughness = 3,
        int fear = 0,
        IEnumerable<WeaponProfile>? weapons = null,
        IEnumerable<IRule>? modelRules = null,
        IEnumerable<IRule>? auras = null) : base(name, quality, defense, toughness, fear, 1, weapons, modelRules, null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        if (quality < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must allow for natural 1 failures.");
        }

        if (toughness <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(toughness), "Toughness must be positive.");
        }

        Auras = (auras ?? Array.Empty<IRule>()).ToArray();
    }

    
    public IReadOnlyList<IRule> Auras { get; init; }

    public string? Mount { get; init; }

    public HeroProfile WithWeapons(params WeaponProfile[] weapons)
    {
        return this with { Weapons = weapons };
    }

    public HeroProfile WithPersonalRules(params IRule[] rules)
    {
        return this with
        {
            Rules = [..Rules, ..rules]
        };
    }

    public HeroProfile WithAuras(params IRule[] rules)
    {
        return this with
        {
            Auras = [..Auras, ..rules]
        };
    }
    
    public HeroProfile WithMount(HeroMount mount)
    {
        return (this with
        {
            Mount = mount.Name,
            Toughness = Toughness + mount.AdditionalToughness,
            Weapons = Weapons.Concat(mount.Weapons).ToArray(),
            Defense = mount.Armor,
            Fear = Fear + mount.Fear
        }).WithPersonalRules(mount.Rules.ToArray());
    }
}

public record HeroMount(string Name, int AdditionalToughness, int Armor, int Fear, ICollection<WeaponProfile> Weapons, params IRule[] Rules);