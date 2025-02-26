using UnityEngine;

public class BlockUtility
{
    public static GameObject EnumToPrefab(BlockEnum blockEnum)
    {
        string prefabName = "Prefabs/Blocks/" + blockEnum;
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        
        if(prefab == null)
        {
            Debug.LogError(blockEnum + "프리팹이 없습니다 ㅠㅠ");
            return null;
        }
        
        return prefab; 
    }

    public static BlockColor BlockToColor(BlockEnum blockEnum)
    {
        switch (blockEnum)
        {
            case BlockEnum.IBlock : return BlockColor.LightRed;
            case BlockEnum.LBlock : return BlockColor.LightGreen;
            case BlockEnum.RLBlock : return BlockColor.Blue;
            case BlockEnum.RZBlock : return BlockColor.LightYellow;
            case BlockEnum.SquareBlock : return BlockColor.LightPurple;
            case BlockEnum.TBlock : return BlockColor.SkyBlue;
            case BlockEnum.ZBlock : return BlockColor.LightBlue;
            default : return BlockColor.LightGray;
        }
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
