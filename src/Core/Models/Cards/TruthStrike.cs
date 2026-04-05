using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfInfinity.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
namespace SpireOfInfinity.Core.Models.Cards;

public class TruthStrike() :
    WodBaseCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true)
{
    public override int CanonicalDarkEnergyCost => 1;
    public override bool DynamicDarkEnergyCost => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(6m),
        new WodDamageVar(ValueProp.Move | ValueProp.Unblockable)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.CalculatedDamage.Calculate(cardPlay.Target), ValueProp.Move | ValueProp.Unblockable, this);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}