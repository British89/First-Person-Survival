//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[RequireComponent (typeof(NetworkIdentity))]

public class ItemSpawner : NetworkBehaviour
{
	public bool SpawnOnStart = true;
	public float timeSpawn = 120;
	public ItemData[] Item;
	public int ItemMax = 3;
	public Vector3 Offset = new Vector3 (0, 0.1f, 0);
	private float timeTemp = 0;
	private List<GameObject> itemList = new List<GameObject> ();

	void Start ()
	{
		this.GetComponent<NetworkIdentity> ().serverOnly = true;
		full = false;
	}

	public override void OnStartClient ()
	{
		full = false;
		if (SpawnOnStart)
			Spawn ();
		base.OnStartClient ();
	}

	bool full = false;

	void Spawn ()
	{
		if (!isServer)
			return;

		if (Item.Length > 0) {
			ItemData itemPick = Item [Random.Range (0, Item.Length)];
			GameObject objitem = null;
			Vector3 spawnPoint = DetectGround (transform.position + new Vector3 (Random.Range (-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range ((int)(-this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));

			objitem = UnitZ.gameNetwork.RequestSpawnItem (itemPick.gameObject, itemPick.NumTag, itemPick.Quantity, spawnPoint, Quaternion.identity);
				
			if (objitem)
				itemList.Add (objitem);
		}
	}

	private int ObjectsNumber;

	void ObjectExistCheck ()
	{
		ObjectsNumber = 0;
		foreach (var obj in itemList) {
			if (obj != null)
				ObjectsNumber++;
		}
	}

	void Update ()
	{
		if (!isServer)
			return;

		ObjectExistCheck ();

		if (ObjectsNumber >= ItemMax) {
			full = true;
		} else {
			if (full) {
				timeTemp = Time.time;
				full = false;
			}

			if (Time.time > timeTemp + timeSpawn) {
				Spawn ();
				timeTemp = Time.time;
			}
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawWireCube (transform.position, this.transform.localScale);
	}

	Vector3 DetectGround (Vector3 position)
	{
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, 1000.0f)) {
			return hit.point + Offset;
		}
		return position;
	}
}
