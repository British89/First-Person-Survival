using UnityEngine;
using System.Collections;

public class TooltipsManager : MonoBehaviour {

	public TooltipInstance[] AllToolTips;

	void Start () {
		if(AllToolTips.Length <=0)
			AllToolTips = (TooltipInstance[])GameObject.FindObjectsOfType(typeof(TooltipInstance));
	}

	void Update () {
	
	}
}
