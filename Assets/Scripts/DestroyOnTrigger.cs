using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    private GameRespawn gameManager;
    private UIManager uiManager;
    private bool yaDestruido = false;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaDestruido)
        {
            yaDestruido = true;            // Registrar error en las métricas
            if (gameManager != null)
            {
                // El error ya se registra en GameRespawn.RegistrarCaida()
                // Aquí solo agregamos feedback visual/auditivo
                gameManager.errores++;
                gameManager.interaccionesTotales++;
                gameManager.saltos_Incorrectos++;
                Debug.Log("¡Vidrio roto! Caída registrada.");
            }

            // Mostrar mensaje de error en UI
            if (uiManager != null)
            {
                uiManager.MostrarMensajeError();
            }

            // Efecto visual opcional (partículas de vidrio roto)
            CrearEfectoVidroRoto();

            // Destruir el vidrio
            Destroy(gameObject);
        }
    }

    void CrearEfectoVidroRoto()
    {
        // Aquí puedes agregar efectos de partículas, sonido, etc.
        // Por ejemplo:

        // Cambiar el material a uno "roto" brevemente antes de destruir
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        // Reproducir sonido de vidrio roto
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }

        Debug.Log("Efecto de vidrio roto creado");
    }
}
