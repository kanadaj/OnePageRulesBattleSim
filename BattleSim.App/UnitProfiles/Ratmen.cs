/*
Ratmen:
Militia: 10 Models, Quality 6, Defense 6, uses Hand Weapon
Warriors: 10 Models, Quality 5, Defense 5, can use Hand Weapons, Spears or Halberds
Storm Veterans: 5 Models, Quality 4, Defense 4, can use Heavy Halberds or Heavy Great Weapons
Night Scouts: 5 Models, Quality 4, Defense 5, Evasive, use Dual Hand Weapons
Rat Ogres: 3 Models, Tough(3), Quality 4, Defense 4, Furious, use Rending Claws or Great Weapon
Storm Ogres: 3 Models, Tough(3), Quality 3, Defense 3, Furious, can use Drill First, Shock Fist or Roller Fist
Giant Beast: 1 Model, Tough(12), Quality 4, Defense 3, Regeneration, Unpredictable Fighter, Fear(2), uses Claw(10) and Stomp(4) both*/

using BattleSim.App;
using BattleSim.Core.Models;
using BattleSim.Core.Rules;
using BattleSim.Core.Rules.Units;
using BattleSim.Core.Rules.Weapons;

public static class Ratmen
{
    public static class Heroes
    {
        public static readonly HeroProfile BattleMaster = new(
            name: "Battle Master",
            quality: 4,
            defense: 4
        );

        public static readonly HeroProfile Champion = new(
            name: "Champion",
            quality: 5,
            defense: 5
        );
    }

    public static class Militia
    {
        public static readonly UnitProfile HandWeapon = new UnitProfile(
            name: "R Militia",
            quality: 6,
            defense: 6,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.HandWeapon,
            }
        );
    }

    public static class Warriors
    {
        public static readonly UnitProfile HandWeapon = new UnitProfile(
            name: "R Warriors",
            quality: 5,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.HandWeapon,
            }
        );

        public static readonly UnitProfile Spear = new UnitProfile(
            name: "R Warriors (Spear)",
            quality: 5,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.Spear,
            }
        );

        public static readonly UnitProfile Halberd = new UnitProfile(
            name: "R Warriors (Halberd)",
            quality: 5,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: new[]
            {
                WeaponProfiles.Halberd,
            }
        );
    }

    public static class WeaponTeams
    {
        private static readonly UnitProfile Base = new UnitProfile(
            name: "R Weapon Team",
            quality: 5,
            defense: 5,
            toughness: 3,
            fear: 0,
            modelCount: 3
        );

        public static readonly UnitProfile GatlingGun = Base with
        {
            Name = "R Gatling Gun Team",
            Weapons = new[]
            {
                WeaponProfiles.GatlingGun,
            }
        };

        public static readonly UnitProfile Flamethrower = Base with
        {
            Name = "R Flamethrower Team",
            Weapons = new[]
            {
                WeaponProfiles.Flamethrower,
            }
        };

        public static readonly UnitProfile OvertunedGatlingGun = Base with
        {
            Name = "R Overtuned Gatling Gun Team",
            Weapons = new[]
            {
                WeaponProfiles.OvertunedGatlingGun,
            }
        };

        public static readonly UnitProfile ToxinMortar = Base with
        {
            Name = "R Toxin Mortar Team",
            Weapons = new[]
            {
                WeaponProfiles.ToxinMortar,
            }
        };
    }

    public static class StormVeterans
    {
        public static readonly UnitProfile HeavyHalberd = new UnitProfile(
            name: "R Storm Veterans",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.HeavyHalberd,
            }
        );

        public static readonly UnitProfile HeavyGreatWeapon = new UnitProfile(
            name: "R Storm Veterans (Heavy Great Weapon)",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.HeavyGreatWeapon(),
            }
        );
    }

    public static class NightScouts
    {
        public static readonly UnitProfile DualHandWeapon = new UnitProfile(
            name: "R Night Scouts",
            quality: 4,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: new[]
            {
                WeaponProfiles.DualHandWeapon,
            },
            defensiveBeforeHitRules: new IBeforeHitDefensiveRule[]
            {
                new EvasionRule(),
                new StealthRule()
            }
        );
    }

    public class RatSwarms
    {
        public static readonly UnitProfile Base = new UnitProfile(
            name: "R Rat Swarm",
            quality: 5,
            defense: 6,
            toughness: 3,
            fear: 0,
            modelCount: 3,
            weapons: new[]
            {
                WeaponProfiles.SwarmAttack(),
            }
        );

        public static UnitProfile UnpredictableFighter => Base with
        {
            Name = "R Rat Swarm (Unpredictable Fighter)",
            Weapons = new []
            {
                WeaponProfiles.SwarmAttack() with
                {
                    AfterHitRules = new IAfterHitRule[]
                    {
                        new UnpredictableFighterRule()
                    }
                },
            },
        };
    }
    public static class RatOgres
    {
        public static readonly UnitProfile RendingClaws = new UnitProfile(
            name: "R Rat Ogres (Rending Claws)",
            quality: 4,
            defense: 4,
            toughness: 3,
            fear: 0,
            modelCount: 3,
            weapons: new[]
            {
                WeaponProfiles.RendingClaws with { AfterHitRules = new IAfterHitRule[] { new FuriousRule() } },
            }
        );

        public static readonly UnitProfile GreatWeapon = new UnitProfile(
            name: "R Rat Ogres (Great Weapon)",
            quality: 4,
            defense: 4,
            toughness: 3,
            fear: 0,
            modelCount: 3,
            weapons: new[]
            {
                WeaponProfiles.GreatWeapon(3) with { AfterHitRules = new IAfterHitRule[] { new FuriousRule() } },
            }
        );
    }

    public static class StormOgres
    {
        private static readonly UnitProfile Base = new UnitProfile(
            name: "R Storm Ogres",
            quality: 3,
            defense: 3,
            toughness: 3,
            fear: 0,
            modelCount: 3
        );

        public static readonly UnitProfile DrillFist = Base with
        {
            Name = "R Storm Ogres (Drill First)",
            Weapons = new[]
            {
                WeaponProfiles.DrillFist with { AfterHitRules = new IAfterHitRule[] { new FuriousRule() } },
            }
        };

        public static readonly UnitProfile ShockFist = Base with
        {
            Name = "R Storm Ogres (Shock Fist)",
            Weapons = new[]
            {
                WeaponProfiles.ShockFist with { AfterHitRules = new IAfterHitRule[] { new FuriousRule() } },
            }
        };

        public static readonly UnitProfile RollerFist = Base with
        {
            Name = "R Storm Ogres (Roller Fist)",
            Weapons = new[]
            {
                WeaponProfiles.RollerFist with { AfterHitRules = new IAfterHitRule[] { new FuriousRule() } },
            }
        };


        public static readonly UnitProfile OvertunedGatlingGun = Base with
        {
            Name = "R Storm Ogres (OT Gatling Gun)",
            Weapons = new[]
            {
                WeaponProfiles.OvertunedGatlingGun,
            }
        };


        public static readonly UnitProfile Gatling = Base with
        {
            Name = "R Storm Ogres (Gatling Gun)",
            Weapons = new[]
            {
                WeaponProfiles.GatlingGun,
            }
        };

        
    }

    public static readonly UnitProfile GiantBeast = new UnitProfile(
        name: "R Giant Beast",
        quality: 4,
        defense: 3,
        toughness: 12,
        fear: 0,
        modelCount: 1,
        weapons: new[]
        {
            WeaponProfiles.Claw(10),
            WeaponProfiles.Stomp(4),
        }
    );


    public static class GiantBlessedBeast
    {
        public static readonly UnitProfile GiantOtter = new UnitProfile(
            name: "R Giant Blessed Beast",
            quality: 3,
            defense: 2,
            toughness: 18,
            fear: 3,
            modelCount: 1,
            weapons: new[]
            {
                WeaponProfiles.GiantClaw(12),
                WeaponProfiles.HeavyStomp(6),
            }
        );
    }
}