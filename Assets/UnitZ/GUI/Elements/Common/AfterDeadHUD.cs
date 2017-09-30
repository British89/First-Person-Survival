using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfterDeadHUD : MonoBehaviour {

	public Text SpawnText;
	public GameObject SpawnButton;

	public void ReSpawn(){
		if(UnitZ.playerManager.PlayingCharacter)
			UnitZ.playerManager.PlayingCharacter.ReSpawn (-1);

		this.gameObject.SetActive (false);
	}


	void Start () {
		
	}
	

	void Update () {
		if (UnitZ.playerManager) {
			if (UnitZ.playerManager.AutoRespawn) {
				if (SpawnText) {
					SpawnText.text = "respawn in "+((int)(UnitZ.playerManager.respawnTimer)).ToString () + " second";
				}
				if (SpawnButton)
					SpawnButton.SetActive (false);
			} else {
				SpawnText.text = "";
				if (SpawnButton)
					SpawnButton.SetActive (true);
			}
		}
	}
}
