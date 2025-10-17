using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    Vector3 CamOff;
    // Start is called before the first frame update
    void Start()
    {
        CamOff = transform.position - target.position;

    }

    private void FixedUpdate()
    {
        transform.position = target.position + CamOff;
    }


}
