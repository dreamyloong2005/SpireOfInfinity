using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class DarknessAwaken() :
    WodBaseCard(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<DarknessAwakenPower>(Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}