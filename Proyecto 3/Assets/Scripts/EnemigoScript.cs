using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoScript : MonoBehaviour
{
    private enum ESTADO {PATRULLAR, MIRARJUGADOR}
    private ESTADO estado;
    private NavMeshAgent agente;
    public GameObject[] posPatrulla;
    private int posAct;
    private Animator animator;
    private bool cambiandoPos;
    public GameObject jugador;
    public AudioClip[] aEncuentroJugador;
    private int aPosAct;
    private AudioSource audio;
    void Start()
    {
        estado = ESTADO.PATRULLAR;
        agente = GetComponent<NavMeshAgent>();
        posAct = 0;
        animator = GetComponent<Animator>();
        cambiandoPos = false;
        audio = GetComponent<AudioSource>();
        aPosAct = -1;
    }
    void Update()
    {
        switch (estado)
        {
            case ESTADO.PATRULLAR:
                Patrullar();
                break;
            case ESTADO.MIRARJUGADOR:
                MirarJugador();
                break;
        }
    }
    // Método de patrulla.
    private void Patrullar()
    {
        if(Vector3.Distance(transform.position, jugador.transform.position) < 5)
        {
            estado = ESTADO.MIRARJUGADOR;
            animator.SetBool("andar", false);
            agente.destination = transform.position;
            int aPosAnt = aPosAct;
            AudioClip a = null;
            while(aPosAnt == aPosAct) // Escoger un audio diferente al anterior.
            {
                aPosAct = Random.Range(0, aEncuentroJugador.Length);
                a = aEncuentroJugador[aPosAct];
            }
            audio.PlayOneShot(a);
            return;
        }
        if(agente.remainingDistance <= agente.stoppingDistance && !cambiandoPos)
        {
            cambiandoPos = true;
            StartCoroutine("CambiarPosicion");
        }
        else if (!cambiandoPos)
        {
            animator.SetBool("andar", true);
        }
    }
    // Corutina para cambiar de posición pasados 3 segundos.
    private IEnumerator CambiarPosicion()
    {
        animator.SetBool("andar", false);
        yield return new WaitForSeconds(3);
        if(estado == ESTADO.PATRULLAR)
        {
            int posAnt = posAct;
            while (posAct == posAnt)
            {
                posAct = Random.Range(0, posPatrulla.Length);
            }
            print("Posicion " + posAct);
            agente.destination = posPatrulla[posAct].gameObject.transform.position;
        }
        cambiandoPos = false;
    }
    // Método para interactuar con el jugador.
    private void MirarJugador()
    {
        if (Vector3.Distance(transform.position, jugador.transform.position) >= 5)
        {
            estado = ESTADO.PATRULLAR;
            return;
        }
        transform.LookAt(new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z));
    }
}
