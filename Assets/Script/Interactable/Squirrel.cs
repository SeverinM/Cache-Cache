﻿using System.Collections;
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

    [SerializeField]
    List<Transform> potentialMoonLandings;

    private void Start()
    {
        currentTree = potentialTrees[0];
        transform.position = currentTree.transform.position;
        transform.SetParent(currentTree);
        currentTree.parent.GetComponent<Tree>().squirrel = this;
    }

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

    }

    public void NextJump()
    {
        GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        StartCoroutine(AnimationJump());
    }

    public override void OnNewValue()
    {
        if (Progress == 2)
        {
            AkSoundEngine.PostEvent("Play_voix01", gameObject);
            currentTree.parent.GetComponent<Tree>().squirrel = null;
            StartCoroutine(AnimationMoon(potentialMoonLandings.OrderBy(x => Vector3.Distance(x.position, transform.position)).ToList()[0]));
        }
    }

    IEnumerator AnimationJump()
    {
        bool openEnd = false;
        Transform nextTree = potentialTrees.Where(x => x != currentTree && x != previousTree).
            OrderBy(x => Vector3.Distance(x.position, transform.position) * Random.Range(0.5f, 2)).ToList()[0];

        //Allow to make make first look correct
        Vector3 lastPosition = (nextTree.position - currentTree.position) * -0.0001f;

        previousTree = currentTree;
        currentTree = nextTree;

        previousTree.parent.GetComponent<Tree>().squirrel = null;

        previousTree.parent.GetComponent<Tree>().FouilleTree();
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
                currentTree.parent.GetComponent<Tree>().FouilleTree();
            }
            yield return null;
        }
        currentTree.parent.GetComponent<Tree>().squirrel = this;
        this.transform.SetParent(currentTree);
    }

    IEnumerator AnimationMoon(Transform trsf)
    {
        transform.SetParent(trsf);

        Vector3 positionDestination = trsf.position;
        Vector3 positionOrigin = transform.position;
        Vector3 forwardOrigin = transform.forward;
        Vector3 forwardDestination = -trsf.up;

        float normalizeTime = 0;
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(positionOrigin, positionDestination, normalizeTime);
            yield return null;
        }

    }
}
