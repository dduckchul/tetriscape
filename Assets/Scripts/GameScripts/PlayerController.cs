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
    public GameManager gameManager;
    public Rigidbody rigidbody;
    
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
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        float moveInput = ctx.ReadValue<float>();

        Vector3 moveVector = new Vector3(moveInput, 0, 0);
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
        
    }

    public void OnHold(InputAction.CallbackContext ctx)
    {
        
    }

    public void OnCollisionEnter(Collision other)
    {
        Block block = other.gameObject.GetComponent<Block>();
        if (block?.isCurrent == true)
        {
            block.isCurrent = false;
            StartCoroutine(gameManager.GameOverByPlayer(block));
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
}
