//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIGameRoomLoader : MonoBehaviour
{
	public RectTransform GameRoomPrefab;
	public RectTransform Canvas;
	public float TimeOut = 10;
	
	private float timeTemp;
	
	void Start ()
	{
		Refresh ();
	}

	void ClearCanvas ()
	{
		if (Canvas == null) 
			return;
			
		foreach (Transform child in Canvas.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	
	void OnEnable ()
	{
		
		Refresh ();
	}
	
	public void Refresh ()
	{
		// function for refreshing.
		if (UnitZ.gameManager) {
			UnitZ.gameManager.Refresh ();	
		}
		StartCoroutine (LoadGameRoom ());
	}

	public void DrawGameLobby ()
	{
		// just draw GameRoomPrefab to canvas
		if (UnitZ.gameManager == null || Canvas == null || GameRoomPrefab == null)
			return;
		
		if (UnitZ.gameNetwork.MatchListResponse != null) {	
			ClearCanvas ();
			for (int i=0; i<UnitZ.gameNetwork.MatchListResponse.Count; i++) {

				GameObject obj = (GameObject)GameObject.Instantiate (GameRoomPrefab.gameObject, Vector3.zero, Quaternion.identity);
				obj.transform.SetParent (Canvas.transform);
				GUIGameRoom room = obj.GetComponent<GUIGameRoom> ();
				RectTransform rect = obj.GetComponent<RectTransform> ();
				if (rect) {
					rect.anchoredPosition = new Vector2 (5, -((GameRoomPrefab.sizeDelta.y * i)));
					rect.localScale = GameRoomPrefab.gameObject.transform.localScale;
				}
						
				if (room.RoomName) {
					room.RoomName.text = UnitZ.gameNetwork.MatchListResponse[i].name + " " + UnitZ.gameNetwork.MatchListResponse[i].currentSize+"/"+UnitZ.gameNetwork.MatchListResponse[i].maxSize;
					room.Match = UnitZ.gameNetwork.MatchListResponse[i];
				}
						
			}
			RectTransform rootRect = Canvas.gameObject.GetComponent<RectTransform> ();
			rootRect.sizeDelta = new Vector2 (rootRect.sizeDelta.x, GameRoomPrefab.sizeDelta.y * UnitZ.gameNetwork.MatchListResponse.Count);
		}
	}
	
	IEnumerator LoadGameRoom ()
	{
		timeTemp = Time.time;
		bool timeOut = false;
		if (UnitZ.gameManager) {
			while (UnitZ.gameManager.IsRefreshing && !timeOut) {
				if (Time.time > timeTemp + TimeOut) {
					timeOut = true;
				}
				yield return new WaitForEndOfFrame();
			}
			// wait untill GameManager is refreshed
			// and all new gameroom are ready.
			timeTemp = Time.time;
			DrawGameLobby ();
		}
	}

}
