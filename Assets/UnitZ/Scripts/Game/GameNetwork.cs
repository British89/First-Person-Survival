using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class GameNetwork : NetworkManager
{

	[HideInInspector]
	public List<MatchInfoSnapshot> MatchListResponse;
	[HideInInspector]
	public MatchInfoSnapshot MatchSelected;
	public GameObject NetworkSyncObject;


	void Start ()
	{

	}

	public override void OnServerReady (NetworkConnection conn)
	{
		if (NetworkServer.active) {
			Debug.Log ("Server is Initialized!");
			if (NetworkSyncObject != null && !UnitZ.NetworkObject ()) {
				GameObject networkobject = (GameObject)GameObject.Instantiate (NetworkSyncObject, Vector3.zero, Quaternion.identity);
				NetworkServer.Spawn (networkobject);
			}
		}
		base.OnServerReady (conn);
	}

	public void RequestSpawnPlayer (Vector3 position, string connectid, string userid, string usename, int characterindex, string characterkey,string team,int spawnpoint, NetworkConnection conn)
	{
		GameObject player = UnitZ.playerManager.InstantiatePlayer (connectid, userid, usename, characterkey, characterindex, team, spawnpoint);
		if (player == null)
			return;
		
		player.GetComponent<CharacterSystem> ().ID = connectid;
		NetworkServer.ReplacePlayerForConnection (conn, player, 0);
		Debug.Log ("Spawn player " + connectid + " info " + characterindex + " key " + characterkey);
	}

	public GameObject RequestSpawnObject(GameObject gameobj,Vector3 position,Quaternion rotation){
		GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
		NetworkServer.Spawn(obj);
		return obj;
	}

	public GameObject RequestSpawnItem(GameObject gameobj,int numtag,int num,Vector3 position,Quaternion rotation){
		//Debug.Log("Request spawn object : "+gameobj+" numtag : "+numtag+" num : "+num);
		GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
		ItemData data = (ItemData)obj.GetComponent<ItemData> ();
		data.SetupDrop(numtag,num);
		NetworkServer.Spawn(obj);
		return obj;
	}

	public GameObject RequestSpawnBackpack(GameObject gameobj,string backpackdata,Vector3 position,Quaternion rotation){
		//Debug.Log("Request spawn object : "+gameobj+" numtag : "+numtag+" num : "+num);
		GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
		ItemBackpack data = (ItemBackpack)obj.GetComponent<ItemBackpack> ();
		data.SetDropItem(backpackdata);
		NetworkServer.Spawn(obj);
		return obj;
	}


	public void FindInternetMatch (string matchName)
	{
		MatchListResponse = null;
		singleton.StartMatchMaker ();
		singleton.matchMaker.ListMatches (0, 50, "", false, 0, 0, OnMatchList);
	}

	public override void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
	{
		MatchListResponse = matchList;
		if (MatchListResponse != null && success) {
			if (matchList.Count != 0) {
				Debug.Log ("Server lists ");
				for (int i = 0; i < MatchListResponse.Count; i++) {
					Debug.Log ("Game " + MatchListResponse [i].name + " " + MatchListResponse [i].currentSize + "/" + MatchListResponse [i].maxSize);
				}
			} else {
				Debug.Log ("No matches in requested room!");
			}
		} else {
			Debug.LogError ("Couldn't connect to match maker");
		}

	}

	public void HostGame (string levelname, bool online)
	{
		onlineScene = levelname;
		MatchSelected = null;
		singleton.networkAddress = networkAddress;
		singleton.networkPort = networkPort;

		if (online) {
			singleton.matchMaker.CreateMatch (matchName, (uint)maxConnections, true, "", "", "", 0,0, OnMatchCreate);
		} else {
			singleton.StartHost ();
		}
	}

	public void HostGameSolo(string levelname){
		onlineScene = levelname;
		MatchSelected = null;
		singleton.StartHost (new ConnectionConfig(),1);
	}

	public void GetStartMatchMaker(bool active){
		if(active){
			singleton.StartMatchMaker();
		}else{
			singleton.StopMatchMaker();
		}
	}

	public void JoinGame ()
	{

		if (MatchSelected != null) {
			singleton.matchMaker.JoinMatch (MatchSelected.networkId, "", "", "", 0, 0, OnMatchJoined);
			Debug.Log ("Connecting to matchMaker");
			MatchSelected = null;
		} else {
			singleton.networkAddress = networkAddress;
			singleton.networkPort = networkPort;
			singleton.StartClient ();
			Debug.Log ("Connecting to IP : " + networkAddress);
		}

	}


	public override void OnMatchCreate (bool success, string extendedInfo,  MatchInfo matchInfo)
	{
		if (matchInfo != null && success) {
			Debug.Log ("Create match succeeded "+matchInfo.networkId+" port:"+matchInfo.port+" domain:"+matchInfo.domain);
			NetworkServer.Listen (matchInfo, 9000);
			singleton.StartHost (matchInfo);
		} else {
			Debug.LogError ("Create match failed");
		}
	}


	public void GameSelected (MatchInfoSnapshot match)
	{
		Debug.Log ("Select Game");
		MatchSelected = match;
	}

	public override void OnMatchJoined (bool success, string extendedInfo, MatchInfo matchInfo)
	{
		Debug.Log ("Connecting success " + success + " " + extendedInfo+" "+matchInfo);
		if (success) {
			singleton.StartClient (matchInfo);
			Debug.Log ("Connected!");
		} else {
			if (UnitZ.popup != null)
				UnitZ.popup.ShowPopup ("Connecting failed");
		}

	}


	public void Disconnect ()
	{
		MatchSelected = null;
		singleton.StopHost ();
	}
}
