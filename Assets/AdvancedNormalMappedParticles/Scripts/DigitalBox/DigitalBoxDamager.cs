using UnityEngine;
using System.Collections;

public class DigitalBoxDamager : MonoBehaviour
{


    public void OnCollisionEnter(Collision colCollision)
    {

        if(colCollision.gameObject.GetComponent<DigitalBoxParticleSubEmitter>() != null)
        {
            colCollision.gameObject.GetComponent<DigitalBoxParticleSubEmitter>().DestroyBox();
        }
    }

}
