using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Characters;

namespace SpireOfInfinity.Core.Models.Powers
{
    public class RevengePower : WodBasePower
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override bool AllowNegative => false;
        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target == Owner)
            {
                if(dealer.IsPlayer)
                {
                    if(dealer.Player.Character is WatchmanOfDarkness)
                    {
                        dealer.Player.PlayerCombatState.GainEnergy(1);
                        dealer.Player.PlayerCombatState.GainDarkEnergy(1);
                        PowerCmd.TickDownDuration(this); // No need to use await since the animation should be played while the damage is being dealed
                    }
                }
            }
	    }
    }
}
