﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnigmeManager : MonoBehaviour
{
    public delegate bool ConditionProgress(params Interactable[] allInter);

    [SerializeField]
    List<ConditionStruct> allConditions;

    float timeBeforeEnd = 600;

    [SerializeField]
    float coeffSize = 5;
    public float CoeffSizeMoon => coeffSize;

    static EnigmeManager _instance = null;

    int characterFound = 0;
    bool pause = true;

    [SerializeField]
    int charactersObjectives = 4;

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);
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
        if (timeBeforeEnd < 0)
        {
            End();
        }
    }

    void End()
    {
        Debug.LogError("fin");
        allConditions.Clear();
        foreach(Interactable inter in GameObject.FindObjectsOfType<Interactable>())
        {
            inter.CanInteract = false;
        }
    }

    public void StartCountdown()
    {
        pause = false;
    }

    public void DiscoveredCharacter(List<Transform> newParents, Transform target, float duration)
    {
        StartCoroutine(DiscoverAnimation(newParents, target, duration));
    }

    IEnumerator DiscoverAnimation(List<Transform> newParents , Transform target , float duration)
    {
        characterFound++;

        AkSoundEngine.PostEvent("Play_voix01", gameObject);
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
        List<GameObject> allGob = new List<GameObject>();
        foreach (Transform landings in newParents)
        {
            GameObject copy = Instantiate(target.gameObject);

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
    }
}
