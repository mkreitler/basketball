using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTransition : MonoBehaviour {

	/******************Public variables*********************/
	[Header("Material of Buildings")]
	public Material BuildingsMaterial;
	[Header("Corresponding Emission Texture")]
	public Texture EmissionTexture; 
	[Header("Max Emission Intensity")] 
	[ColorUsage(true,true,0.0f,8.0f,0.125f,3.0f)] 
	public Color EmmisionOn; 
	[Header("Main directional light in scene")]
	public Light mainLight; 
	[Header("Light intensity during night")]
	public float minLightIntensity; 
	[Header("Light intensity during day")]
	public float maxLightIntensity; 
	[Header("Fading time between day and night (seconds)")]
	public float FadeTime; 
	[Header("Total time between day and night (seconds)")]
	public float Interval; 
	[Header("Blended Skybox in scene")]
	public Material sky; 
	[Header("Status of light effects")]
	public bool VolumetricLight = false;
	public bool halo = false; 
	[Header("Directional lights and lens flares tax performance")]
	public bool LightSource = false;
	public bool lensFlare = false;


	[Header("GameObjects with LightControl script")]
	public GameObject[] LightControledObjects; 
	/********************************************************/



	/*******************Private Variables***************************/
	private float ct=0;
	private float t =0;
	private int x=1;
	private Color EmmisionOff = new Color (0.0F, 0.0F, 0.0F,1.0F); 
	/***************************************************************/



	void setLights(bool a, bool b, bool c, bool d) {
		foreach(GameObject g in LightControledObjects) {
			g.gameObject.GetComponentInChildren<lightsControl> ().SetLights (a, b, c, d); 
			g.gameObject.GetComponent<lightsControl> ().SetLights (a, b, c, d); 
		}
	}

	/********Fading between day and night ***************/
	void Transition (float fadingTime) {
		
		if(t>=1) {
			t = 0;
			x=-x;
			ct = Interval;
		}

		if (x == 1) {
			setLights (false, false, false, false);
			BuildingsMaterial.SetColor ("_EmissionColor", Color.Lerp (EmmisionOff, EmmisionOn, t));
			mainLight.intensity = Mathf.Lerp (maxLightIntensity, minLightIntensity, t); 
			sky.SetFloat ("_Blend", Mathf.Lerp (1.0f, 0.0f, t));
		}

		else if (x==-1) {
			setLights (VolumetricLight,LightSource,lensFlare,halo);

			BuildingsMaterial.SetColor ("_EmissionColor", Color.Lerp (EmmisionOn, EmmisionOff, t));
			mainLight.intensity = Mathf.Lerp (minLightIntensity, maxLightIntensity, t);
			sky.SetFloat ("_Blend", Mathf.Lerp (0.0f, 1.0f, t));
		}
		if(t<1) {
			t += (Time.deltaTime) / fadingTime;
		}

	}

	/************Initialize******************/
	void Start() {
		BuildingsMaterial.SetTexture ("_EmissionMap", EmissionTexture);
		Transition (FadeTime);
		ct = Interval;
	}

	void OnApplicationQuit() {
		setLights (false, false, false, false);
		mainLight.intensity = maxLightIntensity;
		BuildingsMaterial.SetColor ("_EmissionColor", EmmisionOff);
		sky.SetFloat ("_Blend", 1.0f);
		Debug.Log ("session ended");
	}
	/**************loop****************/
	void Update () { 
		if(ct>=0) {
			ct -= (Time.deltaTime);
			return;
			//return null;
		}
		else{
			Transition(FadeTime);
		}

	}
}

