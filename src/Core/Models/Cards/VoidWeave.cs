using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfInfinity.Core.Models.Cards;

public class VoidWeave() : WodBaseCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new BlockVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardSelectorPrefs prefs = new(SelectionScreenPrompt, Convert.ToInt32(DynamicVars.Cards.BaseValue));
        List<CardModel> cardModels = [.. await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)];
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        foreach(var cardModel in cardModels)
        {
            if(cardModel != null)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}