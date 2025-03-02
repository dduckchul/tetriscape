using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject[] environments;
    public GameObject blockParent;

    public GameObject screen;
    private GameObject _ui;
    private TMP_Text _gameOverText;
    private SceneChanger _sceneChanger;
    
    public GameObject player;
    private PlayerController _playerController;
    
    private Coroutine _gameOverCor;
    private bool _pressRestart;    
    
    private void Start()
    {
        _sceneChanger = screen.GetComponent<SceneChanger>();
        _playerController = player.GetComponent<PlayerController>();

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

        block.StopAllCoroutines();
        block.rigidBody.constraints = RigidbodyConstraints.None;
        _playerController.rigidbody.constraints = RigidbodyConstraints.None;
        _playerController.rigidbody.AddExplosionForce(100, Vector3.zero, 10, 1, ForceMode.Impulse);;
        
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
}
