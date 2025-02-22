using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Fireworks : MonoBehaviour
{
    public Sprite[] fireWorkSprites;
    // 불꽃놀이 사라지는 속도
    public float disappearSpeed = 0.1f;
    
    private ObjectPool<GameObject> _pool;
    private SpriteRenderer _sRenderer;
    
    public void SetPool(ObjectPool<GameObject> pool)
    {
        _pool = pool;
    }
    
    private void Start()
    {
        _sRenderer = GetComponent<SpriteRenderer>();
        SetRandomValues();
        StartCoroutine(Shoot());
    }

    // 각종 수치 랜덤하게 초기화 설정
    public void SetRandomValues()
    {
        float randRotZ = Random.Range(-45f, 45f);
        float randX = Random.Range(-5f, 5f);
        float randY = Random.Range(0f, 4.0f);
        
        // 잘보이게 원색계열로 설정
        float randColorR = Random.value > 0.5f ? 1 : 0;
        float randColorG = Random.value > 0.5f ? 1 : 0;
        
        // 검은색은 싫으니까 둘다 0이면 무조건 1로 바꿔준다.
        float randColorB = randColorR == 0 && randColorG == 0 ? 1 : (Random.value > 0.5f ? 1 : 0);
        
        Color randColor = new Color(randColorR, randColorG, randColorB);
        
        transform.position = new Vector3(randX, randY, 0);
        transform.rotation = Quaternion.Euler(0, 0, randRotZ);
        
        _sRenderer.color = randColor;
    }
    
    IEnumerator Shoot()
    {
        for(int i = 0; i < fireWorkSprites.Length; i++)
        {
            _sRenderer.sprite = fireWorkSprites[i];
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        Color origin = _sRenderer.color;
        Color removedAlpha = new Color(origin.r, origin.g, origin.b, 0);
        
        for(float alpha = 0; alpha < 1; alpha += disappearSpeed)
        {
            _sRenderer.color = Color.Lerp(origin, removedAlpha, alpha);
            yield return new WaitForFixedUpdate();
        }
        
        _pool.Release(gameObject);
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
