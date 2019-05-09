using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpot : ClassicSpot
{
    bool occupied = false;
    public override void ReleaseSpot(Draggable dragg)
    {
        //Filtre que les arbres + SI ILS NE SONT PAS DEJA OCCUPE PAR UN ARBRE
        if (dragg is Tree &&! occupied)
        {
            dragg.transform.position = transform.position;
            occupied = true; // JE SAIS PAS QUEL EVENT APPELER POUR METTRE OCCUPIED EN FALSE
        }
    }
}
