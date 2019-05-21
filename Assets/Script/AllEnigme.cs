using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AllEnigme 
{
    //Dés qu'une de ces conditions est rempli , tous les participants voient leur indice de progression incrementé
    public static bool IS_MOON_OPEN(params Interactable[] allInter)
    {
        MoonPart moon1 = (MoonPart)allInter[0];
        MoonPart moon2 = (MoonPart)allInter[1];
        if (!moon1 || !moon2)
            return false;

        return (allInter[0].Progress == 0 && allInter[1].Progress == 0 && moon1.Ratio >= 0.9f && moon2.Ratio >= 0.9f);
    }

    //La boite est consideré ouverte si les deux parties sont ouvertes a 95% en meme temps
    public static bool IS_BOX_OPEN(params Interactable[] allInter)
    {
        Box box1 = (Box)allInter[0];
        Box box2 = (Box)allInter[1];
        if (!box1 || !box2)
            return false;

        return (allInter[0].Progress == 0 && allInter[1].Progress == 0 && box1.Ratio >= 0.95f && box2.Ratio >= 0.95f);
    }

    public static bool IS_SQUIRREL_FOUND(params Interactable[] allInter)
    {
        return (allInter[0] is Squirrel && allInter[0].Progress == 1);       
    }

    //LolJambe
    public static bool IS_CHAR_PULLED(params Interactable[] allInter)
    {
        return (allInter[0] is PersoEnfoui && ((PersoEnfoui)allInter[0]).Ratio > 0.9f);
    }

    //Enigme de la vis
    public static bool IS_NESSIE_BODY_PULLED(params Interactable[] allInter)
    {
        return (allInter[0] is Nessie && (allInter[0] as Nessie).Ratio > 0.9f);
    }

    //Enigme du nessie
    public static bool IS_FIREWORK_BURNING(params Interactable[] allInter)
    {
        if (!(allInter[0] is Draggable)) return false;
        Draggable dragg = allInter[0] as Draggable;
        return (dragg.CurrentSpot != null && dragg.CurrentSpot.tag == "FireWork" &&
            dragg.CurrentSpot.transform.parent.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(Manager.HOUSE_CLOSED));
    }
}
