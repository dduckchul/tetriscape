using UnityEngine;
using static GradientSkyEnum;

public class TitleManager : MonoBehaviour
{
    private Material _mat;
    private bool _isDay = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _mat = GetComponent<Renderer>().material;
        if (_isDay)
        {
            Invoke("ChangeToNight", 1);
        }
    }

    void ChangeToNight()
    {
        _mat.SetColor((int)GradientSkyProp.TopColor, GradientSky.EnumToColor(Black));
        _mat.SetColor((int)GradientSkyProp.MiddleColor, GradientSky.EnumToColor(DarkBlack));
        _mat.SetColor((int)GradientSkyProp.BottomColor, GradientSky.EnumToColor(LightBlack));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.TopColor));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.MiddleColor));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.BottomColor));
        _isDay = false;
    }

    void ChangeToDay()
    {
        _mat.SetColor((int)GradientSkyProp.TopColor, GradientSky.EnumToColor(SkyBlue));
        _mat.SetColor((int)GradientSkyProp.MiddleColor, GradientSky.EnumToColor(LightBlue));
        _mat.SetColor((int)GradientSkyProp.BottomColor, GradientSky.EnumToColor(DarkWhite));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.TopColor));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.MiddleColor));
        Debug.Log(_mat.GetColor((int)GradientSkyProp.BottomColor));
        _isDay = true;
    }
}
