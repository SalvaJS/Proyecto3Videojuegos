using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarolaScript : MonoBehaviour
{
    public bool farolaFundida;
    private bool efectoEnCurso;
    private Light luz;
    public GameObject sol;
    public GameObject foco;
    public Material apagada;
    public Material encendida;
    private Renderer renderer;
    void Start()
    {
        luz = foco.GetComponent<Light>();
        renderer = foco.GetComponent<Renderer>();
        luz.intensity = 0;
        renderer.material = apagada;
        efectoEnCurso = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(sol.transform.rotation.eulerAngles.x > 180)
        {
            if(farolaFundida && !efectoEnCurso)
            {
                efectoEnCurso = true;
                StartCoroutine("EfectoFundido");
            }
            else if (!farolaFundida)
            {
                luz.intensity = 2;
                renderer.material = encendida;
            }
        }
        else
        {
            luz.intensity = 0;
            renderer.material = apagada;
        }
    }
    IEnumerator EfectoFundido()
    {
        luz.intensity = 2;
        renderer.material = encendida;
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        luz.intensity = 0;
        renderer.material = apagada;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        efectoEnCurso = false;
    }
} 
