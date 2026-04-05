using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfInfinity.Core.Models.Powers;
namespace SpireOfInfinity.Core.Models.Cards;

public class Purge() : WodBaseCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
{
    public override int CanonicalDarkEnergyCost => 5;
    public override bool DynamicDarkEnergyCost => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DarkPurgePower>()];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
        if (DarkEnergyCost.CapturedXValue >= 5 || HasDarknessAwakenPower)
        {
            await PowerCmd.Apply<DarkPurgePower>(Owner.Creature, 3m, Owner.Creature, this);
        }else
        {
            await PowerCmd.Apply<PurgePower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}