using SpireOfInfinity.Core.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;

public class DarkStrike() :
    WodBaseCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true)
{
    public override int CanonicalDarkEnergyCost => 1;
    public override bool DynamicDarkEnergyCost => true;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new HealVar(3m)
    ];

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target != null)
        {
            bool HasDarkEnergy = DarkEnergyCost.CapturedXValue > 0;
            bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
            if(HasDarkEnergy || HasDarknessAwakenPower)
            {
                await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Heal.UpgradeValueBy(3m);
    }
}