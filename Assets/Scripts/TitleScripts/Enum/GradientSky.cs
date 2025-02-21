using UnityEngine;

public enum GradientSkyEnum
{
    DarkWhite, LightBlue, SkyBlue, LightRed, 
    DarkRed, DarkBlue, LightBlack, DarkBlack, Black, ColorEnd
}

public class GradientSky
{
    public static Color EnumToColor(GradientSkyEnum enumColor)
    {
        Color color;
        switch (enumColor)
        {
            case GradientSkyEnum.DarkWhite : 
                color = new Color32(214,214,214, 255);
                break;
            case GradientSkyEnum.LightBlue : 
                color = new Color32(74,157,192, 255);
                break;
            case GradientSkyEnum.SkyBlue : 
                color =  new Color32(80,145,255, 255);
                break;
            case GradientSkyEnum.LightRed :
                color = new Color32(255, 131, 131, 255);
                break;
            case GradientSkyEnum.DarkRed :
                color = new Color32(160, 50, 90, 255);
                break;
            case GradientSkyEnum.DarkBlue :
                color = new Color32(40, 70, 170, 255);
                break;
            case GradientSkyEnum.LightBlack :
                color = new Color32(60, 60, 60, 255);
                break;
            case GradientSkyEnum.DarkBlack :
                color = new Color32(30, 30, 30, 255);
                break;
            case GradientSkyEnum.Black :
                color = new Color(0, 0, 0);
                break;
            default : 
                color = Color.magenta.linear;
                break;
        }

        return color;
    }
}