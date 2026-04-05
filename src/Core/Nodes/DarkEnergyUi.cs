using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using SpireOfInfinity.Core.Extensions;

namespace SpireOfInfinity.Core.Nodes;

public partial class DarkEnergyUi : Control
{
    private Label _energyValueLabel;        // 用于显示数字的Label
    private TextureRect _iconTextureRect;   // 显示素材图的控件
    private Player _associatedPlayer;

    public void SetAssociatedPlayer(Player player)
    {
        _associatedPlayer = player;
    }

    public override void _Ready()
    {
        // 创建容器（水平排列）
        var hBox = new HBoxContainer
        {
            Alignment = BoxContainer.AlignmentMode.Center, // 子控件居中对齐
            Size = new Vector2(300, 100)
        };
        AddChild(hBox);

        // 创建图标控件
        _iconTextureRect = new TextureRect
        {
            Texture = ResourceLoader.Load<Texture2D>("res://SpireOfInfinity/images/ui/combat/energy_counters/watchman_of_darkness/dark_energy_icon.png"),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            CustomMinimumSize = new Vector2(105, 105)
        };
        hBox.AddChild(_iconTextureRect);

        // 创建数字Label
        _energyValueLabel = new Label
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Size = new Vector2(150, 100),
            Modulate = Colors.White
        };
        _energyValueLabel.AddThemeFontSizeOverride("font_size", 48);
        hBox.AddChild(_energyValueLabel);

        // 设置锚点为左边缘垂直居中
        AnchorLeft = 0;
        AnchorTop = 0.5f;
        AnchorRight = 0;
        AnchorBottom = 0.5f;

        OffsetLeft   = -25f;
        OffsetTop    = -375f;
        OffsetRight  = 320f;
        OffsetBottom = 0f;

        ZIndex = 0;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        if (_associatedPlayer == null)
        {
            _energyValueLabel.Text = "-- / --";
            return;
        }

        var data = ExtraPlayerCombatDataLoader.GetData(_associatedPlayer.PlayerCombatState);
        if (data == null)
        {
            _energyValueLabel.Text = "-- / --";
            return;
        }

        int cur = data.DarkEnergy;
        int max = data.MaxDarkEnergy;
        _energyValueLabel.Text = $"{cur} / {max}";
        // 根据能量值改变数字颜色
        _energyValueLabel.Modulate = cur > 0 ? new Color(1f, 1f, 1f) : new Color(0.8f, 0f, 0f);
    }
}
