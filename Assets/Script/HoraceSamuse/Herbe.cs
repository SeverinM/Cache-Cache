using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbe : MonoBehaviour
{
    public RotateAround rot1;
    public RotateAround rot2;

    public Transform center;

    public Material myMat;

    private float targetValue = 0;
    private float currentValue = 0;
    [Range(0, 1)]
    public float asymptoticSpeed = 0.9f;

    public float sinInfluence = 0.25f;

    private bool goBackToZero = false;

    public void Start()
    {
        if (myMat == null)
            myMat = this.GetComponent<MeshRenderer>().material;

        myMat.SetVector("_SourceCenter", center.position);
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
            targetValue = 0;
            goBackToZero = false;
        }



        float trg = targetValue + (Mathf.Sin(Time.timeSinceLevelLoad)) / sinInfluence;

        if (trg != currentValue)
        {
            currentValue = (asymptoticSpeed * currentValue) + (1 - asymptoticSpeed) * trg;
            if (Mathf.Abs(currentValue - trg) < 0.01f)
                currentValue = trg;

            myMat.SetFloat("_RotationSpeed", currentValue);
        }


    }

    public void SetUpSpeed(RotateAround rot)
    {
        targetValue = -rot.CurrentSpeed / 25.0f;

        goBackToZero = true;
    }

}
