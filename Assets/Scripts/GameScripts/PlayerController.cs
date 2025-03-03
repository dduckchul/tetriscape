using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 2D, 3D 구분하는 내부변수, 초기값은 2D가 True로 설정
    [Serialize] private bool _isTwoDemensional;
    private GameObject _2dPlayer;
    private GameObject _3dPlayer;

    private Animator _2dAnimator;
    private Animator _3dAnimator;
    
    private Coroutine _moveCoroutine;
    private PlayerInput _playerInput;
    
    public float moveSpeed = 1.0f;
    public GameObject gameManager;

    private GameManager _gameManager;
    public Rigidbody rigidbody;

    public bool isJumping;
    public bool isControllable;
    
    private void Awake()
    {
        _isTwoDemensional = true;

        if (transform.childCount != 2)
        {
            Debug.LogError("플레이어 자식 오브젝트가 2개가 아닙니다.");
            return;
        }
        
        _2dPlayer = transform.GetChild(0).gameObject;
        _3dPlayer = transform.GetChild(1).gameObject;

        _2dAnimator = _2dPlayer.GetComponent<Animator>();
        _3dAnimator = _3dPlayer.GetComponent<Animator>();
        
        if(_2dAnimator == null || _3dAnimator == null)
        {
            Debug.LogError("플레이어 자식 오브젝트에 Animator가 없습니다.");
            return;
        }
        
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.FindActionMap("Player").Enable();
        
        _gameManager = gameManager.GetComponent<GameManager>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!isControllable)
        {
            return;
        }
        
        Vector2 moveInput = ctx.ReadValue<Vector2>();
        
        if (_isTwoDemensional)
        {
            TwoDimensionalMove(moveInput);
        }
        else
        {
            ThreeDimensionalMove(moveInput);
        }
    }

    public void TwoDimensionalMove(Vector2 moveDir)
    {
        if (Mathf.Abs(moveDir.y) > 0)
        {
            return;
        }
        
        Vector3 moveVector = new Vector3(moveDir.x, 0, 0).normalized;
        
        if (_moveCoroutine == null)
        {
            _moveCoroutine = StartCoroutine(MovePlayer(moveVector));  
            StartCoroutine(StartAnimation("IsWalk"));
        }        
    }
    
    public void ThreeDimensionalMove(Vector2 moveDir)
    {
        Vector3 moveVector = new Vector3(moveDir.y, 0, -moveDir.x).normalized;
        
        if (_moveCoroutine == null)
        {
            _moveCoroutine = StartCoroutine(MovePlayer(moveVector));  
            StartCoroutine(StartAnimation("IsWalk"));
        }        
    }
    
    public void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && _moveCoroutine != null)
        {
            StartCoroutine(StopAnimation("IsWalk"));
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (_isTwoDemensional || !isControllable)
        {
            return;
        }

        if (ctx.performed && !isJumping)
        {
            isJumping = true;
            rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    // ToDo : 블록 잡고 움직이기
    public void OnHold(InputAction.CallbackContext ctx)
    {
        
    }
    
    public void OnChangeView(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _isTwoDemensional = !_isTwoDemensional;

            if (_isTwoDemensional)
            {
                _2dPlayer.SetActive(true);
                _3dPlayer.SetActive(false);
                StartCoroutine(_gameManager.ChangeTo2D()); 
            }
            else
            {
                _2dPlayer.SetActive(false);
                _3dPlayer.SetActive(true);
                StartCoroutine(_gameManager.ChangeTo3D());
            }            
        }
    }    

    public void OnCollisionEnter(Collision other)
    {
        Block block = other.gameObject.GetComponent<Block>();
        if (block?.isCurrent == true)
        {
            block.isCurrent = false;
            StartCoroutine(_gameManager.GameOverByPlayer(block));
        }

        if (block?.isFixed == true)
        {
            isJumping = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Bottom"))
        {
            StartCoroutine(_gameManager.GameOverByPlayer(null));
        }

        if (other.tag.Equals("Goal"))
        {
            Debug.Log("Goal");
        }
    }

    private Animator GetAnimator()
    {
        return _isTwoDemensional ? _2dAnimator : _3dAnimator;
    }    
    
    IEnumerator MovePlayer(Vector3 move)
    {
        while (true)
        {
            transform.position += move * (moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator StartAnimation(string paramName)
    {
        GetAnimator().SetBool(paramName, true);
        yield return null;        
    }

    IEnumerator StopAnimation(string paramName)
    {
        GetAnimator().SetBool(paramName, false);
        yield return null;
    }

    public IEnumerator WaitUntilViewChanged()
    {
        isControllable = false;
        yield return new WaitForSeconds(1f);
        isControllable = true;
    }
}
