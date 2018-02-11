using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoSceneControler : MonoBehaviour
{
    [System.Serializable]
    public class DemoScene
    {
        public GameObject _objSceneBaseObject;

        public Color _colDaytimeCameraColour;

        public Color _colNightimeCameraColour;

        public bool _bEnableFlairsAtNight;

        public float _fDaySunBrightness;

        public float _fNightSunBrightness;

        public bool _bEnableDescription;

        public string _strSceneDescription;
    }

    public DemoScene[] _dscDemoScenes;

    int _iSelectedScene;

    public float _fSettingChangeRate = 1;

    public bool _bIsDay;

    public GameObject _objFlareMaster;

    public Light _lhtSun;

    public Camera _camMainCam;

    public Text _txtDayNightText;

    public Text _txtDescriptionText;

    public GameObject _objDescriptionBox;

    public void UpdateFlareState()
    {
        if(_objFlareMaster == null)
        {
            return;
        }

        if(_dscDemoScenes[_iSelectedScene]._bEnableFlairsAtNight && (_bIsDay == false))
        {
            _objFlareMaster.SetActive(true);
        }
        else
        {
            _objFlareMaster.SetActive(false);
        }
    }

    public void UpdateSunBrightness()
    {
        float fTargetBrightness = _dscDemoScenes[_iSelectedScene]._fDaySunBrightness;

        if(_bIsDay == false)
        {
            fTargetBrightness = _dscDemoScenes[_iSelectedScene]._fNightSunBrightness;
        }

        _lhtSun.intensity = Mathf.Lerp(_lhtSun.intensity, fTargetBrightness, Mathf.Clamp01( _fSettingChangeRate * Time.deltaTime));
    }

    public void FadeCamera()
    {
        Color colTargetColour = _dscDemoScenes[_iSelectedScene]._colDaytimeCameraColour;

        if(_bIsDay == false)
        {
            colTargetColour = _dscDemoScenes[_iSelectedScene]._colNightimeCameraColour;
        }

        _camMainCam.backgroundColor = Color.Lerp(_camMainCam.backgroundColor, colTargetColour, Mathf.Clamp01(_fSettingChangeRate * Time.deltaTime));
    }

    public void UpdateSceneEnabledState()
    {
        for (int i = 0; i < _dscDemoScenes.Length; i++)
        {
            if( i != _iSelectedScene)
            {
                _dscDemoScenes[i]._objSceneBaseObject.SetActive(false);
            }
            else
            {
                _dscDemoScenes[i]._objSceneBaseObject.SetActive(true);
            }
        }
    }

    public void UpdateDayNightText()
    {
        if(_bIsDay)
        {
            _txtDayNightText.text = "Day";
        }
        else
        {
            _txtDayNightText.text = "Night";
        }
    }

    public void UpdateDescriptionText()
    {
        _txtDescriptionText.text = _dscDemoScenes[_iSelectedScene]._strSceneDescription;

        _objDescriptionBox.SetActive(_dscDemoScenes[_iSelectedScene]._bEnableDescription);
    }

    public void NextScene()
    {
        _iSelectedScene = (_iSelectedScene + 1) % _dscDemoScenes.Length;
    }

    public void LastScene()
    {
        _iSelectedScene--;

        if(_iSelectedScene < 0)
        {
            _iSelectedScene = _dscDemoScenes.Length - 1;
        }
    }

    public void TogleDayNight()
    {
        _bIsDay = !_bIsDay;
    }

    public void ChackForInputs()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextScene();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LastScene();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            TogleDayNight();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateDayNightText();
        UpdateDescriptionText();
        UpdateSceneEnabledState();
        UpdateFlareState();
        UpdateSunBrightness();
        FadeCamera();

        ChackForInputs();
    }
}
