using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxeadorScript : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void CambiarEstilo()
    {
        //print("Entra en funci�n");
        animator.SetInteger("tipo", Random.Range(0, 5));
    }
}
