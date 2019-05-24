using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nasselle : Interactable
{
    [SerializeField]
    Transform NasselleSpot;

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
        block = false;
        if (other.CompareTag("BaseRoue"))
        {
            GetComponent<Animator>().SetTrigger("nacelleOuverture");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        block = true;
        if (other.CompareTag("BaseRoue"))
        {
            GetComponent<Animator>().SetTrigger("nacelleFermeture");
        }
    }
}
