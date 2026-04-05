using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Cards;
public class Memories() :
    WodBaseCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(5m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner.GainPermanentMaxDarkEnergy(-1);
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        CardSelectorPrefs prefs = new(SelectionScreenPrompt, 1);
        CardPile pile = PileType.Exhaust.GetPile(Owner);
        CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner, prefs)).FirstOrDefault();
        bool flag = cardModel != null;
        bool flag2 = flag;
        if (flag2)
        {
            bool flag3;
            switch (cardModel.Pile?.Type)
            {
                case PileType.Draw:
                case PileType.Exhaust:
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
            await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Bottom);
        }
    }
    protected override void OnUpgrade()
	{
		DynamicVars.MaxHp.UpgradeValueBy(3m);
	}
}