using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static GradientSkyEnum;
using Random = UnityEngine.Random;


public class TitleManager : MonoBehaviour
{
    private Material _mat;
    private bool _isDay = true;
    
    private readonly string _topColor = "_TopColor";
    private readonly string _middleColor = "_MiddleColor";
    private readonly string _bottomColor = "_BottomColor";
    
    public float skyChangeTime = 0.1f;
    
    [Header("타이틀 설정")]
    public TMP_Text titleText;
    public float titleShowTime = 1f;
    
    [Header("불꽃놀이 설정")]
    public GameObject fireWorkPrefab;
    public int fireWorksPoolSize = 10;
    private ObjectPool<GameObject> _fireWorksPool;
    
    // Start is called before the first frame update
    void Start()
    {
        _mat = GetComponent<Renderer>().material;
        _fireWorksPool = new ObjectPool<GameObject>(
            () =>
            {
                GameObject instance = Instantiate(fireWorkPrefab);
                instance.GetComponent<Fireworks>().SetPool(_fireWorksPool);
                return instance;
            },
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            false,
            defaultCapacity:5,
            maxSize:fireWorksPoolSize
        );
        
        if (_isDay)
        {
            StartCoroutine(ChangeToNight());
        }
    }
    

    // 밤으로 바꾸는 연출, 포문 돌면서 정해진 색상으로 바꿔준다
    IEnumerator ChangeToNight()
    {
        for(int i = 0; i < (int)ColorEnd-2; i++)
        {
            Color topBefore = _mat.GetColor(_topColor);
            GradientSkyEnum colorEnum = (GradientSkyEnum)i+2;
            Color topToChange = GradientSky.EnumToColor(colorEnum);

            Color midBefore = _mat.GetColor(_middleColor);
            GradientSkyEnum midColorEnum = (GradientSkyEnum)i+1;
            Color midToChange = GradientSky.EnumToColor(midColorEnum);
            
            Color bottomBefore = _mat.GetColor(_bottomColor);
            GradientSkyEnum bottomColorEnum = (GradientSkyEnum)i;
            Color bottomToChange = GradientSky.EnumToColor(bottomColorEnum);            
            
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                _mat.SetColor(_topColor, Color.Lerp(topBefore, topToChange, t));
                _mat.SetColor(_middleColor, Color.Lerp(midBefore, midToChange, t));
                _mat.SetColor(_bottomColor, Color.Lerp(bottomBefore, bottomToChange, t));
                yield return new WaitForFixedUpdate();
            }
            
            yield return new WaitForSeconds(skyChangeTime);
        }
        
        _isDay = false;
        
        StartCoroutine(ShowTitle());
        StartCoroutine(ShootFireWorks());
    }

    // 불꽃 놀이 쏘기
    IEnumerator ShootFireWorks()
    {
        for(int i = 0; i < 20; i++)
        {        
            _fireWorksPool.Get();
            yield return null;
        }
    }

    // 타이틀 출력하기
    IEnumerator ShowTitle()
    {
        for (float t = 0; t < titleShowTime; t += Time.deltaTime)
        {
            float a = Mathf.Lerp(0, 1, t / titleShowTime);
            titleText.faceColor = new Color(titleText.faceColor.r, titleText.faceColor.g, titleText.faceColor.b, a);
            yield return new WaitForFixedUpdate();
        }
    }
    
    // 시간 되면 낮 -> 밤 연출도 생각해보기
}
