//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterLiving : NetworkBehaviour
{
	
	[SyncVar]
	public int Hungry = 100;
	public int HungryMax = 100;
	[SyncVar]
	public int Water = 100;
	public int WaterMax = 100;

	[HideInInspector]
	public CharacterSystem character;
	[HideInInspector]
	
	void Start ()
	{
		character = this.GetComponent<CharacterSystem> ();
		
		InvokeRepeating ("stomachUpdate", 1.0f, 1.0f);
		InvokeRepeating ("hungryUpdate", 1.0f, 15.0f);
		InvokeRepeating ("thirstilyUpdate", 1.0f, 10.0f);
		
	}

	public void Respawn(){
		Hungry = HungryMax;
		Water = WaterMax;
	}

	void Update ()
	{
		if (Hungry < 0)
			Hungry = 0;
			
		if (Hungry > HungryMax)
			Hungry = HungryMax;
		
		if (Water < 0)
			Water = 0;
			
		if (Water > WaterMax)
			Water = WaterMax;
	}
	
	public void stomachUpdate ()
	{
		if (isServer) {
			if (character == null || (character && !character.IsAlive))
				return;
		
			if (Water <= 0) {	
				character.ApplyDamage (2, Vector3.up, character.ID, "");	
			}
			if (Hungry <= 0) {
			
				character.ApplyDamage (1, Vector3.up, character.ID, "");		
			}
		}
	}
	
	public void hungryUpdate ()
	{
		if (isServer) {
			Hungry -= 1;
		}
	}
	
	public void thirstilyUpdate ()
	{
		if (isServer) {
			Water -= 1;
		}
	}
	
	[Command]
	void CmdEatUpdate (int num)
	{
		Hungry += num;
	}

	public void Eating (int num)
	{
		CmdEatUpdate(num);
	}
	
	[Command]
	void CmdDrinkUpdate (int num)
	{
		Water += num;
	}


	public void Drinking (int num)
	{
		CmdDrinkUpdate(num);
	}
	
	[Command]
	void CmdHealthUpdate (int num)
	{
		if (character == null)
			return;
		
		character.HP += num;
	}

	public void Healing (int num)
	{
		if (character == null)
			return;

		CmdHealthUpdate(num);
	}
}
