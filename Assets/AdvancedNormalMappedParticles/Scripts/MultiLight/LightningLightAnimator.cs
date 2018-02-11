using UnityEngine;
using System.Collections;

public class LightningLightAnimator : MonoBehaviour
{
    public Light _lhtLightingLight;

    public float _fMaxFlashLight;

    public float _fMinFlashLight;

    public float _fNonFlashLight;

    public int _iMaxFlashes;

    public int _iMinFlashes;

    public float _fMinTimeBetweenLighting;

    public float _fMaxTimeBetweenLightning;

    public float _fMinTimeBetweenLightningFlashes;

    public float _fMaxTimeBetweenLightningFlashes;

    public float _fMinFlashLength;

    public float _fMaxFlashLength;

    public float _fFlashLerpTime;



    //handel an individual flash;
    public IEnumerator Flash()
    {
        //calculate flash length
        float flashLength = Random.Range(_fMinFlashLength, _fMaxFlashLength);
        float fFlashBrightness = Random.Range(_fMinFlashLight, _fMaxFlashLight);
        float flerpTimeLeft = _fFlashLerpTime;

        //lerp in flash
        while (flerpTimeLeft > 0)
        {
            flerpTimeLeft -= Time.deltaTime;

            _lhtLightingLight.intensity = Mathf.Lerp(fFlashBrightness, _fNonFlashLight, Mathf.Clamp01( flerpTimeLeft / _fFlashLerpTime));

            yield return null;
        }

        //wait for flash
        yield return new WaitForSeconds(flashLength);


        //lerp out flash

        flerpTimeLeft = _fFlashLerpTime;

        while (flerpTimeLeft > 0)
        {
            flerpTimeLeft -= Time.deltaTime;

            _lhtLightingLight.intensity = Mathf.Lerp(_fNonFlashLight, fFlashBrightness,Mathf.Clamp01( flerpTimeLeft / _fFlashLerpTime));

            yield return null;
        }

    }

    //handel a sequence of flashes
    public IEnumerator FlashSequence()
    {
        int iFlashesInSequence = Random.Range(_iMinFlashes, _iMaxFlashes);

        for(int i = 0; i < iFlashesInSequence; i++)
        {
            //trigger flash
            yield return StartCoroutine(Flash());

            //wit for nex flash
            yield return new WaitForSeconds(Random.Range(_fMinTimeBetweenLightningFlashes, _fMaxTimeBetweenLightningFlashes));
        }
        
    }

    public IEnumerator LightningManager()
    {
        while(Application.isPlaying)
        {
            //wiat for lighting 
            yield return new WaitForSeconds(Random.Range(_fMinTimeBetweenLighting, _fMaxTimeBetweenLightning));

            //activate lightning 
            yield return StartCoroutine(FlashSequence());
        }
    }

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(LightningManager());
    }

}
