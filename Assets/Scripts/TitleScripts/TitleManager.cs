using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static GradientSkyEnum;

public class TitleManager : MonoBehaviour
{
    private Material _mat;
    private bool _isDay = true;
    
    private readonly string _topColor = "_TopColor";
    private readonly string _middleColor = "_MiddleColor";
    private readonly string _bottomColor = "_BottomColor";
    
    private Coroutine _changeToNightCorountine;
    private Coroutine _shootFireWorksCorountine;
    private Coroutine _showTitleCoroutine;

    private bool _isLoading = false;
    
    public float skyChangeTime = 0.1f;
    public SoundManager _soundManager;
    
    [Header("타이틀 설정")]
    public TMP_Text titleText;
    public float titleShowTime = 1f;
    
    [Header("불꽃놀이 설정")]
    public GameObject fireWorkPrefab;
    public int fireWorksPoolSize = 10;
    // 최대 다음 불꽃놀이 발사 될때까지 걸리는 시간
    public float maxRandomizeShootTime = 1f;
    private ObjectPool<GameObject> _fireWorksPool;

    [Header("로딩 애니메이션 설정")]
    public GameObject aruru;
    public GameObject screen;
    private SceneChanger _sceneChanger;
    [SerializeField] private int _seenTitle;
    private Animator _animator;
    
    void Awake()
    {
        _seenTitle = PlayerPrefs.GetInt("seenTitle", 0);
        _animator = aruru.GetComponent<Animator>();
    }
    
    void Start()
    {
        _animator.SetBool("IsRun", true);
        _mat = GetComponent<Renderer>().material;
        _sceneChanger = screen.GetComponent<SceneChanger>();
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
            true,
            fireWorksPoolSize,
            fireWorksPoolSize
        );
        
        if (_isDay)
        {
            _changeToNightCorountine = StartCoroutine(ChangeToNight());
        }
        
        _soundManager.Play("Title");
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
            
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                _mat.SetColor(_topColor, Color.Lerp(topBefore, topToChange, t));
                _mat.SetColor(_middleColor, Color.Lerp(midBefore, midToChange, t));
                _mat.SetColor(_bottomColor, Color.Lerp(bottomBefore, bottomToChange, t));
                yield return new WaitForFixedUpdate();
            }
            
            yield return new WaitForSeconds(skyChangeTime);
        }

        PlayerPrefs.SetInt("seenTitle", 1);
        PlayerPrefs.Save();

        _seenTitle = PlayerPrefs.GetInt("seenTitle");
        
        _isDay = false;
        _showTitleCoroutine = StartCoroutine(ShowTitle());
        _shootFireWorksCorountine = StartCoroutine(ShootFireWorks());
    }

    // 불꽃 놀이 쏘기
    IEnumerator ShootFireWorks()
    {
        while (!_isDay)
        {
            int randomToShoot = Random.Range(1, 5);
            for(int i = 0; i < randomToShoot; i++)
            {        
                _fireWorksPool.Get();
                yield return new WaitForSeconds(Random.Range(0, maxRandomizeShootTime));
            }
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
    IEnumerator ChangeToDay()
    {
        Color32 nightBottom = GradientSky.EnumToColor(LightBlack);
        Color32 nightMid = GradientSky.EnumToColor(DarkBlack);
        Color32 nightTop = GradientSky.EnumToColor(Black);

        Color32 dayBottom = GradientSky.EnumToColor(DarkWhite);
        Color32 dayMid = GradientSky.EnumToColor(LightBlue);
        Color32 dayTop = GradientSky.EnumToColor(SkyBlue);
        
        if (!_isDay)
        {
            StopCoroutine(_shootFireWorksCorountine);

            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                _mat.SetColor(_topColor, Color.Lerp(nightTop, dayTop, t));
                _mat.SetColor(_middleColor, Color.Lerp(nightMid, dayMid, t));
                _mat.SetColor(_bottomColor, Color.Lerp(nightBottom, dayBottom, t));
                yield return new WaitForFixedUpdate();
            }
        }

        if (_isDay && _changeToNightCorountine != null)
        {
            // 중간에 키 눌렸을때 코루틴 멈추고 바로 로딩 & 시작화면으로
            StopCoroutine(_changeToNightCorountine);

            if (_shootFireWorksCorountine != null)
            {
                StopCoroutine(_shootFireWorksCorountine);                
            }

            if(_showTitleCoroutine != null)
            {
                StopCoroutine(_showTitleCoroutine);
            }

            _mat.SetColor(_topColor, dayTop);
            _mat.SetColor(_middleColor, dayMid);
            _mat.SetColor(_bottomColor, dayBottom);
            
            titleText.faceColor = new Color(titleText.faceColor.r, titleText.faceColor.g, titleText.faceColor.b, 1);
        }
    }

    private void GameStart()
    {
        _soundManager.Stop("Title");
        _soundManager.Play("GameStart");
        StartCoroutine(_sceneChanger.Loading(SceneEnum.MainScene));
        StartCoroutine(ChangeToDay());
        StartCoroutine(ActiveAruru());
    }

    // 일단 불꽃놀이 한번 봐야 다음 부터 시작 가능
    private void OnStartGame()
    {
        if (_seenTitle == 1 && _isLoading == false)
        {
            _isLoading = true;
            GameStart();
        }
    }

    IEnumerator ActiveAruru()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveAruru());
    }

    IEnumerator MoveAruru()
    {
        bool isAruruMiddle = false;

        while (aruru.transform.position.x < 15)
        {
            aruru.transform.position += new Vector3(0.15f, 0, 0);
            yield return new WaitForFixedUpdate();

            if (aruru.transform.position.x > 0 && !isAruruMiddle)
            {
                isAruruMiddle = true;
                _animator.SetTrigger("triggerLookBack");
                for(float wait = 0 ; wait < 1.5; wait += Time.deltaTime)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        _sceneChanger.activateScene = true;
    }
}
