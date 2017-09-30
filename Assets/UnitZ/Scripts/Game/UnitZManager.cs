using UnityEngine;
using System.Collections;

public class UnitZManager : MonoBehaviour {
	
	public string GameKeyVersion = "first_build";
	void Start(){
		
	}

	void Awake () {
		UnitZ.gameNetwork = (GameNetwork)GameObject.FindObjectOfType(typeof(GameNetwork));
		UnitZ.gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));
		UnitZ.characterManager = (CharacterManager)GameObject.FindObjectOfType(typeof(CharacterManager));
		UnitZ.itemManager = (ItemManager)GameObject.FindObjectOfType(typeof(ItemManager));
		UnitZ.itemCraftManager = (ItemCrafterManager)GameObject.FindObjectOfType(typeof(ItemCrafterManager));
		UnitZ.playerManager = (PlayerManager)GameObject.FindObjectOfType(typeof(PlayerManager));
		UnitZ.playerSave = (PlayerSave)GameObject.FindObjectOfType(typeof(PlayerSave));
		UnitZ.popup = (Popup)GameObject.FindObjectOfType(typeof(Popup));
		UnitZ.sceneManager = (SceneManager)GameObject.FindObjectOfType(typeof(SceneManager));
		UnitZ.aiManager = (AIManager)GameObject.FindObjectOfType(typeof(AIManager));
		UnitZ.Hud = (CharacterHUDCanvas)GameObject.FindObjectOfType(typeof(CharacterHUDCanvas));
		UnitZ.GameKeyVersion = GameKeyVersion;

	}



}
public static class UnitZ{
	public static AIManager aiManager;
	public static GameNetwork gameNetwork;
	public static GameManager gameManager;
	public static CharacterManager characterManager;
	public static ItemManager itemManager;
	public static ItemCrafterManager itemCraftManager;
	public static PlayerManager playerManager;
	public static PlayerSave playerSave;
	public static Popup popup;
	public static SceneManager sceneManager;
	public static CharacterHUDCanvas Hud;
	public static string GameKeyVersion = "";
	public static bool IsOnline = false;

	public static NetworkSyncObject NetworkObject(){
		return (NetworkSyncObject)GameObject.FindObjectOfType(typeof(NetworkSyncObject));
	}

}