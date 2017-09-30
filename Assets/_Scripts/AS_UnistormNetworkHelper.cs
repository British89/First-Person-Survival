/// <summary>
/// Adds all Unistorm components to the local players camera
/// written by Paul Tricklebank
/// Twitter: https://twitter.com/bigfiregamesuk
/// Facebook: https://www.facebook.com/bigfiregames
/// Facebook: https://www.facebook.com/aftersanctuary
/// </summary>


using UnityEngine;
using System.Collections;

public class AS_UnistormNetworkHelper : MonoBehaviour
{

    //drag this script onto your local player
    // and drag the camera into the slot in the inspector.
    private GameObject RainParticles;
    private GameObject Butterflies;
    private GameObject LightningPosition;
    private GameObject Snow;
    private GameObject SnowDust;
    private GameObject Leaves;
    private GameObject RainStreaks;
    private GameObject RainMist;
    public Camera FPCamera;


    void Start()
    {
        //find our unistorm weather systems and attach them all to the camera.
        //you may ned to play with the local positions depending on your
        //level.
        RainParticles = GameObject.Find("Rain New");
        Butterflies = GameObject.Find("Butterflies");
        LightningPosition = GameObject.Find("Lightning Position");
        Snow = GameObject.Find("Snow New");
        SnowDust = GameObject.Find("Snow Dust");
        Leaves = GameObject.Find("Fall Leaves");
        RainStreaks = GameObject.Find("Rain Streaks");
        RainMist = GameObject.Find("Rain Mist");

        if (RainParticles != null)
        {
            RainParticles.transform.parent = FPCamera.transform;
            RainParticles.transform.localPosition = new Vector3(0, 17, 0);
        }
        if (Butterflies != null)
        {
            Butterflies.transform.parent = FPCamera.transform;
            Butterflies.transform.localPosition = new Vector3(0, 12, 0);
        }
        if (Snow != null)
        {
            Snow.transform.parent = FPCamera.transform;
            Snow.transform.localPosition = new Vector3(0, 17, 0);
        }
        if (SnowDust != null)
        {
            SnowDust.transform.parent = FPCamera.transform;
            SnowDust.transform.localPosition = new Vector3(0, 12, 0);
        }
        if (Leaves != null)
        {
            Leaves.transform.parent = FPCamera.transform;
            Leaves.transform.localPosition = new Vector3(0, 12, 0);
        }
        if (RainStreaks != null)
        {
            RainStreaks.transform.parent = FPCamera.transform;
            RainStreaks.transform.localPosition = new Vector3(0, 12, 0);
        }
        if (RainMist != null)
        {
            RainMist.transform.parent = FPCamera.transform;
            RainMist.transform.localPosition = new Vector3(0, 12, 0);
        }
        if (LightningPosition != null)
        {
            LightningPosition.transform.parent = FPCamera.transform;
            LightningPosition.transform.localPosition = new Vector3(0, 12, 10);
        }
    }


}