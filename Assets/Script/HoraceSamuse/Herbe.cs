using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbe : MonoBehaviour
{
    public RotateAround rot1;
    public RotateAround rot2;

    public Material myMat;

    private bool goBackToZero = false;

    public void Start()
    {
        if (myMat == null)
            myMat = this.GetComponent<MeshRenderer>().material;
    }

    public void Update()
    {
        if(rot1.CurrentSpeed != 0)
        {
            SetUpSpeed(rot1);
        }
        else if(rot2.CurrentSpeed != 0)
        {
            SetUpSpeed(rot2);
        }
        else if (goBackToZero)
        {
            SetUpZero();
            goBackToZero = false;
        }
    }

    public void SetUpSpeed(RotateAround rot)
    {
        myMat.SetFloat("_RotationSpeed", -rot.CurrentSpeed * 10);

        goBackToZero = true;
    }

    public void SetUpZero()
    {
        myMat.SetFloat("_RotationSpeed", 0);
    }

}
