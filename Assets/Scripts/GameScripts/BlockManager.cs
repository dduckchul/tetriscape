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
    private int _layerMask;
    private Dictionary<int, Collider[]> _collidersDict;

    [Header("게임 환경 설정")]
    public int fieldWidth = 10;
    
    public GameObject blockFactoryObj;
    public GameObject deadZone;
    public GameObject blockParent;
    private BlockFactory _blockFactory;
    private Collider _deadZoneCollider;
    
    [Header("게임 진행 설정")]
    // block 떨어지는 속도 조절
    public float fallSpeed = 0.015f;
    // 스왑 한번만 쓰도록 bool값으로 제어
    private bool _useSwap;
    
    private Vector3 _spawnPos;
    private Vector3 _holdPos;
    private Vector3 _nextPos;
    
    private GameManager _gameManager;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _playerInput = GetComponent<PlayerInput>();        
        _playerInput.actions.FindActionMap("Block").Enable();
        
        _collidersDict = new Dictionary<int, Collider[]>();
        _blockQueue = new Queue<Block>();        
        _layerMask = LayerMask.GetMask("Blocks");        
    }
    private void Start()
    { 
        _blockFactory = blockFactoryObj.GetComponent<BlockFactory>();
        _deadZoneCollider = deadZone.GetComponent<Collider>();
      
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
    
    public void OnBlockMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        _currentBlock.Move(dir);
    }

    public void OnBlockSpin()
    {
        _currentBlock.Rotate();
    }

    public void OnBlockHold()
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
            _holdBlock.transform.position = _holdPos;
        }
        else
        {
            _holdBlock.transform.position = _holdPos;  
            _currentBlock.transform.position = _spawnPos;
            _currentBlock.isCurrent = true;
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
        
        for(int i = 0; i < 5; i++)
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
    
    // 조작 끝나면 확인할 것들
    // 이벤트 바인딩 해제, 스왑 플래그 돌리기, 게임오버확인, 줄삭제, 블럭 큐잉, 다음 블럭 시작 
    private void ControlFinished()
    {
        _currentBlock.OnBlockFinishedEvents -= ControlFinished;
        _useSwap = false;
        
        if (CheckToGameOver())
        {
            StartCoroutine(_gameManager.GameOverByBlock());
            return;
        }
        
        CheckToRemoveLine();
        EnqueueSomeBlocks();
        Dequeue();
        StartCurrentBlock();
    }


    // 마지막 블럭이 데드존과 충돌했는지 확인
    private bool CheckToGameOver()
    {
        Collider[] colliders = _currentBlock.transform.GetComponentsInChildren<Collider>();

        foreach (Collider blocks in colliders)
        {
            if (blocks.bounds.Intersects(_deadZoneCollider.bounds))
            {
                return true;
            }
        }

        return false;
    }
    
    // 지울 라인 확인하기, 자식이 없는 블록들 있으면 탐색 위해 디스트로이
    private void CheckToRemoveLine()
    {
        _collidersDict.Clear();
        for (int i = 0; i < _currentBlock.transform.childCount; i++)
        {
            Transform child = _currentBlock.transform.GetChild(i);
            int roundedY = Mathf.RoundToInt(child.position.y);
            Vector3 checkPos = new Vector3(0.5f, roundedY, 0);
            
            float checkToColldeWidth = (fieldWidth - 1f) / 2;
            Vector3 lineCheckerSize = new Vector3(checkToColldeWidth, 0.05f, 0.05f);
            Collider[] collides = Physics.OverlapBox(checkPos, lineCheckerSize, Quaternion.identity, _layerMask);
            
            if(collides.Length == fieldWidth)
            {
                _collidersDict.TryAdd(roundedY, collides);
            }
        }

        RemoveLine();
        StartCoroutine(DestroyNonChildBlocks());
    }

    // 라인 지우기
    private void RemoveLine()
    {
        if (_collidersDict.Count == 0)
        {
            return;
        }
        
        foreach (Collider[] collides in _collidersDict.Values)
        {
            foreach (Collider coll in collides)
            {
                Destroy(coll.gameObject);
            }
        }
        
        FindAllBlocksAbove();
    }

    // 지우고 그 위 블록들 모두 아래로 내리기
    private void FindAllBlocksAbove()
    {
        foreach (Block block in blockParent.GetComponentsInChildren<Block>())
        {
            if (!block.isFixed)
            {
                continue;
            }
            
            for (int i = 0; i < block.transform.childCount; i++)
            {
                int downCount = 0;
                Transform child = block.transform.GetChild(i);
                
                int tempPosY = Mathf.RoundToInt(child.position.y);
                foreach (int key in _collidersDict.Keys)
                {
                    if (tempPosY > key)
                    {
                        downCount++;
                    }
                }
                child.position += Vector3.down * downCount;
            }
        }
    }
    
    // 점점 탐색 느려질거니까 정리해주자
    IEnumerator DestroyNonChildBlocks()
    {
        foreach (Block block in blockParent.GetComponentsInChildren<Block>())
        {
            if (block.transform.childCount == 0)
            {
                Destroy(block.gameObject);
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void RenderBlocksTo2D()
    {
        Debug.Log("Render Blocks To 2D");
    }

    public void RenderBlocksTo3D()
    {
        Debug.Log("Render Blocks To 3D");
    }
    
    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        // if (_currentBlock == null)
        // {
        //     return;
        // }
        //
        // for (int i = 0; i < _currentBlock.transform.childCount; i++)
        // {
        //     Transform child = _currentBlock.transform.GetChild(i);
        //     float roundedY = Mathf.Round(child.position.y);
        //     Vector3 checkPos = new Vector3(0.5f, roundedY, 0);
        //
        //     float checkToColldeWidth = fieldWidth -1f;
        //     Vector3 blockSize = new Vector3(checkToColldeWidth, 0.05f, 0.05f);
        //
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireCube(checkPos, blockSize * 2);
        // }
    }    
}
