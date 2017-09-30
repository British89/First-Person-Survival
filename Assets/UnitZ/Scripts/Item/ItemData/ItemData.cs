//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]

public class ItemData : NetworkBehaviour
{
	//public Texture2D Image;
	public Sprite ImageSprite;
	public string ItemName;
	public string Description;
	public int Price;
	public bool Stack = true;
	public FPSItemEquipment ItemFPS;
	public ItemEquipment ItemEquip;
	[HideInInspector][SyncVar]
	public int Quantity = 1;
	[HideInInspector][SyncVar]
	public int NumTag = -1;
	public AudioClip SoundPickup;
	public string ItemID;


	public virtual void Pickup (CharacterSystem character)
	{
		character.SendMessage("PickupItemCallback",this);
		RemoveItem();
	}

	public void SetupDrop(int numtag,int num){
		//Debug.Log("Setup drop");
		NumTag = numtag;
		Quantity = num;
	}



	public void RemoveItem ()
	{
		Destroy(this.gameObject);
	}

	void Start ()
	{

	}
	
	public void FixedUpdate ()
	{
		ShowInfo = false;	
	}
	
	public void GetInfo ()
	{
		ShowInfo = true;
	}
	
	public bool ShowInfo;
	
	void OnGUI ()
	{
		if (ShowInfo && Camera.main) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label (new Rect (screenPos.x, Screen.height - screenPos.y, 200, 60), "Press F to Pickup\n" + ItemName + " x " + Quantity);
		}
	}
}
