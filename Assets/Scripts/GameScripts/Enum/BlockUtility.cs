using UnityEngine;

public class BlockUtility
{
    public static string EnumToPrefab(BlockEnum blockEnum)
    {
        string prefabName;
        switch (blockEnum)
        {
            case BlockEnum.IBlock :
                prefabName = "Prefabs/IBlock";
                break;
            case BlockEnum.LBlock :
                prefabName = "Prefabs/LBlock";
                break;
            case BlockEnum.RLBlock :
                prefabName = "Prefabs/RLBlock";
                break;
            case BlockEnum.RZBlock:
                prefabName = "Prefabs/SBlock";
                break;
            case BlockEnum.SqaureBlock :
                prefabName = "Prefabs/SquareBlock";
                break;                
            case BlockEnum.TBlock :
                prefabName = "Prefabs/TBlock";
                break;
            case BlockEnum.ZBlock :
                prefabName = "Prefabs/ZBlock";
                break;
            default : 
                Debug.Log("Error : BlockEnum is not valid");
                prefabName = "";
                break;
        }
        return prefabName;
    } 
    
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
            case BlockColor.Blue :
                color = Color.blue;
                break;
            case BlockColor.LightYellow :
                color = Color.yellow;
                break;
            case BlockColor.LightPurple :
                color = Color.magenta;
                break;
            case BlockColor.SkyBlue :
                color = Color.cyan;
                break;
            case BlockColor.LightBlue :
                color = new Color(0, 0.2f, 0.8f, 1);
                break;
            default : 
                color = new Color32(100, 100, 100, 255);
                break;
        }
        return color;        
    }
}
