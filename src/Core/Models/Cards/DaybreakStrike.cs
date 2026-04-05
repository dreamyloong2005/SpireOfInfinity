using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfInfinity.Core.Models.Cards;

public class DaybreakStrike() :
    WodBaseCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true)
{
    public override bool HasDarkEnergyCostX => true;
    public override bool DynamicDarkEnergyCost => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target != null)
        {
            int num = ResolveDarkEnergyXValue();
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .WithHitCount(1+num)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}