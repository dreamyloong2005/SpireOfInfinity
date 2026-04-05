using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Localization.DynamicVars;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class Execute() :
    WodBaseCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
{
    public override bool HasDarkEnergyCostX => true;
    public override bool DynamicDarkEnergyCost => true;
    public override DamageType ExtraDamageType => DamageType.LostHpPercent;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RevengePower>(), HoverTipFactory.FromPower<GrievousWoundsPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10m),
        new ExtraDamageVar(10m),
        new WodDamageVar(ValueProp.Move),
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool HasDarkEnergy = DarkEnergyCost.CapturedXValue > 0;
        bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
        int MaxHp = cardPlay.Target.MaxHp;
        int CurrentHp = cardPlay.Target.CurrentHp;
        int LostHp = MaxHp - CurrentHp;
        if(HasDarkEnergy || HasDarknessAwakenPower)
        {
            await DamageCmd.Attack(LostHp * (DynamicVars.CalculationBase.BaseValue+DynamicVars.ExtraDamage.BaseValue) / 100)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
            await PowerCmd.Apply<RevengePower>(cardPlay.Target, 1m, Owner.Creature, this);
            await PowerCmd.Apply<GrievousWoundsPower>(cardPlay.Target, 30m, Owner.Creature, this);

            if (cardPlay.Target.CurrentHp <= DarkEnergyCost.CapturedXValue * cardPlay.Target.GetPowerAmount<RevengePower>())
            {
                await CreatureCmd.Kill(cardPlay.Target);
            }
        
        }else
        {
            await DamageCmd.Attack(LostHp * DynamicVars.CalculationBase.BaseValue / 100)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
            await PowerCmd.Apply<RevengePower>(cardPlay.Target, 1m, Owner.Creature, this);
            await PowerCmd.Apply<GrievousWoundsPower>(cardPlay.Target, 30m, Owner.Creature, this);

            if (cardPlay.Target.CurrentHp <= cardPlay.Target.GetPowerAmount<RevengePower>())
            {
                await CreatureCmd.Kill(cardPlay.Target);
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(5m);
        DynamicVars.ExtraDamage.UpgradeValueBy(5m);
    }
}