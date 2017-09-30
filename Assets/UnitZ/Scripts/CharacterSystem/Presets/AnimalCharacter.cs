//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class AnimalCharacter : CharacterSystem
{

	public float DamageDirection = 0.5f;
	public float Force = 70;
	public float StandAttackDuration = 0.5f;
	private float timeTmp;
	
	void Awake ()
	{	
		SetupAwake ();	
	}
	
	void Start ()
	{	
		
	}

	void Update ()
	{
		UpdateFunction ();
		if (Time.time > timeTmp + StandAttackDuration) {
			spdMovAtkMult = 1;
		}
	}
	
	public void Leap ()
	{	

	}
	
	public void AfterAttack ()
	{

	}
	
	public void DoAttack ()
	{
		DoOverlapDamage (this.transform.position + DamageOffset, this.transform.forward * Force, Damage, DamageLength, DamageDirection, "", Team);
	}

	public override void PlayMoveAnimation (float magnitude)
	{
		if (animator) {
			if (magnitude > 0.4f) {
				animator.SetInteger ("StateAnimation", 1);
			} else {
				animator.SetInteger ("StateAnimation", 0);
			}
		}

		base.PlayMoveAnimation (magnitude);
	}

	public override void PlayAttackAnimation (bool attacking, int attacktype)
	{
		if (animator) {
			if (attacking) {
				animator.SetTrigger ("Attacking");
				spdMovAtkMult = 0;
				timeTmp = Time.time;
			}
		}
		base.PlayAttackAnimation (attacking, attacktype);
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

		if (isServer) {
			ItemDropAfterDead dropafterdead = this.GetComponent<ItemDropAfterDead> ();
			if (dropafterdead)
				dropafterdead.DropItem ();
		}

		base.OnThisThingDead ();
	}
}
