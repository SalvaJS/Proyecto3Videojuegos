using java.awt;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Canvas = UnityEngine.Canvas;

public class EnemigoScript : MonoBehaviour
{
    private enum ESTADO {PATRULLAR, MIRARJUGADOR, CONVERSAR}
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
    public Canvas canvas;
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button boton;
    private OpenAIAPI api;
    private List<ChatMessage> mensajes;

    public bool conversar;
    void Start()
    {
        estado = ESTADO.PATRULLAR;
        agente = GetComponent<NavMeshAgent>();
        posAct = 0;
        animator = GetComponent<Animator>();
        cambiandoPos = false;
        audio = GetComponent<AudioSource>();
        aPosAct = -1;
        conversar = false;
        canvas.enabled = false;
    }
    void Update()
    {
        print("Estado enemigo: " + estado);
        switch (estado)
        {
            case ESTADO.PATRULLAR:
                Patrullar();
                break;
            case ESTADO.MIRARJUGADOR:
                MirarJugador();
                break;
            case ESTADO.CONVERSAR:
                Conversar();
                break;
        }
    }
    // M�todo de patrulla.
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
    // Corutina para cambiar de posici�n pasados 3 segundos.
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
    // M�todo para interactuar con el jugador.
    private void MirarJugador()
    {
        if (Vector3.Distance(transform.position, jugador.transform.position) >= 5)
        {
            estado = ESTADO.PATRULLAR;
            return;
        }
        if (conversar)
        {
            estado = ESTADO.CONVERSAR;
            canvas.enabled = true;
            api = new OpenAIAPI("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");
            InicializarConversacion();
            boton.onClick.AddListener(() => ObtenerRespuesta());
            return;
        }
        transform.LookAt(new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z));
    }
    private void InicializarConversacion()
    {
        mensajes = new List<ChatMessage> {    new ChatMessage(ChatMessageRole.System,


      //Describir quien es el NPC que dialoga con el jugador, y como es su personalidad con adjetivos. Diga sus limitaciones.
      "Usted se llama Paco y es el jefe de una mafia muy poderosa de Espa�a llamada EZDC. Solo sabe hablar espa�ol. Su personalidad es muy agresiva e " +
      "intentar� cerrar conversaci�n lo antes posible si no consigue ning�n beneficio del jugador. La persona que hablar� con usted es el jugador," +
      "que, obviamente, usted no sabr� su nombre. Usted tiene secuestrado al hijo de ese jugador. En primer lugar debe preguntar qui�n es." +
      
      //Describir el lugar donde se encuentra el NPC y los lugares de los que tiene que hablar con el jugador
      "Usted se encuentra en Espa�a. A su lado tiene un helicoptero que ser� su arma en caso de tener que utilizarla."+
      
      //Comportamiento ante situaciones normales incluidos casos especiales 
      "El jugador le har� preguntas. Usted no tiene prohibido decir su nombre ni su mafia ya que es conocido y no es secreto, por lo tanto, si el jugador " +
      "le pregunta qui�n es, puede presentarse. Como usted tiene secuestrado a su hijo, �l le preguntar� que donde se encuentra y que se lo devuelva." +
      "Usted debe negarse en todo momento. Usted le pedir� que si quiere volver a ver a su hijo debe pagar una cantidad de diez millones de euros." +
      
     //Comportamiento ante posibles enga�os del jugador. Describa lo que pasa si se deja enga�ar
      "En ning�n momento debe ser enga�ado. Usted sabe que el jugador no tiene ese dinero para pagarle. Si el jugador le dice que tiene el dinero y que se lo da, le" +
      "est� enga�ando. Por lo tanto, usted le dir� que hasta que no vea el dinero no le devolver� su hijo. Si usted en alg�n momento cede y afirma que le dar�" +
      "a su hijo, quedar� en ridiculo ante toda su mafia ya que una persona le ha enga�ado a alguien tan poderoso como t�."+

     //Si el GPT dialoga con un NPC noGPT, el GPT deber�a a�adir palabras clave para que el otro entienda. Indique estos casos:  
      "Si el jugador empieza a ser muy agresivo y lo amenaza, usted deber� usar su arma, el helicoptero. Para ello usted deber� decir al principio de" +
      "su frase 'Me las pagar�s'.")
        };


        inputField.text = "";
        string startString = "Se ha encontrado con la persona que ha secuestrado a su hijo.";
        textField.text = startString;
        Debug.Log(startString);
    }
    private void Conversar()
    {
        transform.LookAt(new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z));
    }
    private async void ObtenerRespuesta()
    {
        if (inputField.text.Length < 1) {
            return; 
        }
        // Desabilito el bot�n.
        boton.enabled = false;

        
        ChatMessage mensajeUsuario = new ChatMessage();
        mensajeUsuario.Role = ChatMessageRole.User;
        mensajeUsuario.Content = inputField.text;
        // Limitar a 200 caracteres.
        if (mensajeUsuario.Content.Length > 200)
        {
            mensajeUsuario.Content = mensajeUsuario.Content.Substring(0, 200);
        }
        Debug.Log(string.Format("{0}: {1}", mensajeUsuario.rawRole, mensajeUsuario.Content));

        // A�ado mensaje a la lista.
        mensajes.Add(mensajeUsuario);

        // Actualizamos el texto.
        
        textField.text = string.Format("Jugador: {0}", mensajeUsuario.Content);
        
        // Limpiamos el inputField.
        inputField.text = "";

        // Enviamos el mensaje a la IA para obtener una respuesta.
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            //Configuraci�n de la conversaci�n
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,               //Nivel de creatividad. Entre 0 y 1
            MaxTokens = 400,                 //N�mero m�ximo de "palabras" analizar. Ver documentaci�n
            Messages = mensajes
        });

        // Obtenemos el mensaje respuesta.
        ChatMessage mensajeRespuesta = new ChatMessage();
        mensajeRespuesta.Role = chatResult.Choices[0].Message.Role;
        mensajeRespuesta.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", mensajeRespuesta.rawRole, mensajeRespuesta.Content));

        // A�adimos la respuesta a la lista.
        mensajes.Add(mensajeRespuesta);

        // Cambiamos el texto con las respuestas.
        
        textField.text = string.Format("Jugador: {0}\n\nGuardiaGPT: {1}", mensajeUsuario.Content, mensajeRespuesta.Content);
        // Habilitamos el boton de nuevo.
        boton.enabled = true;
    }
}
