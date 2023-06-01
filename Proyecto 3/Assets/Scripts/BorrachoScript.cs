using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BorrachoScript : MonoBehaviour
{
    private NavMeshAgent agente;
    public GameObject[] posPatrulla;
    private int posAct;
    private Animator animator;
    private bool cambiandoPos;
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        posAct = Random.Range(0, posPatrulla.Length);
        agente.destination = posPatrulla[posAct].transform.position;
        animator.SetInteger("andar", Random.Range(1, 3));
        cambiandoPos = false;
    }
    void Update()
    {
        if(agente.remainingDistance <= agente.stoppingDistance && !cambiandoPos)
        {
            cambiandoPos = true;
            StartCoroutine("CambioDestino");
        }
    }
    IEnumerator CambioDestino()
    {
        animator.SetInteger("andar", 0);
        animator.SetInteger("nIdle", Random.Range(0, 3));
        yield return new WaitForSeconds(Random.Range(0, 11));
        int posAnt = posAct;
        while(posAnt == posAct)
        {
            posAct = Random.Range(0, posPatrulla.Length);
        }
        agente.destination = posPatrulla[posAct].transform.position;
        animator.SetInteger("nIdle", 0);
        animator.SetInteger("andar", Random.Range(1, 3));
        cambiandoPos = false;
    }
}
