using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    /* 블록 환경 찾기 */
    public GameObject[] environments;
    public GameObject blockParent;
    public Light directionLight;

    /* UI 찾기 */ 
    public GameObject screen;
    private GameObject _ui;
    private TMP_Text _gameOverText;
    private SceneChanger _sceneChanger;
    
    /* 게임 연관 컴포넌트들 찾기 */
    private CinemachineManager _cinemachineManager;
    private SoundManager _soundManager;
    private BlockManager _blockManager;
    
    public GameObject player;
    private PlayerController _playerController;
    
    private Coroutine _gameOverCor;
    private bool _pressRestart;

    private Quaternion _day = Quaternion.Euler(70, 0, 0);
    private Quaternion _night = Quaternion.Euler(200, 0, 0);
    
    private void Awake()
    {
        _sceneChanger = screen.GetComponent<SceneChanger>();
        _playerController = player.GetComponent<PlayerController>();        
        
        _blockManager = gameObject.GetComponent<BlockManager>();
        _cinemachineManager = gameObject.GetComponent<CinemachineManager>();
        _soundManager = gameObject.GetComponent<SoundManager>();
    }
    
    private void Start()
    {
        // UI 다 끌것들 체크
        Transform uiTransform = screen.transform.parent.Find("UI");
        Transform gameOverTransform = screen.transform.parent.Find("GameOverText");

        if(uiTransform != null)
        {
            _ui = uiTransform.gameObject;
        }

        if (gameOverTransform != null)
        {
            _gameOverText = gameOverTransform.GetComponent<TMP_Text>();
        }

        StartCoroutine(_playerController.WaitUntilViewChanged());
    }

    public IEnumerator GameOverByBlock()
    {
        foreach (GameObject env in environments)
        {
            Rigidbody rigid = env.GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.constraints = RigidbodyConstraints.FreezePositionZ;
        }

        foreach (Rigidbody blockRigid in blockParent.GetComponentsInChildren<Rigidbody>())
        {
            blockRigid.isKinematic = false;
            blockRigid.useGravity = true;   
            blockRigid.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    
        _playerController.rigidbody.constraints = RigidbodyConstraints.None;
        environments[0].GetComponent<Rigidbody>().AddExplosionForce(100, Vector3.zero, 10, 10, ForceMode.Impulse);

        _ui.SetActive(false);
        yield return new WaitForSeconds(1f);

        StartCoroutine(_sceneChanger.Loading(SceneEnum.MainScene));                
        StartCoroutine(_sceneChanger.FadeOut());
        StartCoroutine(WaitUntilSceneLoadFinished());
    }

    public IEnumerator GameOverByPlayer(Block block)
    {
        _ui.SetActive(false);
        _playerController.rigidbody.constraints = RigidbodyConstraints.None;

        if (block != null)
        {
            block.StopAllCoroutines();
            block.rigidBody.constraints = RigidbodyConstraints.None;
            _playerController.rigidbody.AddExplosionForce(100, Vector3.zero, 10, 1, ForceMode.Impulse);;            
        }
        
        yield return new WaitForSeconds(1f);

        StartCoroutine(_sceneChanger.Loading(SceneEnum.MainScene));
        StartCoroutine(_sceneChanger.FadeOut());
        StartCoroutine(WaitUntilSceneLoadFinished());
    }
    
    public void OnRestart(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _pressRestart = true;            
        }
    }

    // 0.9 이상이고 리스타트 누를 때 까지 대기
    IEnumerator WaitUntilSceneLoadFinished()
    {
        StartCoroutine(ShowGameOverText());
        yield return new WaitUntil(() => _sceneChanger.asyncOp.progress >= 0.9f);
        yield return new WaitUntil(() => _pressRestart);
        _pressRestart = false;
        yield return new WaitForFixedUpdate();
        _sceneChanger.activateScene = true;
    }
    
    IEnumerator ShowGameOverText()
    {
        _gameOverText.gameObject.SetActive(true);
        for(float t = 0; t < 1f; t += Time.deltaTime)
        {
            _gameOverText.color = new Color(_gameOverText.color.r, _gameOverText.color.g, _gameOverText.color.b, t);
            yield return new WaitForFixedUpdate();
        }
    }

    /* 2D 모드 변경하며 변경해 줄 것들 */
    public IEnumerator ChangeTo2D()
    {
        StartCoroutine(_playerController.WaitUntilViewChanged());
        _cinemachineManager?.Change2DView();
        _soundManager?.ChangeTo2DMusic();
        _blockManager?.CurrentBlock.StartBlock(_blockManager.fallSpeed);
        _ui.SetActive(true);

        for(float t = 0; t < 1; t += Time.deltaTime)
        {
            directionLight.transform.rotation = Quaternion.Lerp(_day, _night, t);
            yield return new WaitForFixedUpdate();
        }
    }

    /* 3D 모드 변경하며 변경해 줄 것들 */
    public IEnumerator ChangeTo3D()
    {
        StartCoroutine(_playerController.WaitUntilViewChanged());
        _cinemachineManager?.Change3DView();
        _soundManager?.ChangeTo3DMusic();
        _blockManager?.CurrentBlock.StopBlock();
        _ui.SetActive(false);
        
        for(float t = 0; t < 1; t += Time.deltaTime)
        {
            directionLight.transform.rotation = Quaternion.Lerp(_night, _day, t);
            yield return new WaitForFixedUpdate();
        }
    }
}
