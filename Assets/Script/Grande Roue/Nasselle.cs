using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nasselle : Interactable
{
    [SerializeField]
    Transform NasselleSpot;

    [HideInInspector]
    public bool open = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
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
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("nacelle_fermee"))
        {
            GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        }
            
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = NasselleSpot.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BaseRoue") && !open)
        {
            //Evite le son au tout debut du jeu
            if (Time.timeSinceLevelLoad > 0.1f)
                AkSoundEngine.PostEvent("Play_nacelle_open", gameObject);
            GetComponent<Animator>().SetTrigger("nacelleOuverture");
            open = true;
            block = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BaseRoue") && open)
        {
            AkSoundEngine.PostEvent("Play_nacelle_close", gameObject);
            open = false;
            block = true;
            GetComponent<Animator>().SetTrigger("nacelleFermeture");
        }
    }
}
