using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class JugadorScript : MonoBehaviour
{
    private Rigidbody rb;
    private float movX;
    private float movZ;
    private float mouseX;
    private float mouseY;
    public float velB; // Velocidad base del personaje.
    public float velR; // Velocidad al correr del personaje.
    private float velAct; // Velocidad actual del personaje.
    public float velMouse; // Sensibilidad del mouse.
    public GameObject ojos;
    public float fSalto; // Fuerza del salto.
    private float rotAcumX; // Variable para ir acumulando la rotación de la cámara en el eje x.
    private Animator animator;
    private bool salto;

    private RaycastHit hit;

    private enum ESTADO {MOVIMIENTO, CONVERSACION}
    private ESTADO estado;

    void Start()
    {
        estado = ESTADO.MOVIMIENTO;
        rb = GetComponent<Rigidbody>();
        rotAcumX = 0;
        velAct = velB;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        switch (estado)
        {
            case ESTADO.MOVIMIENTO:
                Movimiento();
                break;
            case ESTADO.CONVERSACION:
                break;
        }
        
    }
    private void Movimiento()
    {
        // Condición para empezar a conversar
        if (rayoInteraccion()
            && hit.collider.gameObject != null
            && hit.collider.gameObject.CompareTag("enemigo")
            && Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) < 5
            && Input.GetKeyDown(KeyCode.E))
        {
            hit.collider.gameObject.GetComponent<EnemigoScript>().conversar = true;
            estado = ESTADO.CONVERSACION;
            return;
        }
        // Movimiento del jugador:
        mover();
        // Giro del jugador:
        girar();
        // Saltar
        if (Input.GetButtonDown("Jump") && !animator.GetBool("saltar")) // Si se pulsa espacio, saltamos.
        {
            StartCoroutine("saltar");
            //saltar();
        }
    }
    IEnumerator saltar()
    {
        animator.SetBool("saltar", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("saltar", false);
    }
    private void mover()
    {
        movX = Input.GetAxis("Horizontal");
        movZ = Input.GetAxis("Vertical");
        if (movZ > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            { // Correr.
                animator.SetBool("correr", true);
            }
            else
            {
                animator.SetBool("correr", false);
            }
            animator.SetBool("andar", true);
            animator.SetBool("atras", false);
        }
        else if (movZ < 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            { // Correr.
                animator.SetBool("correrAtras", true);
            }
            else
            {
                animator.SetBool("correrAtras", false);
            }
            animator.SetBool("atras", true);
            animator.SetBool("andar", false);
        }
        else
        {
            animator.SetBool("andar", false);
            animator.SetBool("atras", false);
        }
        if(movX > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            { // Correr.
                animator.SetBool("correrDerecha", true);
            }
            else
            {
                animator.SetBool("correrDerecha", false);
            }
            animator.SetBool("andarDerecha", true);
            animator.SetBool("andarIzquierda", false);
        }
        else if(movX < 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            { // Correr.
                animator.SetBool("correrIzquierda", true);
            }
            else
            {
                animator.SetBool("correrIzquierda", false);
            }
            animator.SetBool("andarIzquierda", true);
            animator.SetBool("andarDerecha", false);
        }
        else
        {
            animator.SetBool("andarDerecha", false);
            animator.SetBool("andarIzquierda", false);
        }
    }
    private void girar()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y") * -1; // Multiplico por -1 porque está invertido.
        transform.Rotate(0, mouseX * velMouse, 0); // Rotación del personaje en el eje y.
        // Rotación de la cámara en el eje x:
        rotAcumX += mouseY;
        rotAcumX = Mathf.Clamp(rotAcumX, -60 / velMouse, 60 / velMouse); // Limitación de la cámara en el eje x.
        ojos.transform.localRotation = Quaternion.Euler(rotAcumX * velMouse, 0, 0); // Giro de la cámara en el eje x.
    }
    /*
    private void saltar()
    {
        if (salto)
        {
            float masa = rb.mass;
            rb.AddForce(new Vector3(0, masa * fSalto, 0), ForceMode.Impulse); // Salto.
            salto = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            salto = true;
        }
    }
    */
    private bool rayoInteraccion()
    {
        bool deteccion = Physics.Raycast(ojos.transform.position, ojos.transform.forward, out hit);
        Debug.DrawRay(ojos.transform.position, ojos.transform.forward * 10, Color.red);
        return deteccion;
    }
}
