using UnityEngine;
using System.Collections;

public class DigitalBoxParticleSubEmitter : MonoBehaviour
{
    //public Vector3 vecTempSpawnPos;


    public GameObject _objDigitalBox;

    public Material _matCubeMaterial;

    public Color _colActionEmissive;

    public Collider _colBulletColider;

    protected Vector3 _vecScale;

    public float _fDestroyTime;

    public float _fRespawnTime;

    public float _fSpawnInLerpTime;

    public ParticleSystem _psyBoxParticelSystem;

    public Transform _trnSpawnBox;

    public float _fColissionRadious;

    public float _fParticleSize;

    public float _fMinSizeToSpawn;

    public Vector2 _vecLifetime;

    public Vector2 _vecStartRotation;

    public Color _colColour;

    public Vector3 _vecSpawnVolume;

    public Vector3 _vecSpawnDevisionsPerAxis;

    public float _fTimeBetweenSpawns;
	
    public void SpawnInitalParticles()
    {
#if UNITY_5_2
        Debug.Log("Functionality nor supported in this unity version");
#else

        for ( int x = 0; x < _vecSpawnDevisionsPerAxis.x; x++)
        {
            for (int y= 0; y < _vecSpawnDevisionsPerAxis.y; y++)
            {
                for (int z = 0; z < _vecSpawnDevisionsPerAxis.z; z++)
                {
                    
                    Vector3 vecCordPercent = new Vector3(x / (_vecSpawnDevisionsPerAxis.x -1), y / (_vecSpawnDevisionsPerAxis.y - 1), z / (_vecSpawnDevisionsPerAxis.z - 1));

                    vecCordPercent -= new Vector3(0.5f, 0.5f, 0.5f);

                    vecCordPercent = Vector3.Scale(vecCordPercent, _vecSpawnVolume);

                    Vector3 vecWorldCord = _trnSpawnBox.TransformPoint(vecCordPercent);

                    //transfer world to particle effect
                  //  Vector3 vecParticelCord = _psyBoxParticelSystem.transform.worldToLocalMatrix * vecWorldCord;

                    //Debug.Log("ParticleLocal " + vecParticelCord.ToString() + "  world " + vecWorldCord.ToString());

                    float fSize = _fParticleSize * Mathf.Min(new float[] { _trnSpawnBox.lossyScale.x, _trnSpawnBox.lossyScale.y, _trnSpawnBox.lossyScale.z });

                    ParticleSystem.EmitParams empEmitParamiters = new ParticleSystem.EmitParams();

                    empEmitParamiters.startColor = _colColour;
                    empEmitParamiters.velocity = Vector3.up;
                    empEmitParamiters.position = vecWorldCord;
                    empEmitParamiters.startSize = fSize;
                    empEmitParamiters.startLifetime = Random.Range(_vecLifetime.x, _vecLifetime.y);
                    empEmitParamiters.rotation = Random.Range(_vecStartRotation.x, _vecStartRotation.y);

                    //spawn particle
                    _psyBoxParticelSystem.Emit(empEmitParamiters,1);
                   
                }
            }


        }
#endif
    }

    public IEnumerator TriggerParticleSpawn()
    {
        while (Application.isPlaying)
        { 
            yield return new WaitForSeconds(_fTimeBetweenSpawns);

            yield return StartCoroutine( BlowUpBox());
        }
    }

    public IEnumerator RespawnBox()
    {
        float fCurrentRespawnTime = _fSpawnInLerpTime;

        _objDigitalBox.SetActive(true);

        _objDigitalBox.transform.localScale = new Vector3(0, _vecScale.y, _vecScale.z);

        while (fCurrentRespawnTime > 0)
        {
            fCurrentRespawnTime -= Time.deltaTime;

            //calculate lerp factor
            float fLerpFactor = 1 - Mathf.Clamp01(fCurrentRespawnTime / _fSpawnInLerpTime);

            //set scale
            _objDigitalBox.transform.localScale = new Vector3(_vecScale.x * fLerpFactor, _vecScale.y, _vecScale.z);

            //fade material emissive
            _objDigitalBox.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _colActionEmissive * Mathf.LinearToGammaSpace(1 - fLerpFactor));

            yield return null;
        }

        _objDigitalBox.transform.localScale = _vecScale;

        _colBulletColider.enabled = true;

        yield return null;

    }

    public IEnumerator BlowUpBox()
    {
        float fcurrentDestroyTime = _fDestroyTime;
        _colBulletColider.enabled = false;
        while (fcurrentDestroyTime > 0)
        {
            fcurrentDestroyTime -= Time.deltaTime;

            //calculate lerp factor
            float fLerpFactor = 1 - Mathf.Clamp01(fcurrentDestroyTime / _fDestroyTime);

            //fade material emissive
            _objDigitalBox.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _colActionEmissive * Mathf.LinearToGammaSpace(fLerpFactor));

            yield return null;
        }

        //disble the box
        _objDigitalBox.SetActive(false);

        //spawn the particels
        SpawnInitalParticles();

        //start the respawn process
        yield return StartCoroutine(WaitForRespawn());
    }

    public IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(_fRespawnTime);

        yield return StartCoroutine(RespawnBox());
    }
    // Use this for initialization
	void Start ()
    {
        //store box scale
        _vecScale = _objDigitalBox.transform.localScale;

       StartCoroutine(TriggerParticleSpawn());
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void DestroyBox()
    {
        StartCoroutine(BlowUpBox());
    }

}
