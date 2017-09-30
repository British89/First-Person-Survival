using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class DoorFrame : NetworkBehaviour {

	public Animator animator;
	[SyncVar(hook="OnDoorOpen")]
	public bool IsOpen;
	private float timeTemp;
	public float Cooldown = 0.5f;
	public string DoorKey = "";

	void Start ()
	{
		if (animator == null)
			animator = this.GetComponent<Animator> ();
	}
	public override void OnStartClient ()
	{
		OnDoorOpen(IsOpen);
		base.OnStartClient ();
	}

	void OnDoorOpen(bool open){
		IsOpen = open;
		if (animator) {
			animator.SetBool ("IsOpen", IsOpen);	
		}

	}
	
	public void Access (CharacterSystem character)
	{
		AccessDoor (DoorKey);
	}


	private void AccessDoor (string key)
	{
		if (key == DoorKey) {
			if (Time.time > timeTemp + Cooldown) {
				IsOpen = !IsOpen;
				timeTemp = Time.time;
			}
		}
	}
}
