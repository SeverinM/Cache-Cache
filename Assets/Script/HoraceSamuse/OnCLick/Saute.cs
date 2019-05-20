using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saute : MonoBehaviour
{
    public AnimationCurve hauteurSautFonctionDuTemps = AnimationCurve.Constant(0, 1, 0);
    public float dureeDuSaut = 1;

    private float LerpValue = 1;
    private float startYPos;

    // Start is called before the first frame update
    void Start()
    {
        startYPos = transform.localPosition.y;
    }
    private void Update()
    {
        if (LerpValue != 1)
        {
            Vector3 tmp = this.transform.localPosition;
            tmp.y = startYPos + hauteurSautFonctionDuTemps.Evaluate(LerpValue);
            this.transform.localPosition = tmp;
            
            LerpValue += Time.deltaTime / dureeDuSaut;
            if (LerpValue > 1)
                LerpValue = 1;
        }
    }

    [ContextMenu("Touche me")]
    public void OnMouseDown()
    {
        LerpValue = 0;
    }
}
