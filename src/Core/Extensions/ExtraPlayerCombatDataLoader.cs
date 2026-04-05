using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Extensions;

public static class ExtraPlayerCombatDataLoader
{
    public class ExtraPlayerCombatData
    {
        public int DarkEnergy;
        public int MaxDarkEnergy;
        public bool isWod;
        public bool uiInjected;
        public bool shouldInjectUi;
        public Player Player;
        public int GrudgeCount = 0;
        public ExtraPlayerCombatData()
        {
            DarkEnergy = 0;
            MaxDarkEnergy = 5;
            isWod = false;
            uiInjected = false;
            shouldInjectUi = false;
            GrudgeCount = 0;
        }
    }

    public static ConditionalWeakTable<PlayerCombatState, ExtraPlayerCombatData> _extensions = new ConditionalWeakTable<PlayerCombatState, ExtraPlayerCombatData>();

    public static ExtraPlayerCombatData GetData(this PlayerCombatState state)
    {
        if (state == null) return null;
        _extensions.TryGetValue(state, out var data);
        return data;
    }

    public static void SetDarkEnergy(this PlayerCombatState state, int newDarkEnergy)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            if (data.Player.Creature.HasPower<InfiniteDarknessPower>())
            {
                data.DarkEnergy = Math.Max(newDarkEnergy, 0);
            }
            else
            {
                data.DarkEnergy = Math.Max(Math.Min(newDarkEnergy, data.MaxDarkEnergy), 0);
            }
        }
    }

    public static void GainDarkEnergy(this PlayerCombatState state, int deltaDarkEnergy)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            if (data.Player.Creature.HasPower<InfiniteDarknessPower>())
            {
                data.DarkEnergy = Math.Max(data.DarkEnergy + deltaDarkEnergy, 0);
            }
            else
            {
                data.DarkEnergy = Math.Max(Math.Min(data.DarkEnergy + deltaDarkEnergy, data.MaxDarkEnergy), 0);
            }
        }
    }

    public static void SetMaxDarkEnergy(this PlayerCombatState state, int newMaxDarkEnergy)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            data.MaxDarkEnergy = Math.Max(newMaxDarkEnergy, 0);

            // 如果新的最大值变小，且没有无限能力，且当前暗能量超过新最大值，则削减
            if (!data.Player.Creature.HasPower<InfiniteDarknessPower>() && data.DarkEnergy > data.MaxDarkEnergy)
            {
                data.DarkEnergy = data.MaxDarkEnergy;
            }
        }
    }

    public static void GainMaxDarkEnergy(this PlayerCombatState state, int deltaMaxDarkEnergy)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            data.MaxDarkEnergy = Math.Max(data.MaxDarkEnergy + deltaMaxDarkEnergy, 0);

            // 如果最终的最大值变小（delta为负），且没有无限能力，且当前暗能量超过新最大值，则削减
            if (!data.Player.Creature.HasPower<InfiniteDarknessPower>() && data.DarkEnergy > data.MaxDarkEnergy)
            {
                data.DarkEnergy = data.MaxDarkEnergy;
            }
        }
    }

    public static void UpdateIsWod(this PlayerCombatState state, bool newIsWod)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            data.isWod = newIsWod;
        }
    }
    
    public static void UpdateUiInjectState(this PlayerCombatState state, bool newUiInjected)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            data.uiInjected = newUiInjected;
        }
    }

    public static void CheckShoudInjectUi(this PlayerCombatState state)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            if (data.DarkEnergy > 0 || data.isWod)
            {
                data.shouldInjectUi = true;
            }
            if (data.uiInjected)
            {
                data.shouldInjectUi = false;
            }
            if (!LocalContext.IsMe(data.Player))
            {
                data.shouldInjectUi = false;
            }
        }
    }

    internal static void Initialize(PlayerCombatState state, Player player)
    {
        if (_extensions.TryGetValue(state, out _)) return;

        var data = new ExtraPlayerCombatData()
        {
            Player = player
        };
        var playerData = player.GetPlayerData();
        if (playerData != null)
        {
            data.MaxDarkEnergy = playerData.PermanentMaxDarkEnergy;
        }
        _extensions.Add(state, data);
    }

    public static void CheckDarkEnergy(this PlayerCombatState state)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            // 只有没有无限能力时，才进行限制
            if (!data.Player.Creature.HasPower<InfiniteDarknessPower>() && data.DarkEnergy > data.MaxDarkEnergy)
            {
                data.DarkEnergy = data.MaxDarkEnergy;
            }
        }
    }
    public static void GainGrudgeCount(this PlayerCombatState state, int num)
    {
        if (state == null) return;
        if (_extensions.TryGetValue(state, out var data))
        {
            data.GrudgeCount += num;
        }
    }
    public static void SetGrudgeCount(this PlayerCombatState state, int num)
    {
        if (state == null) return;
        if(_extensions.TryGetValue(state, out var data))
        {
            data.GrudgeCount = num;
        }
    }
}
