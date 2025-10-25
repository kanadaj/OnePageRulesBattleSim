using System;
using System.Collections.Generic;
using System.Linq;
using BattleSim.Core.Rules;

namespace BattleSim.Core.Models;

/// <summary>
/// Describes the weapon profile available to a unit's models.
/// </summary>
public sealed record WeaponProfile
{
    public WeaponProfile(
        string name,
        int attacksPerModel,
        int armorPenetration = 0,
        bool isRanged = false,
        IEnumerable<IBeforeHitOffensiveRule>? beforeHitRules = null,
        IEnumerable<IAfterHitRule>? afterHitRules = null,
        IEnumerable<IAfterDefenseRule>? afterDefenseRules = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Weapon name cannot be null or whitespace.", nameof(name));
        }

        if (attacksPerModel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attacksPerModel), "Attacks per model cannot be negative.");
        }

        Name = name;
        AttacksPerModel = attacksPerModel;
        ArmorPenetration = armorPenetration;
        IsRanged = isRanged;
        BeforeHitRules = (beforeHitRules ?? Array.Empty<IBeforeHitOffensiveRule>()).ToArray();
        AfterHitRules = (afterHitRules ?? Array.Empty<IAfterHitRule>()).ToArray();
        AfterDefenseRules = (afterDefenseRules ?? Array.Empty<IAfterDefenseRule>()).ToArray();
    }

    public string Name { get; }

    /// <summary>
    /// Number of attack dice rolled per surviving model.
    /// </summary>
    public int AttacksPerModel { get; }

    /// <summary>
    /// Armor penetration modifier applied against the defender's saves.
    /// </summary>
    public int ArmorPenetration { get; }

    /// <summary>
    /// True if the weapon is a ranged weapon.
    /// </summary>
    public bool IsRanged { get; }

    public IReadOnlyList<IBeforeHitOffensiveRule> BeforeHitRules { get; init; }

    public IReadOnlyList<IAfterHitRule> AfterHitRules { get; init; }

    public IReadOnlyList<IAfterDefenseRule> AfterDefenseRules { get; init; }
}
