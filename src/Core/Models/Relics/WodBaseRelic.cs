using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireOfInfinity.Core.Models.RelicPools;

namespace SpireOfInfinity.Core.Models.Relics;

[Pool(typeof(WodRelicPool))]
public abstract class WodBaseRelic : CustomRelicModel
{
    // 小图标
    public override string PackedIconPath => $"res://SpireOfInfinity/images/relics/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://SpireOfInfinity/images/relics/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://SpireOfInfinity/images/relics/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
}
