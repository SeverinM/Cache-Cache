using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAround : Interactable
{
    [SerializeField]
    Transform source;

    [SerializeField]
    Transform target;

    [SerializeField]
    float speed;
    float currendSpeed = 0;

    private void Update()
    {
        source.RotateAround(target.transform.position, Vector3.up, currendSpeed * Time.deltaTime);
    }

    public override void MouseDown(int id)
    {
        currendSpeed += speed;
        GetComponent<Image>().color = Color.clear;
    }

    public override void MouseEnter(int id)
    {
        Debug.Log("enter");
    }

    public override void MouseLeave(int id)
    {
        Debug.Log("exit");
    }

    public override void MouseMove(int id, Vector2 delta)
    {
    }

    public override void MouseUp(int id)
    {
        currendSpeed -= speed;
        GetComponent<Image>().color = Color.black;
    }
}
