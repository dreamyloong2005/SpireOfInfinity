using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SpireOfInfinity.Core.Extensions;

public static class ExtraPlayerDataLoader
{
    public class ExtraPlayerData
    {
        public int PermanentMaxDarkEnergy { get; set; }

        public ExtraPlayerData()
        {
            PermanentMaxDarkEnergy = 5;
        }
    }

    private static ConditionalWeakTable<Player, ExtraPlayerData> _extensions = new ConditionalWeakTable<Player, ExtraPlayerData>();

    public static ExtraPlayerData GetPlayerData(this Player player)
    {
        if (player == null) return null;
        _extensions.TryGetValue(player, out var data);
        return data;
    }

    public static void SetPermanentMaxDarkEnergy(this Player player, int newValue)
{
    if (player == null) return;
    if (_extensions.TryGetValue(player, out var data))
    {
        int oldValue = data.PermanentMaxDarkEnergy;
        int clampedNew = Math.Max(newValue, 0);
        data.PermanentMaxDarkEnergy = clampedNew;

        // 如果玩家处于战斗中，同步调整战斗状态的最大暗能量（增量）
        if (player.PlayerCombatState != null)
        {
            int delta = clampedNew - oldValue;
            if (delta != 0)
            {
                ExtraPlayerCombatDataLoader.GainMaxDarkEnergy(player.PlayerCombatState, delta);
            }
        }
    }
}

    public static void GainPermanentMaxDarkEnergy(this Player player, int delta)
    {
        if (player == null) return;
        if (_extensions.TryGetValue(player, out var data))
        {
            int oldValue = data.PermanentMaxDarkEnergy;
            int newValue = oldValue + delta;
            int clampedNew = Math.Max(newValue, 0);
            data.PermanentMaxDarkEnergy = clampedNew;

            if (player.PlayerCombatState != null)
            {
                int actualDelta = clampedNew - oldValue;
                if (actualDelta != 0)
                {
                    ExtraPlayerCombatDataLoader.GainMaxDarkEnergy(player.PlayerCombatState, actualDelta);
                }
            }
        }
    }

    public static void LosePermanentMaxDarkEnergy(this Player player, int delta)
    {
        if (player == null) return;
        if (delta <= 0) return; // 忽略非正减少

        if (_extensions.TryGetValue(player, out var data))
        {
            int oldPer = data.PermanentMaxDarkEnergy;
            int playerCombatMax = oldPer; // 默认假设不在战斗中
            bool inCombat = player.PlayerCombatState != null;
            var playerCombatData = inCombat ? player.PlayerCombatState.GetData() : null;
            if (inCombat && playerCombatData != null)
            {
                playerCombatMax = playerCombatData.MaxDarkEnergy;
            }

            // 计算临时超出部分（局内临时增益）
            int extra = Math.Max(0, playerCombatMax - oldPer);

            // 需要从默认值中扣除的量（可能超过旧默认值，后续会钳位）
            int defDelta = Math.Max(0, delta - extra);
            int newPer = Math.Max(0, oldPer - defDelta);
            int defReduced = oldPer - newPer; // 实际减少的默认值

            int extraUsed = Math.Min(extra, delta); // 实际消耗的临时部分
            int totalReduction = extraUsed + defReduced; // 总共减少的战斗最大黑暗能量

            // 更新
            data.PermanentMaxDarkEnergy = newPer;

            // 如果在战斗中，同步减少战斗中的最大黑暗能量
            if (inCombat && totalReduction > 0 && player.PlayerCombatState != null)
            {
                ExtraPlayerCombatDataLoader.GainMaxDarkEnergy(player.PlayerCombatState, -totalReduction);
            }
        }
    }

    internal static void Initialize(Player player)
    {
        if (player == null) return;
        if (_extensions.TryGetValue(player, out _)) return;
        _extensions.Add(player, new ExtraPlayerData());
    }
}