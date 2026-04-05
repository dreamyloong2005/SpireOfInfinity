using BaseLib.Abstracts;
using BaseLib.Extensions;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Models.Powers;

public abstract class WodBasePower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
