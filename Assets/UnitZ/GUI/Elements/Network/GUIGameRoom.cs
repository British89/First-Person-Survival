//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.UI;

public class GUIGameRoom : MonoBehaviour {
	
	public Text RoomName;
	public MatchInfoSnapshot Match;
	
	void Start () {
	}

	public void JoinRoom(){
		if(UnitZ.gameNetwork){
			UnitZ.gameNetwork.GameSelected (Match);
			MainMenuManager panelsManage = (MainMenuManager)GameObject.FindObjectOfType (typeof(MainMenuManager));
			if(panelsManage){
				panelsManage.StartType = UnitZGameType.Connect;
				panelsManage.OpenPanelByName("LoadCharacter");
			}
		}
	}
}
