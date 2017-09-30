//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum UnitZGameType{
	Connect,HostOnline,HostLan,Single
}

public class MainMenuManager : PanelsManager
{
	public string SceneStart = "zombieland";
	public Text CharacterName;
	public GameObject Preloader;
	[HideInInspector]
	public UnitZGameType StartType = UnitZGameType.Single;
	private CharacterCreatorCanvas characterCreator;

	void Start ()
	{
		MouseLock.MouseLocked = false;
		characterCreator = (CharacterCreatorCanvas)GameObject.FindObjectOfType (typeof(CharacterCreatorCanvas));
		// load latest scene played
		if (PlayerPrefs.GetString ("StartScene") != "") {
			SceneStart = PlayerPrefs.GetString ("StartScene");
		}
	}

	void Update ()
	{
		if (CharacterName && UnitZ.gameManager) {
			CharacterName.text = UnitZ.gameManager.UserName;
		}
	}


	public void LevelSelected (string name)
	{
		SceneStart = name;
		// save scene for the next time.
		PlayerPrefs.SetString ("StartScene", SceneStart);

	}

	public void StopFindGame(){
		if(UnitZ.gameNetwork)
			UnitZ.gameNetwork.StopMatchMaker();
	}

	public void ConnectIP ()
	{
		StartType = UnitZGameType.Connect;
		OpenPanelByName ("LoadCharacter");
	}

	public void StartSinglePlayer ()
	{
		if (UnitZ.gameManager) {
			UnitZ.gameNetwork.StopMatchMaker();
			StartType = UnitZGameType.Single;
			OpenPanelByName ("LoadCharacter");
		}
	}

	public void HostGameOnline ()
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.StartMatchMaker();
			StartType = UnitZGameType.HostOnline;
			OpenPanelByName ("LoadCharacter");
		}		
	}
	public void HostGame ()
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.StopMatchMaker();
			StartType = UnitZGameType.HostLan;
			OpenPanelByName ("LoadCharacter");
		}		
	}

	public void FindInternetGame(){
		UnitZ.gameNetwork.StartMatchMaker();
		OpenPanelByName ("FindGame");
	}

	public void EnterWorld ()
	{
		if (UnitZ.gameManager) {
			if (characterCreator) 
				characterCreator.SetCharacter ();
			
			UnitZ.gameManager.StartGame (SceneStart,StartType);
			OpenPanelByName ("Connecting");
		}
	}

	public void ConnectingDeny ()
	{
		UnitZ.gameNetwork.Disconnect ();
	}

	public void ExitGame ()
	{
		UnitZ.gameNetwork.Disconnect ();
		Application.Quit ();	
	}
}
