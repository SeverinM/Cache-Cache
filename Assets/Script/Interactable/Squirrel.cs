using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Squirrel : Interactable
{
    [SerializeField]
    List<Transform> potentialTrees;

    [SerializeField]
    float duration;

    [Range(0,1)]
    [SerializeField]
    float openAt;

    [SerializeField]
    AnimationCurve curveY;

    Transform currentTree;
    Transform previousTree;

    private void Awake()
    {
        currentTree = potentialTrees[0];
        transform.position = currentTree.transform.position;
        currentTree.parent.GetComponent<Interactable>().Related.Add(this);
        canInteract = false;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            Debug.Log("NO");
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

    public void NextJump()
    {
        StartCoroutine(AnimationJump());
    }

    public override void OnNewValue()
    {
        if (Progress == 2)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator AnimationJump()
    {
        bool openEnd = false;
        Transform nextTree = potentialTrees.Where(x => x != currentTree && x != previousTree && !x.parent.GetComponent<Draggable>().Dragging).
            OrderBy(x => Vector3.Distance(x.position, transform.position) * Random.Range(0.5f, 2)).ToList()[0];

        //Allow to make make first look correct
        Vector3 lastPosition = (nextTree.position - currentTree.position) * -0.0001f;

        previousTree = currentTree;
        currentTree = nextTree;

        previousTree.parent.GetComponent<Interactable>().Related.Remove(this);

        previousTree.parent.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        float normalizeTime = 0;
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime / duration;
            Vector3 temporaryPosition = Vector3.Lerp(previousTree.position, currentTree.position, normalizeTime);
            temporaryPosition += new Vector3(0, curveY.Evaluate(normalizeTime), 0);
            lastPosition = transform.position;
            transform.position = temporaryPosition;
            transform.forward = (transform.position - lastPosition);

            if (normalizeTime > openAt && !openEnd)
            {
                openEnd = true;
                currentTree.parent.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
            }
            yield return null;
        }
        currentTree.parent.GetComponent<Interactable>().Related.Add(this);
        canInteract = (currentTree.parent.tag == "Hiver");
    }
}
