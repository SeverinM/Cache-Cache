﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nessie : Interactable
{
    [SerializeField]
    float maxDist;

    [SerializeField]
    float speedScrewing;

    [SerializeField]
    float maxDragDist;

    [SerializeField]
    Camera cam;

    public Vector3 originPos;
    float angleCamera = 0;
    Vector3 directionCam;
    bool isDown;

    [SerializeField]
    List<Transform> fishs;

    [SerializeField]
    AnimationCurve curveFish;

    [SerializeField]
    float duration = 1;

    float normalizedTime = 0;
    bool reversed = false;

    [SerializeField]
    float depthFish;

    float minYFish;
    float maxYFish;

    [SerializeField]
    float speedFish;

    public float Ratio => Vector3.Distance(originPos, transform.localPosition) / maxDist;

    protected override void Awake()
    {
        directionCam = (cam.transform.position - transform.position).normalized;

        if (fishs.Count == 0) return;
        minYFish = fishs[0].transform.position.y;
        maxYFish = minYFish + depthFish;
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        isDown = false;
        if (Echo)
        {
            (Echo as Nessie).isDown = false;
            if (Echo.Progress != Progress)
                Echo.Progress = Progress;
        } 
    }

    public void Update()
    {
        if (Progress == 3)
        {
            Debug.Log(Ratio);
        }

        if (normalizedTime == 0)
        {
            reversed = false;
        }

        if (normalizedTime == 1)
        {
            reversed = true;
        }

        normalizedTime += (Time.deltaTime / duration) * (reversed ? -1 : 1);
        normalizedTime = Mathf.Clamp(normalizedTime, 0, 1);

        //Gestion des poisssons
        foreach (Transform fish in fishs)
        {
            float posY = fish.transform.position.y;
            fish.RotateAround(transform.position, Vector3.up, -speedFish * Time.deltaTime * curveFish.Evaluate(normalizedTime));
            fish.position = new Vector3(fish.position.x, posY, fish.position.z);

            //Les poissons montent ou descendent selon si le click est maintenu ou non
            Vector3 temporaryPosition = fish.position + (new Vector3(0, speedFish * Time.deltaTime * 0.1f, 0) * (isDown ? 1 : -1));
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, minYFish, maxYFish), temporaryPosition.z);
            fish.transform.position = temporaryPosition;

            //Plus proche de la surface = plus proche de 0
            float ratioY = (maxYFish - fish.transform.position.y) / (maxYFish - minYFish);
            fish.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, ratioY), fish.localEulerAngles.y, fish.localEulerAngles.z);
        }

        Vector3 tempDir = (cam.transform.position - transform.position).normalized;
        angleCamera = Vector3.Angle(tempDir, directionCam);

        //Tourné dans le sens horaire
        if ((cam.worldToCameraMatrix.MultiplyVector(tempDir) - cam.worldToCameraMatrix.MultiplyVector(directionCam)).x < 0)
        {
            angleCamera *= -1;
        }
        directionCam = tempDir;

        //Maintien en cours
        if (isDown)
        {
            Vector3 temporaryPosition = transform.position + new Vector3(0, angleCamera * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, originPos.y + maxDist), temporaryPosition.z);
            transform.position = temporaryPosition;
            if (Ratio < 1 && Ratio > 0) transform.Rotate(Vector3.up, -angleCamera);
        }


    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 3 && !echo)
        {
            originPos = transform.localPosition;
            isDown = true;            
            if (Echo)
            {
                (Echo as Nessie).isDown = true;
                Echo.CanInteract = false;
                (Echo as Nessie).originPos = Echo.transform.localPosition;
            }
                
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        //N'agit que lorsque l'on a pas assez tourné la maquette
        if (isDown && transform.localPosition.y <= originPos.y + maxDragDist && EnigmeManager.nessie_out)
        {
            Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, (originPos.y + maxDragDist) - 0.001f), temporaryPosition.z);
            transform.localPosition = temporaryPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 3 && !echo)
        {
            transform.localPosition = originPos;
            isDown = false;
            if (Echo)
            {
                (Echo as Nessie).isDown = false;
                Echo.CanInteract = true;
                Echo.transform.localPosition = (Echo as Nessie).originPos;
            }
        }
    }

    public override bool IsHandCursor()
    {
        return (Progress == 3);
    }
}
