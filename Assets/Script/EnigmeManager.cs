using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnigmeManager : MonoBehaviour
{
    public delegate bool ConditionProgress(params Interactable[] allInter);

    [SerializeField]
    List<ConditionStruct> allConditions;

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
                case Condition.MOON_OPEN:
                    return AllEnigme.IS_MOON_OPEN;
                case Condition.SQUIRREL_FOUND:
                    return AllEnigme.IS_SQUIRREL_FOUND;
                default:
                    return AllEnigme.IS_BOX_OPEN;
            }
        }
    }

    public enum Condition
    {
        MOON_OPEN,
        BOX_OPEN,
        SQUIRREL_FOUND
    }

    public void Update()
    {
        allConditions = allConditions.Where(x => !x.Evaluate()).ToList();
    }
}
