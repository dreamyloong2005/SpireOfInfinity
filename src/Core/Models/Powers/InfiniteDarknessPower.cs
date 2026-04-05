using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Powers
{
    public class InfiniteDarknessPower : WodBasePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override bool AllowNegative => false;
        public override async Task AfterRemoved(Creature oldOwner)
        {
            await base.AfterRemoved(oldOwner);
            if (oldOwner?.IsPlayer == true && oldOwner.Player?.PlayerCombatState != null)
            {
                var combatState = oldOwner.Player.PlayerCombatState;
                // 检查并修正暗能量，使其不超过最大上限
                combatState.CheckDarkEnergy();
            }
        }
    }
}