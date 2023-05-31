using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuiaScript : MonoBehaviour
{
    public GameObject helicoptero;
    private NavMeshAgent agente;
    public enum Estado {BAJOHELICOPTERO, SEGUIRJUGADOR}
    public Estado estado;
    public GameObject jugador;
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.enabled = false;
        estado = Estado.BAJOHELICOPTERO;
    }

    void Update()
    {
        //print("Estado guia: " + estado);
        switch (estado)
        {
            case Estado.BAJOHELICOPTERO:
                BajoHelicoptero();
                break;
            case Estado.SEGUIRJUGADOR:
                SeguirJugador();
                break;
        }
    }
    private void SeguirJugador()
    {
        agente.destination = new Vector3(jugador.transform.position.x, 0, jugador.transform.position.z);
        agente.speed = jugador.GetComponent<Rigidbody>().velocity.magnitude;
    }
    private void BajoHelicoptero()
    {
        transform.position = new Vector3(helicoptero.transform.position.x, 0, helicoptero.transform.position.z);
    }
    public void CambiarASeguirJugador()
    {
        agente.enabled = true;
        estado = Estado.SEGUIRJUGADOR;
    }
}
