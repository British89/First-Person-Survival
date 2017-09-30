//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

	public string UserName = "";
	public string Team = "";
	public string UserID = "";
	public string CharacterKey = "";

	[HideInInspector]
	public bool IsRefreshing = false;
	[HideInInspector]
	public string PlayerID = "";
	[HideInInspector]
	public AsyncOperation loadingScene;


	void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);

	}

	void Start ()
	{
		UserName = PlayerPrefs.GetString ("user_name");
	}


	void Update ()
	{
		if (IsRefreshing) {
			if (UnitZ.gameNetwork.MatchListResponse != null && UnitZ.gameNetwork.MatchListResponse.Count > 0) {
				IsRefreshing = false;
			}
		}
	}


	public void StartGame (string level, UnitZGameType gametype)
	{
		if (gametype == UnitZGameType.Single) {
			Debug.Log("Single Player Game");
			UnitZ.gameNetwork.HostGameSolo (level);
		} 

		if (gametype == UnitZGameType.HostOnline) {
			Debug.Log("Host Game");
			UnitZ.gameNetwork.HostGame (level,true);
		}

		if (gametype == UnitZGameType.HostLan) {
			Debug.Log("Host Game");
			UnitZ.gameNetwork.HostGame (level,false);
		}

		if (gametype == UnitZGameType.Connect) {
			Debug.Log("Connect Game");
			UnitZ.gameNetwork.JoinGame ();
		}

		PlayerPrefs.SetString ("user_name", UserName);
	}


	public void RestartGame ()
	{
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();
	}

	public void QuitGame ()
	{
		
		if (UnitZ.NetworkObject() != null)
			UnitZ.NetworkObject().chatLog.Clear ();
		
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();

		UnitZ.gameNetwork.Disconnect ();
		
	}


	public void Refresh ()
	{
		
		UnitZ.gameNetwork.FindInternetMatch ("");
		IsRefreshing = true;
	}


	public void StartLoadLevel (string level)
	{
		loadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level);
	}

}
