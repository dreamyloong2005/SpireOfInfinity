using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireOfInfinity.Core.Models.Cards;
using SpireOfInfinity.Core.Models.CardPools;
using SpireOfInfinity.Core.Models.RelicPools;
using SpireOfInfinity.Core.Models.PotionPools;
using SpireOfInfinity.Core.Models.Relics;

namespace SpireOfInfinity.Core.Models.Characters;

public class WatchmanOfDarkness : PlaceholderCharacterModel
{
    public const string CharacterId = "WatchmanOfDarkness";
    public override string CustomVisualPath => $"res://SpireOfInfinity/scenes/creature_visuals/test.tscn";
    public override string CustomEnergyCounterPath => $"res://SpireOfInfinity/scenes/combat/energy_counters/watchman_of_darkness_energy_counter.tscn";
    public override string CustomIconPath => $"res://SpireOfInfinity/scenes/ui/watchman_of_darkness_icon.tscn";
    public override string CustomCharacterSelectBg => $"res://SpireOfInfinity/scenes/screens/char_select/char_select_watchman_of_darkness.tscn";
    public static readonly Color Color = new("ffffff");

    public const string energyColorName = "watchman_of_darkness";
    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;

    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    public override Color EnergyLabelOutlineColor => new Color("801212FF");

	public override Color DialogueColor => new Color("590700");

	public override Color MapDrawingColor => new Color("CB282B");

	public override Color RemoteTargetingLineColor => new Color("E15847FF");

    public override Color RemoteTargetingLineOutline => new Color("801212FF");
    
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<DarkStrike>(),
        ModelDb.Card<DarkStrike>(),
        ModelDb.Card<DaybreakStrike>(),
        ModelDb.Card<DestructionStrike>(),
        ModelDb.Card<TruthStrike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<DarkHeart>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<WodCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WodRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WodPotionPool>(); 
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => $"res://SpireOfInfinity/images/ui/top_panel/character_icon_watchman_of_darkness.png";
    public override string CustomCharacterSelectIconPath => $"res://SpireOfInfinity/images/packed/character_select/char_select_watchman_of_darkness.png";
    public override string CustomCharacterSelectLockedIconPath => $"res://SpireOfInfinity/images/packed/character_select/char_select_watchman_of_darkness_locked.png";
    public override string CustomMapMarkerPath => $"res://SpireOfInfinity/images/packed/map/icons/map_marker_watchman_of_darkness.png";
}
