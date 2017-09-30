using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncObject : NetworkBehaviour {

	public PlayersManager playersManager;
	public ScoreManager scoreManager;
	public ChatLog chatLog;
	public EnvironmentManager environment;

	void Awake () {
		playersManager = this.GetComponent<PlayersManager>();
		scoreManager = this.GetComponent<ScoreManager>();
		chatLog = this.GetComponent<ChatLog>();
		environment = this.GetComponent<EnvironmentManager>();

	}
	

	void Update () {
	
	}

}
