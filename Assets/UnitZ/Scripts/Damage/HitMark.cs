//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public struct DamagePackage
{
	public Vector3 Position;
	public Vector3 Direction;
	public Vector3 Normal;
	public int Damage;
	public string ID;
	public string Team;
	public byte DamageType;
}

public class HitMark : MonoBehaviour
{

	public DamageManager DamageManage;
	public GameObject HitFX;
	public float DamageMult = 1;
	public bool Freeze = false;

	void Start ()
	{
		if (this.transform.root) {
			DamageManage = this.transform.root.GetComponent<DamageManager> ();
		} else {
			DamageManage = this.transform.GetComponent<DamageManager> ();
		}
	}

	public void OnHit (DamagePackage pack)
	{
		if (DamageManage) {

			// apply damage to damage manager
			DamageManage.ApplyDamage ((int)((float)pack.Damage * DamageMult), pack.Direction, pack.ID, pack.Team);

			// show hit effect in crosshair
			if (UnitZ.gameManager != null && UnitZ.gameManager.PlayerID == pack.ID) {
				if (UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null) {
					if(UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment!=null){
						if(UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair> ()){
							UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair> ().Hit();
						}
					}
				}
			}
		}

		if (!Freeze) {
			this.transform.position += pack.Direction.normalized;
		}

		// add particle effect at hit position
		ParticleFX (pack.Position, pack.Normal);
		
	}
	public void OnHitTest (DamagePackage pack)
	{
		if (DamageManage) {
			// show hit effect in crosshair
			if (UnitZ.gameManager != null && UnitZ.gameManager.PlayerID == pack.ID) {
				if (UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null) {
					if(UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment!=null){
						if(UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair> ()){
							UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair> ().Hit();
						}
					}
				}
			}
		}

		ParticleFX (pack.Position, pack.Normal);
		
	}
	public void ParticleFX (Vector3 position, Vector3 normal)
	{
		if (HitFX) {
			GameObject fx = (GameObject)GameObject.Instantiate (HitFX, position, Quaternion.identity);
			fx.transform.forward = normal;
			GameObject.Destroy (fx, 3);
		}
	}
}
