using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireOfInfinity.Core.Models.Powers;

public class GrievousWoundsPower : WodBasePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}