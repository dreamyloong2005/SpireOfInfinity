using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Cards;
public class DarkSurge() :
    WodBaseCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new DynamicVar("DarkEnergyGain", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExtraPlayerCombatDataLoader.GainDarkEnergy(Owner.PlayerCombatState, Convert.ToInt32(DynamicVars["DarkEnergyGain"].BaseValue));
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["DarkEnergyGain"].UpgradeValueBy(1m);
    }
}