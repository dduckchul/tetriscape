using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockManager : MonoBehaviour
{
    private Block _currentBlock;
    private Block _nextblock;
    private Block _holdBlock;
    private Queue<Block> _blockQueue;

    public GameObject blockFactoryObj;
    private BlockFactory _blockFactory;

    public float fallSpeed = 0.015f;
    // 스왑 한번만 쓰도록 bool값으로 제어
    private bool _useSwap = false;
    private Vector3 _spawnPos;
    private Vector3 _holdPos;
    private Vector3 _nextPos;
    
    private void Start()
    {
        _blockQueue = new Queue<Block>();
        _blockFactory = blockFactoryObj.GetComponent<BlockFactory>();
        _spawnPos = new Vector3(0, blockFactoryObj.transform.position.y, 0);
        _holdPos = new Vector3(-10, 14, 0);
        _nextPos = new Vector3(10, 14, 0);

        InitBlockQueue();
        Dequeue();
        
        // 일단 여기다 넣읍시다, 잠시후 출발할거임
        Invoke("StartCurrentBlock", 1f);
    }
    
    private void InitBlockQueue()
    {
        _blockQueue.Clear();
        
        for (int i = 0; i < 2; i++)
        {
            _blockQueue.Enqueue(_blockFactory.CreateBlock(_nextPos));
        }
    }
    
    private void StartCurrentBlock()
    {
        _currentBlock.isCurrent = true;
        _currentBlock.transform.position = _spawnPos;
        _currentBlock.StartBlock(fallSpeed);
        _currentBlock.OnBlockFinishedEvents += ControlFinished;
    }
    
    private void OnBlockMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        _currentBlock.Move(dir);
    }

    private void OnBlockSpin()
    {
        _currentBlock.Rotate();
    }

    private void OnBlockHold()
    {
        if (_useSwap)
        {
            return;
        }
        
        SwapToHold();

        // 스왑했는데 없으면? 홀드에 저장만 하고 다음 거 뽑기, 있으면? 홀드 위치로 이동 하고 스타트
        if (_currentBlock == null)
        {
            Dequeue();
            StartCurrentBlock();
        }
        else
        {
            _holdBlock.transform.position = _holdPos;  
            _currentBlock.transform.position = _spawnPos;
            _currentBlock.StartBlock(fallSpeed);            
        }
    }

    // 큐 바닥나있다 싶으면 5개정도만 만듦
    public void EnqueueSomeBlocks()
    {
        if(_blockQueue.Count > 1)
        {
            return;
        }
        
        for(int i = 0; i < 2; i++)
        {
            _blockQueue.Enqueue(_blockFactory.CreateBlock(_nextPos));
        }
    }    
    
    private void Dequeue()
    {
        Block block = _blockQueue.Dequeue();
        _currentBlock = block;
        _currentBlock.rigidBody.isKinematic = false;
    }

    // 스왑 눌렀을때 실행
    private void SwapToHold()
    {
        _useSwap = true;
        _currentBlock.isCurrent = false;
        _currentBlock.StopBlock();
        
        Block temp = _currentBlock;
        _currentBlock = _holdBlock;
        _holdBlock = temp;
    }
    
    // To-Do 블록 놓기 완료되고 할 것들
    private void ControlFinished()
    {
        _currentBlock.OnBlockFinishedEvents -= ControlFinished;
        EnqueueSomeBlocks();
        Dequeue();
        StartCurrentBlock();
    }
}
