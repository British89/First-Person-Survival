using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerConnector : NetworkBehaviour
{
	[SyncVar]
	public GameObject PlayingCharacter;
	public NetworkInstanceId NetID;
	public float SpawnwDelay = 1;
	public bool AutoSpawn = true;

	[SyncVar]
	public string ConnectID;
	private float timetmp;

	void Start ()
	{
		timetmp = Time.time;
	}

	public override void OnStartLocalPlayer ()
	{
		GetNetID ();
		base.OnStartLocalPlayer ();
	}

	void Update ()
	{
		if (!isLocalPlayer || ConnectID == "")
			return;

		if (PlayingCharacter == null) {
			if (UnitZ.playerManager.PlayingCharacter != null) {
				PlayingCharacter = UnitZ.playerManager.PlayingCharacter.gameObject;

				if (PlayingCharacter != null)
					CmdTellServerMyCharacter (PlayingCharacter.gameObject);
			}
		}

		UnitZ.gameManager.PlayerID = ConnectID;

		if (isLocalPlayer && PlayingCharacter == null && AutoSpawn) {
			if (Time.time > timetmp + SpawnwDelay) {
				CmdRequestSpawnPlayer (Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey,-1);
				Destroy(this.gameObject);
				timetmp = Time.time;
			}
		}
	}

	public void RequestSpawnWithTeam (string team, int spawnpoint)
	{
		CmdRequestSpawnWithTeam (Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey, team, spawnpoint);
		Destroy(this.gameObject);
	}

	public void RequestSpawn (int spawnpoint)
	{
		CmdRequestSpawnPlayer (Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey, spawnpoint);
		Destroy(this.gameObject);
	}

	[Command]
	public void CmdRequestSpawnPlayer (Vector3 position, string connectid, string userid, string usename, int characterindex, string characterkey, int spawn)
	{
		UnitZ.gameNetwork.RequestSpawnPlayer (position, connectid, userid, usename, characterindex, characterkey, "", spawn, this.connectionToClient);
		NetworkServer.Destroy (this.gameObject);
	}

	[Command]
	public void CmdRequestSpawnWithTeam (Vector3 position, string connectid, string userid, string usename, int characterindex, string characterkey, string team, int spawn)
	{
		UnitZ.gameNetwork.RequestSpawnPlayer (position, connectid, userid, usename, characterindex, characterkey, team, spawn, this.connectionToClient);
		NetworkServer.Destroy (this.gameObject);
	}

	[Client]
	void GetNetID ()
	{
		NetID = this.GetComponent<NetworkIdentity> ().netId;
		ConnectID = NetID.ToString ();
		CmdTellServerMyInfo (NetID.ToString (), UnitZ.gameManager.UserName, UnitZ.gameManager.Team, UnitZ.GameKeyVersion);
	}

	[Command]
	void CmdTellServerMyInfo (string id, string username, string team, string gamekey)
	{
		ConnectID = id;
		if (UnitZ.NetworkObject () && UnitZ.NetworkObject ().playersManager)
			UnitZ.NetworkObject ().playersManager.UpdatePlayerInfo (id, 0, 0, username, team, gamekey, true);

		Debug.Log (id + " has connect to the server");
	}

	[Command]
	void CmdTellServerMyCharacter (GameObject player)
	{
		PlayingCharacter = player;
	}

	/*void OnGUI ()
	{
		if (isLocalPlayer && PlayingCharacter == null) {
      
			MouseLock.MouseLocked = false;

			if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2, 200, 40), "Re Spawn")) {
				CmdRequestSpawnPlayer (Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey);
				Destroy (this.gameObject);
			}
		}
	}*/


}
