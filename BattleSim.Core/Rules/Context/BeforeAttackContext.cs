using BattleSim.Core.Models;

namespace BattleSim.Core.Rules.Context;

public record BeforeAttackContext(UnitProfile Attacker, UnitProfile Defender, int Quality, int AttacksToMake, bool RegenerationAllowed);