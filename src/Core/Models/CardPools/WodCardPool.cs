using BaseLib.Abstracts;
using Godot;
using SpireOfInfinity.Core.Models.Characters;

namespace SpireOfInfinity.Core.Models.CardPools;

public class WodCardPool : CustomCardPoolModel
{
    public override string Title => WatchmanOfDarkness.CharacterId; //This is not a display name.
    public override string EnergyColorName => WatchmanOfDarkness.energyColorName;

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0f; //Hue; changes the color.
    public override float S => 0f; //Saturation
    public override float V => 0.3f; //Brightness
    
    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");
    
    public override bool IsColorless => false;
}
