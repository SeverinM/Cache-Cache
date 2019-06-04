using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class EnigmeManager : MonoBehaviour
{
    public delegate bool ConditionProgress(params Interactable[] allInter);

    [SerializeField]
    List<ConditionStruct> allConditions;

    [SerializeField]
    GameObject particleHurray;

    [SerializeField]
    List<GameObject> toDisable;

    [SerializeField]
    float timeBeforeEnd = 600;

    [SerializeField]
    float coeffSize = 5;
    public float CoeffSizeMoon => coeffSize;

    static EnigmeManager _instance = null;

    int characterFound = 0;
    bool pause = true;

    [SerializeField]
    int charactersObjectives = 4;

    [System.Serializable]
    class Rotate
    {
        public Camera cam;
        public Transform trsf;
    }

    [SerializeField]
    Rotate rot1;

    [SerializeField]
    GameObject victoire;

    [SerializeField]
    GameObject defaite;

    [SerializeField]
    List<Canvas> allCan;

    [SerializeField]
    Rotate rot2;

    float durationFirework = 0;

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(FireworkScrut());
    }

    public static EnigmeManager getInstance()
    {
        if (!_instance)
            _instance = new GameObject().AddComponent<EnigmeManager>();

        return _instance;
    }

    [System.Serializable]
    class ConditionStruct
    {
        [SerializeField]
        List<Interactable> allParameters;
        public List<Interactable> AllParameters => allParameters;

        [SerializeField]
        Condition cond;

        ConditionProgress progressCondition;
        bool init = false;

        public bool Evaluate()
        {
            if (!init)
            {
                progressCondition = GetCondition(cond);
                init = true;
            }
            
            bool output = progressCondition(allParameters.ToArray());
            if (output)
            {
                allParameters.ForEach(x =>
                {
                    x.Progress++;
                    if (x.Echo)
                        x.Echo.Progress++;
                });
            }

            return output;
        }

        ConditionProgress GetCondition(Condition cond)
        {
            switch (cond)
            { 
                case Condition.BOX_OPEN:
                    return AllEnigme.IS_BOX_OPEN;
                case Condition.SQUIRREL_FOUND:
                    return AllEnigme.IS_SQUIRREL_FOUND;
                case Condition.LOLJAMBE_PULLED:
                    return AllEnigme.IS_CHAR_PULLED;
                case Condition.NESSIE_BODY_PULLED:
                    return AllEnigme.IS_NESSIE_BODY_PULLED;
                case Condition.FIREWORK_BURNED:
                    return AllEnigme.IS_FIREWORK_BURNING;
                default:
                    return AllEnigme.IS_BOX_OPEN;
            }
        }
    }

    public enum Condition
    {
        BOX_OPEN,
        SQUIRREL_FOUND,
        LOLJAMBE_PULLED,
        NESSIE_BODY_PULLED,
        FIREWORK_BURNED
    }

    public void Update()
    {
        if (allConditions.Count > 0)
            allConditions = allConditions.Where(x => !x.Evaluate()).ToList();

        if (!pause) timeBeforeEnd -= Time.deltaTime;
        if (timeBeforeEnd < 0 && !pause)
        {
            pause = true;
            AllCharacterFound();
        }
    }

    public void StartCountdown()
    {
        pause = false;
    }

    public void DiscoveredCharacter(List<Transform> newParents, Transform target, string keyAnim ,float duration)
    {
        StartCoroutine(DiscoverAnimation(newParents, target, keyAnim, duration));
    }

    IEnumerator DiscoverAnimation(List<Transform> newParents , Transform target , string keyAnim, float duration)
    {
        AddFirework(1);
        GameObject gobParticle = Instantiate(particleHurray, target.position, Quaternion.identity);
        Destroy(gobParticle, 2);

        //Can c'est possible , fait jouer une animation au personnage joué
        target.GetComponent<Interactable>().CanInteract = false;
        if (target.GetComponent<Animator>())
            target.GetComponent<Animator>().SetTrigger(keyAnim);

        characterFound++;

        Manager.GetInstance().PlayByDistance("Play_voix", target, false);

        float normalizedTime = 0;;
        Vector3 originScale = target.lossyScale;

        if (target.GetComponent<Animator>())
        {
            target.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_FOUND);
        }

        //Le perso disaparait...
        while (normalizedTime < 1)
        {
            normalizedTime += Time.deltaTime / duration;
            target.transform.localScale = Vector3.Lerp(originScale, Vector3.zero, normalizedTime);
            yield return null;
        }

        //... puis reapparait sur la lune...
        Manager.GetInstance().PlayBoth("Play_Music_reward");

        List<GameObject> allGob = new List<GameObject>();
        foreach (Transform landings in newParents)
        {
            GameObject copy = Instantiate(target.gameObject);

            GameObject gobParticle2 = Instantiate(particleHurray, landings.position, Quaternion.identity);
            Destroy(gobParticle2, 2);

            //Detruit les scripts
            Destroy(copy.GetComponent<Interactable>());
            copy.transform.SetParent(landings);
            copy.transform.localPosition = Vector3.zero;
            copy.transform.localEulerAngles = Vector3.zero;
            allGob.Add(copy);
        }

        //progressivement
        while (normalizedTime > 0)
        {
            normalizedTime -= Time.deltaTime / duration;
            yield return null;
            foreach (GameObject gob in allGob)
            {
                gob.transform.localScale = Vector3.Lerp(originScale, Vector3.zero, normalizedTime) * getInstance().CoeffSizeMoon;
            }
        }
        Destroy(target.gameObject);

        foreach (GameObject gob in allGob)
        {
            if (gob.GetComponent<Animator>())
                gob.GetComponent<Animator>().SetTrigger(keyAnim);
        }

        if (characterFound >= charactersObjectives)
        {
            AllCharacterFound();
        }
    }

    void AllCharacterFound()
    {
        StopAllCoroutines();
        foreach(GameObject gob in toDisable)
        {
            gob.SetActive(false);
        }

        foreach(TeleportSpot sp in GameObject.FindObjectsOfType<TeleportSpot>())
        {
            sp.SetMoonAnimation(false, () => { });
        }

        foreach(Interactable inter in GameObject.FindObjectsOfType<Interactable>())
        {
            inter.CanInteract = false;
        }

        foreach (Transform trsf in rot1.trsf)
        {
            trsf.gameObject.SetActive(false);
        }

        foreach (Transform trsf in rot2.trsf)
        {
            trsf.gameObject.SetActive(false);
        }

        foreach (CameraWaypoints waypoints in GameObject.FindObjectsOfType<CameraWaypoints>())
        {
            waypoints.StartNextWaypoint(() => {
                RotateChar();
            });
        }
    }

    void RotateChar()
    {
        StartCoroutine(RotateCor());
    }

    public void AddFirework(float duration)
    {
        durationFirework += duration;
        ArtificeMaker.instance.pause = false;
        ArtificeMaker.instance2.pause = false;
    }

    IEnumerator FireworkScrut()
    {
        while (true)
        {
            yield return null;
            if (durationFirework > 0)
                durationFirework -= Time.deltaTime;
            else
            {
                ArtificeMaker.instance.pause = true;
                ArtificeMaker.instance2.pause = true;
            }
        }
    }

    IEnumerator RotateCor()
    {
        foreach(Canvas can in allCan)
        {
            Instantiate(characterFound == charactersObjectives ? victoire : defaite , can.transform);
        }

        if (characterFound == charactersObjectives)
        {
            ArtificeMaker.instance.pause = false;
            ArtificeMaker.instance.launchEveryDelay = true;
            ArtificeMaker.instance.delayBetweenLaunch = 1.2f;
            ArtificeMaker.instance.numberArtificePerDelay = 2;

            ArtificeMaker.instance2.pause = false;
            ArtificeMaker.instance2.launchEveryDelay = true;
            ArtificeMaker.instance2.delayBetweenLaunch = 1.2f;
            ArtificeMaker.instance2.numberArtificePerDelay = 2;
        }
        while (true)
        {
            rot1.cam.transform.RotateAround(rot1.trsf.position, Vector3.up, 5 * Time.deltaTime);
            rot2.cam.transform.RotateAround(rot2.trsf.position, Vector3.up, 5 * Time.deltaTime);

            yield return null;
        }
    }
}
