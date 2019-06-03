using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonGrandeRoue : Interactable
{
    [HideInInspector]
    public bool turnTheWheel = false;

    [SerializeField]
    GameObject light;

    [SerializeField]
    Animator anim;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        turnTheWheel = true;
        anim.SetBool("appui", true);
        light.SetActive(true);
        AkSoundEngine.PostEvent("Play_grande_roue", gameObject);
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {

    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {

    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        turnTheWheel = false;
        anim.SetBool("appui", false);
        light.SetActive(false);
        AkSoundEngine.PostEvent("Stop_grande_roue", gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        light.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
