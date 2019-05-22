using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpot : Spot
{
    static TeleportSpot spot1;
    static TeleportSpot spot2;

    [SerializeField]
    public Transform partieHaute;

    [SerializeField]
    public Transform partieBasse;

    [SerializeField]
    float distY;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    float duration;

    [SerializeField]
    float maxDistance;
    bool busy = false;

    float normalizedTime = 0;
    bool isAvailable = true;
    public bool IsAvailable
    {
        get
        {
            return isAvailable;
        }

        set
        {
            isAvailable = value;
            GetComponent<Collider>().enabled = value;
        }
    }

    bool canOpen = false;
    public bool CanOpen
    {
        get
        {
            return canOpen;
        }

        set
        {
            canOpen = value;
            SetMoonAnimation(canOpen);
        }
    }

    private void Awake()
    {
        if (!spot1)
        {
            spot1 = this;
            foreach(Draggable dragg in GameObject.FindObjectsOfType<Draggable>())
            {
                dragg.AddSpot(this);
            }
            return;
        }
        
        if (!spot2)
        {
            foreach (Draggable dragg in GameObject.FindObjectsOfType<Draggable>())
            {
                dragg.AddSpot(this);
            }
            spot2 = this;
        }
    }

    TeleportSpot GetOtherPart()
    {
        return (this == spot1 ? spot2 : spot1);
    }

    public override void PressSpot(Draggable dragg)
    {
        CurrentHold = null;
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
        Center();
        StopAllCoroutines();
        StartCoroutine(Transfert());
    }

    public override void EnterSpot(Draggable dragg)
    {
        base.EnterSpot(dragg);
        foreach (Transform trsf in dragg.transform)
        {
            if (trsf.tag == "Center")
            {
                Vector3 delta = transform.position - trsf.position;
                dragg.transform.position += delta;
                break;
            }
        }
    }

    public override void ResetSpot(Draggable dragg)
    {
        CurrentHold = dragg;
        dragg.transform.position = transform.position;
        Center();
    }

    private void Update()
    {
        IsAvailable = (!spot1.CurrentHold && !spot2.CurrentHold);
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance && CurrentHold != dragg && IsAvailable)
        {
            CanOpen = value;
        }
    }

    void SetMoonAnimation(bool reversed)
    {
        StopAllCoroutines();
        StartCoroutine(MoonAnimation(reversed));
    }

    IEnumerator MoonAnimation(bool reversed)
    {
        //true = de fermé à ouvert
        if (reversed)
        {
            while (normalizedTime <= 1)
            {
                normalizedTime += Time.deltaTime / duration;
                partieHaute.localPosition = new Vector3(0, Mathf.Lerp(0, distY, curve.Evaluate(normalizedTime)), 0);
                partieBasse.localPosition = new Vector3(0, Mathf.Lerp(0, -distY, curve.Evaluate(normalizedTime)), 0);
                yield return null;
            }
        }
        else
        {
            while (normalizedTime >= 0)
            {
                normalizedTime -= Time.deltaTime / duration;
                partieHaute.localPosition = new Vector3(0, Mathf.Lerp(0, distY, curve.Evaluate(normalizedTime)), 0);
                partieBasse.localPosition = new Vector3(0, Mathf.Lerp(0, -distY, curve.Evaluate(normalizedTime)), 0);
                yield return null;
            }
        }
    }

    IEnumerator Transfert()
    {
        //On ferme la lune...
        while (normalizedTime >= 0)
        {
            normalizedTime -= Time.deltaTime / duration;
            partieHaute.localPosition = new Vector3(0, Mathf.Lerp(0, distY, curve.Evaluate(normalizedTime)), 0);
            partieBasse.localPosition = new Vector3(0, Mathf.Lerp(0, -distY, curve.Evaluate(normalizedTime)), 0);
            yield return null;
        }

        //...puis on transfert
        currentHold.transform.SetParent(GetOtherPart().transform);
        currentHold.transform.localPosition = Vector3.zero;
        currentHold.CurrentSpot = GetOtherPart();
        GetOtherPart().CurrentHold = currentHold;
        currentHold = null;
        GetOtherPart().Center();

        GetOtherPart().CanOpen = true;
    }

    public void Center()
    {
        foreach(Transform trsf in CurrentHold.transform)
        {
            if (trsf.tag == "Center")
            {
                Vector3 delta = transform.position - trsf.position;
                CurrentHold.transform.position += delta;
                break;
            }
        }
    }
}
