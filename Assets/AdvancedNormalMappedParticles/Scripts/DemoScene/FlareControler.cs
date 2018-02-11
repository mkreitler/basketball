// KREITLER: Fixed line endings.
using UnityEngine;
using System.Collections;

public class FlareControler : MonoBehaviour
{

    public ParticleSystem _parFlareEmitter;

    public Light _lhtFlareLight;

    public float _fMaxLifeTime;

    public float _fMinLifeTime;

    public float _fSetLifeTime;

    public float _fLifeRemaining;

    public float _fLightBrightness;

    public AnimationCurve _amcBrightnessOverLife;

    public Color[] _colPossibleColours;

    public Color _colSetColour;

    public AnimationCurve _amcEmitColourBrightnessOverLife;

    public float _fFallRate;

    public AnimationCurve _amcFallRateOverLife;

    public float _fDeathCoolDown;

    public bool _bDestroying = false;

    public AnimationCurve _amcParticleAlpha;

	// Use this for initialization
	void Start ()
    {
        Setup();
    }
	
	// Update is called once per frame
	void Update ()
    {
        _fLifeRemaining -= Time.deltaTime;

        if(_fLifeRemaining < 0)
        {
            if (_bDestroying == false)
            {
                _bDestroying = true;

                StartCoroutine( DeathCoolDown());
            }
        }
        else
        {
            UpdateParticleColor();

            UpdateLightBrightness();

            UpdatePosittion();
        }
	}

    public void Setup()
    {
        //set life time
        _fSetLifeTime = Random.Range(_fMinLifeTime, _fMaxLifeTime);
        _fLifeRemaining = _fSetLifeTime;

        //set colour
        _colSetColour = _colPossibleColours[Random.Range(0, _colPossibleColours.Length )];

        //set light colour
        _lhtFlareLight.color = _colSetColour;
    }

    public void UpdatePosittion()
    {
        Vector3 vecPos = this.transform.position;

        float fFallRate = _fFallRate * Mathf.Clamp01(_amcFallRateOverLife.Evaluate(GetPercentThroughLife()));

        vecPos.y -= fFallRate * Time.deltaTime;

        this.transform.position = vecPos; 
    }

    public float GetPercentThroughLife()
    {
        return Mathf.Clamp01((_fSetLifeTime - _fLifeRemaining) / _fSetLifeTime);
    }

    public void UpdateParticleColor()
    {
        float fSmokeEmissive = Mathf.Clamp01(_amcEmitColourBrightnessOverLife.Evaluate(GetPercentThroughLife()));

        // KREITLER: Fixed warning below.
        // _parFlareEmitter.startColor = new Color(_colSetColour.r * fSmokeEmissive, _colSetColour.g * fSmokeEmissive, _colSetColour.b * fSmokeEmissive, _amcParticleAlpha.Evaluate(GetPercentThroughLife()));
        var main = _parFlareEmitter.main;
        main.startColor = new Color(_colSetColour.r * fSmokeEmissive, _colSetColour.g * fSmokeEmissive, _colSetColour.b * fSmokeEmissive, _amcParticleAlpha.Evaluate(GetPercentThroughLife()));
    }

    public void UpdateLightBrightness()
    {
        float fLuminance = _amcBrightnessOverLife.Evaluate(GetPercentThroughLife());

        _lhtFlareLight.intensity = _fLightBrightness * fLuminance;
    }

    public IEnumerator DeathCoolDown()
    {

       ParticleSystem.EmissionModule emmEmission = _parFlareEmitter.emission;

        // KREITLER: Fixed warning below.
        // emmEmission.rate = new ParticleSystem.MinMaxCurve(0);
        emmEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);


        //turn off light
        _lhtFlareLight.enabled = false;

        yield return new WaitForSeconds(_fDeathCoolDown);

        Destroy(this.gameObject);

        yield return null;
    }
}
