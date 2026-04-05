using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfInfinity.Core.Models.Cards;

public class TheFinalForge() :
    WodBaseCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    public override int CanonicalDarkEnergyCost => 5;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<TheFinalSword>(IsUpgraded)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DarkEnergyCost", 5m), new MaxHpVar(10m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel sword = CombatState.CreateCard<TheFinalSword>(Owner);
        await CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, DynamicVars.MaxHp.BaseValue, isFromCard: true);
        await CardPileCmd.AddGeneratedCardToCombat(sword, PileType.Hand, addedByPlayer: true);
        if (!IsUpgraded)
        {
            return;
        }
        else
        {
            CardCmd.Upgrade(sword);
        }
    }
}