using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : MonoBehaviour
{
    public Transform trackPos;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = trackPos.position + offset;
    }
}
