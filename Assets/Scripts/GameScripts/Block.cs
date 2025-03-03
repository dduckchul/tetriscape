using System;
using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour, IMoveable, IRotatable, IColorable
{
    // 생성할때 세팅해주는 값들
    private readonly float _lastRotateTime = 0.5f;
    private BlockEnum _blockType;
    private BlockColor _blockColor;
    [SerializeField] private Vector3[] _relativeBlockPos;
    private int _layerMask;

    // 가변적으로 변할수 있는 값들
    public bool isCurrent;
    public bool isFixed;
    // 벽판정, 블록을 고정시키는 판정은은 아니고 이동, 로테이션 제한
    public bool isCollide;
    
    // 내부적으로 변하는 변수들
    private bool _lastMove;
    private float _fallSpeed;
    
    public Rigidbody rigidBody;
    public Material blockMaterial;

    public BlockEnum BlockType => _blockType;
    public BlockColor BlockColor => _blockColor;
    
    private Coroutine _fallCoroutine;
    private Coroutine _rotateCoroutine;

    public event Action OnBlockFinishedEvents;

    public void Start()
    {
        _layerMask = LayerMask.GetMask("Blocks");
    }
    
    // 초기 블록 세팅, 1. 위치, 2. 색상, 3. 떨어지는 속도, 4. 자식 블록 위치들
    public void InitBlock(BlockEnum blockEnum, BlockColor blockColor, Vector3 pos, Vector3[] relativeBlockPos)
    {
        _blockType = blockEnum;
        _blockColor = blockColor;
        _relativeBlockPos = relativeBlockPos;
        transform.position = pos;
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

    public void Move(Vector2 dir)
    {
        StopBlock();

        Vector3 tempPos = transform.position;
        tempPos += new Vector3(dir.x, dir.y, 0);
        
        // Debug.Log("tempPos : " + tempPos);
        
        bool isCollided = PreCollideBlocks(tempPos, _relativeBlockPos);
        if (!isCollided)
        {
            transform.position += new Vector3(dir.x, dir.y, 0);
        }
        
        StartBlock(_fallSpeed);
    }    
    
    // 돌려도 의미없는 네모 블럭은 예외처리
    public void Rotate()
    {
        if (BlockType == BlockEnum.SquareBlock)
        {
            return;
        }
        
        StopBlock();
        RotateIfAvailable();
        StartBlock(_fallSpeed);
    }

    // 블록 회전시 상대적인 블록 위치 변경
    private Vector3[] RotateRelativeBlockPos(int rotation)
    {
        Vector3[] origin = BlockUtility.EnumToRelativeBlockPos(BlockType);
        Vector3[] rotated = origin;

        int rotatedTimes = rotation / 90 % 4;
        
        if (rotatedTimes == 1 || rotatedTimes == -3)
        {
            for(int i = 0; i < origin.Length; i++)
            {
                rotated[i] = new Vector3(-origin[i].y, origin[i].x);
            }
        } else if (rotatedTimes == 2 || rotatedTimes == -2)
        {
            for(int i = 0; i < origin.Length; i++)
            {
                rotated[i] = new Vector3(-origin[i].x, -origin[i].y);
            }
        } else if (rotatedTimes == 3 || rotatedTimes == -1)
        {
            for(int i = 0; i < origin.Length; i++)
            {
                rotated[i] = new Vector3(origin[i].y, origin[i].x);
            }            
        }

        return rotated;
    }

    // 블록을 미리 회전시키고, 회전한 블럭들에서 하나라도 충돌한게 있으면 다시 원래대로
    private void RotateIfAvailable()
    {
        int rotateAngle = Mathf.RoundToInt(transform.rotation.eulerAngles.z + 90f);
        Vector3[] rotatedBlocks = RotateRelativeBlockPos(rotateAngle);

        bool isCollided = PreCollideBlocks(transform.position, rotatedBlocks);

        if (!isCollided)
        {
            transform.Rotate(Vector3.forward, 90f);
            _relativeBlockPos = rotatedBlocks;
        }
    }

    // 미리 블록을 그려서 충돌시켜본다
    private bool PreCollideBlocks(Vector3 originPos, Vector3 [] relatedBlocks)
    {
        foreach (Vector3 blockPos in relatedBlocks)
        {
            // Physics.OverlabBox에서는 콜라이더를 다르게 가져올 수 있음
            var collides = Physics.OverlapBox
            (
                originPos + blockPos, 
                Vector3.one * 0.49f, 
                transform.rotation, 
                _layerMask
            );
            
            foreach (Collider coll in collides)
            {
                // 자기 자신과의 충돌 방지
                if (coll.transform.parent.gameObject == gameObject) { continue; }
                
                Block collsionBlock = coll.gameObject.GetComponent<Block>();

                // 충돌하는 물체들의 Block 스크립트 체크, 같은 테트로미노면 부모에 달렸으므로 부모까지 체크
                if (collsionBlock == null)
                {
                    Block parentsBlock = coll.transform.parent.gameObject.GetComponent<Block>();

                    if (parentsBlock == null) { continue; }

                    collsionBlock = parentsBlock;
                }
                
                if (collsionBlock.isFixed || collsionBlock.isCollide)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // ToDo : 2D - 3D 간 색상 변경시 블록 색상 변경.. 필요할려나..?
    public void ChangeColor()
    {

    }
    
    // 충돌 이벤트 감지
    private void OnCollisionEnter(Collision other)
    {
        if (!isCurrent)
        {
            return;
        }
        
        if (!isCurrent)
        {
            return;
        }
        
        Block otherBlock = other.gameObject.GetComponent<Block>();

        if (otherBlock == null)
        {
            return;
        }
        
        // 고정된 블럭에 부딛혔다.
        if (otherBlock.isFixed)
        {
            _lastMove = true;
            StopBlock();
            ToIntPosition();
            OnBlockFinished();
            // _rotateCoroutine = StartCoroutine(WaitForLastRotate());
        }
    }

    // ToDo : T스핀 위해 잠시 대기, 구현 가능하면 다시 해보기
    // IEnumerator WaitForLastRotate()
    // {
    //     for(float t = 0; t < _lastRotateTime; t += Time.deltaTime)
    //     {
    //         yield return new WaitForFixedUpdate();
    //     }
    //
    //     float roundedX = Mathf.Round(transform.position.x);
    //     float roundedY = Mathf.Round(transform.position.y);
    //     transform.position = new Vector3(roundedX, roundedY, 0);        
    //     
    //     OnBlockFinished();
    // }

    // 블럭 조작이 끝났을때, 다음 블럭을 위해 현재 블럭 상태를 변경
    private void OnBlockFinished()
    {
        isCurrent = false;
        isFixed = true;
        rigidBody.isKinematic = true;
        StopAllCoroutines();
        OnBlockFinishedEvents?.Invoke();
    }

    private void ToIntPosition()
    {
        float roundedX = Mathf.Round(transform.position.x);
        float roundedY = Mathf.Round(transform.position.y);
        transform.position = new Vector3(roundedX, roundedY, 0);        
    }
}