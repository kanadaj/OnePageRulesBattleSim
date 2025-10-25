using BattleSim.Core.Models;
using BattleSim.Core.Rules;

namespace BattleSim.App.UnitProfiles;


public static class HighElves
{
    public static class Heroes
    {
        public static readonly HeroProfile HighNoble = new(
            name: "High Noble",
            quality: 3,
            defense: 4
        );

        public static readonly HeroProfile Champion = new(
            name: "Champion",
            quality: 4,
            defense: 5
        );

        public static class Mounts
        {
            public static readonly HeroMount Dragon = new("Dragon", 12, 3, 2, [
                WeaponProfiles.GiantClaw(6),
                WeaponProfiles.Stomp(),
                WeaponProfiles.BreathAttack()
            ]);
        }
    }

    public static class Warriors
    {
        public static readonly UnitProfile HandWeapon = new UnitProfile(
            name: "HE Warriors",
            quality: 4,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.HandWeapon,
            }
        );

        public static readonly UnitProfile Halberd = new UnitProfile(
            name: "HE Warriors (Halberd)",
            quality: 4,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.Halberd,
            }
        );

        public static readonly UnitProfile Spear = new UnitProfile(
            name: "HE Warriors (Spear)",
            quality: 4,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.Spear,
            }
        );
    }

    public static class WeaponMasters
    {
        public static readonly UnitProfile GreatWeapon = new UnitProfile(
            name: "HE Weapon Masters (Great Weapon)",
            quality: 3,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.GreatWeapon(),
            }
        );

        public static readonly UnitProfile DualHandWeapon = new UnitProfile(
            name: "HE Weapon Masters (Dual Hand Weapon)",
            quality: 3,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.DualHandWeapon,
            }
        );
    }

    public static class LionWarriors
    {
        public static readonly UnitProfile GreatAxe = new UnitProfile(
            name: "HE Lion Warriors",
            quality: 3,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.GreatAxe,
            },
            defensiveAfterDefenseRules: new IAfterDefenseRule[]
            {
                new ResistanceRule(),
            }
        );

        public static UnitProfile GreatAxeWithPiercing => GreatAxe with
        {
            Name = "HE Lion Warriors (with Piercing Tag)",
            AttackerBeforeHitRules = GreatAxe.AttackerBeforeHitRules.Append(new PiercingTagRule(2)).ToArray()
        };
    }

    public static class PhoenixWarriors
    {
        public static readonly UnitProfile HeavyHalberd = new UnitProfile(
            name: "HE Phoenix Warriors",
            quality: 3,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.HeavyHalberd,
            },
            defensiveAfterDefenseRules: new IAfterDefenseRule[]
            {
                new RegenerationRule(),
            }
        );

        public static UnitProfile HeavyHalberdWithPiercing => HeavyHalberd with
        {
            Name = "HE Phoenix Warriors (with Piercing Tag)",
            AttackerBeforeHitRules = HeavyHalberd.AttackerBeforeHitRules.Append(new PiercingTagRule(2)).ToArray()
        };
    }

    public static class Archers
    {
        private static readonly UnitProfile Base = new UnitProfile(
            name: "HE Archers",
            quality: 4,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 5
        );

        public static readonly UnitProfile Longbow = Base with
        {
            Weapons = new[]
            {
                WeaponProfiles.Longbow(),
            }
        };

        public static readonly UnitProfile MasterCraftedBow = Base with
        {
            Weapons = new[]
            {
                WeaponProfiles.MasterCraftedBow(),
            }
        };
    }

    public static class SilverCavalry
    {
        public static readonly UnitProfile Lance = new UnitProfile(
            name: "HE Silver Cavalry",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            attackerBeforeHitRules: new IBeforeHitOffensiveRule[]
            {
                new CavalryCapRule(),
            },
            weapons: new[]
            {
                WeaponProfiles.Lance,
                WeaponProfiles.Impact()
            }
        );
    }

    public static class DragonCavalry
    {
        public static readonly UnitProfile HeavyLance = new UnitProfile(
            name: "HE Dragon Cavalry",
            quality: 3,
            defense: 3,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            attackerBeforeHitRules: new IBeforeHitOffensiveRule[]
            {
                new CavalryCapRule(),
            },
            weapons: new[]
            {
                WeaponProfiles.HeavyLance,
                WeaponProfiles.HeavyImpact()
            }
        );
    }

    public static class GreatEagles
    {
        public static readonly UnitProfile Eagles = new UnitProfile(
            name: "HE Great Eagles",
            quality: 4,
            defense: 4,
            toughness: 3,
            fear: 0,
            modelCount: 3,
            weapons: new[]
            {
                WeaponProfiles.Impact(2),
            }
        );
    }

    public static class IcePhoenix
    {
        public static readonly UnitProfile Phoenix = new UnitProfile(
            name: "HE Ice Phoenix",
            quality: 3,
            defense: 3,
            toughness: 6,
            fear: 0,
            modelCount: 1,
            defensiveAfterDefenseRules: new IAfterDefenseRule[]
            {
                new RegenerationRule(),
            },
            defensiveBeforeHitRules: new IBeforeHitDefensiveRule[]
            {
                new EvasionRule(),
            },
            weapons: new[]
            {
                WeaponProfiles.IceClaws,
            }
        );

        public static UnitProfile WithPiercingTag => Phoenix with
        {
            AttackerBeforeHitRules = Phoenix.AttackerBeforeHitRules.Append(new PiercingTagRule(2)).ToArray()
        };
    }

    public static class FirePhoenix
    {
        public static readonly UnitProfile Phoenix = new UnitProfile(
            name: "HE Fire Phoenix",
            quality: 4,
            defense: 3,
            toughness: 6,
            fear: 0,
            modelCount: 1,
            defensiveAfterDefenseRules: new IAfterDefenseRule[]
            {
                new RegenerationRule(),
            },
            weapons: new[]
            {
                WeaponProfiles.FlameClaws,
                WeaponProfiles.CrossingBarrage(),
            }
        );

        public static UnitProfile WithPiercingTag => Phoenix with
        {
            AttackerBeforeHitRules = Phoenix.AttackerBeforeHitRules.Append(new PiercingTagRule(2)).ToArray()
        };
    }

    public static class BullGiant
    {
        public static readonly UnitProfile Greathammer = new UnitProfile(
            name: "HE Bull Giant (Greathammer)",
            quality: 3,
            defense: 3,
            toughness: 12,
            fear: 0,
            modelCount: 1,
            weapons: new[]
            {
                WeaponProfiles.BreathAttack(),
                WeaponProfiles.Greathammer,
            }
        );

        public static readonly UnitProfile DualHammer = new UnitProfile(
            name: "HE Bull Giant (Dual Hammer)",
            quality: 3,
            defense: 3,
            toughness: 12,
            fear: 0,
            modelCount: 1,
            weapons: new[]
            {
                WeaponProfiles.BreathAttack(),
                WeaponProfiles.DualHammer,
            }
        );
    }
}
