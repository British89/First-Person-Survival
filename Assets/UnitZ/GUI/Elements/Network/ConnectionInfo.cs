//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionInfo : MonoBehaviour
{

	public InputField PortText;
	public InputField ServerIPText;

	void Start ()
	{
		if (UnitZ.gameNetwork) {
			if (PortText)
				PortText.text = UnitZ.gameNetwork.networkPort.ToString ();
			if (ServerIPText)
				ServerIPText.text = UnitZ.gameNetwork.networkAddress;	
		}
	}
	
	public void SetServerIP (InputField num)
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.networkAddress = num.text;
		}
	}

	public void SetPort (InputField num)
	{
		if (UnitZ.gameNetwork) {
			int val = UnitZ.gameNetwork.networkPort;
			if (int.TryParse (num.text, out val)) {
				UnitZ.gameNetwork.networkPort = val;
			}
		}
	}
	
	
		

}
