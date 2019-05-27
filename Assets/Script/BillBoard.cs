using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BillBoard : MonoBehaviour
{
    [SerializeField]
    List<Camera> allCams;

    Camera cam;

    private void Start()
    {
        //Billboard sur la camera la plus proche de l'objet
        cam = allCams.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = -cam.transform.forward;
    }
}
