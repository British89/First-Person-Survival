//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(CharacterFootStep))]
[RequireComponent (typeof(FPSRayActive))]


public class CharacterSystem : DamageManager
{
	[HideInInspector][SyncVar]
	public string CharacterKey = "";
	[HideInInspector]
	public CharacterInventory inventory;
	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public FPSRayActive rayActive;
	[HideInInspector]
	public CharacterController controller;
	[HideInInspector]
	public CharacterMotor Motor;
	[HideInInspector]
	public bool Sprint;

	public float MoveSpeed = 0.7f;
	public float MoveSpeedMax = 5;
	public float TurnSpeed = 5;
	public float PrimaryWeaponDistance = 1;
	public int PrimaryItemType;
	public int AttackType = 0;
	public int Damage = 2;
	public float DamageLength = 1;
	public int Penetrate = 1;
	public Vector3 DamageOffset = Vector3.up;
	public AudioClip[] DamageSound;
	public AudioClip[] SoundIdle;
	[HideInInspector]
	public float spdMovAtkMult = 1;
	[SyncVar]
	private Vector3 positionSync;
	[SyncVar]
	private Quaternion rotationSync;
	public float SendRate = 5;
	[HideInInspector]
	public bool isSeeAround;
	[HideInInspector]
	public float currentSendingRate = 0;
	
	void Awake ()
	{
		SetupAwake ();
	}

	public void SetupAwake ()
	{
		/*if (PredictionObject)
			predictioner = (GameObject)GameObject.Instantiate (PredictionObject.gameObject, this.transform.position, this.transform.rotation);

		if (PredictionObjectLate)
			predictionerlate = (GameObject)GameObject.Instantiate (PredictionObjectLate.gameObject, this.transform.position, this.transform.rotation);

		*/
		DontDestroyOnLoad (this.gameObject);
		Motor = this.GetComponent<CharacterMotor> ();
		controller = this.GetComponent<CharacterController> ();
		Audiosource = this.GetComponent<AudioSource> ();
		animator = this.GetComponent<Animator> ();
		rayActive = this.GetComponent<FPSRayActive> ();
		inventory = this.GetComponent<CharacterInventory> ();
		spdMovAtkMult = 1;
		positionSync = this.transform.position;
		rotationSync = this.transform.rotation;
		positionLate = positionSync;
		positionLastTrip = positionSync;
	}

	void Update ()
	{
		UpdateFunction ();
	}

	public void UpdateFunction ()
	{
		currentSendingRate = SendRate;

		if (!isSeeAround && ID == "") {
			// reduce send rate if far from player
			currentSendingRate = 1;
		}

		UpdatePosition ();
		DamageUpdate ();
	}

	public virtual void PlayAttackAnimation (bool attacking, int attacktype)
	{

	}

	public virtual void PlayMoveAnimation (float magnitude)
	{
	}

	public void MoveAnimation ()
	{
		PlayMoveAnimation (Motor.OjectVelocity.magnitude);
	}

	public void MoveTo (Vector3 dir)
	{
		float speed = MoveSpeed;
		if (Sprint)
			speed = MoveSpeedMax;
		
		Move (dir * speed * spdMovAtkMult);
		MoveAnimation ();
	}

	public void MoveToPosition (Vector3 position)
	{
		float speed = MoveSpeed;
		if (Sprint)
			speed = MoveSpeedMax;
		Vector3 direction = (position - transform.position);
		direction = Vector3.ClampMagnitude (direction, 1);
		direction.y = 0;
		Move (direction.normalized * speed * direction.magnitude * spdMovAtkMult);
		if (direction != Vector3.zero) {
			Quaternion newrotation = Quaternion.LookRotation (direction);
			transform.rotation = Quaternion.Slerp (transform.rotation, newrotation, Time.deltaTime * TurnSpeed * direction.magnitude);
		}
		MoveAnimation ();
	}

	[Command (channel = 1)]
	public void CmdUpdateTransform (Vector3 position, Quaternion rotation)
	{
		RpcUpdateTransform (position, rotation);
	}


	[ClientRpc (channel = 1)]
	void RpcUpdateTransform (Vector3 position, Quaternion rotation)
	{
		positionLate = positionLastTrip;
		positionSync = position;
		rotationSync = rotation;


		latencyTime = Time.time - timeLastTrip;
		timeLastTrip = Time.time;
		Motor.OjectVelocity = (position - positionLastTrip);


		positionLastTrip = position;
		positionInterpolate = this.transform.position;
		rotationInterpolate = this.transform.rotation;

		/*if (predictioner) {
			predictioner.transform.position = position;
		}

		if (predictionerlate) {
			predictionerlate.transform.position = positionLate;
		}*/
	}


	private Vector3 positionLastTrip;
	private Vector3 positionLate;

	private float timeTmpsending;
	private float timeLastTrip;
	private float latencyTime = 0;
	private Vector3 positionInterpolate;
	private Quaternion rotationInterpolate;

	/*public GameObject PredictionObject;
	public GameObject PredictionObjectLate;
	private GameObject predictioner, predictionerlate;*/

	public void UpdatePosition ()
	{
		float fps = (1 / Time.deltaTime);
		float delay = (fps / currentSendingRate) * Time.deltaTime; // calculate delay of sending 

		if (Time.time > timeTmpsending + delay) { // sending by time
			if (IsAlive) {
				if (isLocalPlayer) {
					CmdUpdateTransform (this.transform.position, this.transform.rotation);
				} else {
					if (isServer && !isLocalPlayer && ID == "") {
						CmdUpdateTransform (this.transform.position, this.transform.rotation);
					}
				}
			}
			timeTmpsending = Time.time;
		}
			
		if ((!isLocalPlayer && ID != "") || (!isServer && ID == "")) {
			
			float lerpValue = (Time.time - timeLastTrip) / latencyTime;
			if (this.transform.parent == null) {
				positionInterpolate = Vector3.Lerp (positionLate, positionSync, lerpValue);
				this.transform.position = Vector3.Lerp (this.transform.position, positionInterpolate, 0.5f);
			}

			rotationInterpolate = Quaternion.Lerp (this.transform.rotation, rotationSync, lerpValue);
			this.transform.rotation =  rotationInterpolate;
		} 
		MoveAnimation ();
	}


	public void AttackAnimation (int attacktype)
	{
		AttackType = attacktype;
		CmdAttackAnimation (attacktype);
	}

	public void AttackAnimation ()
	{
		CmdAttackAnimation (AttackType);
	}

	[ClientRpc (channel = 0)]
	private void RpcAttackAnimation (int attacktype)
	{
		PlayAttackAnimation (true, attacktype);
	}

	[Command (channel = 0)]
	private void CmdAttackAnimation (int attacktype)
	{
		RpcAttackAnimation (attacktype);
	}

	public void AttackTo (Vector3 direction, int attacktype)
	{
		CmdattackTo (direction, attacktype);
	}

	[Command (channel = 0)]
	private void CmdattackTo (Vector3 direction, int attacktype)
	{
		PlayAttackAnimation (true, attacktype);
	}

	[Command (channel = 0)]
	public void CmddoDamage (Vector3 origin, Vector3[] direction, int damage, float distance, byte penetrate, string id, string team)
	{
		RpcdoDamage (origin, direction, damage, distance, penetrate, id, team);
	}

	[ClientRpc (channel = 0)]
	public void RpcdoDamage (Vector3 origin, Vector3[] direction, int damage, float distance, byte penetrate, string id, string team)
	{
		doDamage (origin, direction, damage, distance, (int)penetrate, id, team);
	}


	public void doDamage (Vector3 origin, Vector3[] direction, int damage, float distance, int penetrate, string id, string team)
	{
		if (rayActive) {
			if (rayActive.ShootRay (origin, direction, damage, distance, penetrate, id, team))
				PlayDamageSound ();
		}
		if (inventory && !IsMine) {
			inventory.EquipmentOnAction ();	
		}
	}

	public void DoDamage (Vector3 origin, Vector3[] direction, int damage, float distance, int penetrate, string id, string team)
	{
		CmddoDamage (origin, direction, damage, distance, (byte)penetrate, id, team);
	}

	public void DoDamage ()
	{
		Vector3[] direction = { this.transform.forward };
		CmddoDamage (this.transform.position + DamageOffset, direction, Damage, DamageLength, (byte)Penetrate, ID, Team);
	}

	[Command (channel = 0)]
	public void CmddoOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		RpcdoOverlapDamage (origin, direction, damage, distance, dot, id, team);
	}

	[ClientRpc (channel = 0)]
	public void RpcdoOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		doOverlapDamage (origin, direction, damage, distance, dot, id, team);
	}

	public void doOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		if (rayActive) {
			if (rayActive.Overlap (origin, direction, damage, distance, dot, id, team))
				PlayDamageSound ();
		}
				
		if (inventory && !IsMine) {
			inventory.EquipmentOnAction ();	
		}
	}

	public void DoOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		if (rayActive) {
			if (rayActive.OverlapTest (origin, direction, damage, distance, dot, id, team))
				PlayDamageSound ();
		}
		CmddoOverlapDamage (origin, direction, damage, distance, dot, id, team);
	}

	public void Checking (Vector3 origin, Vector3 direction)
	{
		if (rayActive) {
			rayActive.CheckingRay (origin, direction);
		}	
	}

	public void Interactive (Vector3 origin, Vector3 direction)
	{
		if (isLocalPlayer) {
			CmdInteractive (origin, direction);

			if (rayActive) {
				rayActive.ActiveLocalRay (origin, direction);
			}
		}
	}

	public void PickupItemCallback (ItemData item)
	{
		TargetReciveItem (connectionToClient, item.ItemID, item.NumTag, item.Quantity);
	}

	[TargetRpc]
	public void TargetReciveItem (NetworkConnection target, string itemid, int numtag, int num)
	{
		ItemData item = UnitZ.itemManager.GetItemDataByID (itemid);
		if (inventory != null && item != null) {
			if (inventory.AddItemTest (item, num)) {
				inventory.AddItemByItemData (item, num, numtag, -1);
				if (item.SoundPickup) {
					AudioSource.PlayClipAtPoint (item.SoundPickup, this.transform.position);	
				}
			}
		}
	}

	public void PickupItemBackpackCallback (ItemBackpack item)
	{
		TargetReciveItemBackpack (connectionToClient, item.SyncItemdata);
	}

	[TargetRpc]
	public void TargetReciveItemBackpack (NetworkConnection target, string itemdata)
	{
		
		if (inventory != null && itemdata != "") {
			inventory.AddItemFromText (itemdata);
		}
	}

	public void PickupStockCallback (ItemStocker stocker)
	{
		inventory.PeerTrade = stocker.inventory;
		stocker.inventory.PeerTrade = this.inventory;
		TargetReciveStock (connectionToClient, stocker.netId);
	}

	[TargetRpc]
	public void TargetReciveStock (NetworkConnection target, NetworkInstanceId objectid)
	{
		GameObject obj = ClientScene.FindLocalObject (objectid);
		if (obj) {
			ItemStocker itemstock = obj.GetComponent<ItemStocker> ();
			if (itemstock) {
				itemstock.inventory.PeerTrade = this.inventory;
				inventory.PeerTrade = itemstock.inventory;
				itemstock.PickUpStock (this);
			}
		}
	}

	[Command (channel = 0)]
	private void CmdInteractive (Vector3 origin, Vector3 direction)
	{
		if (rayActive) {
			rayActive.ActiveRay (origin, direction);
		}
	}

	public void Move (Vector3 directionVector)
	{
		if (Motor && Motor.isActiveAndEnabled) {
			Motor.inputMoveDirection = directionVector;
		}
	}

	public void PlayIdleSound ()
	{
		if (Audiosource && SoundIdle.Length > 0) {
			Audiosource.PlayOneShot (SoundIdle [Random.Range (0, SoundIdle.Length)]);	
		}
	}

	public void PlayDamageSound ()
	{
		if (Audiosource && DamageSound.Length > 0) {
			Audiosource.PlayOneShot (DamageSound [Random.Range (0, DamageSound.Length)]);	
		}
	}

	public void RemoveCharacterData ()
	{
		if (isServer) {
			if (UnitZ.playerSave) {
				UnitZ.playerSave.DeleteSave (UserID, CharacterKey, UserName);
			}
		}
	}


	public void SaveCharacterData ()
	{
		string savedata = UnitZ.playerSave.GetPlayerSaveToText (this);
		//Debug.Log ("Save data");
		CmdSaveCharacterData (savedata);
	}

	[Command (channel = 0)]
	void CmdSaveCharacterData (string savedata)
	{
		//Debug.Log("Save to server :"+savedata);
		if (UnitZ.playerSave) {
			UnitZ.playerSave.SaveToServer (savedata);
		}
	}

	public void LoadCharacterData ()
	{
		string hasKey = UserID + "_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name + "_" + CharacterKey + "_" + UserName;	
		if (isLocalPlayer) {
			//Debug.Log("get load "+hasKey);
			CmdGetSaveData (hasKey);
		}
	}

	[Command (channel = 0)]
	void CmdGetSaveData (string hasKey)
	{
		if (UserID == "")
			return;

		//Debug.Log ("Load from server " + hasKey);
		string data = UnitZ.playerSave.GetDataFromServer (hasKey);
		TargetGetData (connectionToClient, data);
	}

	[TargetRpc (channel = 0)]
	public void TargetGetData (NetworkConnection target, string data)
	{

		if (UnitZ.playerSave) {
			//Debug.Log("recived data "+data);
			UnitZ.playerSave.ReceiveDataAndApply (data, this);
		}
	}

	[Command (channel = 0)]
	public void CmdRequestSpawnObject (Vector3 position, Quaternion rotation, string itemID, string itemData)
	{

		ItemData itemdata = UnitZ.itemManager.GetItemDataByID (itemID);
		if (itemdata) {
			if (itemdata.ItemFPS) {
				FPSItemPlacing fpsplacer = itemdata.ItemFPS.GetComponent<FPSItemPlacing> ();
				if (fpsplacer) {
					if (fpsplacer.Item) {
						GameObject obj = UnitZ.gameNetwork.RequestSpawnObject (fpsplacer.Item, position, rotation);
						if (obj) {
							ObjectPlacing objplaced = obj.GetComponent<ObjectPlacing> ();
							objplaced.SetItemUID (objplaced.GetUniqueID ());
							objplaced.SetItemID (itemID);
							objplaced.SetItemData (itemData);
						}
					}
				}
			}
		}
	}

	[Command (channel = 0)]
	public void CmdRequestThrowObject (Vector3 position, Quaternion rotation, string itemID, Vector3 force)
	{
		ItemData itemdata = UnitZ.itemManager.GetItemDataByID (itemID);
		if (itemdata) {
			if (itemdata.ItemFPS) {
				FPSItemThrow fpsthrow = itemdata.ItemFPS.GetComponent<FPSItemThrow> ();
				if (fpsthrow) {
					if (fpsthrow.Item) {
						GameObject obj = UnitZ.gameNetwork.RequestSpawnObject (fpsthrow.Item, position, rotation);
						if (obj) {
							DamageBase dm = obj.GetComponent<DamageBase> ();
							if (dm) {
								dm.OwnerID = ID;
								dm.OwnerTeam = Team;
							}
							if (obj.GetComponent<Rigidbody> ())
								obj.GetComponent<Rigidbody> ().AddForce (force, ForceMode.Impulse);
						}
					}
				}
			}
		}
	}


	[Command (channel = 0)]
	public void CmdSendMessage (string text)
	{
		RpcGotMessage (text);
	}

	[ClientRpc (channel = 0)]
	void RpcGotMessage (string text)
	{
		if (UnitZ.NetworkObject ())
			UnitZ.NetworkObject ().chatLog.AddLog (text);
	}

}
