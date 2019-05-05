using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Squirrel : Interactable
{
    [SerializeField]
    List<GameObject> potentialTrees;

    GameObject currentTree;
    GameObject previousTree;

    private void Awake()
    {
        currentTree = potentialTrees[Random.Range(0, 5)];
        transform.position = currentTree.transform.position;
        StartCoroutine(JumpTree());
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            Progress++;
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

    IEnumerator JumpTree()
    {
        //As long as the squirrel is not caught
        while (Progress == 0)
        {
            yield return new WaitForSeconds(2f);
            Teleport();
        }
    }

    public override void OnNewValue()
    {
        if (Progress == 2)
        {
            Destroy(gameObject);
        }
    }

    void Teleport()
    {
        GameObject nextTree = potentialTrees.Where(x => x != currentTree && x != previousTree && !x.GetComponent<Draggable>().Dragging).
            OrderBy(x => Vector3.Distance(x.transform.position, transform.position) * Random.Range(0.5f, 2)).ToList()[0];
        transform.position = nextTree.transform.position;
        previousTree = currentTree;
        currentTree = nextTree;
    }
}
