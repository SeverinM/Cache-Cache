using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAnim : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Clique";


    [ContextMenu("Touche me")]
    public void OnMouseDown()
    {
        animator.SetTrigger(triggerName);
    }
}
