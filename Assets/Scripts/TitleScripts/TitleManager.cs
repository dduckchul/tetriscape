using System;
using System.Collections;
using UnityEngine;
using static GradientSkyEnum;


public class TitleManager : MonoBehaviour
{
    private Material _mat;
    private bool _isDay = true;
    
    private readonly string _topColor = "_TopColor";
    private readonly string _middleColor = "_MiddleColor";
    private readonly string _bottomColor = "_BottomColor";    
    
    public float skyChangeTime = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _mat = GetComponent<Renderer>().material;
        if (_isDay)
        {
            StartCoroutine(ChangeToNight());
        }
    }

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
    }

    void ChangeToDay()
    {
        _mat.SetColor(_topColor, GradientSky.EnumToColor(SkyBlue));
        _mat.SetColor(_middleColor, GradientSky.EnumToColor(LightBlue));
        _mat.SetColor(_bottomColor, GradientSky.EnumToColor(DarkWhite));

        _isDay = true;
    }
}
