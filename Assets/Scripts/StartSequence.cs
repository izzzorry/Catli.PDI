using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSequence : MonoBehaviour
{
    public RawImage rawImage;          // Imagen que aparecerá al inicio
    public Text typingText;           // Texto para la animación de escritura
    public Text instructionsText;     // Texto para las instrucciones
    public AudioSource backgroundMusic; // Música de fondo
    public Button startButton;        // Botón de iniciar
    public string initialMessage = "CATLI";  // Mensaje inicial
    public string instructions = "Selecciona tu plantilla: Escoge entre mariposas, colibríes y gatos representativos de Cali.\n\n\nColorea tu diseño: Usa los materiales proporcionados para personalizar la plantilla con tus colores favoritos.\n\n\nEscanea tu figura: Coloca tu diseño en el escáner y sigue las indicaciones en pantalla.\n\n\nObserva la magia: Mira cómo tu creación cobra vida digitalmente en una experiencia interactiva.\n\n\nDisfruta y comparte: Explora tu diseño animado y toma una foto como recuerdo para compartir la experiencia.";  // Instrucciones
    public float typingSpeed = 0.1f;  // Velocidad de escritura (en segundos)

    void Start()
    {
        // Asegurarnos de que los elementos estén en el estado inicial
        rawImage.gameObject.SetActive(true);
        typingText.gameObject.SetActive(true);
        instructionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);

        // Reproducir la música de fondo
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }

        // Iniciar la animación de escritura
        StartCoroutine(ShowTypingText());
    }

    IEnumerator ShowTypingText()
    {
        typingText.text = ""; // Limpiar el texto al inicio

        // Animación de escritura letra por letra
        foreach (char letter in initialMessage.ToCharArray())
        {
            typingText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Esperar entre letras
        }

        // Esperar unos segundos antes de borrar el texto y mostrar instrucciones
        yield return new WaitForSeconds(2f);

        // Ocultar el texto inicial
        typingText.gameObject.SetActive(false);

        // Mostrar las instrucciones y activar el botón
        instructionsText.gameObject.SetActive(true);
        instructionsText.text = instructions;
        startButton.gameObject.SetActive(true);

        // Asignar evento al botón
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    public void OnStartButtonClicked()
    {
        // Desactivar RawImage, instrucciones y el botón de inicio
        rawImage.gameObject.SetActive(false);
        instructionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);

        // Puedes añadir aquí la lógica para iniciar el juego (como cambiar de escena)
        Debug.Log("El juego ha comenzado");
    }
}
