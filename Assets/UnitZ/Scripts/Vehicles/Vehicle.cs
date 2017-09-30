//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CarControl))]
public class Vehicle : DamageManager
{

	public Seat[] Seats;
	public string VehicleName;
	[SyncVar]
	public string VehicleID;
	[HideInInspector]
	public bool incontrol;
	[SyncVar (hook = "OnSeatDataChanged")]
	public string SeatsData;
	[SyncVar]
	private Vector3 positionSync;
	[SyncVar]
	private Quaternion rotationSync;
	public bool hasDriver;

	void Awake ()
	{
		if (Seats.Length <= 0) {
			var seat = this.GetComponentsInChildren (typeof(Seat));
			Seats = new Seat[seat.Length];
			for (int i = 0; i < seat.Length; i++) {
				Seats [i] = seat [i].GetComponent<Seat> ();	
			}
		}
	}

	public override void OnDestroyed ()
	{
		for (int i = 0; i < Seats.Length; i++) {
			Seats [i].CleanSeat();
		}
		base.OnDestroyed ();
	}

	public override void OnStartClient ()
	{
		if (isServer) {
			VehicleID = netId.ToString ();
		}
		OnSeatDataChanged (SeatsData);
		base.OnStartClient ();
	}

	void OnSeatDataChanged (string seatsdata)
	{
		SeatsData = seatsdata;
		string[] passengerData = seatsdata.Split ("," [0]);
		if (passengerData.Length >= Seats.Length) {
			for (int i = 0; i < Seats.Length; i++) {
				Seats [i].PassengerID = passengerData [i];
			}
		}
	}

	void GenSeatsData ()
	{
		string seatdata = "";
		for (int i = 0; i < Seats.Length; i++) {
			if (Seats [i].PassengerID != "") {
				seatdata += Seats [i].PassengerID + ",";
			} else {
				seatdata += ",";
			}
		}
		SeatsData = seatdata;
	}

	void UpdatePassengerOnSeats ()
	{
		hasDriver = false;
		for (int i = 0; i < Seats.Length; i++) {
			if (Seats [i].PassengerID != "") {
				NetworkInstanceId passengerid = new NetworkInstanceId (uint.Parse (Seats [i].PassengerID));
				GameObject obj = ClientScene.FindLocalObject (passengerid);
				if (obj) {
					CharacterDriver driver = obj.GetComponent<CharacterDriver> ();
					if (driver) {
						driver.transform.position = Seats [i].transform.position;
						driver.transform.parent = Seats [i].transform;
						driver.CurrentVehicle = this;
						driver.character.controller.enabled = false;
						driver.DrivingSeat = Seats [i];
						hasDriver = true;
						if(driver.character.IsAlive == false){
							Seats [i].PassengerID = "";
						}
					}
				}
			} else {
				Seats [i].CleanSeat ();
			}
		}

		if (isServer) {
			GenSeatsData ();
		}
	}



	public void GetOutTheVehicle (CharacterDriver driver)
	{
		Debug.Log ("Get out this car " + driver.netId.ToString ());
		for (int i = 0; i < Seats.Length; i++) {
			if (Seats [i].PassengerID == driver.netId.ToString ()) {
				Seats [i].PassengerID = "";
				return;
			}
		}
	}

	public virtual void Pickup (CharacterSystem character)
	{
		character.SendMessage ("PickupCarCallback", this);
	}

	public void GetInTheVehicle (CharacterDriver driver, int seatID)
	{
		if (driver && seatID != -1 && seatID >= 0 && seatID < Seats.Length) {
			driver.CurrentVehicle = this;
			Seats [seatID].PassengerID = driver.netId.ToString ();
			Seats [seatID].passenger = driver;
		}
	}

	public int FindOpenSeatID ()
	{
		for (int i = 0; i < Seats.Length; i++) {
			if (Seats [i].PassengerID == "") {
				return i;
			} 
		}
		return -1;
	}


	public virtual void Drive (Vector2 input, bool brake)
	{

	}


	public void UpdateFunction ()
	{
		DamageUpdate ();
		UpdatePassengerOnSeats ();

		if(isServer){
			positionSync = this.transform.position;
			rotationSync = this.transform.rotation;
		}

		this.transform.position = Vector3.Lerp (this.transform.position, positionSync, 0.5f);
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotationSync, 0.5f);
	}

	void Update ()
	{
		UpdateFunction ();
		UpdateDriver ();
	}


	public void UpdateDriver ()
	{
		for (int i = 0; i < Seats.Length; i++) {
			if (Seats [i].IsDriver && Seats [i].passenger != null) {
				return;
			} 
		}
		incontrol = false;	
	}


	public void FixedUpdate ()
	{
		ShowInfo = false;	
	}

	public void GetInfo ()
	{
		ShowInfo = true;
	}

	public bool ShowInfo;

	void OnGUI ()
	{
		if (ShowInfo) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label (new Rect (screenPos.x, Screen.height - screenPos.y, 200, 60), "Get in\n" + VehicleName);
		}
	}

}
