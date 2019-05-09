using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AllEnigme 
{
    public static bool IS_MOON_OPEN(params Interactable[] allInter)
    {
        MoonPart moon1 = (MoonPart)allInter[0];
        MoonPart moon2 = (MoonPart)allInter[1];
        if (!moon1 || !moon2)
            return false;

        return (allInter[0].Progress == 0 && allInter[1].Progress == 0 && moon1.Ratio >= 0.999f && moon2.Ratio >= 0.999f);
    }

    public static bool IS_BOX_OPEN(params Interactable[] allInter)
    {
        Box box1 = (Box)allInter[0];
        Box box2 = (Box)allInter[1];
        if (!box1 || !box2)
            return false;

        return (allInter[0].Progress == 0 && allInter[1].Progress == 0 && box1.Ratio >= 0.9f && box2.Ratio >= 0.9f);
    }

    public static bool IS_SQUIRREL_FOUND(params Interactable[] allInter)
    {
        return (allInter[0] is Squirrel && allInter[0].Progress == 1);       
    }
}
