using UnityEngine;
using System.Collections;

public class CannonBallManager : MonoBehaviour
{

    public Transform _objExplosion;

    void OnCollisionEnter(Collision collision)
    {
        //spawn explosion at this location
        Instantiate<Transform>(_objExplosion).position = this.transform.position;

        //destroy the cannonball
        Destroy(this.gameObject);
    }


}
