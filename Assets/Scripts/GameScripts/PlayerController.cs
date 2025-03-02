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
    private InputAction _moveAction;
    
    public float moveSpeed = 1.0f;

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
        _moveAction = _playerInput.actions.FindActionMap("Player").FindAction("Move");
    }
    
    private void OnEnable()
    {
        _moveAction.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        _moveAction.canceled -= OnMoveCanceled;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 moveInput = ctx.ReadValue<Vector2>();
        if (moveInput.y > 0)
        {
            return;
        }
        
        Vector3 moveVector = new Vector3(moveInput.x, 0, 0);
        _moveCoroutine = StartCoroutine(MovePlayer(moveVector));            
    }

    public void OnJump()
    {
        
    }

    public void OnHold()
    {
        
    }
    
    private Animator GetAnimator()
    {
        return _isTwoDemensional ? _2dAnimator : _3dAnimator;
    }    
    
    IEnumerator MovePlayer(Vector3 move)
    {
        StartCoroutine(StartAnimation("IsWalk"));
        while (true)
        {
            transform.position += move * (moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // 이동 코루틴 실행중이라면 중지
        if (_moveCoroutine != null)
        {
            StartCoroutine(StopAnimation("IsWalk"));
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
            
            Debug.Log(_moveCoroutine);
            
            if(_moveCoroutine == null)
            {
                Debug.Log("why not stop?");
            }
        }
        

    }

    IEnumerator StartAnimation(string paramName)
    {
        GetAnimator().SetBool("IsWalk", true);
        yield return null;        
    }

    IEnumerator StopAnimation(string paramName)
    {
        GetAnimator().SetBool("IsWalk", false);
        yield return null;
    }
}
