//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class FPSInputController : NetworkBehaviour
{
	private FPSController fpsControl;

	void Start ()
	{
		fpsControl = this.GetComponent<FPSController> ();
	}

	void Awake ()
	{
		MouseLock.MouseLocked = true;
	}


	void Update ()
	{

		if (isLocalPlayer && fpsControl != null) {
			fpsControl.MoveCommand (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")), Input.GetButton ("Jump"));

			if (Input.GetKeyDown (KeyCode.F)) {
				fpsControl.OutVehicle ();
			}

			if (Input.GetKey (KeyCode.LeftShift)) {
				fpsControl.Boost (1.4f);	
			}
			
			if (MouseLock.MouseLocked) {

				fpsControl.Aim (new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y")));
				fpsControl.Trigger1 (Input.GetButton ("Fire1"));
				fpsControl.Trigger2 (Input.GetButtonDown ("Fire2"));
			}

			if (Input.GetKeyDown (KeyCode.F)) {
				fpsControl.Interactive ();
			}

			if (Input.GetKeyDown (KeyCode.R)) {
				fpsControl.Reload ();
			}

			fpsControl.Checking ();
		}
	}

}
