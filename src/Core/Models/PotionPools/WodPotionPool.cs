using BaseLib.Abstracts;
using Godot;
using SpireOfInfinity.Core.Models.Characters;

namespace SpireOfInfinity.Core.Models.PotionPools;

public class WodPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => WatchmanOfDarkness.CharacterId;
    public override Color LabOutlineColor => WatchmanOfDarkness.Color;
}
