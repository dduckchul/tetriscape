using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IMoveable, IRotatable, IColorable
{
    // 생성할때 세팅해주는 값들
    private BlockEnum _blockType;
    private BlockColor _blockColor;
    
    // 가변적으로 변할수 있는 값들
    private bool _isCurrent;
    private bool _isFixed;
    private float _fallSpeed;
    
    public Material blockMaterial;
    
    public BlockEnum BlockType => _blockType;
    public BlockColor BlockColor => _blockColor;
    
    Coroutine _fallCoroutine;

    public bool IsCurrent
    {
        get => _isCurrent;
        set => _isCurrent = value;
    }

    // 초기 블록 세팅, 1. 위치, 2. 색상, 3. 떨어지는 속도
    public void InitBlock(BlockEnum blockEnum, BlockColor blockColor, Vector3 pos)
    {
        _blockType = blockEnum;
        _blockColor = blockColor;
        transform.position = pos;
    }
    
    public void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x, dir.y, 0);
    }

    public void StartBlock(float fallSpeed)
    {
        _fallSpeed = fallSpeed;
        _fallCoroutine = StartCoroutine(Fall());
    }

    public void StopBlock()
    {
        StopCoroutine(_fallCoroutine);
    }
    
    public IEnumerator Fall()
    {
        while (_isCurrent)
        {
            transform.position += new Vector3(0, -_fallSpeed, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    // 돌려도 의미없는 네모 블럭은 예외처리
    public void Rotate()
    {
        if (BlockType == BlockEnum.SquareBlock)
        {
            Debug.Log(BlockType);
            return;
        }
        
        StopBlock();
        transform.Rotate(Vector3.forward, 90f);
        StartBlock(_fallSpeed);
    }

    // To-Do : 2D - 3D 간 색상 변경시 블록 색상 변경
    public void ChangeColor()
    {

    }
}