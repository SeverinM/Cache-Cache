using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeuArtifice : MonoBehaviour
{
    [System.Serializable]
    public class SousArtifice
    {
        public Transform tr;
        public Gradient colorGradient;
        public SpriteRenderer sP;
        public AnimationCurve scaleCurve;
        public AnimationCurve speedCurve;

        public float randomGravity = 1;
        public float randomSpeed = 1;

        public TrailRenderer trail;
    }

    public Vector3 positionInSpace = new Vector3(1, 0, 0);
    [Tooltip("littleOffsetJustToMakeItNotFlat")]
    public float oof = 0.001f;


    [Header("Ascending")]
    public SpriteRenderer sP;
    public float ascendingDuration;
    public float ascendingSpeed;
    public Vector2 minMaxDecal = new Vector2(-0.1f, 0.1f);
    public Gradient ascendingColor;
    public AnimationCurve ascendingCurve;
    public AnimationCurve lateralCurve;
    public AnimationCurve scaleCurve;
    public TrailRenderer tR;
    private float littleRandomHeight = 1f;

    [Header("Wait")]
    public float waitBeforeSplosion;

    [Header("Explosion")]
    public GameObject prefabSousArtifice;
    public Vector2 minMaxBranches = new Vector2(4, 8);
    public AnimationCurve sousArtificeScaleCurve;
    public AnimationCurve sousArtificeSpeedCurve;

    [Header("Branches")]
    public float brancheDuration;
    public float brancheSpeed;
    public List<Gradient> colorGradient = new List<Gradient>();
    public AnimationCurve gravitationCurve;
    public Vector2 randGraviMinMax = new Vector2(0.95f, 1.05f);
    public Vector2 randSpeedMinMax = new Vector2(0.8f, 1.2f);

    [SerializeField]
    public List<SousArtifice> branches = null;

    // Start is called before the first frame update
    void Start()
    {
        if (sP == null)
            sP = this.GetComponent<SpriteRenderer>();
        if (tR == null)
            tR = this.GetComponent<TrailRenderer>();
        StartCoroutine(Ascending());
    }

    IEnumerator Ascending()
    {
        float lerpVal = 0;
        Vector3 directionLeftRight = positionInSpace * Random.Range(minMaxDecal.x, minMaxDecal.y);
        littleRandomHeight = Random.Range(0.7f, 1.5f);
        Vector3 directionUp = new Vector3(Random.Range(-oof, oof), ascendingSpeed * littleRandomHeight, Random.Range(-oof, oof));
        while (lerpVal < 1)
        {
            this.transform.position += directionUp * ascendingCurve.Evaluate(lerpVal) + directionLeftRight * lateralCurve.Evaluate(lerpVal);
            this.transform.localScale = Vector3.one * scaleCurve.Evaluate(lerpVal) * littleRandomHeight;
            tR.startWidth = transform.localScale.y * 0.22f;

            sP.color = ascendingColor.Evaluate(lerpVal);
            tR.startColor = sP.color;

            lerpVal += Time.deltaTime / ascendingDuration;


            yield return new WaitForSeconds(0.01f);
        }
        sP.enabled = false;
        yield return new WaitForSeconds(waitBeforeSplosion);
        this.transform.localScale = Vector3.one;
        Explosion();
    } 

    public void Explosion()
    {
        branches = new List<SousArtifice>();
        int nmbrBranch = Random.Range((int)minMaxBranches.x, (int)minMaxBranches.y);
        int indexGradient = Random.Range(0, colorGradient.Count);
        for (int i = 0; i < nmbrBranch; i++)
        {
            Quaternion rot = this.transform.rotation;
            GameObject g0 = Instantiate(prefabSousArtifice, this.transform.position, rot, this.transform);
            SousArtifice sA = new SousArtifice();
            sA.tr = g0.transform;
            sA.sP = g0.GetComponent<SpriteRenderer>();
            sA.trail = g0.GetComponent<TrailRenderer>();
            sA.colorGradient = colorGradient[indexGradient];
            sA.scaleCurve = sousArtificeScaleCurve;
            sA.speedCurve = sousArtificeSpeedCurve;
            branches.Add(sA);

            sA.tr.Rotate(0, 0, i * (float)360 / (float)nmbrBranch);

            sA.randomGravity = Random.Range(randGraviMinMax.x, randGraviMinMax.y);
            sA.randomSpeed = Random.Range(randSpeedMinMax.x, randSpeedMinMax.y);
        }
        this.transform.Rotate(0, 0, Random.Range(0, 359));
        StartCoroutine(ExplosionBranches());
    }

    IEnumerator ExplosionBranches()
    {
        float lerpVal = 0;
        Vector3 directionBase = new Vector3(Random.Range(-oof, oof), brancheSpeed, Random.Range(-oof, oof));
        while (lerpVal < 1)
        {
            foreach (SousArtifice sA in branches)
            {
                sA.tr.Translate(directionBase * sA.speedCurve.Evaluate(lerpVal) * sA.randomSpeed, Space.Self);
                sA.tr.position += Vector3.down * gravitationCurve.Evaluate(lerpVal) * sA.randomGravity;
                sA.tr.localScale = Vector3.one * sA.scaleCurve.Evaluate(Mathf.Clamp01(lerpVal * sA.randomSpeed));
                sA.trail.startWidth = sA.tr.localScale.y * 0.7f;
                sA.sP.color = sA.colorGradient.Evaluate(lerpVal);
                sA.trail.startColor = sA.sP.color;
            }
            lerpVal += Time.deltaTime / brancheDuration;
            yield return new WaitForSeconds(0.01f);
        }
        foreach (SousArtifice sA in branches)
        {
            Destroy(sA.tr.gameObject);
        }
        Destroy(this.gameObject);
    }
}
