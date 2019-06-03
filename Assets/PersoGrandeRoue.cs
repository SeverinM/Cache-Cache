using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoGrandeRoue : Interactable
{

    [SerializeField]
    List<Transform> potentialMoonsLandings;

    [SerializeField]
    Nasselle ouverture;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if(ouverture.open == true)
        {
            EnigmeManager.getInstance().DiscoveredCharacter(potentialMoonsLandings, transform, Manager.TRIGGER_INTERACTION, 1);
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
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
