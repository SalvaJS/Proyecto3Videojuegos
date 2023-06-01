using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SolScript : MonoBehaviour
{
    public float velocidadRotaci�n;

    // Update is called once per frame
    void Update()
    {
        print(transform.rotation.eulerAngles.x);
        print(velocidadRotaci�n);
        // D�as m�s cortos, noches m�s largas.
        if(transform.rotation.eulerAngles.x > 180)
        {
            velocidadRotaci�n = 5;
            GetComponent<Light>().cullingMask = 0;
        }
        else
        {
            velocidadRotaci�n = 10;
            GetComponent<Light>().cullingMask = -1;
        }
        transform.Rotate(Vector3.right * velocidadRotaci�n * Time.deltaTime);
    }
}
