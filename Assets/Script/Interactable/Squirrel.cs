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
            Debug.Log("tp");
            AkSoundEngine.PostEvent("Play_voix01", gameObject);
            currentTree.parent.GetComponent<Tree>().squirrel = null;
            currentTree = null;
            StartCoroutine(AnimationMoon());
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

    //A personnaliser
    IEnumerator AnimationMoon()
    {
        float normalizedTime = 0;
        Vector3 originScale = transform.localScale;

        //Le perso disaparait...
        while (normalizedTime < 1)
        {
            normalizedTime += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(originScale, Vector3.zero, normalizedTime);
            yield return null;
        }

        //... puis reapparait sur la lune...
        List<GameObject> allGob = new List<GameObject>();
        foreach (Transform landings in potentialMoonLandings)
        {
            GameObject copy = Instantiate(gameObject);
            Destroy(copy.GetComponent<Squirrel>());
            copy.transform.parent = landings;
            copy.transform.localPosition = Vector3.zero;
            allGob.Add(copy);
        }

        //progressivement
        while (normalizedTime > 0)
        {
            normalizedTime -= Time.deltaTime / duration;
            yield return null;   
            foreach(GameObject gob in allGob)
            {
                gob.transform.localScale = Vector3.Lerp(originScale, Vector3.zero, normalizedTime);
            }
        }
        Destroy(gameObject);
    }
}
