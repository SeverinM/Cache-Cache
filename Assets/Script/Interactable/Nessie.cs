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

    [SerializeField]
    List<Transform> fishs;

    [SerializeField]
    float depthFish;

    float minYFish;
    float maxYFish;

    [SerializeField]
    float speedFish;

    public float Ratio => (originPos == Vector3.zero ? 0 : Vector3.Distance(originPos, transform.position) / maxDist);

    protected override void Awake()
    {
        directionCam = (cam.transform.position - transform.position).normalized;
        minYFish = fishs[0].transform.position.y;
        maxYFish = minYFish + depthFish;
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        isDown = false;
    }

    public void Update()
    {
        //Gestion des poisssons
        foreach (Transform fish in fishs)
        {
            float posY = fish.transform.position.y;
            fish.RotateAround(transform.position, Vector3.up, -speedFish * Time.deltaTime);
            fish.position = new Vector3(fish.position.x, posY, fish.position.z);

            //Les poissons montent ou descendent selon si le click est maintenu ou non
            Vector3 temporaryPosition = fish.position + (new Vector3(0, speedFish * Time.deltaTime * 0.1f, 0) * (isDown ? 1 : -1));
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, minYFish, maxYFish), temporaryPosition.z);
            fish.transform.position = temporaryPosition;

            //Plus proche de la surface = plus proche de 0
            float ratioY = (maxYFish - fish.transform.position.y) / (maxYFish - minYFish);
            fish.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, ratioY), fish.localEulerAngles.y, fish.localEulerAngles.z);
        }

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
            Vector3 temporaryPosition = transform.position + new Vector3(0, angleCamera * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, originPos.y + maxDist), temporaryPosition.z);
            transform.position = temporaryPosition;
        }


    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 1)
        {
            originPos = transform.position;
            isDown = true;
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
        if (isDown && transform.position.y <= originPos.y + maxDragDist && Progress == 1)
        {
            Vector3 temporaryPosition = transform.position + new Vector3(0, -mouse.delta.y * speedScrewing, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, originPos.y, (originPos.y + maxDragDist) - 0.001f), temporaryPosition.z);
            transform.position = temporaryPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 1)
        {
            transform.position = originPos;
            isDown = false;
        }
    }

    public override bool IsHandCursor()
    {
        return (Progress == 0);
    }
}
