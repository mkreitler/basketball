using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITYADS
using UnityEngine.Advertisements;
#endif
#if ADMOB
using GoogleMobileAds.Api;
#endif
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(_Advertisements_admin))]
public class AdEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		_Advertisements_admin _cnf = (_Advertisements_admin)target;
		//----------------------------------------------
		if (GUILayout.Button ("Use Only Admob")) {
			_cnf._only_admob ();
		}
		//----------------------------------------------
		if (GUILayout.Button ("Use Only UnityAds")) {
			_cnf._only_unityads ();
		}
		//----------------------------------------------
		if (GUILayout.Button ("Use Admob and UnityAds")) {
			_cnf._all ();
		}
		//----------------------------------------------
	}
}
#endif
//---------------------------------------
[System.Serializable]
public enum _action {Open_Game,Start_Game,Retry_Game,Game_Over,No_Action};
//---------------------------------------
[System.Serializable]
public class _unityads{
public _action _display_on;
}
//---------------------------------------
[System.Serializable]
public enum _banner_size{Standard_320x50,SmartBanner,IAB_Medium_Rectangle_300x250,IAB_Full_Size_728x90,IAB_Leaderboard_728x90};
[System.Serializable]
public enum _banner_position{Top_Center,Top_Left,Top_Right,Bottom_center,Bottom_Left,Bottom_Right};
[System.Serializable]
public class _banner{
	public string _ID;
	public _banner_size _banner_size;
	public _banner_position _banner_position;
	public _action _display_on;
    public _action _hidden_on = _action.No_Action;
}
[System.Serializable]
public class _interstitial{
	public string _ID;
	public _action _display_on;
}
//---------------------------------------
[System.Serializable]
public class _unity_ads{
public _action _display_on;
public _reward _reward;
}
[System.Serializable]
public enum _reward_type{No_Reward,Coins};
[System.Serializable]
public class _reward{
public _reward_type _type;
public int _quantity;
}
//----------------------------------------------
//----------------------------------------------
public class _Advertisements_admin : MonoBehaviour {
	//---------------------------------------
	public static _Advertisements_admin Instance;
	//---------------------------------------
	//---------------------------------------
//	#if UNITY_ANDROID || UNITY_IOS
	//---------------------------------------
	#if ADMOB
	public _banner[] _banner;
	public _interstitial[] _interstitial;

	private List<BannerView> bannerView = new List<BannerView>();
	private List<InterstitialAd> interstitial = new List<InterstitialAd>();
	#endif
	//---------------------------------------
	#if UNITYADS
	public _unity_ads[] _Unity_Ads;
	#endif
	bool _rewarded = false;
	_reward_type _REW;
	int _REWQ = 0;
	#if UNITY_EDITOR
	//---------------------------------------
	public void _only_admob(){
		//---------------------------------------
		string _d = "-define:ADMOB";
		_savefiles (_d);
		//---------------------------------------
	}
	//---------------------------------------
	public void _only_unityads(){
		//---------------------------------------
		string _d = "-define:UNITYADS";
		_savefiles (_d);
		//---------------------------------------
	}
	//---------------------------------------
	public void _all (){
		//---------------------------------------
		string _d = "-define:UNITYADS;ADMOB";
		_savefiles (_d);
		//---------------------------------------
	}
	//---------------------------------------
	void _savefiles(string _d){
		//---------------------------------------
		System.IO.File.WriteAllText("Assets/gmcs.rsp", _d);
		System.IO.File.WriteAllText("Assets/smcs.rsp", _d);
		System.IO.File.WriteAllText("Assets/us.rsp", _d);
		//---------------------------------------
	}
	#endif
	//---------------------------------------
	void Awake(){
		DontDestroyOnLoad (this);
		//---------------------------------------
		if(_Advertisements_admin.Instance==null){
				Instance = this;
		}
		//---------------------------------------
		#if ADMOB
		//---------------------------------------
		// Check banners
		//---------------------------------------
		if (bannerView.Count < _banner.Length) {
			_create_banners ();
		}
		//---------------------------------------
		if (interstitial.Count < _interstitial.Length) {
			_create_interstitial ();
		}
		//---------------------------------------
		#endif
		//---------------------------------------
		//---------------------------------------
		_Advertisements_admin.Instance._check_banner_action (_action.Open_Game, _action.No_Action);
		//---------------------------------------
	}
	#if UNITYADS
	//---------------------------------------
	IEnumerator ShowUnityAd() {
	while (!Advertisement.IsReady()) {
			yield return null;
		}
	    var options = new ShowOptions { resultCallback = HandleShowResult};
	    Advertisement.Show(options);
	}
	#endif
	//---------------------------------------
	// ONLY UNITYADS
	//---------------------------------------
	public void _Unity_Rewarded (){
		//---------------------------------------
		#if UNITYADS
		for(int i=0;i<_Unity_Ads.Length;i++){
		if(_Unity_Ads[i]._reward._type != _reward_type.No_Reward){
	      _REW = _Unity_Ads[i]._reward._type;
	      _REWQ = _Unity_Ads[i]._reward._quantity;
	      _rewarded = true;
		  StartCoroutine (ShowUnityAd ());
		  break;
		  }
		}
		#endif
		//---------------------------------------
	}
	//---------------------------------------
	public void _show_unityads(_action _A)
	{
		#if UNITYADS
		//---------------------------------------
		for(int i=0;i<_Unity_Ads.Length;i++){
			if(_Unity_Ads[i]._display_on == _A){
		       StartCoroutine (ShowUnityAd ());
				break;
			}
		}
		#endif
		//---------------------------------------
	}
	//---------------------------------------
	#if UNITYADS
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Time.timeScale = 1f;
			if (_rewarded) {
	            _finished_ad_reward();
			}
			_rewarded = false;
			break;
		case ShowResult.Skipped:
			Time.timeScale = 1f;
			_rewarded = false;
			break;
		case ShowResult.Failed:
			Time.timeScale = 1f;
			_rewarded = false;
			break;
		}
	}
	#endif
	//---------------------------------------
	//---------------------------------------
	// ONLY ADMOB
	//---------------------------------------
	#if ADMOB
	void _create_banners(){

		for (int i=0; i<_banner.Length; i++) {
			// CREATE NEW BANNER
			//---------------------------------------
			BannerView BV = new BannerView(_banner[i]._ID, _adsize(_banner[i]._banner_size), _adpos(_banner[i]._banner_position));
			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the banner with the request.
			BV.LoadAd(request);
			bannerView.Add(BV);
		}
	}
	//---------------------------------------
	//---------------------------------------
	void _create_interstitial(){

		for (int i=0; i<_interstitial.Length; i++) {
			// CREATE NEW INTERSTITIAL
			//---------------------------------------
			InterstitialAd BI = new InterstitialAd (_interstitial[i]._ID);
			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder ().Build ();
			// Load the banner with the request.
			BI.LoadAd (request);
			interstitial.Add(BI);
		}
	}
	//---------------------------------------
	//---------------------------------------
	AdPosition _adpos(_banner_position _pos){
		AdPosition _r = AdPosition.Bottom;
		
		switch (_pos) {
			
		case _banner_position.Bottom_center:
			_r = AdPosition.Bottom;
			break;
			//---------------------------------------
		case _banner_position.Bottom_Left:
			_r = AdPosition.BottomLeft;
			break;
			//---------------------------------------
		case _banner_position.Bottom_Right:
			_r = AdPosition.BottomRight;
			break;
			//---------------------------------------
		case _banner_position.Top_Center:
			_r = AdPosition.Top;
			break;
			//---------------------------------------
		case _banner_position.Top_Left:
			_r = AdPosition.TopLeft;
			break;
			//---------------------------------------
		case _banner_position.Top_Right:
			_r = AdPosition.TopRight;
			break;
		}
		return _r;
	}
	//---------------------------------------
	//---------------------------------------
	AdSize _adsize(_banner_size _siz){
		AdSize _r = AdSize.SmartBanner;

		switch (_siz) {

		case _banner_size.Standard_320x50:
			_r = AdSize.Banner;
			break;
		//---------------------------------------
		case _banner_size.SmartBanner:
			_r = AdSize.SmartBanner;
			break;
		//---------------------------------------
		case _banner_size.IAB_Leaderboard_728x90:
			_r = AdSize.Leaderboard;
			break;
		//---------------------------------------
		case _banner_size.IAB_Medium_Rectangle_300x250:
			_r = AdSize.MediumRectangle;
			break;
			//---------------------------------------
		}

		return _r;
	}
	//---------------------------------------
	//INTERSTITIAL
	//---------------------------------------
	//---------------------------------------
	
	public void show_interstitial (_action _C) {
		for(int i=0;i<_interstitial.Length;i++){
			if(_interstitial[i]._display_on == _C){
				interstitial[i].Show();
				break;
			}
		}
	}
	//---------------------------------------
	//---------------------------------------
	//BANNER
	//---------------------------------------
	//---------------------------------------
	public void show_banner (_action _C) {
		for(int i=0;i<_banner.Length;i++){
				if(_banner[i]._display_on == _C){
					bannerView[i].Show();
					break;
				}
		}
	}
	//---------------------------------------
	//---------------------------------------
	public void hidden_banner (_action _C) {
		for(int i=0;i<_banner.Length;i++){
	            if(_banner[i]._hidden_on == _C && _banner[i]._hidden_on != _action.No_Action){
					bannerView[i].Hide();
					break;
				}
		}
	}
	//---------------------------------------
#endif
	//---------------------------------------
	public void _check_banner_action(_action _show, _action _hidden = _action.No_Action){
		//---------------------------------------
		#if ADMOB
		show_banner (_show);
		show_interstitial (_show);
		if (_hidden != null) {
			hidden_banner (_hidden);
		}
		#endif
		//---------------------------------------
		#if UNITYADS
		_show_unityads(_show);
		#endif
		//---------------------------------------
	}
	//---------------------------------------

	void _finished_ad_reward(){
		switch(_REW)
		{
		}
	}
	//---------------------------------------
	//---------------------------------------
//	#endif
	//---------------------------------------
}