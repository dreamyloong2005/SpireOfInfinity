using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Entities.Cards;
using SpireOfInfinity.Core.Models.CardPools;

namespace SpireOfInfinity.Core.Models.Cards;

public enum DamageType
{
    Fixed,          // 固定数值
    MaxHpPercent,   // 目标最大生命值百分比
    CurrentHpPercent, // 目标当前生命值百分比
    LostHpPercent  // 目标已损失生命值百分比（易损生命值）
}

[Pool(typeof(WodCardPool))]
public abstract class WodBaseCard(int cost, CardType type, CardRarity rarity, TargetType target, bool ShouldShowInCardLibrary) :
    CustomCardModel(cost, type, rarity, target, ShouldShowInCardLibrary)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"res://SpireOfInfinity/images/packed/card_portraits/wod/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/wod/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"res://SpireOfInfinity/images/packed/card_portraits/wod/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string BetaPortraitPath => $"res://SpireOfInfinity/images/packed/card_portraits/wod/beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    private DarkEnergyCost? _darkEnergyCost;

    public virtual int CanonicalDarkEnergyCost => 0;
    public virtual bool HasDarkEnergyCostX => false;

    public virtual bool DynamicDarkEnergyCost => false;

    public virtual DamageType BaseDamageType => DamageType.Fixed;
    public virtual DamageType ExtraDamageType => DamageType.Fixed;

    public DarkEnergyCost DarkEnergyCost
    {
        get
        {
            if (_darkEnergyCost == null)
            {
                _darkEnergyCost = new DarkEnergyCost(this, CanonicalDarkEnergyCost, HasDarkEnergyCostX, DynamicDarkEnergyCost);
            }
            return _darkEnergyCost;
        }
    }

    /// 当黑暗能量成本发生变化时触发此事件。
    public event Action? DarkEnergyCostChanged;

    /// 触发黑暗能量成本变化事件（由 DarkEnergyCost 内部调用）。
    public void InvokeDarkEnergyCostChanged() => DarkEnergyCostChanged?.Invoke();

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields(); // 假设基类已调用基实现

        // 克隆黑暗能量成本
        _darkEnergyCost = _darkEnergyCost?.Clone(this);
    }


    protected override void AfterCloned()
    {
        base.AfterCloned(); // 假设基类已调用基实现

        // 重置事件，避免克隆后的实例残留旧监听器
        DarkEnergyCostChanged = null;
    }

    /// 解析当前黑暗能量的 X 值（仅当卡牌为黑暗能量 X 费时有效）。
    public int ResolveDarkEnergyXValue()
    {
        if (!HasDarkEnergyCostX)
            throw new InvalidOperationException("This card does not have a Dark Energy X-cost.");
        return Hook.ModifyXValue(CombatState, this, DarkEnergyCost.CapturedXValue);
    }
}