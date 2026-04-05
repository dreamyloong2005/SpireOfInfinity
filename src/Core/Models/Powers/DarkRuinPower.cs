using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfInfinity.Core.Models.Powers;

public class DarkRuinPower : WodBasePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        List<PowerModel> OwnerPowers = [.. Owner.Powers];
        foreach (var power in OwnerPowers)
        {
            if (power.Type == PowerType.Buff && power.Amount > 0 && power.Id.Entry != "MINION_POWER" && power.Id.Entry != "ESCAPE_ARTIST_POWER")
            {
                await PowerCmd.Remove(power);
            }
        }
        if(Owner.HasPower<RuinPower>())
        {
            await PowerCmd.Remove(Owner.GetPower<RuinPower>());
        }
    }
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == Owner && power.Type == PowerType.Buff && amount > 0)
        {
            await PowerCmd.Remove(power);
        }
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.TickDownDuration(this);
		}
	}
}