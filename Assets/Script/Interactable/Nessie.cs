using System.Collections;
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

    Vector3 originPos;
    float angleCamera = 0;
    Vector3 directionCam;
    bool isDown;

    public float Ratio => (originPos == Vector3.zero ? 0 : Vector3.Distance(originPos, transform.localPosition) / maxDist);

    protected override void Awake()
    {
        directionCam = (cam.transform.position - transform.position).normalized;
    }

    public void Update()
    {
        //Attention temporaire , passera a 1 plus tard
        if (Progress != 0) return;

        Vector3 tempDir = (cam.transform.position - transform.position).normalized;
        angleCamera = Vector3.Angle(tempDir, directionCam);

        //Tourné dans le sens horaire
        if ((cam.worldToCameraMatrix.MultiplyVector(tempDir) - cam.worldToCameraMatrix.MultiplyVector(directionCam)).x < 0)
        {
            angleCamera *= -1;
        }
        directionCam = tempDir;

        if (isDown)
        {
            Vector3 temporaryPosition = transform.localPosition + new Vector3(0, angleCamera * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, originPos.y + maxDist), temporaryPosition.z);
            transform.localPosition = temporaryPosition;
        }
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        originPos = transform.localPosition;
        isDown = true;
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
        if (isDown && transform.localPosition.y <= originPos.y + maxDragDist && Progress == 0)
        {
            Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, (originPos.y + maxDragDist) - 0.001f), temporaryPosition.z);
            transform.localPosition = temporaryPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 0)
        {
            transform.localPosition = originPos;
            isDown = false;
        }
    }
}
