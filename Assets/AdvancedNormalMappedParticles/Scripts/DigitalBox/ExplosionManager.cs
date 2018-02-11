using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public bool _bFireOnce = false;

    public bool _bDestroyAfterTime = false;

    public float _fLifetime = 0;

    public float _fMinFrequency;
    public float _fMaxFrequency;

    public float _fTimeUntillNext = 0;

    public float _fWindZoneActiveTime;

    public float _fCurrentActiveTime = float.MaxValue;

    public float _fWindZoneMaxForce;

    public AnimationCurve _amcWindForceOverTime;

    public WindZone _wznExplosionWindZone;

    public ParticleSystem _prsParticleSystem;

    public int _iParticlesToEmmit;

    public void UpdateWindZone()
    {

        if (_fCurrentActiveTime > _fWindZoneActiveTime)
        {
            _wznExplosionWindZone.gameObject.SetActive(false);
            return;
        }
        else if(_fCurrentActiveTime > 0)
        {
            _wznExplosionWindZone.gameObject.SetActive(true);
        }

        //get percent through time
        float fPercentThroughTime = _fCurrentActiveTime / _fWindZoneActiveTime;

        //set wind force
        _wznExplosionWindZone.windMain = _amcWindForceOverTime.Evaluate(fPercentThroughTime) * _fWindZoneMaxForce;

        //update explosion time
        _fCurrentActiveTime += Time.deltaTime;

    }

    public void SpawnParticleEffect()
    {

        _prsParticleSystem.Emit(_iParticlesToEmmit);

    }

    public void TriggerExplosion()
    {
        _fCurrentActiveTime = 0;

        SpawnParticleEffect();

        _fTimeUntillNext = Mathf.Lerp(_fMinFrequency, _fMaxFrequency, Random.Range(0.0f, 1.0f));
    }

    // Use this for initialization
    void Start ()
    {
        if (_bFireOnce == true)
        {

            _fCurrentActiveTime = 0;
            SpawnParticleEffect();
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_bFireOnce == false)
        {

            //reduce the time untill the next explosion
            _fTimeUntillNext -= Time.deltaTime;

            if (_fTimeUntillNext < 0)
            {
                TriggerExplosion();
            }
        }
        //update particel effects
        UpdateWindZone();

        if(_bDestroyAfterTime == true)
        {
            _fLifetime -= Time.deltaTime;

            if (_fLifetime < 0)
            {
                Destroy(this.gameObject);
            }
        }

    }
}
