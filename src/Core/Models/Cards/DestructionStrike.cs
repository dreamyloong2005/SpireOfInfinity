using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Cards;

public class DestructionStrike() :
    WodBaseCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true)
{
    public override int CanonicalDarkEnergyCost => 1;
    public override bool DynamicDarkEnergyCost => true;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RevengePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
    ];
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
                await PowerCmd.Apply<RevengePower>(cardPlay.Target, 2m, Owner.Creature, this);
            }else
            {
                await PowerCmd.Apply<RevengePower>(cardPlay.Target, 1m, Owner.Creature, this);
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}