using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SolScript : MonoBehaviour
{
    public float velocidadRotación;

    // Update is called once per frame
    void Update()
    {
        print(transform.rotation.eulerAngles.x);
        print(velocidadRotación);
        // Días más cortos, noches más largas.
        if(transform.rotation.eulerAngles.x > 180)
        {
            velocidadRotación = 5;
            GetComponent<Light>().cullingMask = 0;
        }
        else
        {
            velocidadRotación = 10;
            GetComponent<Light>().cullingMask = -1;
        }
        transform.Rotate(Vector3.right * velocidadRotación * Time.deltaTime);
    }
}
