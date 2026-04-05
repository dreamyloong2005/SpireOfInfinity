using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SpireOfInfinity.Core.Models.Cards;
public class EmbracedByPowers() :
    WodBaseCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    private CardModel? _mockSelectedCard;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardToAdd;
        IEnumerable<CardModel> cardContents = [];

        foreach (CardModel? c in ModelDb.CardPool<IroncladCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<SilentCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<RegentCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<NecrobinderCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<DefectCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<ColorlessCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare)
            {
                cardContents = cardContents.Append(c);
            }
        }

        foreach (CardModel? c in ModelDb.CardPool<EventCardPool>().AllCards)
        {
            if (c.Rarity == CardRarity.Rare || c.Rarity == CardRarity.Ancient)
            {
                cardContents = cardContents.Append(c);
            }
        }

        List<CardModel> cards = [];
        cards.AddRange(from c in cardContents.TakeRandom(3, Owner.RunState.Rng.CombatCardGeneration)
                       select Owner.Creature.CombatState.CreateCard(c, Owner)
        );
        
        if (_mockSelectedCard == null)
        {
            cardToAdd = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, canSkip: true);
        }
        else
        {
            cardToAdd = _mockSelectedCard;
        }
        if (cardToAdd != null)
        {
            cardToAdd.EnergyCost.SetThisTurnOrUntilPlayed(0);
            await CardPileCmd.AddGeneratedCardToCombat(cardToAdd, PileType.Hand, addedByPlayer: true);
        }
    }
    
    protected override void OnUpgrade()
	{
		RemoveKeyword(CardKeyword.Exhaust);
	}

	public void MockSelectedCard(CardModel card)
	{
		AssertMutable();
		_mockSelectedCard = card;
	}
}