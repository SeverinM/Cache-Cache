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
    static float animationSpeed = 2f;

    static Vector3 OuvertHaut1;
    static Vector3 OuvertBas1;
    static Vector3 OuvertHaut2;
    static Vector3 OuvertBas2;

    static bool busy = false;
    static bool hiverToEte;
    static bool eteToHiver;
    
    private void Update()
    {
        //vérifie si la lune contient un objet transféré
        if(busy && spot1.transform.childCount == 0 && spot2.transform.childCount == 0)
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

            Debug.LogError("initialisation " + gameObject);
            //sauvegarde les positions des parties des lunes

            if(this == spot1)
            {
                OuvertHaut1 = PartieHaute.transform.localPosition;
                OuvertBas1 = PartieBasse.transform.localPosition;
                OuvertHaut2 = PartieHaute.Echo.transform.localPosition;
                OuvertBas2 = PartieBasse.Echo.transform.localPosition;

                eteToHiver = true;
                hiverToEte = false;
            }
            if(this == spot2)
            {
                OuvertHaut2 = PartieHaute.transform.localPosition;
                OuvertBas2 = PartieBasse.transform.localPosition;
                OuvertHaut1 = PartieHaute.Echo.transform.localPosition;
                OuvertBas1 = PartieBasse.Echo.transform.localPosition;

                eteToHiver = false;
                hiverToEte = true;
            }
            

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

            PartieHaute.transform.localPosition = Vector3.Lerp(OuvertHaut1, Vector3.zero, normalizeTime);
            PartieBasse.transform.localPosition = Vector3.Lerp(OuvertBas1, Vector3.zero, normalizeTime);

            PartieHaute.Echo.transform.localPosition = Vector3.Lerp(OuvertHaut2, Vector3.zero, normalizeTime);
            PartieBasse.Echo.transform.localPosition = Vector3.Lerp(OuvertBas2, Vector3.zero, normalizeTime);

            yield return new WaitForSeconds(0.01f);
        }

        //ouvre la lune de destination
        if(eteToHiver)
        {
            while (normalizeTime < 2)
            {
                normalizeTime += Time.deltaTime * animationSpeed;

                PartieHaute.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHaut2, normalizeTime - 1);
                PartieBasse.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBas2, normalizeTime - 1);

                yield return new WaitForSeconds(0.01f);
            }
        }

        if (hiverToEte)
        {
            while (normalizeTime < 2)
            {
                normalizeTime += Time.deltaTime * animationSpeed;

                PartieHaute.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHaut1, normalizeTime - 1);
                PartieBasse.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBas1, normalizeTime - 1);

                yield return new WaitForSeconds(0.01f);
            }
        }

    }

    IEnumerator AnimationReset()
    {
        float normalizeTime = 0;

        //ouvre la lune de départ
        if(eteToHiver)
        {
            while (normalizeTime < 1)
            {
                normalizeTime += Time.deltaTime * animationSpeed;

                PartieHaute.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHaut1, normalizeTime);
                PartieBasse.Echo.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBas1, normalizeTime);

                yield return new WaitForSeconds(0.01f);
            }
        }

        if (hiverToEte)
        {
            while (normalizeTime < 1)
            {
                normalizeTime += Time.deltaTime * animationSpeed;

                PartieHaute.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertHaut2, normalizeTime);
                PartieBasse.transform.localPosition = Vector3.Lerp(Vector3.zero, OuvertBas2, normalizeTime);

                yield return new WaitForSeconds(0.01f);
            }
        }

    }
}
