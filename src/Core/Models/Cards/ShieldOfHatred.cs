using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class ShieldOfHatred() :
    WodBaseCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RevengePower>(1m),
        new BlockVar(5m, ValueProp.Move)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<RevengePower>(CombatState.HittableEnemies, DynamicVars["RevengePower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["RevengePower"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}