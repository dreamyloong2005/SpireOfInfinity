using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using SpireOfInfinity.Core.Models.RelicPools;
using SpireOfInfinity.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using SpireOfInfinity.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using SpireOfInfinity.Core.Nodes;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using SpireOfInfinity.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.Core.Runs;
using SpireOfInfinity.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Managers;
using SpireOfInfinity.Core.Models.Characters;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
	public const string ModId = "SpireOfInfinity"; //At the moment, this is used only for the Logger and harmony names.

	private static Harmony? _harmony;
	public static void Initialize()
	{
		_harmony = new Harmony(ModId);
		_harmony.PatchAll();
		ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);
		Log.Info("[SpireOfInfinity] 初始化成功！");
	}
}

[HarmonyPatch(typeof(Player), MethodType.Constructor,
[
    typeof(CharacterModel), 
    typeof(ulong), 
    typeof(int), 
    typeof(int), 
    typeof(int), 
    typeof(int), 
    typeof(int), 
    typeof(int), 
    typeof(RelicGrabBag), 
    typeof(UnlockState), 
    typeof(List<ModelId>), 
    typeof(List<ModelId>), 
    typeof(List<string>), 
    typeof(List<ModelId>), 
    typeof(List<ModelId>) 
])]
public static class Player_Ctor_Patch
{
    public static void Postfix(Player __instance)
	{
		Log.Info("[SPIREOFINFINITY] PLAYER_CTOR_PATCH VALID!!!!!!");
        ExtraPlayerDataLoader.Initialize(__instance);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
public static class PlayerCombatState_Ctor_Patch
{
	public static void Postfix(PlayerCombatState __instance, Player player)
	{
		ExtraPlayerCombatDataLoader.Initialize(__instance, player);
		if (player.Character.CardPool == ModelDb.CardPool<WodCardPool>())
		{
			ExtraPlayerCombatDataLoader.UpdateIsWod(__instance, true);
		}
	}
}

[HarmonyPatch(typeof(EnergyIconHelper), "GetPath")]
[HarmonyPatch([typeof(string)])]
public static class EnergyIconHelper_GetPath_Patch
{
	public static bool Prefix(string prefix, ref string __result)
	{
		if (prefix == "watchman_of_darkness")
		{
			__result = $"res://SpireOfInfinity/images/packed/sprite_fonts/watchman_of_darkness_energy_icon.png"; // 例如 "mod://icons/watchman.png"
			return false;
		}
		return true;
	}
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class OnPlayWrapperPatch
{
    public static async void Postfix(CardModel __instance, Task __result)
    {
        await __result;
        Log.Info("[SpireOfInfinity] CARD_MODEL PATCH ACTIVATED");

        var player = __instance.Owner;
        if (player == null) return;

        ExtraPlayerCombatDataLoader.Check(player.PlayerCombatState);
        if (!ExtraPlayerCombatDataLoader.GetData(player.PlayerCombatState).shouldInjectUi) return;

        var sceneTree = Engine.GetMainLoop() as SceneTree;
        if (sceneTree == null) return;

        var root = sceneTree.Root;
        var combatUi = root.FindChild("CombatUi", true, false) as Control;

        if (combatUi != null && combatUi.IsInsideTree())
        {
            var darkEnergyUi = new DarkEnergyUi();
            darkEnergyUi.SetAssociatedPlayer(player);
            combatUi.AddChild(darkEnergyUi);
            ExtraPlayerCombatDataLoader.UpdateUiInjectState(player.PlayerCombatState, true);
            Log.Info($"[SpireOfInfinity] DarkEnergyUI 成功注入到 CombatUi！");
        }
        else
        {
            Log.Info("[SpireOfInfinity] OnPlayWrapper已触发，但未找到 CombatUi。");
        }
    }
}

[HarmonyPatch(typeof(TouchOfOrobas), "GetStarterRelic")]
public static class TouchOfOrobas_GetStarterRelic_Patch
{
	public static bool Prefix(Player p, ref RelicModel __result)
	{
		if (p.Character.RelicPool == ModelDb.RelicPool<WodRelicPool>())
		{
			__result = ModelDb.Relic<DarkHeart>();
			return false;
		}
		else
		{
			return true;
		}
	}
}

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
public static class TouchOfOrobas_GetUpgradedStarterRelic_Patch
{
	public static bool Prefix(RelicModel starterRelic, ref RelicModel __result)
	{
		if (starterRelic == ModelDb.Relic<DarkHeart>())
		{
			__result = ModelDb.Relic<DarkerHeart>();
			return false;
		}
		else
		{
			return true;
		}
	}
}

[HarmonyPatch(typeof(CombatManager), "StartTurn")]
public static class Patch_CombatManager_StartTurn
{
	public static void Postfix(CombatManager __instance)
	{
		// 通过 Traverse 获取私有字段 _state
		var state = Traverse.Create(__instance).Field("_state").GetValue<CombatState>();
		if (state == null) return;
		// 读取所有玩家信息
		foreach (var player in state.Players)
		{
			if (LocalContext.IsMe(player))
			{
				ExtraPlayerCombatDataLoader.Check(player.PlayerCombatState);
				if (!ExtraPlayerCombatDataLoader.GetData(player.PlayerCombatState).shouldInjectUi) return;

				var sceneTree = Engine.GetMainLoop() as SceneTree;
				if (sceneTree == null) return;

				var root = sceneTree.Root;
				var combatUi = root.FindChild("CombatUi", true, false) as Control;

				if (combatUi != null && combatUi.IsInsideTree())
				{
					var darkEnergyUi = new DarkEnergyUi();
					darkEnergyUi.SetAssociatedPlayer(player);
					combatUi.AddChild(darkEnergyUi);
					ExtraPlayerCombatDataLoader.UpdateUiInjectState(player.PlayerCombatState, true);
					Log.Info($"[SpireOfInfinity] DarkEnergyUI 成功注入到 CombatUi！");
				}
				else
				{
					Log.Info("[SpireOfInfinity] StartTurn已触发，但未找到 CombatUi。");
				}
			}
		}
	}
}

public static class DarkEnergyUnplayableReasons
    {
        public const UnplayableReason CurrentDarkEnergyCostTooHigh = (UnplayableReason)0x1000;
        public const UnplayableReason MaxDarkEnergyCostTooHigh = (UnplayableReason)0x2000;
        public const UnplayableReason PermanentMaxDarkEnergyCostTooHigh = (UnplayableReason)0x3000;
    }

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
public static class PlayerCombatState_HasEnoughResourcesFor_Patch
{
    public static void Postfix(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        if (card is not WodBaseCard wodCard)
            return;

        var playerCombatData = __instance.GetData();
        if (playerCombatData == null)
            return;

        // 1. 永久最大黑暗能量检查（实际可消耗上限为当前最大黑暗能量）
        int permanentNeed = wodCard.PermanentMaxDarkEnergyCost.GetAmountToSpend();
        if (permanentNeed > 0 && playerCombatData.MaxDarkEnergy < permanentNeed)
        {
            reason |= DarkEnergyUnplayableReasons.PermanentMaxDarkEnergyCostTooHigh;
            if (__result) __result = false;
        }

        // 2. 最大黑暗能量检查
        int maxNeed = wodCard.MaxDarkEnergyCost.GetAmountToSpend();
        if (maxNeed > 0 && playerCombatData.MaxDarkEnergy < maxNeed)
        {
            reason |= DarkEnergyUnplayableReasons.MaxDarkEnergyCostTooHigh;
            if (__result) __result = false;
        }

        // 3. 当前黑暗能量检查
        int currentNeed = wodCard.DarkEnergyCost.GetAmountToSpend();
        if (currentNeed > 0 && playerCombatData.DarkEnergy < currentNeed && !wodCard.DynamicDarkEnergyCost)
        {
            reason |= DarkEnergyUnplayableReasons.CurrentDarkEnergyCostTooHigh;
            if (__result) __result = false;
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
public static class CardModel_SpendResources_Patch
{
    public static void Postfix(CardModel __instance)
    {
        if (__instance is not WodBaseCard wodCard)
            return;

        var player = wodCard.Owner;
        if (player == null)
            return;

        var playerCombatState = player.PlayerCombatState;
        if (playerCombatState == null)
            return;

        var playerCombatData = playerCombatState.GetData();
        if (playerCombatData == null)
            return;

        // ----- 1. 消耗永久最大黑暗能量（优先消耗临时超出部分）-----
        int permanentCost = wodCard.PermanentMaxDarkEnergyCost.GetAmountToSpend();

        // 如果是 X 费卡牌，记录实际消耗值（此时为需要消耗的总量，包含临时部分）
        wodCard.PermanentMaxDarkEnergyCost.CapturedXValue = player.GetPlayerData()?.PermanentMaxDarkEnergy ?? 0;
        player.LosePermanentMaxDarkEnergy(permanentCost);


        // ----- 2. 消耗最大黑暗能量 -----
        int maxCost = wodCard.MaxDarkEnergyCost.GetAmountToSpend();

        wodCard.MaxDarkEnergyCost.CapturedXValue = playerCombatData.MaxDarkEnergy;
        playerCombatState.GainMaxDarkEnergy(-maxCost);

        // ----- 3. 消耗当前黑暗能量 -----
        int currentCost = wodCard.DarkEnergyCost.GetAmountToSpend();

        wodCard.DarkEnergyCost.CapturedXValue = playerCombatData.DarkEnergy;
        playerCombatState.GainDarkEnergy(-currentCost);
    }
}

[HarmonyPatch(typeof(CardModel), "SelectionScreenPrompt", MethodType.Getter)]
public static class CardModel_SelectionScreenPrompt_Patch
{
	public static bool Prefix(CardModel __instance, ref LocString __result)
	{
		// 检查当前卡牌 ID 是否需要重定向
		if (__instance.Id.Entry == "SPIREOFINFINITY-REGRET_STRIKE" || __instance.Id.Entry == "SPIREOFINFINITY-MEMORIES")
		{
			// 构造目标卡牌的 LocString
			LocString targetLoc = new("cards", "COSMIC_INDIFFERENCE.selectionScreenPrompt");

			// 确保目标字符串存在
			if (targetLoc.Exists())
			{
				// 将当前卡牌的动态变量添加到目标 LocString 中
				__instance.DynamicVars.AddTo(targetLoc);
				__result = targetLoc; // 设置返回值
				return false; // 跳过原始 getter 的执行
			}
			// 如果目标不存在，继续执行原始 getter（可能会抛出异常或返回原字符串）
		}
		// 对于其他卡牌，继续执行原始 getter
		return true;
	}
}

[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
public static class ProgressSaveManager_CheckFifteenElitesDefeatedEpoch_Patch
{
    public static void Prefix(Player localPlayer)
    {
        if (localPlayer.Character is WatchmanOfDarkness)
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
public static class ProgressSaveManager_CheckFifteenBossesDefeatedEpoch_Patch
{
    public static void Prefix(Player localPlayer)
    {
        if (localPlayer.Character is WatchmanOfDarkness)
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
public static class ProgressSaveManager_ObtainCharUnlockEpoch_Patch
{
    public static void Prefix(Player localPlayer)
    {
        if (localPlayer.Character is WatchmanOfDarkness)
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(EpochModel), nameof(EpochModel.Get), [typeof(string)])]
public static class EpochModel_Get_Patch
{
    public static bool Prefix(string id, ref EpochModel __result)
    {
        if (string.IsNullOrEmpty(id))
            return true;

        // 如果id="SPIREOFINFINITY-WATCHMAN_OF_DARKNESS?_EPOCH"，直接跳过原方法
        const string prefix = "SPIREOFINFINITY-WATCHMAN_OF_DARKNESS";
        const string suffix = "_EPOCH";

        if (id.StartsWith(prefix) && id.EndsWith(suffix))
        {
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyHealAmount))]
public static class Hook_ModifyHealAmount_Patch
{
    public static bool Prefix(IRunState runState, CombatState? combatState, Creature creature, decimal amount, ref decimal __result)
    {
        if(creature.HasPower<GrievousWoundsPower>())
        {
            decimal num = amount;
            foreach (AbstractModel item in runState.IterateHookListeners(combatState))
            {
                num = item.ModifyHealAmount(creature, num);
            }
            __result = creature.GetPowerAmount<GrievousWoundsPower>() <= 100 ? (100 - creature.GetPowerAmount<GrievousWoundsPower>()) * num / 100 : 0m;
            return false;
        }else
        {
            return true;
        }
    }
}