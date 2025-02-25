using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    private Image _screen;
    
    public AsyncOperation asyncOp;
    public bool activateScene = false;

    void Start()
    {
        _screen = GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    // 로딩 끝나고, activateScene 을 true로 바꾸면 씬이 활성화된다.
    public IEnumerator Loading(SceneEnum sceneEnum)
    {
        asyncOp = SceneManager.LoadSceneAsync(sceneEnum.ToString());

        if (asyncOp == null)
        {
            Debug.LogError("Error : 씬 이름이 없습니다! SceneEnum을 확인해주세요");
            yield break;
        }
        
        asyncOp.allowSceneActivation = false;
        yield return null;

        StartCoroutine(ActiveScene());
    }

    IEnumerator ActiveScene()
    {
        yield return new WaitUntil(() => activateScene);        
        activateScene = false;
        asyncOp.allowSceneActivation = true;
    }

    IEnumerator FadeIn()
    {
        // 타이틀은 FadeIn 예외
        if (SceneManager.GetActiveScene().name.Equals(SceneEnum.TitleScene.ToString()))
        {
            yield break;
        }
        
        if (_screen is not null)
        {
            Color color = _screen.color;
            color = new Color(color.r, color.g, color.b, 1f);
            _screen.color = color;
            
            for(float t = 0; t < 1f; t += Time.deltaTime)
            {
                Color toColor = new Color(color.r, color.g, color.b, 0f);
                _screen.color = Color.Lerp(color, toColor, t);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator FadeOut()
    {
        if (_screen is not null)
        {
            Color color = _screen.color;
            color = new Color(color.r, color.g, color.b, 0f);
            _screen.color = color;
            
            for(float t = 0; t < 1f; t += Time.deltaTime)
            {
                Color toColor = new Color(color.r, color.g, color.b, 1f);
                _screen.color = Color.Lerp(color, toColor, t);
                yield return new WaitForFixedUpdate();
            }
        }        
    }
}
