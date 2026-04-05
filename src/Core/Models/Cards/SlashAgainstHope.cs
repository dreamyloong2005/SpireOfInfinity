using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class SlashAgainstHope() :
    WodBaseCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
{
    public override int CanonicalDarkEnergyCost => 1;
    public override bool DynamicDarkEnergyCost => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<RevengePower>(1m),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RevengePower>()];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool HasDarkEnergy = DarkEnergyCost.CapturedXValue > 0;
        bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .Execute(choiceContext);

        if (HasDarkEnergy || HasDarknessAwakenPower)
        {
            await PowerCmd.Apply<RevengePower>(CombatState.HittableEnemies, DynamicVars["RevengePower"].BaseValue + 1m, Owner.Creature, this);
        }else
        {
            await PowerCmd.Apply<RevengePower>(CombatState.HittableEnemies, DynamicVars["RevengePower"].BaseValue, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["RevengePower"].UpgradeValueBy(1m);
    }
}