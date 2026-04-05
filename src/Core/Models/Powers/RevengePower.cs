using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Cards;
using SpireOfInfinity.Core.Models.Characters;

namespace SpireOfInfinity.Core.Models.Powers
{
    public class RevengePower : WodBasePower
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override bool AllowNegative => false;
        public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target == Owner)
            {
                if(dealer.IsPlayer)
                {
                    if(dealer.Player.Character is WatchmanOfDarkness)
                    {
                        dealer.Player.PlayerCombatState.GainEnergy(1);
                        dealer.Player.PlayerCombatState.GainDarkEnergy(1);
                        await PowerCmd.TickDownDuration(this);
                    }
                }
            }
	    }
    }
}