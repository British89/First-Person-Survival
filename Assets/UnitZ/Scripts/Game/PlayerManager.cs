//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour
{
	[HideInInspector]
	public CharacterSystem PlayingCharacter;
	public float AutoRespawnDelay = 3;
	public bool AutoRespawn = false;
	public bool AskForRespawn = true;
	public float SaveInterval = 5;
	public bool SavePlayer = true;

	private SpectreCamera Spectre;
	private float timeTemp = 0;
	private bool savePlayerTemp;
	private float timeAlivetmp = 0;
	private bool autoRespawntmp = false;
	private bool askForRespawntmp = true;
	[HideInInspector]
	public float respawnTimer;

	void Start ()
	{
		savePlayerTemp = SavePlayer;
		autoRespawntmp = AutoRespawn;
		askForRespawntmp = AskForRespawn;
	}

	public void Reset ()
	{
		SavePlayer = savePlayerTemp;
		AutoRespawn = autoRespawntmp;
		AskForRespawn = askForRespawntmp;
	}

	void Update ()
	{
		OnPlaying ();
	}


	public void Respawn (int spawner)
	{
		if (PlayingCharacter && !PlayingCharacter.IsAlive) {
			PlayingCharacter.ReSpawn (spawner);
		}
	}

	public void Respawn (string team, int spawner)
	{
		if (PlayingCharacter && !PlayingCharacter.IsAlive) {
			PlayingCharacter.ReSpawn (team, spawner);
		}
	}


	public void OnPlaying ()
	{
		if (PlayingCharacter) {
			if (UnitZ.playerSave && PlayingCharacter.IsAlive) {
				if (Time.time >= timeTemp + SaveInterval) {
					timeTemp = Time.time;
					if (SavePlayer) {
						PlayingCharacter.SaveCharacterData ();
						//Debug.Log ("Chaacter saved ("+Time.timeSinceLevelLoad+")");
					}
				}
				if (UnitZ.Hud.IsPanelOpened ("Afterdead")) {
					UnitZ.Hud.ClosePanelByName ("Afterdead");
				}
				timeAlivetmp = Time.time;
			}
			if (!PlayingCharacter.IsAlive && AutoRespawn) {
				respawnTimer = (timeAlivetmp + AutoRespawnDelay) - Time.time;

				if (Time.time > timeAlivetmp + AutoRespawnDelay) {
					Respawn (-1);
					UnitZ.Hud.ClosePanelByName ("Afterdead");
					Debug.Log ("Chaacter respawned ("+Time.timeSinceLevelLoad+")");
				}
			}
			if (!PlayingCharacter.IsAlive && AskForRespawn) {
				MouseLock.MouseLocked = false;
				if (!UnitZ.Hud.IsPanelOpened ("Afterdead")) {
					UnitZ.Hud.OpenPanelByName ("Afterdead");
				}
			}

		} else {
			findPlayerCharacter ();
			if (PlayingCharacter == null) {
				//Debug.LogWarning ("Can't find player (CharacterSystem) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
			}
		} 
		
		if (Spectre != null && PlayingCharacter) {
			if (!PlayingCharacter.IsAlive) {
				Spectre.Active (true);
			} else {
				Spectre.Active (false);	
				Spectre.LookingAt (PlayingCharacter.gameObject.transform.position);
				PlayingCharacter.spectreThis = true;
			}
		} else {
			
			Spectre = (SpectreCamera)GameObject.FindObjectOfType (typeof(SpectreCamera));	
			if (Spectre == null) {
				//Debug.LogWarning ("Can't find (SpectreCamera) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
			}
		}
	}

	private void findPlayerCharacter ()
	{
		if (PlayingCharacter == null) {
			CharacterSystem[] go = (CharacterSystem[])GameObject.FindObjectsOfType (typeof(CharacterSystem));
			for (int i = 0; i < go.Length; i++) {
				CharacterSystem character = go [i];
				if (character) {
					if (character.IsMine) {
						PlayingCharacter = character;
						PlayingCharacter.LoadCharacterData ();
					}
				}
			}
		}
	}

	public Vector3 FindASpawnPoint (int spawner)
	{
		Vector3 spawnposition = Vector3.zero;
		PlayerSpawner[] spawnPoint = (PlayerSpawner[])GameObject.FindObjectsOfType (typeof(PlayerSpawner));
		
		if (spawner < 0 || spawner >= spawnPoint.Length) {
			spawner = Random.Range (0, spawnPoint.Length);
		}
		if (spawnPoint.Length > 0) {
			spawnposition = spawnPoint [spawner].transform.position;
		}

		return spawnposition;
	}

	public GameObject InstantiatePlayer (string playerID, string userID, string userName, string characterKey, int characterIndex, string team, int spawner)
	{
		if (UnitZ.characterManager == null || UnitZ.characterManager.CharacterPresets.Length <= characterIndex || characterIndex < 0)
			return null;

		CharacterSystem characterspawn = UnitZ.characterManager.CharacterPresets [characterIndex].CharacterPrefab;

		if (characterspawn) {

			GameObject player = (GameObject)GameObject.Instantiate (characterspawn.gameObject, FindASpawnPoint (spawner), Quaternion.identity);
			CharacterSystem character = player.GetComponent<CharacterSystem> ();
			character.ID = playerID;
			character.Team = team;
			character.CharacterKey = characterKey;
			character.UserName = userName;
			character.UserID = userID;

			MouseLock.MouseLocked = true;
			return player;
		}
		return null;
	}

}
