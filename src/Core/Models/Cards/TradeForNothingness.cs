using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class TradeForNothingness() :
    WodBaseCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    public override bool HasDarkEnergyCostX => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = ResolveDarkEnergyXValue();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<IntangiblePower>(Owner.Creature, DynamicVars["IntangiblePower"].BaseValue * num, Owner.Creature, this);
    }
    protected override void OnUpgrade()
	{
        RemoveKeyword(CardKeyword.Ethereal);
	}
}