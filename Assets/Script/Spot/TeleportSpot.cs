using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpot : Spot
{
    static TeleportSpot spot1;
    static TeleportSpot spot2;

    [SerializeField]
    float maxDistance = 500;

    [SerializeField]
    MoonPart PartieHaute;
    [SerializeField]
    MoonPart PartieBasse;
    [SerializeField]
    Transform AutreTPSpot;

    [SerializeField]
    float animationSpeed = 2f;

    Vector3 OuvertHaut;
    Vector3 OuvertBas;
    Vector3 OuvertHautEcho;
    Vector3 OuvertBasEcho;

    static bool busy = false;
    
    private void Update()
    {
        //vérifie si la lune contient un objet transféré
        if(busy && this.transform.childCount == 0 /*&& this.spot2.transform.childCount == 0*/) //comment on vérifie que le spot echo n'a pas d'enfant non plus ?
        {
            StartCoroutine(AnimationReset());
            busy = false;
        }
    }

    public override void EnterSpot(Draggable dragg)
    {
    }

    public override void ExitSpot(Draggable dragg)
    {
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        if(!busy)
        {
            busy = true;

            SetValue(dragg, false);

            //sauvegarde les positions des parties des lunes
            OuvertHaut = PartieHaute.transform.localPosition;
            OuvertBas = PartieBasse.transform.localPosition;
            OuvertHautEcho = PartieHaute.Echo.transform.localPosition;
            OuvertBasEcho = PartieBasse.Echo.transform.localPosition;

            StartCoroutine(AnimationTransfert());

            dragg.transform.position = (this != spot1 ? spot1 : spot2).transform.position;
            dragg.transform.SetParent((this != spot1 ? spot1 : spot2).transform);
        }
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = value;
            gameObject.GetComponent<Collider>().enabled = value;
        }
            
    }

    private void Awake()
    {
        bool chosen = false;
        if (!spot1)
        {
            spot1 = this;
            chosen = true;
        }

        if (!spot2 && !chosen)
        {
            spot2 = this;
        }
    }

    IEnumerator AnimationTransfert()
    {
        float normalizeTime = 0;

        //ferme les deux lunes
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime * animationSpeed;

            PartieHaute.transform.localPosition = Vector3.Lerp(OuvertHaut, Vector3.zero, normalizeTime);
            PartieBasse.transform.localPosition = Vector3.Lerp(OuvertBas, Vector3.zero, normalizeTime);

            PartieHaute.Echo.transform.localPosition = Vector3.Lerp(OuvertHautEcho, Vector3.zero, normalizeTime);
            PartieBasse.Echo.transform.localPosition = Vector3.Lerp(OuvertBasEcho, Vector3.zero, normalizeTime);

            yield return new WaitForSeconds(0.01f);
        }

        //ouvre la lune de destination
        while (normalizeTime < 2)
        {
            normalizeTime += Time.deltaTime * animationSpeed;

            PartieHaute.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHautEcho, normalizeTime-1);
            PartieBasse.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBasEcho, normalizeTime-1);

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator AnimationReset()
    {
        float normalizeTime = 0;

        //ouvre la lune de départ
        while (normalizeTime < 1)
        {
            normalizeTime += Time.deltaTime * animationSpeed;

            PartieHaute.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHautEcho, normalizeTime);
            PartieBasse.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBasEcho, normalizeTime);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
