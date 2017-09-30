//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(NetworkIdentity))]
[RequireComponent (typeof(AudioSource))]
public class DamageManager : NetworkBehaviour
{
	public bool IsAlive = true;
	[SyncVar (hook = "OnHPChanged")]
	public int HP = 100;
	[SyncVar]
	public int HPmax = 100;
	[SyncVar]
	public int Armor = 0;
	[SyncVar]
	public int Armormax = 100;
	public GameObject DeadReplacement;
	public float DeadReplaceLifeTime = 180;
	public bool DestroyOnDead = true;
	public AudioClip[] SoundPain;
	public AudioSource Audiosource;


	[HideInInspector]
	public bool dieByLifeTime = false;
	[HideInInspector]
	public bool spectreThis = false;
	[HideInInspector][SyncVar]
	public string Team = "";
	[HideInInspector][SyncVar]
	public string ID = "";
	[HideInInspector][SyncVar]
	public string UserID = "";
	[HideInInspector][SyncVar]
	public string UserName = "";
	[HideInInspector][SyncVar]
	public string LastHitByID = "";

	[HideInInspector]
	public bool IsMine;
	private Vector3 directionHit;



	void Start ()
	{
		Audiosource = this.GetComponent<AudioSource> ();
	}

	public override void OnStartClient ()
	{
		if (HP <= 0) {
			SetEnable (false);
		}

		base.OnStartClient ();
	}


	void Update ()
	{
		DamageUpdate ();	
	}

	public void DamageUpdate ()
	{
		if (HP > HPmax)
			HP = HPmax;
			
		if (Armor > Armormax)
			Armor = Armormax;

		if (UnitZ.gameManager && UnitZ.gameManager.PlayerID == ID && ID != "") {
			IsMine = true;	
		} else {
			IsMine = false;	
		}
	}

	public void DirectDamage (DamagePackage pack)
	{
		Debug.Log ("Hit ");
		ApplyDamage ((int)((float)pack.Damage), pack.Direction, pack.ID, pack.Team);
	}

	public void ApplyDamage (int damage, Vector3 direction, string attackerID, string team)
	{
		lastHP = HP;
		directionHit = direction;
		DoApplyDamage (damage, direction, attackerID, team);
		if (Audiosource && SoundPain.Length > 0) {
			Audiosource.PlayOneShot (SoundPain [Random.Range (0, SoundPain.Length)]);	
		}
	}

	public void DoApplyDamage (int damage, Vector3 direction, string attackerID, string team)
	{
		if (isServer) {
			directionHit = direction;
			LastHitByID = attackerID;
			if (Team != team || team == "") {	
				if (HP <= 0)
					return;
				HP -= damage;
			}	
		}
	}

	private int lastHP = 0;

	void OnHPChanged (int hp)
	{
		HP = hp;

		if (hp <= 0) {
			SetEnable (false);
		} else {
			if (hp >= HPmax) {
				if (!IsAlive) {
					SetEnable (true);
				}
			}
		}

		if (!IsAlive && lastHP > 0) {

			if (isServer)
				CmdOnDead (LastHitByID, ID, "Kill");


			SpawnDeadBody ();
		}
		lastHP = HP;
	}


	void SpawnDeadBody ()
	{
		if (!isQuitting) {
			if (DeadReplacement) {
				GameObject deadbody = (GameObject)GameObject.Instantiate (DeadReplacement, this.transform.position, Quaternion.identity);
				if (spectreThis && deadbody) {
					LookAfterDead (deadbody.gameObject);
				}
				CopyTransformsRecurse (this.transform, deadbody);
				if (dieByLifeTime)
					DeadReplaceLifeTime = 3;
				GameObject.Destroy (deadbody, DeadReplaceLifeTime);
			}
		}
	}

	[Command]
	void CmdOnDead (string killer, string me, string killtype)
	{
		RpcODead (killer, me, killtype);
	}

	[ClientRpc]
	void RpcODead (string killer, string me, string killtype)
	{
		OnThisThingDead ();
		OnKilled (killer, me, killtype);
		if (DestroyOnDead)
			GameObject.Destroy (this.gameObject,5);
	}

	public void ReSpawn (string team, int spawner)
	{
		CmdRespawn (team, spawner);
	}

	public void ReSpawn (int spawner)
	{
		CmdRespawn (Team, spawner);
	}

	[Command]
	void CmdRespawn (string team, int spawner)
	{
		HP = HPmax;
		Team = team;
		RpcRespawn (team, spawner);
	}

	[ClientRpc]
	void RpcRespawn (string team, int spawner)
	{
		HP = HPmax;
		Team = team;
		if (isLocalPlayer) {
			this.transform.position = UnitZ.playerManager.FindASpawnPoint (spawner);
			Debug.Log ("RPC respawn");
		}
		OnRespawn ();
		this.SendMessage ("Respawn", SendMessageOptions.DontRequireReceiver);
	}


	public void SetEnable (bool enable)
	{
		IsAlive = enable;

		foreach (Transform ob in this.transform) {
			ob.gameObject.SetActive (enable);
		}

		if (this.GetComponent<PlayerView> ())
			this.GetComponent<PlayerView> ().enabled = enable;

		if (this.GetComponent<FPSInputController> ()) 
			this.GetComponent<FPSInputController> ().enabled = enable;

		if (this.GetComponent<FPSController> ())
			this.GetComponent<FPSController> ().enabled = enable;

		if (this.GetComponent<CharacterMotor> ()) {
			this.GetComponent<CharacterMotor> ().enabled = enable;
			this.GetComponent<CharacterMotor> ().Reset ();
		}

		if (this.GetComponent<CharacterController> ())
			this.GetComponent<CharacterController> ().enabled = enable;

		if (this.GetComponent<NetworkTransform> ())
			this.GetComponent<NetworkTransform> ().enabled = enable;

		if (this.GetComponent<CharacterDriver> ())
			this.GetComponent<CharacterDriver> ().NoVehicle ();

		Renderer[] renderers = this.GetComponentsInChildren<Renderer> ();

		foreach (Renderer ob in renderers) {
			ob.enabled = enable;
		}
	}

	public virtual void OnKilled (string killer, string me, string killtype)
	{
		// Do something when get killed
	}

	public virtual void OnThisThingDead ()
	{
		// Do something when dying
	}

	public virtual void OnRespawn ()
	{
		// Do something when respawn
	}

	public virtual void OnDestroyed ()
	{
		// De something before removed
	}

	public void CopyTransformsRecurse (Transform src, GameObject dst)
	{
		dst.transform.position = src.position;
		dst.transform.rotation = src.rotation;
		if (dst.GetComponent<Rigidbody> ())
			dst.GetComponent<Rigidbody> ().AddForce (directionHit / (float)dst.transform.childCount, ForceMode.VelocityChange);

		foreach (Transform child in dst.transform) {
			var curSrc = src.Find (child.name);
			if (curSrc) {
				CopyTransformsRecurse (curSrc, child.gameObject);
			}
		}
	}

	void LookAfterDead (GameObject obj)
	{
		//tell the spectre cam to look at this corpse
		SpectreCamera spectre = (SpectreCamera)GameObject.FindObjectOfType (typeof(SpectreCamera));
		if (spectre) {
			spectre.LookingAtObject (obj);
		}
	}

	private bool isQuitting = false;

	void OnApplicationQuit ()
	{ 
		isQuitting = true; 
	}

	public override void OnNetworkDestroy ()
	{
		if (isQuitting)
			return;

		OnDestroyed ();
		if (isServer) {
			if (ID != "") {
				if (UnitZ.NetworkObject ().playersManager != null) {
					UnitZ.NetworkObject ().playersManager.RemovePlayer (ID);
				}
			}
		}
		base.OnNetworkDestroy ();
	}


}
