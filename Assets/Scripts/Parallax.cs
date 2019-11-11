using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cam;
    public float moveRate;
    private float startPointX,startPointY;
    public bool lockY;
    void Start()
    {
        startPointX = transform.position.x;
        startPointY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (lockY)
        {
            transform.Translate(new Vector2(startPointX+cam.position.x,transform.position.y));
        }
        else
        {
            transform.Translate(new Vector2(startPointX + cam.position.x, startPointY+cam.position.y));
        }
    }
}
