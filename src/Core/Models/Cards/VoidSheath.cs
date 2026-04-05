using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfInfinity.Core.Models.Cards;

public class VoidSheath() : WodBaseCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 1, Owner);
        CardSelectorPrefs prefs = new(SelectionScreenPrompt, 1);
        CardModel cardModel = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        bool flag = cardModel != null;
        bool flag2 = flag;
        if (flag2)
        {
            bool flag3;
            switch (cardModel.Pile?.Type)
            {
                case PileType.Draw:
                case PileType.Hand:
                    flag3 = true;
                    break;
                default:
                    flag3 = false;
                    break;
            }
            flag2 = flag3;
        }
        if (flag2)
        {
            await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
        }
    }
    protected override void OnUpgrade()
	{
        DynamicVars.Cards.UpgradeValueBy(1);
	}
}