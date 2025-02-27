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

    public static Vector2 [] BlockToRelativeBlock(BlockEnum blockEnum)
    {
        Vector2[] blocksPos = new Vector2[4]; 
        switch (blockEnum)
        {
            case BlockEnum.IBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(0, 1);
                blocksPos[2] = new Vector2(0, 2);
                blocksPos[3] = new Vector2(0, 3);
                break;
            case BlockEnum.LBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(0, 1);
                blocksPos[2] = new Vector2(0, 2);
                blocksPos[3] = new Vector2(1, 0);
                break;
            case BlockEnum.RLBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(1, 0);
                blocksPos[2] = new Vector2(1, 1);
                blocksPos[3] = new Vector2(2, 1);
                break;
            case BlockEnum.RZBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(0, 1);
                blocksPos[2] = new Vector2(1, 1);
                blocksPos[3] = new Vector2(1, 2);
                break;
            case BlockEnum.SquareBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(1, 0);
                blocksPos[2] = new Vector2(0, 1);
                blocksPos[3] = new Vector2(1, 1);
                break;
            case BlockEnum.TBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(0, 1);
                blocksPos[2] = new Vector2(-1, 1);
                blocksPos[3] = new Vector2(1, 1);
                break;
            case BlockEnum.ZBlock :
                blocksPos[0] = new Vector2(0, 0);
                blocksPos[1] = new Vector2(-1, 0);
                blocksPos[2] = new Vector2(-1, 1);
                blocksPos[3] = new Vector2(-2, 1);
                break;
        }
        return blocksPos;
    }
}
