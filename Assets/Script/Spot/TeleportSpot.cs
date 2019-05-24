using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

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
    AnimationCurve curveFouille;

    [SerializeField]
    float duration;

    [SerializeField]
    float maxDistance;
    bool busy = false;
    public bool Busy
    {
        get
        {
            return busy;
        }
        set
        {
            busy = value;
        }
    }

    bool seeking = false;
    public bool Seeking
    {
        get
        {
            return seeking;
        }
    }

    float normalizedTime = 0;
    public float NormalizedTime => normalizedTime;

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

    //L'objet a quitté le spot
    public override void HoldObjectLeft(Draggable dragg)
    {
        base.HoldObjectLeft(dragg);
        CurrentHold = null;
        SetMoonAnimation(false, () => { });
    }

    public TeleportSpot GetOtherPart()
    {
        return (this == spot1 ? spot2 : spot1);
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
        //La lune doit etre fermé et ne contenir aucun objet
        IsAvailable = (!spot1.CurrentHold && !spot2.CurrentHold && GetOtherPart().normalizedTime == 0);
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance && IsAvailable && !GetOtherPart().Busy)
        {
            busy = true;
            SetMoonAnimation(value, () => { });
        }
    }

    void SetMoonAnimation(bool reversed, UnityAction afterAnim)
    {
        StopAllCoroutines();
        StartCoroutine(MoonAnimation(reversed, afterAnim));
    }

    IEnumerator MoonAnimation(bool reversed, UnityAction afterAnim)
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
        normalizedTime = Mathf.Clamp(normalizedTime, 0, 1);
        afterAnim();
        busy = false;
    }

    public IEnumerator Transfert()
    {
        busy = true;
        GetOtherPart().Busy = true;
        AkSoundEngine.PostEvent("Play_transfert_in", gameObject);
        //Une fois l'animation de transfert lancé on ne peut rien faire jusqu'a la fin
        currentHold.CanInteract = false;
        //On ferme la lune...
        while (normalizedTime >= 0)
        {
            normalizedTime -= Time.deltaTime / duration;
            partieHaute.localPosition = new Vector3(0, Mathf.Lerp(0, distY, curve.Evaluate(normalizedTime)), 0);
            partieBasse.localPosition = new Vector3(0, Mathf.Lerp(0, -distY, curve.Evaluate(normalizedTime)), 0);
            yield return null;
        }
        normalizedTime = Mathf.Clamp(normalizedTime, 0, 1);

        //...puis on transfert
        currentHold.transform.SetParent(GetOtherPart().transform);
        currentHold.transform.localPosition = Vector3.zero;
        currentHold.CurrentSpot = GetOtherPart();
        GetOtherPart().CurrentHold = currentHold;
        currentHold.CanInteract = true;
        currentHold = null;
        GetOtherPart().Center();
        GetOtherPart().SetMoonAnimation(true, () => {
            busy = false;
            GetOtherPart().Busy = false;
        });

        AkSoundEngine.PostEvent("Play_transfert_out", gameObject);
    }

    public IEnumerator FouilleMoon()
    {
        busy = true;
        normalizedTime = 0;
        while (normalizedTime < 1)
        {
            normalizedTime += Time.deltaTime / duration;
            partieHaute.localPosition = new Vector3(0, Mathf.Lerp(0, distY, curveFouille.Evaluate(normalizedTime)), 0);
            partieBasse.localPosition = new Vector3(0, Mathf.Lerp(0, -distY, curveFouille.Evaluate(normalizedTime)), 0);
            yield return null;
        }
        normalizedTime = 0;
        busy = false;
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

    public override void PressSpot(Draggable dragg)
    {
    }
}
