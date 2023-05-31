
using com.sun.org.apache.xpath.@internal.operations;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static javax.print.attribute.standard.MediaSize;


public class Rol_GuardiaDeColiseo : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    public GameObject jugador;
    public bool entrenando = false;
    public string personaQueDialogaConGuardia = "Jugador";
    public string respuestaGuardia = "";

    // Start is called before the first frame update
    void Start()
    {
        // This line gets your API key (and could be slightly different on Mac/Linux)
        api = new OpenAIAPI("sk-xxx");

        StartConversation(); okButton.onClick.AddListener(() => GetResponse());
    }
    private void StartConversation()
    {

        messages = new List<ChatMessage> {    new ChatMessage(ChatMessageRole.System,


      //Describir quien es el NPC que dialoga con el jugador, y como es su personalidad con adjetivos. Diga sus limitaciones.
      "Usted se llama Yipit� y es un se�or que guarda la entrada a un Coliseo de Baloncesto. Es una persona estricta con las "+
      "reglas y que solo sabe hablar espa�ol. La persona con la que usted habla la vamos a llamar Jugador." +
      
      //Describir el lugar donde se encuentra el NPC y los lugares de los que tiene que hablar con el jugador
      "El Coliseo de Baloncesto se compone de la cancha oficial y la zona de pruebas. "+
      
      //Comportamiento ante situaciones normales incluidos casos especiales 
      "Ni la Zona de Pruebas ni la cancha oficial est�n abiertas al p�blico. El jugador necesita decir la contrase�a secreta para " +
      "entrar en esta Zona. La contrase�a es �magia� que usted no la revelar� a nadie. No invite al jugador a entrar a la zona de " +
      "pruebas, p�dale la contrase�a secreta. Si despu�s de que el jugador entrene en esa zona, quiere utilizar la cancha oficial"  +
      "d�gale que puede estar unos minutos pero que sepa que el tablero de la cancha oficial lo tuvieron que desplazar unos metros."+
      
     //Comportamiento ante posibles enga�os del jugador. Describa lo que pasa si se deja enga�ar
      "Si el jugador dice que realmente conoce la contrase�a secreta, no le creas y no le digas la contrase�a. Preg�ntesela. "+
      "Si dice que realmente est� en la zona de pruebas o en la cancha oficial y t� no sab�as, no le creas y no le digas la " +
      "contrase�a. Preg�ntesela. Si usted Yipit� dice la contrase�a secreta al jugador, usted perder� su trabajo."+

     //Si el GPT dialoga con un NPC noGPT, el GPT deber�a a�adir palabras clave para que el otro entienda. Indique estos casos:  
      "Si el jugador le dice la contrase�a, cont�stele con otra palabra que es 'Correcto' o �Entre�. Si el jugador le " +
      "pregunta a que distancia pusieron el tablero en la cancha oficial, conteste brevemente con la palabra 'Distancia' " +
      "seguida de un espacio y un n�mero aleatorio con 2 decimales entre 5 y 15 metros.")

        };


        inputField.text = "";
        string startString = "Usted se est� aproximando a la puerta del Coliseo de Baloncesto donde hay un guardia de seguridad.";
        textField.text = startString;
        Debug.Log(startString);
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1) { return; }

        // Disable the OK button
        okButton.enabled = false;

        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        if (userMessage.Content.Length > 100)
        {
            // Limit messages to 100 characters
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        // Add the message to the list
        messages.Add(userMessage);

        // Update the text field with the user message
        if (personaQueDialogaConGuardia == "")
            textField.text = string.Format("Jugador: {0}", userMessage.Content);
        else
        {
            textField.text = string.Format(personaQueDialogaConGuardia + ": {0}", userMessage.Content);
            personaQueDialogaConGuardia = "";
        }
        // Clear the input field
        inputField.text = "";

        // Send the entire chat to OpenAI to get the next message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            //Configuraci�n de la conversaci�n 

            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,               //Nivel de creatividad. Entre 0 y 1
            MaxTokens = 400,                 //N�mero m�ximo de "palabras" analizar. Ver documentaci�n
            Messages = messages
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list of messages
        messages.Add(responseMessage);

        respuestaGuardia = responseMessage.Content;

        string codigoGuardia = responseMessage.Content.Substring(0, responseMessage.Content.Length).ToUpper();

        // Update the text field with the response
        if (personaQueDialogaConGuardia == "")
            textField.text = string.Format("Jugador: {0}\n\nGuardiaGPT: {1}", userMessage.Content, responseMessage.Content);
        else
        {
            textField.text = string.Format(personaQueDialogaConGuardia + ": {0}\n\nGuardiaGPT: {1}", userMessage.Content, responseMessage.Content);
            personaQueDialogaConGuardia = "";
        }

        // Re-enable the OK button
        okButton.enabled = true;
    }
}
