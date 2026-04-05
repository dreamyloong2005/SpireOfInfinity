using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.RelicPools;

namespace SpireOfInfinity.Core.Models.Relics;

[Pool(typeof(WodRelicPool))]
public class DarkHeart : WodBaseRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            ExtraPlayerCombatDataLoader.GainDarkEnergy(Owner.PlayerCombatState, 1);
        }
    }
}
