using BattleSim.App;
using BattleSim.App.UnitProfiles;
using BattleSim.Core.Models;
using BattleSim.Core.Rules;
using BattleSim.Core.Simulation;

var simulator = new BattleSimulator();

Console.WriteLine("BattleSim – Fantasy Wargame Combat");
Console.WriteLine();

// RunBattle(simulator, HighElves.LionWarriors.GreatAxeWithPiercing.AsCombinedUnit, Ratmen.GiantBlessedBeast.GiantOtter);
// RunBattle(simulator, HighElves.LionWarriors.GreatAxe.AsCombinedUnit, Ratmen.GiantBlessedBeast.GiantOtter);

// var lionHero = HighElves.Heroes.HighNoble.WithWeapons(WeaponProfiles.HeavyGreatWeapon(3))
// 	.WithPersonalRules(new ResistanceRule())
// 	.WithAuras(new ShieldRule());

var lionHero = HighElves.Heroes.HighNoble.WithWeapons(WeaponProfiles.HeavyGreatWeapon(3))
	.WithPersonalRules(new ResistanceRule())
	.WithAuras(new ShieldRule());

var spearHero = HighElves.Heroes.Champion.WithWeapons(WeaponProfiles.Spear)
	.WithPersonalRules(new ShieldRule());

// battleReports.Add(RunBattle(simulator, HighElves.LionWarriors.GreatAxeWithPiercing.AsCombinedUnit.WithHero(lionHero), Ratmen.GiantBlessedBeast.GiantOtter));
// battleReports.Add(RunBattle(simulator, HighElves.LionWarriors.GreatAxe.AsCombinedUnit.WithHero(lionHero), Ratmen.GiantBlessedBeast.GiantOtter));

var archerHeroLongbow = HighElves.Heroes.Champion.WithWeapons(WeaponProfiles.Longbow(3))
	.WithPersonalRules(new ResistanceRule())
	.WithAuras(new RelentlessRule());

var ratBaneHero = Ratmen.Heroes.Champion
	.WithAuras(new BaneRule());

var battleReports = new List<BattleReport>();

battleReports.Add(RunBattle(simulator, HighElves.Warriors.Spear.AsCombinedUnit, Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit));
battleReports.Add(RunBattle(simulator, Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit, HighElves.Warriors.Spear.AsCombinedUnit));
battleReports.Add(RunBattle(simulator, HighElves.Warriors.Spear.AsCombinedUnit.WithHero(spearHero), Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit));
battleReports.Add(RunBattle(simulator, Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit, HighElves.Warriors.Spear.AsCombinedUnit.WithHero(spearHero)));

battleReports.Add(RunBattle(simulator, HighElves.LionWarriors.GreatAxe.AsCombinedUnit, Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit));

battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.Warriors.HandWeapon, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.Warriors.HandWeapon.AsCombinedUnit, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.StormVeterans.HeavyHalberd.AsCombinedUnit, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.NightScouts.DualHandWeapon.AsCombinedUnit, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.RatSwarms.Base.AsCombinedUnit, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.WeaponTeams.GatlingGun, SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, HighElves.Archers.Longbow.AsCombinedUnit.WithHero(archerHeroLongbow), Ratmen.WeaponTeams.GatlingGun.AsCombinedUnit, SimulationMode.Ranged));

battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit, HighElves.DragonCavalry.HeavyLance.AsCombinedUnit.WithHero(lionHero), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit, lionHero.WithMount(HighElves.Heroes.Mounts.Dragon), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit, HighElves.LionWarriors.GreatAxe.AsCombinedUnit.WithHero(lionHero), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit, HighElves.Warriors.Spear.AsCombinedUnit, SimulationMode.Ranged));

battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit.WithHero(ratBaneHero), HighElves.DragonCavalry.HeavyLance.AsCombinedUnit.WithHero(lionHero), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit.WithHero(ratBaneHero), lionHero.WithMount(HighElves.Heroes.Mounts.Dragon), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit.WithHero(ratBaneHero), HighElves.LionWarriors.GreatAxe.AsCombinedUnit.WithHero(lionHero), SimulationMode.Ranged));
battleReports.Add(RunBattle(simulator, Ratmen.WeaponTeams.OvertunedGatlingGun.AsCombinedUnit.WithHero(ratBaneHero), HighElves.Warriors.Spear.AsCombinedUnit, SimulationMode.Ranged));

Console.WriteLine("Combined battle summary:");
PrintCombinedBattleTable(battleReports);

static BattleReport RunBattle(
	BattleSimulator simulator,
	UnitProfile attacker,
	UnitProfile defender,
	SimulationMode mode = SimulationMode.Melee)
{
	var result = simulator.Simulate(attacker, defender, mode);
	var modeSuffix = mode == SimulationMode.Ranged ? " [Ranged]" : string.Empty;
	var attackerName = attacker.GetCombinedName();
	var defenderName = defender.GetCombinedName();
	Console.WriteLine($"{attackerName} (attacking) vs {defenderName} (defending){modeSuffix}");

	var rows = new List<string[]>
	{
		new[] { "Metric", "Attacker", "Defender", "Notes" },
		new[] { "Avg wounds", FormatDecimal(result.AverageWoundsToUnitA), FormatDecimal(result.AverageWoundsToUnitB), string.Empty },
		new[] { "Avg hits", FormatDecimal(result.AverageHitsByUnitA), FormatDecimal(result.AverageHitsByUnitB), string.Empty },
		new[] { "Avg models lost", FormatDecimal(result.AverageModelsLostByUnitA), FormatDecimal(result.AverageModelsLostByUnitB), string.Empty }
	};

	if (mode == SimulationMode.Melee)
	{
		rows.Add(new[]
		{
			"Win chance",
			FormatPercent(result.ProbabilityUnitAWins),
			FormatPercent(result.ProbabilityUnitBWins),
			$"Tie: {FormatPercent(result.ProbabilityOfTie)}"
		});

		rows.Add(new[]
		{
			"Wipe chance",
			FormatPercent(result.ProbabilityUnitADestroysUnitB),
			FormatPercent(result.ProbabilityUnitBDestroysUnitA),
			string.Empty
		});
	}
	else
	{
		rows.Add(new[]
		{
			"Wipe chance",
			FormatPercent(result.ProbabilityUnitADestroysUnitB),
			FormatPercent(result.ProbabilityUnitBDestroysUnitA),
			string.Empty
		});
		rows.Add(new[]
		{
			"Morale check chance",
			FormatPercent(result.ProbabilityUnitAWins),
			string.Empty,
			string.Empty
		});
	}

	PrintTable(rows);

	return new BattleReport(
		attacker.GetName() + $" [{attacker.ModelCount}]",
		attacker is HeroProfile ? "" : attacker.Hero is null ? "N" : "Y",
		defender.GetName() + $" [{defender.ModelCount}]",
		defender is HeroProfile ? "" : defender.Hero is null ? "N" : "Y",
		mode,
		result,
		attacker.Hero is {} h1 ? h1 : null,
		defender.Hero is {} h2 ? h2 : null
	);
}

static void PrintCombinedBattleTable(IReadOnlyList<BattleReport> reports)
{
	if (reports.Count == 0)
	{
		Console.WriteLine("  No battles to display.");
		Console.WriteLine();
		return;
	}

	var rows = new List<string[]>
	{
		new[]
		{
			"Attacker",
			"Hero",
			"Defender",
			"Hero",
			"RNG",
			"~Wounds (Atk)",
			"~Wounds (Def)",
			"~Hits (Atk)",
			"~Hits (Def)",
			//"Avg Models Lost (Atk)",
			//"Avg Models Lost (Def)",
			"Win % (Atk)",
			"Win % (Def)",
			"Tie",
			"Notes"
		}
	};

	foreach (var report in reports)
	{
		var result = report.Result;
		var winChanceAttacker = FormatPercent(result.ProbabilityUnitAWins);
		var winChanceDefender = FormatPercent(result.ProbabilityUnitBWins);
		var tieChance = FormatPercent(result.ProbabilityOfTie);
		var notes = $"Wipe atk: {FormatPercent(result.ProbabilityUnitADestroysUnitB)}; Wipe def: {FormatPercent(result.ProbabilityUnitBDestroysUnitA)}";
    
		if(report.AttackerHeroProfile?.Auras.Any() == true)
		{
			notes += $" | Atk Hero: {string.Join(",", report.AttackerHeroProfile.Auras.Select<IRule, string>(x => x.GetType().Name.Replace("Rule", "").ToString()))}";
		}

		if(report.DefenderHeroProfile?.Auras.Any() == true)
		{
			notes += $" | Def Hero: {string.Join(",", report.DefenderHeroProfile.Auras.Select<IRule, string>(x => x.GetType().Name.Replace("Rule", "").ToString()))}";
		}

		rows.Add(new[]
		{
			report.AttackerName,
			report.AttackerHero,
			report.DefenderName,
			report.DefenderHero,
			report.Mode == SimulationMode.Ranged ? "R" : "M",
			FormatDecimal(result.AverageWoundsToUnitA),
			FormatDecimal(result.AverageWoundsToUnitB),
			FormatDecimal(result.AverageHitsByUnitA),
			FormatDecimal(result.AverageHitsByUnitB),
			//FormatDecimal(result.AverageModelsLostByUnitA),
			//FormatDecimal(result.AverageModelsLostByUnitB),
			winChanceAttacker,
			winChanceDefender,
			tieChance,
			notes
		});
	}

	PrintTable(rows, string.Empty);
}

static string FormatDecimal(double value) => value.ToString("F2");

static string FormatPercent(double value) => value.ToString("P1");

static void PrintTable(IReadOnlyList<string[]> rows, string indent = "  ")
{
	if (rows.Count == 0)
	{
		return;
	}

	var columnWidths = new int[rows[0].Length];
	foreach (var row in rows)
	{
		for (var i = 0; i < columnWidths.Length; i++)
		{
			var cell = row[i];
			if (cell.Length > columnWidths[i])
			{
				columnWidths[i] = cell.Length;
			}
		}
	}

	for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
	{
		var row = rows[rowIndex];
		var paddedCells = new string[row.Length];
		for (var i = 0; i < row.Length; i++)
		{
			var cell = row[i];
			paddedCells[i] = cell.PadRight(columnWidths[i]);
		}

		Console.WriteLine(indent + string.Join("  ", paddedCells));

		if (rowIndex == 0)
		{
			var separatorCells = new string[columnWidths.Length];
			for (var i = 0; i < columnWidths.Length; i++)
			{
				separatorCells[i] = new string('-', columnWidths[i]);
			}
			Console.WriteLine(indent + string.Join("  ", separatorCells));
		}
	}

	Console.WriteLine();
}

file sealed record BattleReport(
	string AttackerName,
  	string AttackerHero,
	string DefenderName,
	string DefenderHero,
	SimulationMode Mode,
	BattleResult Result,
	HeroProfile? AttackerHeroProfile = null,
	HeroProfile? DefenderHeroProfile = null);
