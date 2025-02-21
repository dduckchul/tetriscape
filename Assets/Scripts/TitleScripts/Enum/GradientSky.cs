using UnityEngine;

public enum GradientSkyEnum
{
    DarkWhite, LightBlue, SkyBlue, LightRed, 
    DarkRed, DarkBlue, LightBlack, DarkBlack, Black, ColorEnd
}

public enum GradientSkyProp
{
    TopColor, MiddleColor, BottomColor
}

public class GradientSky
{
    public static Color EnumToColor(GradientSkyEnum enumColor)
    {
        Color color;
        switch (enumColor)
        {
            case GradientSkyEnum.DarkWhite : 
                color = new Color(214,214,214);
                break;
            case GradientSkyEnum.LightBlue : 
                color = new Color(74,157,192);
                break;
            case GradientSkyEnum.SkyBlue : 
                color =  new Color(80,145,255);
                break;
            case GradientSkyEnum.LightRed :
                color = new Color(255, 131, 131);
                break;
            case GradientSkyEnum.DarkRed :
                color = new Color(160, 50, 90);
                break;
            case GradientSkyEnum.DarkBlue :
                color = new Color(40, 70, 170);
                break;
            case GradientSkyEnum.LightBlack :
                color = new Color(60, 60, 60);
                break;
            case GradientSkyEnum.DarkBlack :
                color = new Color(30, 30, 30);
                break;
            case GradientSkyEnum.Black :
                color = new Color(0, 0, 0);
                break;
            default : 
                Debug.LogError("Color not found");
                color = Color.magenta.linear;
                break;
        }

        return color;
    }
}