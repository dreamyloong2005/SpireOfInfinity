using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Localization.DynamicVars;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class DimensionAttack() :
    WodBaseCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
{
    public override int CanonicalDarkEnergyCost => 1;
    public override bool DynamicDarkEnergyCost => true;
    public override DamageType BaseDamageType => DamageType.MaxHpPercent;
    public override DamageType ExtraDamageType => DamageType.MaxHpPercent;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10m),
        new ExtraDamageVar(10m),
        new WodDamageVar(ValueProp.Move | ValueProp.Unblockable)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            bool HasDarkEnergy = DarkEnergyCost.CapturedXValue > 0;
            bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
            if(HasDarkEnergy || HasDarknessAwakenPower)
            {
                await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.CalculationBase.BaseValue * cardPlay.Target.MaxHp / 50, ValueProp.Move | ValueProp.Unblockable, this);
            }else
            {
                await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.CalculationBase.BaseValue * cardPlay.Target.MaxHp / 100, ValueProp.Move | ValueProp.Unblockable, this);
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(5m);
        DynamicVars.ExtraDamage.UpgradeValueBy(5m);
    }
}