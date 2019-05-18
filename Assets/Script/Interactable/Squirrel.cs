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
        //Cliquer sur le perso revient a cliquer sur son arbre
        if (currentTree)
            currentTree.parent.GetComponent<Interactable>().MouseDown(btn, mouse);
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
            //Permet d'eviter d'etre joué par les copies
            if (!currentTree) return;
            AkSoundEngine.PostEvent("Play_voix01", gameObject);
            currentTree.parent.GetComponent<Tree>().squirrel = null;
            currentTree = null;
            EnigmeManager.getInstance().DiscoveredCharacter(potentialMoonLandings, gameObject.transform, duration);
        }
    }

    IEnumerator AnimationJump()
    {
        bool openEnd = false;
        Transform nextTree = potentialTrees.Where(x => x != currentTree && x != previousTree).
            OrderBy(x => Vector3.Distance(x.position, transform.position) * Random.Range(0.5f, 2)).ToList()[0];

        previousTree = currentTree;
        currentTree = nextTree;
        previousTree.parent.GetComponent<Tree>().squirrel = null;
        previousTree.parent.GetComponent<Tree>().FouilleTree();

        transform.forward = currentTree.position - previousTree.position;
        Debug.Break();

        float normalizeTime = 0;
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime / duration;
            Vector3 temporaryPosition = Vector3.Lerp(previousTree.position, currentTree.position, normalizeTime);
            temporaryPosition += new Vector3(0, curveY.Evaluate(normalizeTime), 0);
            transform.position = temporaryPosition;
            transform.localEulerAngles = new Vector3(Mathf.Lerp(-90, 90, normalizeTime), transform.localEulerAngles.y, transform.localEulerAngles.z);

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
}
