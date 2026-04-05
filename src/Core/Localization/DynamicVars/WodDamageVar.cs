using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfInfinity.Core.Models.Cards;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Localization.DynamicVars
{
    public class WodDamageVar : CalculatedDamageVar
    {
        public WodDamageVar() : this(ValueProp.Move) { }

        public WodDamageVar(ValueProp props) : base(props)
        {
            WithMultiplier((card, target) =>
            {
                if (card is WodBaseCard wodCard)
                {
                    if (wodCard.DynamicDarkEnergyCost && !wodCard.HasDarkEnergyCostX)
                    {
                        if (CombatManager.Instance.IsInProgress && card.CombatState != null)
                        {
                            int captured = wodCard.DarkEnergyCost.CapturedXValue;
                            bool HasDarknessAwakenPower = card.Owner.Creature.HasPower<DarknessAwakenPower>();
                            return (captured > 0 || HasDarknessAwakenPower) ? 1m : 0m;
                        }
                        return 0m;
                    }
                    else if (wodCard.HasDarkEnergyCostX)
                    {
                        if (CombatManager.Instance.IsInProgress && card.CombatState != null)
                        {
                            int xValue = wodCard.DarkEnergyCost.CapturedXValue;
                            return xValue;
                        }
                        return 0m;
                    }
                }
                return 0m;
            });
        }

        // ---------- 实际战斗中的数值转换（重写基类）----------
        protected override decimal GetBaseValueForIConvertible()
        {
            if (_owner is not CardModel card) return 0m;

            // 获取当前乘数（复用乘数委托逻辑）
            decimal multiplier = GetMultiplierForCurrentContext(card);

            // 根据伤害类型计算最终伤害
            decimal finalDamage = CalculateFinalDamage(card, null, multiplier, true);

            return finalDamage;
        }

        // ---------- 预览更新（重写基类）----------
        public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
        {
            // 处理附魔（与父类一致）
            EnchantmentModel enchantment = card.Enchantment;
            if (enchantment != null)
            {
                decimal baseValue = GetBaseVar().BaseValue;
                baseValue += enchantment.EnchantDamageAdditive(baseValue, Props);
                baseValue *= enchantment.EnchantDamageMultiplicative(baseValue, Props);
                baseValue = Math.Max(baseValue, 0m);
                if (card.IsEnchantmentPreview)
                {
                    PreviewValue = baseValue;
                }
                else
                {
                    EnchantedValue = baseValue;
                }
            }

            // 预览时使用当前暗能量计算乘数
            decimal multiplier = GetPreviewMultiplier(card, target);
            // 计算预览伤害（基于目标，若 target 为 null，则使用默认目标生命值）
            decimal num = CalculateFinalDamage(card, target, multiplier, runGlobalHooks);

            // 全局钩子或附魔后处理（与父类一致）
            if (runGlobalHooks)
            {
                CombatState combatState = card.CombatState ?? card.Owner.Creature.CombatState;
                PreviewValue = Hook.ModifyDamage(
                    card.Owner.RunState,
                    combatState,
                    target,
                    IsFromOsty ? card.Owner.Osty : card.Owner.Creature,
                    num,
                    Props,
                    card,
                    ModifyDamageHookType.All,
                    previewMode,
                    out IEnumerable<AbstractModel> _
                );
            }
            else if (!card.IsEnchantmentPreview)
            {
                if (enchantment != null)
                {
                    num += enchantment.EnchantDamageAdditive(num, Props);
                    num *= enchantment.EnchantDamageMultiplicative(num, Props);
                }
                PreviewValue = num;
            }

            PreviewValue = Math.Max(PreviewValue, 0m);
        }

        private decimal GetMultiplierForCurrentContext(CardModel card)
        {
            if (card is not WodBaseCard wodCard) return 0m;

            if (wodCard.DynamicDarkEnergyCost && !wodCard.HasDarkEnergyCostX)
            {
                int captured = wodCard.ResolveDarkEnergyXValue();
                bool HasDarknessAwakenPower = card.Owner.Creature.HasPower<DarknessAwakenPower>();
                return (captured > 0 || HasDarknessAwakenPower) ? 1m : 0m;
            }
            else if (wodCard.HasDarkEnergyCostX)
            {
                return wodCard.ResolveDarkEnergyXValue();
            }
            return 0m;
        }

        private decimal GetPreviewMultiplier(CardModel card, Creature? target)
        {
            if (card is not WodBaseCard wodCard) return 0m;

            var darkEnergy = card.Owner?.PlayerCombatState?.GetData()?.DarkEnergy ?? 0;
            bool HasDarknessAwakenPower = card.Owner.Creature.HasPower<DarknessAwakenPower>();

            if (wodCard.DynamicDarkEnergyCost && !wodCard.HasDarkEnergyCostX)
            {
                return (darkEnergy > 0 || HasDarknessAwakenPower) ? 1m : 0m;
            }
            else if (wodCard.HasDarkEnergyCostX)
            {
                return darkEnergy;
            }
            return 0m;
        }

        private decimal CalculateFinalDamage(CardModel card, Creature? target, decimal multiplier, bool applyGlobalHooks)
        {
            if (card is not WodBaseCard wodCard) return 0m;

            // 获取原始数值（百分比或固定值）
            decimal baseRaw = GetBaseVar().BaseValue;
            decimal extraRaw = GetExtraVar().BaseValue;

            // 根据类型将原始数值转换为实际伤害
            decimal baseDamage = ConvertRawDamage(baseRaw, target, wodCard.BaseDamageType);
            decimal extraDamage = ConvertRawDamage(extraRaw, target, wodCard.ExtraDamageType);

            // 最终伤害 = 基础伤害 + 额外伤害 × 乘数
            decimal rawDamage = baseDamage + extraDamage * multiplier;

            // 附魔处理（仅当调用者要求应用全局钩子时，预览时可能已在外层处理）
            if (applyGlobalHooks)
            {
                EnchantmentModel enchantment = card.Enchantment;
                if (enchantment != null)
                {
                    rawDamage += enchantment.EnchantDamageAdditive(rawDamage, Props);
                    rawDamage *= enchantment.EnchantDamageMultiplicative(rawDamage, Props);
                }
            }

            return rawDamage;
        }

        /// 根据伤害类型将原始数值转换为实际伤害值
        private decimal ConvertRawDamage(decimal rawValue, Creature? target, DamageType type)
        {
            if (target == null)
            {
                // 预览时无目标，返回原始数值（固定值）或基于默认生命值（如 100）的百分比
                if (type == DamageType.Fixed) return rawValue;
                // 对于百分比，可返回 0 或基于假想目标（如 100）的数值，这里简单返回 0
                return 0m;
            }

            switch (type)
            {
                case DamageType.Fixed:
                    return rawValue;
                case DamageType.MaxHpPercent:
                    return target.MaxHp * rawValue / 100m;
                case DamageType.CurrentHpPercent:
                    return target.CurrentHp * rawValue / 100m;
                case DamageType.LostHpPercent:
                    return (target.MaxHp - target.CurrentHp) * rawValue / 100m;
                default:
                    return rawValue;
            }
        }
    }
}
