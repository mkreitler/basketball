using UnityEngine;
using System.Collections;

public class FlareSpawner : MonoBehaviour
{

    public GameObject _objFlare;

    public Transform[] _trnFlareSpawnPoints;

    public float _fMinTimeBetweenFlareSpawns;

    public float _fMaxTimeBetweenFlareSpawns;

    public float _fTimeUntilNextSpawn;

    public float _fRandomSpawnVariation;

	// Update is called once per frame
	void Update ()
    {

        //count down the time untill the next flare 
        _fTimeUntilNextSpawn -= Time.deltaTime;

        if(_fTimeUntilNextSpawn < 0)
        {
            _fTimeUntilNextSpawn += Random.Range(_fMinTimeBetweenFlareSpawns, _fMaxTimeBetweenFlareSpawns);

            SpawnFlare();
        }
	
	}

    public void SpawnFlare()
    {

        //spawn flare
        GameObject objFlare = Instantiate<GameObject>(_objFlare);

        //get random spawn point
        Vector3 vecSpawnPoint = _trnFlareSpawnPoints[Random.Range(0, _trnFlareSpawnPoints.Length)].position;

        //add random variation
        vecSpawnPoint += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * _fRandomSpawnVariation;

        //set the flare spawn point
        objFlare.transform.position = vecSpawnPoint;
    }
}
