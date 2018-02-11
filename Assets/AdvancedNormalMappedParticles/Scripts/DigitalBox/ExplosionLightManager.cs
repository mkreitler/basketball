using UnityEngine;
using System.Collections;

public class ExplosionLightManager : MonoBehaviour
{
    public Light _lhtLight;

    public float _fBrightness;

    public float _fLife;

    public AnimationCurve _amcLightLifeBrightness;

    public IEnumerator ManageLight()
    {
        float fLifeRemaining = _fLife;

        while(fLifeRemaining > 0)
        {
            fLifeRemaining -= Time.deltaTime;

            float fLerpValue = 1 - Mathf.Clamp01(fLifeRemaining / _fLife);

            _lhtLight.intensity = _amcLightLifeBrightness.Evaluate(fLerpValue) * _fBrightness;

            yield return null;
        }
    }

	// Use this for initialization
	void Start ()
    {
        //manage te light brightness
        StartCoroutine(ManageLight());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
