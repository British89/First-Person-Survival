using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnvironmentManager : NetworkBehaviour {

	private DayNightCycle dayNight;
	private TreesManager Trees;
	[SyncVar]
	private float dayTimeSync;
	[SyncVar (hook="OnTreesChanged")]
	public string TreesCutData;

	public override void OnStartLocalPlayer ()
	{
		Trees.OnClientStart();
		base.OnStartLocalPlayer ();
	}

	private void OnTreesChanged(string treecutdata){
		TreesCutData = treecutdata;
		Trees.UpdateRemovedTrees (treecutdata);
	}

	public void UpdateTrees(string treecutdata){
		if(isServer){
			TreesCutData = treecutdata;
		}
	}

	void Start () {
		Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
	}
	
	void Update ()
	{
		if (dayNight == null) {
			dayNight = (DayNightCycle)GameObject.FindObjectOfType (typeof(DayNightCycle));
		}

		if(isServer)
			dayTimeSync = dayNight.Timer;
		
		dayNight.Timer = dayTimeSync;

	}
}
