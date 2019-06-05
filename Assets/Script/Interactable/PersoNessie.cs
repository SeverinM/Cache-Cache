using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoNessie : Interactable
{
    [SerializeField]
    Nessie nessieBody;

    [SerializeField]
    List<Transform> potentialMoonsLandings;

    [SerializeField]
    bool checkNessieOut;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        //A changer a 2 apres
        if ((!nessieBody || nessieBody.Progress >= 3) && (!checkNessieOut || EnigmeManager.nessie_out))
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
}
