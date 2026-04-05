using BaseLib.Abstracts;
using Godot;
using SpireOfInfinity.Core.Models.Characters;

namespace SpireOfInfinity.Core.Models.RelicPools;

public class WodRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => WatchmanOfDarkness.CharacterId;
    public override Color LabOutlineColor => WatchmanOfDarkness.Color;
}
