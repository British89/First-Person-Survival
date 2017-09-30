using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent (typeof(CharacterDriver))]
[RequireComponent (typeof(CharacterLiving))]
[RequireComponent (typeof(CharacterInventory))]
[RequireComponent (typeof(FPSController))]
[RequireComponent (typeof(CharacterInventory))]

public class PlayerCharacter : CharacterSystem
{
	[HideInInspector]
	public bool ToggleFlashlight = false;
	[SyncVar]
	public Quaternion CameraRotation;

	void Awake ()
	{
		
		SetupAwake ();	
	}

	void Start ()
	{	
		DestroyOnDead = false;
		if (animator)
			animator.SetInteger ("Shoot_Type", AttackType);
	}

	void Update ()
	{
		if (animator == null)
			return;
		
		animator.SetInteger ("UpperState", 1);
		UpdateFunction ();
		
		if (Motor.controller.velocity.y < -20)
			ApplyDamage (10000, Motor.controller.velocity, "", "");
		
	}

	public override void PlayMoveAnimation (float magnitude)
	{
		if (animator) {
			if (magnitude > 0.4f) {
				animator.SetInteger ("LowerState", 1);
			} else {
				animator.SetInteger ("LowerState", 0);
			}
		}
	
		base.PlayMoveAnimation (magnitude);
	}

	public override void PlayAttackAnimation (bool attacking, int attacktype)
	{
		if (animator) {
			if (attacking) {
				animator.SetTrigger ("Shoot");
			}
			animator.SetInteger ("Shoot_Type", attacktype);
		}
		base.PlayAttackAnimation (attacking, attacktype);
	}

	
	public override void OnThisThingDead ()
	{
		if(ID != ""){
			RemoveCharacterData();
		}
		
		if (UnitZ.NetworkObject ().scoreManager) {
			UnitZ.NetworkObject ().scoreManager.AddDead (1, ID);
			if (ID != LastHitByID)
				UnitZ.NetworkObject ().scoreManager.AddScore (1, LastHitByID);
		}
		
		
		CharacterItemDroper itemdrop = this.GetComponent<CharacterItemDroper> ();
		if (itemdrop)
			itemdrop.DropItem ();
		base.OnThisThingDead ();
	}
}
