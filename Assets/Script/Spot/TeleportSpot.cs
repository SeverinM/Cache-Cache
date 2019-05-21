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
    float duration;
    public float Duration => duration;

    Vector3 objectScale;
    float distance;
    bool busy = false;
    bool closing = false;

    public IEnumerator currentAnimation;

    public void Init()
    {
        distance = PartieHaute.transform.localPosition.y;
        currentHold = null;
    }

    public override void EnterSpot(Draggable dragg)
    {
        bool found = false;
        foreach (Transform trsf in dragg.transform)
        {
            if (trsf.gameObject.tag == "Center")
            {
                CenterSpotSetup(trsf, transform);
                found = true;
                break;
            }
        }
        if (!found)
            base.EnterSpot(dragg);
    }

    //On a pris l'objet dans la lune , on ferme l'autre
    public override void PressSpot(Draggable dragg)
    {
        CurrentHold = null;
        GetOtherPart().StartCoroutine(GetOtherPart().AnimationMoon(true));
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        AkSoundEngine.PostEvent("Play_transfert_in", gameObject);
        dragg.transform.SetParent(transform);
        SetValue(dragg, false);
        currentHold = dragg;

        foreach(Transform trsf in dragg.transform)
        {
            if (trsf.gameObject.tag == "Center")
            {
                CenterSpotSetup(trsf, transform);
                break;
            }
        }

        StartCoroutine(AnimationClone());
    }

    //Il revient a son spot , on la rouvre
    public override void ResetSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
        currentHold = dragg;

        foreach (Transform trsf in CurrentHold.transform)
        {
            if (trsf.gameObject.tag == "Center")
            {
                CenterSpotSetup(trsf, transform);
                break;
            }
        }
        GetOtherPart().StartCoroutine(GetOtherPart().AnimationMoon(false));
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (!dragg || (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance && (currentHold == null || !value) && !busy))
        {
            GetComponent<Collider>().enabled = value;
            if (!value)
            {
                GetComponent<ParticleSystem>().Stop();
            }
            else
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
            
    }

    private void Awake()
    {
        bool chosen = false;
        if (!spot1)
        {
            spot1 = this;
            chosen = true;
            SetValue(null, false);
        }

        if (!spot2 && !chosen)
        {
            spot2 = this;
            SetValue(null, false);
        }
    }

    TeleportSpot GetOtherPart()
    {
        return (this != spot1 ? spot1 : spot2);
    }

    IEnumerator AnimationMoon(bool reversed)
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            //Reversed : ouvert a fermé
            float yValue = Mathf.Lerp(0, distance, reversed ? 1 - normalizedTime : normalizedTime);
            PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);

            if (!reversed && currentHold)
            {
                foreach (Transform trsf in CurrentHold.transform)
                {
                    if (trsf.gameObject.tag == "Center")
                    {
                        CenterSpotSetup(trsf, transform);
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator AnimationClone()
    {
        if (currentHold)
            currentHold.CanInteract = false;

        busy = true;
        float normalizedTime = 0;
        bool reversed = true;

        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            //Reversed : ouvert a fermé
            float yValue = Mathf.Lerp(0, distance, reversed ? 1 - normalizedTime : normalizedTime);
            PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);
            GetOtherPart().PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            GetOtherPart().PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);

            if (!reversed && currentHold)
            {
                foreach (Transform trsf in CurrentHold.transform)
                {
                    if (trsf.gameObject.tag == "Center")
                    {
                        CenterSpotSetup(trsf, transform);
                        break;
                    }
                }
            }           
            yield return null;
        }

        //La copie est lié a l'autre lune
        GameObject copy = Instantiate(CurrentHold.gameObject);
        copy.transform.SetParent(GetOtherPart().transform);
        copy.transform.localPosition = Vector3.zero;
        GetOtherPart().CurrentHold = copy.GetComponent<Draggable>();
        copy.transform.rotation = CurrentHold.transform.rotation;
        copy.GetComponent<Draggable>().CurrentSpot = GetOtherPart();
        copy.transform.localScale = CurrentHold.transform.localScale;

        while (normalizedTime >= 0)
        {
            normalizedTime -= Time.deltaTime / duration;
            float yValue = Mathf.Lerp(0, distance, reversed ? 1 - normalizedTime : normalizedTime);
            PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);
            GetOtherPart().PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            GetOtherPart().PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);

            foreach (Transform trsf in CurrentHold.transform)
            {
                if (trsf.gameObject.tag == "Center")
                {
                    CenterSpotSetup(trsf, transform);
                    break;
                }
            }

            foreach (Transform trsf in GetOtherPart().CurrentHold.transform)
            {
                if (trsf.gameObject.tag == "Center")
                {
                    CenterSpotSetup(trsf, GetOtherPart().transform);
                    break;
                }
            }
            yield return null;
        }

        if (currentHold)
            currentHold.CanInteract = true;

        busy = false;
    }

    //Nouveau spot trouvé , on supprime l'autre
    public override void HoldObjectLeft(Draggable dragg)
    {
        Destroy(GetOtherPart().CurrentHold.gameObject);
        base.HoldObjectLeft(dragg);
        dragg.transform.SetParent(null);
        GetOtherPart().StartCoroutine(GetOtherPart().AnimationMoon(false));
    }

    void CenterSpotSetup(Transform center , Transform positionToGo)
    {
        Vector3 delta = positionToGo.position - center.position;
        center.parent.transform.position += delta;
    }
}
