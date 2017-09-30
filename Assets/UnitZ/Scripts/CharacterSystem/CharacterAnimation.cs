//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterAnimation : NetworkBehaviour {

	private Animator animator;
	public Transform upperSpine;
	public Transform headCamera;
	public Quaternion CameraRotation;
	private CharacterSystem character;
	[SyncVar]
	private Quaternion rotationSync;

	
	// *************************
	// For legacy animation to rotation upper part along with camera.
	
	void Start () {
		animator = this.GetComponent<Animator>();
		character = this.GetComponent<CharacterSystem>();
		if(headCamera == null){
			FPSCamera fpscam = this.GetComponentInChildren<FPSCamera>();
			headCamera = fpscam.gameObject.transform;
		}

		if (upperSpine != null) {
			CameraRotation = upperSpine.transform.rotation;
			rotationSync = CameraRotation;
		}
	}


	[Command(channel = 1)]
	void CmdCameraUpdate(Quaternion quaternion){
		RpcRotation(quaternion);
	}

	private Quaternion rotationLate;
	private Quaternion rotationLastTrip;
	private float timeLastTrip;
	private float latencyTime;

	[ClientRpc (channel = 1)]
	void RpcRotation (Quaternion quaternion)
	{
		rotationLate = rotationLastTrip;
		latencyTime = Time.time - timeLastTrip;
		timeLastTrip = Time.time;
		rotationLastTrip = quaternion;
		rotationSync = quaternion;
	}

	float timeTmpsending;
	void Update () {
		if(animator == null || character == null)
			return;

        // this is for legacy animation
        // if you using Mecanim in unity Pro, 
        if (headCamera)
            animator.SetLookAtPosition(headCamera.transform.forward * 10);
		
		if(upperSpine != null){
			
			if(isLocalPlayer){
				// get rotation from Upper Spin
				CameraRotation = upperSpine.localRotation;
				CameraRotation.eulerAngles = new Vector3(upperSpine.localRotation.eulerAngles.x,upperSpine.localRotation.eulerAngles.y,-headCamera.transform.rotation.eulerAngles.x);

				float fps = (1 / Time.deltaTime);
				float delay = (fps / character.currentSendingRate) * Time.deltaTime;
				if (Time.time > timeTmpsending + delay) { 
					CmdCameraUpdate (CameraRotation);
					timeTmpsending = Time.time;
				}
			}else{
				float lerpValue = (Time.time - timeLastTrip) / latencyTime;
				CameraRotation = Quaternion.Lerp (rotationLate, rotationSync, lerpValue);
			}
			
			// rotation Upper spin along with camera angle
			upperSpine.transform.localRotation = CameraRotation;
			// update animation transform
			Animation anim = animator.GetComponent<Animation>();
			if(anim && anim[anim.clip.name])
				anim[anim.clip.name].AddMixingTransform(upperSpine);

		}
		
	}


}
