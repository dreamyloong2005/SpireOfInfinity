using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;

public class Grudge() : WodBaseCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new ExtraDamageVar(1m),
        new CalculatedVar("CalculatedAmount").WithMultiplier((CardModel card, Creature? _) => card.Owner.PlayerCombatState.GetData().GrudgeCount),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => card.Owner.PlayerCombatState.GetData().GrudgeCount),
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
        if (DynamicVars["CalculatedAmount"] is CalculatedVar CalculatedAmount)
        {
            await PowerCmd.Apply<RevengePower>(cardPlay.Target, CalculatedAmount.Calculate(cardPlay.Target), Owner.Creature, this);
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}