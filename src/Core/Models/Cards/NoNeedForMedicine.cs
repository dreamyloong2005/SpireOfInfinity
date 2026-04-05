using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;
public class NoNeedForMedicine() :
    WodBaseCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DarkEnergyGain", 2m), new DynamicVar("MaxDarkEnergyGain", 1m), new PowerVar<StrengthPower>(3m), new HpLossVar(6m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<GrievousWoundsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, new DamageVar(DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move), this);
        await PowerCmd.Apply<GrievousWoundsPower>(Owner.Creature, 10m, Owner.Creature, this);
        NPowerUpVfx.CreateNormal(Owner.Creature);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
        ExtraPlayerCombatDataLoader.GainMaxDarkEnergy(Owner.PlayerCombatState, Convert.ToInt32(DynamicVars["MaxDarkEnergyGain"].BaseValue));
        ExtraPlayerCombatDataLoader.GainDarkEnergy(Owner.PlayerCombatState, Convert.ToInt32(DynamicVars["DarkEnergyGain"].BaseValue));
    }
    protected override void OnUpgrade()
	{
        DynamicVars["StrengthPower"].UpgradeValueBy(2m);
        DynamicVars["DarkEnergyGain"].UpgradeValueBy(1m);
        DynamicVars["MaxDarkEnergyGain"].UpgradeValueBy(1m);
	}
}