using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class BloodyRevenge() :
    WodBaseCard(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RevengePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RevengePower>(3m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RevengePower>(cardPlay.Target, DynamicVars["RevengePower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["RevengePower"].UpgradeValueBy(2m);
    }
}