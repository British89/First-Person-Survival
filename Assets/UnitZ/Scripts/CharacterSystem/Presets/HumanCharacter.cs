//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HumanCharacter : CharacterSystem
{

	void Awake ()
	{
		SetupAwake ();	
	}
	
	void Start ()
	{	
		if(animator)
			animator.SetInteger ("Shoot_Type", AttackType);
	}

	void Update ()
	{
		if(animator == null)
			return;
		
		animator.SetInteger ("UpperState", 1);
		UpdateFunction ();
		
		//if (Motor.controller.velocity.y < -20)
			//ApplyDamage (10000, Motor.controller.velocity, "", "");
		
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

	public override void OnKilled (string killer, string me, string killtype)
	{
		if (UnitZ.NetworkObject ().scoreManager) {
			if (ID != LastHitByID && LastHitByID != "" && me != "") {
				UnitZ.NetworkObject ().scoreManager.AddKillText (LastHitByID, ID, "Kill");
			}
		}
		base.OnKilled (killer, me, killtype);
	}
	
	public override void OnThisThingDead ()
	{
		// reset when dead

		if(ID != ""){
			RemoveCharacterData();
		}

		if (UnitZ.NetworkObject().scoreManager) {
			UnitZ.NetworkObject().scoreManager.AddDead (1, ID);
			if (ID != LastHitByID){
				UnitZ.NetworkObject().scoreManager.AddScore (1, LastHitByID);
			}
		}
		
		CharacterItemDroper itemdrop = this.GetComponent<CharacterItemDroper> ();
		if (itemdrop)
			itemdrop.DropItem ();

		if (isServer) {
			ItemDropAfterDead dropafterdead = this.GetComponent<ItemDropAfterDead> ();
			if (dropafterdead)
				dropafterdead.DropItem ();
		}

		base.OnThisThingDead ();
	}

	public override void OnRespawn ()
	{
		if (this.GetComponent<CharacterInventory> ())
			this.GetComponent<CharacterInventory> ().SetupStarterItem ();
		base.OnRespawn ();
	}
}
