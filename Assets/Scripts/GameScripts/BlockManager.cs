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

    private Vector3 _spawnPos;
    public float fallSpeed = 0.015f;
    
    private void Start()
    {
        _blockQueue = new Queue<Block>();
        _blockFactory = blockFactoryObj.GetComponent<BlockFactory>();
        _spawnPos = new Vector3(0, blockFactoryObj.transform.position.y, 0);

        InitBlockQueue();
        
        // 일단 여기다 넣읍시다, 잠시후 출발할거임
        Invoke("ControlBlock", 1f);
    }

    private void InitBlockQueue()
    {
        _blockQueue.Clear();
        _currentBlock = _blockFactory.CreateBlock(_spawnPos);
        _currentBlock.IsCurrent = true;
        
        for (int i = 0; i < 5; i++)
        {
            _blockQueue.Enqueue(_blockFactory.CreateBlock(_spawnPos));
        }
    }

    private void ControlBlock()
    {
        _currentBlock.StartBlock(fallSpeed);
    }
    
    private void OnBlockMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        _currentBlock.Move(dir / 2);
    }

    private void OnBlockSpin()
    {
        _currentBlock.Rotate();
    }
    
    private void OnBlockHold()
    {
        
    }

    private void Dequeue()
    {
        _currentBlock = _blockQueue.Dequeue();
    }

    private void SwapToHold()
    {
        
    }
}
