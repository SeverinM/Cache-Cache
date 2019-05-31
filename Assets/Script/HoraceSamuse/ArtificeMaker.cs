using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificeMaker : MonoBehaviour
{
    public static ArtificeMaker instance;
    public static ArtificeMaker instance2;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance2 == null)
        {
            instance2 = this;
        }
    }

    public Transform cameraOpposite;

    public GameObject artifice;

    public bool launchEveryDelay = false;
    public float delayBetweenLaunch = 4f;
    private float lastLaunchTime = 0f;
    public uint numberArtificePerDelay = 1;
    public bool pause = true;

    
    private void Update()
    {
        if (Time.timeSinceLevelLoad - lastLaunchTime > delayBetweenLaunch && !pause)
        {
            lastLaunchTime = Time.timeSinceLevelLoad;
            LaunchArtifice((int)numberArtificePerDelay);
        }


        /*if (launch)
        {
            launch = false;
            LaunchArtifice(numberOf);
            numberOf *= 2;
        }*/
    }

    //For Debug
    //public bool launch = false;
    //public int numberOf = 2;
    
    public void LaunchArtifice(int numberOfArtifice)
    {
        Vector3 position = this.transform.position;
        //where is the cam ?
        bool camInZ = cameraOpposite.position.z < 0;
        bool camInX = cameraOpposite.position.x < 0;
        bool feuInX = Mathf.Abs(cameraOpposite.position.z) < Mathf.Abs(cameraOpposite.position.x);
        for (int i = 0; i < numberOfArtifice; i++)
        {
            if (feuInX)
            {
                position.x = camInX ? 10 : -10;
                position.z = camInZ ? Random.Range(-10.0f, 10.0f) : Random.Range(-10.0f, 10.0f);
            }
            else
            {
                position.x = camInX ? Random.Range(-10.0f, 10.0f) : Random.Range(-10.0f, 10.0f);
                position.z = camInZ ? 10 : -10;
            }
            
            Quaternion rotation = Quaternion.identity;
            FeuArtifice feu = Instantiate(artifice, position, rotation, this.transform).GetComponent<FeuArtifice>();

            if (feuInX)
            {
                feu.transform.Rotate(Vector3.up * 90);
                feu.positionInSpace = Vector3.forward;
            }

        }
    }
}
