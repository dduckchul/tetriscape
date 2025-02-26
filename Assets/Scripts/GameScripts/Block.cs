using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IMoveable, IRotatable, IColorable
{
    // 생성할때 세팅해주는 값들
    private readonly float _lastRotateTime = 0.5f;
    private BlockEnum _blockType;
    private BlockColor _blockColor;

    // 가변적으로 변할수 있는 값들
    public bool isCurrent;
    public bool isFixed;
    
    // 내부적으로 변하는 변수들
    private bool _lastMove;
    private float _fallSpeed;
    
    private Rigidbody _rigidBody;
    public Material blockMaterial;

    public BlockEnum BlockType => _blockType;
    public BlockColor BlockColor => _blockColor;
    
    private Coroutine _fallCoroutine;
    private Coroutine _rotateCoroutine;

    public event Action OnBlockFinishedEvents;
    
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
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
        if (_fallCoroutine != null)
        {
            StopCoroutine(_fallCoroutine);            
        }
    }
    
    public IEnumerator Fall()
    {
        while (isCurrent)
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

    public void OnCollisionEnter(Collision other)
    {
        Block block = other.gameObject.GetComponent<Block>();
        
        // 고정된 블럭에 부딛혔다.
        if (block != null && block.isFixed)
        {
            _lastMove = true;
            StopBlock();
            _rigidBody.isKinematic = true;
            float roundedX = Mathf.Round(transform.position.x);
            float roundedY = Mathf.Round(transform.position.y);
            transform.position = new Vector3(roundedX, roundedY, 0);
            _rotateCoroutine = StartCoroutine(WaitForLastRotate());
        }
    }
    
    IEnumerator WaitForLastRotate()
    {
        for(float t = 0; t < _lastRotateTime; t += Time.deltaTime)
        {
            yield return new WaitForFixedUpdate();
        }

        OnBlockFinished();
    }

    // 블럭 조작이 끝났을때, 다음 블럭을 위해 현재 블럭 상태를 변경
    private void OnBlockFinished()
    {
        isCurrent = false;
        isFixed = true;
        StopAllCoroutines();
        OnBlockFinishedEvents?.Invoke();
    }
}