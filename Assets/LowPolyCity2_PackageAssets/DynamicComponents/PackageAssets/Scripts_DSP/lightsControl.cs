using UnityEngine;
using System.Collections;

// To control light effects like volumetric lights, lens flares, halo, spotlights etc.
// If you do not want light effects like in day scenes, just uncheck light status

public class lightsControl : MonoBehaviour {

	public GameObject volumetricLight; // which volmetric light gameobject?
	public GameObject lightSource; // which light source gameobject? (resource intensive)
	public GameObject lensFlare; // which lens flare or halo gameobject? (resource intensive)
	public GameObject halo; // which halo or gameobject?
	
	//public bool lightStatus=true; // Above objects on or off. 

	public bool volumetricLightStatus=false;
	public bool lightSourceStatus=false;
	public bool lensFlareStatus=false;
	public bool haloStatus=false;

	// Uncheck this if you do not want light effects like valumetric lights, lens flares or spotlights

	//------------------------------------------------------------------------------------------------

	// Function to switch on or off light effects. Means set their status
	// This function is Public 
	// It is public so that you can use it in your own scripts
	// Like a script to control time of day and accordingly set light effect status

	public void SetLights(bool a, bool b, bool c, bool d) {
		//
		if(volumetricLight!=null && lightSource!=null && lensFlare!=null && halo!=null)
		{
		volumetricLight.SetActive (a);
		lightSource.SetActive (b);
		lensFlare.SetActive (c);
		halo.SetActive (d); 
		}
	}
	//------------------------------------------------------------------------------------------------

	// Set light effect status
	void Start() {
		SetLights (volumetricLightStatus,lightSourceStatus,lensFlareStatus,haloStatus);
	}
}
