using UnityEngine;
using System.Collections;

public class _pausegame : MonoBehaviour {
	//---------------------------------------
	public void _pause () {
		Time.timeScale = 0f;
		GetComponent<hud_control> ()._objects_hud_control [6].SetActive (true);

		#if ADMOB
		//---------------------------------------
		// CHEK BANNER
		//---------------------------------------
		_admob_admin.Instance._check_banner_action (_admob_active.Pause,_admob_hidden.Pause);
		//---------------------------------------
		//---------------------------------------
		#endif
	}
	//---------------------------------------
	public void _resume () {
		Time.timeScale = 1f;
		GetComponent<hud_control> ()._objects_hud_control [6].SetActive (false);

		#if ADMOB
		//---------------------------------------
		// CHEK BANNER
		//---------------------------------------
		_admob_admin.Instance._check_banner_action (_admob_active.Pause_Resume, _admob_hidden.Pause_Resume);
		//---------------------------------------
		//---------------------------------------
		#endif
	}
	//---------------------------------------
}
