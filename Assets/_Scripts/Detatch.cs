using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Detatch : NetworkBehaviour {
    // Use this for initialization
    void Start() {
        transform.parent = null;

    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            Destroy(gameObject);
        }
    }
}