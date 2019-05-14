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
    }

    public override void ExitSpot(Draggable dragg)
    {
    }

    public override void PressSpot(Draggable dragg)
    {
        CurrentHold = null;
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        SetValue(dragg, false);
        currentHold = dragg;
        CanOpen = false;
        GetOtherPart().CanOpen = false;
        GetOtherPart().StartCoroutine(GetOtherPart().StartDelayed(duration));
    }

    public override void ResetSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
        currentHold = dragg;
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance && (currentHold == null || !value) && !busy)
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

    TeleportSpot GetOtherPart()
    {
        return (this != spot1 ? spot1 : spot2);
    }

    IEnumerator StartDelayed(float duration)
    {
        yield return new WaitForSeconds(duration);
        CanOpen = true;
    }

    void Transfert()
    {
        currentHold.transform.position = GetOtherPart().transform.position;
        GetOtherPart().CurrentHold = currentHold;
        currentHold.CurrentSpot = GetOtherPart();
        currentHold = null;
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
            float yValue = Mathf.Lerp(0, distance, reversed ? 1 - normalizedTime : normalizedTime);
            PartieHaute.transform.localPosition = new Vector3(0, yValue, 0);
            PartieBasse.transform.localPosition = new Vector3(0, -yValue, 0);
            if (currentHold)
            {
                currentHold.transform.localScale = new Vector3(1, 1, 1) * (reversed ? 1 - normalizedTime : normalizedTime);
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
}
