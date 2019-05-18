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

    float distance;
    bool canOpen = true;
    bool busy = false;
    public bool CanOpen
    {
        get
        {
            return canOpen;
        }
        set
        {
            if(canOpen != value) StartCoroutine(AnimationMoon(canOpen));
            canOpen = value;
        }
    }

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

    public override void PressSpot(Draggable dragg)
    {
        CurrentHold = null;
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        AkSoundEngine.PostEvent("Play_transfert_in", gameObject);
        dragg.transform.SetParent(transform);
        SetValue(dragg, false);
        currentHold = dragg;
        CanOpen = false;
        GetOtherPart().CanOpen = false;
        GetOtherPart().StartCoroutine(GetOtherPart().StartDelayed(duration));

        foreach(Transform trsf in dragg.transform)
        {
            if (trsf.gameObject.tag == "Center")
            {
                CenterSpotSetup(trsf, transform);
                break;
            }
        }
    }

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

    IEnumerator StartDelayed(float duration)
    {
        yield return new WaitForSeconds(duration);
        AkSoundEngine.PostEvent("Play_transfert_out", gameObject);
        CanOpen = true;
    }

    void Transfert()
    {
        currentHold.transform.SetParent(GetOtherPart().transform);
        currentHold.transform.localPosition = Vector3.zero;
        GetOtherPart().CurrentHold = currentHold;
        currentHold.CurrentSpot = GetOtherPart();
        currentHold = null;

        foreach (Transform trsf in GetOtherPart().CurrentHold.transform)
        {
            if (trsf.gameObject.tag == "Center")
            {
                CenterSpotSetup(trsf, GetOtherPart().transform);
                break;
            }
        }
    }

    IEnumerator AnimationMoon(bool reversed)
    {
        if (currentHold)
            currentHold.CanInteract = false;

        busy = true;
        float normalizedTime = 0;

        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            //Reversed : ouvert a fermé
            float yValue = Mathf.Lerp(0, distance, reversed ? 1 - normalizedTime : normalizedTime);
            PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);
            Debug.LogError(transform.lossyScale);
            if (currentHold)
            {
                currentHold.transform.localScale = new Vector3(1, 1, 1) * (reversed ? 1 - normalizedTime : normalizedTime);
            }

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

        if (currentHold)
            currentHold.CanInteract = true;

        if (reversed && (currentHold != null))
        {
            yield return new WaitForEndOfFrame();
            Transfert();
        }

        busy = false;
    }

    public override void HoldObjectLeft(Draggable dragg)
    {
        base.HoldObjectLeft(dragg);
        dragg.transform.SetParent(null);
        GetOtherPart().CanOpen = true;
    }

    void CenterSpotSetup(Transform center , Transform positionToGo)
    {
        Vector3 delta = positionToGo.position - center.position;
        center.parent.transform.position += delta;
    }
}
