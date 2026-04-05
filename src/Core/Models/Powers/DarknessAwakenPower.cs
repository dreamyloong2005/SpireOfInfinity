using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Powers
{
    public class DarknessAwakenPower : WodBasePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override bool AllowNegative => false;
        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if(side == CombatSide.Player)
            {
                if(Owner.IsPlayer && Owner.Player.PlayerCombatState != null)
                {
                    await PowerCmd.Apply<StrengthPower>(Owner, Owner.GetPowerAmount<PowerFromThePastPower>(), Owner, null);
                    await PowerCmd.Apply<DexterityPower>(Owner, Owner.GetPowerAmount<PowerFromThePastPower>(), Owner, null);
                }
            }
        }
    }
}