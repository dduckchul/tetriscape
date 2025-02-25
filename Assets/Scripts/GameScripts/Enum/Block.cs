using UnityEngine;

public enum BlockEnum
{
    
}

public enum UnusualBlockEnum
{
    
}

// Light Gray 색은 일반적으로 안쓰도록. (디폴트랑 색상 같음)
public enum BlockColor
{
    LightRed, LightGreen, LightBlue, LightYellow, 
    LightPurple, LightSkyBlue, LightGray
}

public class Block {

    public static Color EnumToColor(BlockColor enumColor)
    {
        Color color;
        switch (enumColor)
        {
            case BlockColor.LightRed :
                color = Color.red;
                break;
            case BlockColor.LightGreen :
                color = Color.green;
                break;
            case BlockColor.LightBlue :
                color = Color.blue;
                break;
            case BlockColor.LightYellow :
                color = Color.yellow;
                break;
            case BlockColor.LightPurple :
                color = Color.magenta;
                break;
            case BlockColor.LightSkyBlue :
                color = Color.cyan;
                break;
            default : 
                color = new Color32(100, 100, 100, 255);
                break;
        }

        return color;        
    }
}