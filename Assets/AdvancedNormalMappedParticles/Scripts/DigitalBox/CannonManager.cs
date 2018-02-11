using UnityEngine;
using System.Collections;

public class CannonManager : MonoBehaviour
{
    public Transform _trnFireFromPoint;

    [Range(0,0.5f)]
    public float _fAimVariation;

    public float _fFireSpeed;

    public float _fMinTimeBetweenShots;

    public float _fMaxTimeBetweenShots;

    public float _fTimeUntillNextShot;

    public Rigidbody _rgbCannonBall;

    public ParticleSystem _psyCannonFireEffect;

    public int _iNumberOfParticlesToSpawn;

    void Start()
    {

        //update the time untill the next cannon shot
        _fTimeUntillNextShot = Random.Range(_fMinTimeBetweenShots, _fMaxTimeBetweenShots);
    }

	// Update is called once per frame
	void Update ()
    {
        //reduce time untill next cannon fireing
        _fTimeUntillNextShot -= Time.deltaTime;

        //check if cannon needs to shoot
        if(_fTimeUntillNextShot < 0)
        {
            FireCannon();

            _fTimeUntillNextShot = Random.Range(_fMinTimeBetweenShots, _fMaxTimeBetweenShots);
        }
    }
   
    //fire the cannon
    public void FireCannon()
    {
       //make cannon smoke
        _psyCannonFireEffect.Emit(_iNumberOfParticlesToSpawn);

        //spawn ball
        Rigidbody rgbBallToFire = Instantiate<Rigidbody>(_rgbCannonBall);

        //move ball to fire posittion
        rgbBallToFire.transform.position = _trnFireFromPoint.position;

        //get fire direction
        Vector3 vecFireDirection = _trnFireFromPoint.forward;

        //add random variation
        Vector3 vecRandomVariation = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * _fAimVariation;

        //build final direction
        vecFireDirection = (vecFireDirection + vecRandomVariation).normalized;

        //launch ball
        rgbBallToFire.velocity = vecFireDirection * _fFireSpeed;
    }
}
