using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfInfinity.Core.Models.Cards;
public class RegretStrike() :
    WodBaseCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
        CardSelectorPrefs prefs = new(SelectionScreenPrompt, 1);
        CardPile pile = PileType.Discard.GetPile(Owner);
        CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner, prefs)).FirstOrDefault();
        bool flag = cardModel != null;
        bool flag2 = flag;
        if (flag2)
        {
            bool flag3;
            switch (cardModel.Pile?.Type)
            {
                case PileType.Draw:
                case PileType.Discard:
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
		DynamicVars.Damage.UpgradeValueBy(2m);
	}
}