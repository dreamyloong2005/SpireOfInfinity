using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfInfinity.Core.Models.Powers;

public class PowerFromThePastPower : WodBasePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == Owner && power.Id.Entry == "SPIREOFINFINITY-POWER_FROM_THE_PAST_POWER" && power.Amount >= 10 && !Owner.HasPower<DarknessAwakenPower>())
        {
            await PowerCmd.Apply<DarknessAwakenPower>(Owner, 1m, Owner, null);
        }
    }
}