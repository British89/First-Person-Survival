using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour
{

	public Camera MainCamera;
	public GameObject FPSItemView;
	public Transform Root;

	public float HorizonSway = 0.4f;
	public float VerticalSway = 0.4f;

	public Vector3 aimOffset;

	private Vector3 swayOffset;
	private Vector3 positionTmpOffset;
	private Vector3 rootDirTmp;
	private Vector3 fpsDirTmp;

	private float rootDirHDot;
	private float rootDirVDot;
	private Vector3 rootDifPos;

	void Start ()
	{
		if (Root == null) {
			Root = this.gameObject.transform.root;
		}
		positionTmpOffset = FPSItemView.transform.localPosition;
	}

	public void Aim (Vector3 aimOffset)
	{
		aimOffset = aimOffset;
	}

	void Update ()
	{
		if (FPSItemView == null)
			return;
		
		if (Root) {

			float DirH = Vector3.Dot (Root.transform.right.normalized, rootDirTmp.normalized) * HorizonSway;
			float DirV = Vector3.Dot (this.transform.up.normalized, fpsDirTmp.normalized) * VerticalSway;

			if (aimOffset != Vector3.zero) {
				DirH = 0;
				DirV = 0;
			}

			rootDirHDot = Mathf.Lerp (rootDirHDot, DirH, 10 * Time.deltaTime);
			rootDirVDot = Mathf.Lerp (rootDirVDot, DirV, 10 * Time.deltaTime);


			swayOffset.x = rootDirHDot;
			swayOffset.y = rootDirVDot;
		}

		Vector3 offsetTarget = positionTmpOffset + (-swayOffset) + aimOffset;
		FPSItemView.transform.localPosition = Vector3.Lerp (FPSItemView.transform.localPosition, offsetTarget, 5 * Time.deltaTime);

		if (Root) {
			rootDirTmp = Root.transform.forward;
			fpsDirTmp = this.transform.forward;
		}
	}

}
