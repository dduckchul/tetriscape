using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour, IMoveable, IRotatable, IColorable
{
    private BlockEnum _blockType;
    private BlockColor _blockColor;
    private string _prefabPath;
    private bool _isActive;
    
    public BlockEnum BlockType => _blockType;
    public BlockColor BlockColor => _blockColor;
    public string PrefabPath => _prefabPath;

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public void InitBlock(Vector3 pos)
    {
        
    }
    
    public void Move()
    {

    }
    
    public IEnumerator Fall()
    {
        yield return new WaitForFixedUpdate();
    }    

    public void Rotate()
    {
        // 돌려도 의미없는 네모 블럭 하나만 예외처리
        if (BlockType == BlockEnum.SqaureBlock)
        {
            return;
        }
    }

    // To-Do : 2D - 3D 간 색상 변경시 블록 색상 변경
    public void ChangeColor()
    {

    }
}