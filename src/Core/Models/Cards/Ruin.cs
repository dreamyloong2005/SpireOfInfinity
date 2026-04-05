using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Models.Cards;

public class Ruin() : WodBaseCard(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
{
    public override int CanonicalDarkEnergyCost => 5;
    public override bool DynamicDarkEnergyCost => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DarkRuinPower>()];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            bool HasDarknessAwakenPower = Owner.Creature.HasPower<DarknessAwakenPower>();
            await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);
            if(DarkEnergyCost.CapturedXValue >= 5 || HasDarknessAwakenPower)
            {
                await PowerCmd.Apply<DarkRuinPower>(cardPlay.Target, 3m, Owner.Creature, this);
            }else
            {
                await PowerCmd.Apply<RuinPower>(cardPlay.Target, 1m, Owner.Creature, this);   
            }
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}