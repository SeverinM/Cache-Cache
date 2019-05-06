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

    [SerializeField]
    AnimationCurve curveY;

    Transform currentTree;
    Transform previousTree;

    private void Awake()
    {
        currentTree = potentialTrees[0];
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
            StartCoroutine(AnimationJump());
        }
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

        previousTree = currentTree;
        currentTree = nextTree;

        previousTree.parent.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        float normalizeTime = 0;
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime / duration;
            Vector3 temporaryPosition = Vector3.Lerp(previousTree.position, currentTree.position, normalizeTime);
            temporaryPosition += new Vector3(0, curveY.Evaluate(normalizeTime), 0);
            transform.position = temporaryPosition;
            if (normalizeTime > 0.7f && !openEnd)
            {
                openEnd = true;
                currentTree.parent.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
            }

            yield return null;
        }
    }
}
