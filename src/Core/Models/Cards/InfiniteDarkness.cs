using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class InfiniteDarkness() :
    WodBaseCard(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        Owner.GainPermanentMaxDarkEnergy(1);
        await PowerCmd.Apply<InfiniteDarknessPower>(Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}