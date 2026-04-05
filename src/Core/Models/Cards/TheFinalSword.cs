using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Localization.DynamicVars;

namespace SpireOfInfinity.Core.Models.Cards;

public class TheFinalSword() :
    WodBaseCard(0, CardType.Attack, CardRarity.None, TargetType.AllEnemies, false)
{
    public override bool HasDarkEnergyCostX => true;
    public override bool DynamicDarkEnergyCost => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(30m),
        new ExtraDamageVar(15m),
        new WodDamageVar(ValueProp.Move)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(6m);
        DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}