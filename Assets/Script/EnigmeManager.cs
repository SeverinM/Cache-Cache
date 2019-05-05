using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmeManager
{
    public delegate void ConditionProgress(params Interactable[] allInter);

    List<ConditionProgress> allConditions = new List<ConditionProgress>();

}
