using UnityEngine;

public class CountOnCorrect : MonoBehaviour
{
    private GameRespawn gameManager;
    private UIManager uiManager; private bool yaContado = false; // Para evitar múltiples conteos
    private Color? colorOriginal = null; // Guardar el color original para restaurar correctamente
    private string bloqueID; // Identificador único del bloque

    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        uiManager = FindFirstObjectByType<UIManager>();

        // Debug adicional
        Debug.Log($"CountOnCorrect iniciado en: {gameObject.name}");
        Debug.Log($"GameManager encontrado: {gameManager != null}");
        Debug.Log($"UIManager encontrado: {uiManager != null}");

        // Verificar el collider
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log($"Collider en {gameObject.name}: isTrigger={collider.isTrigger}");
        }
        else
        {
            Debug.LogWarning($"¡No hay Collider en {gameObject.name}!");
        }

        // Asignar un identificador único al bloque (puede ser el nombre inicial)
        bloqueID = gameObject.name;
    }    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaContado)
        {
            Debug.Log($"¡Acierto por trigger en {gameObject.name}!");
            RegistrarAcierto();
        }
    }    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !yaContado)
        {
            Debug.Log($"¡Acierto por colisión en {gameObject.name}!");
            RegistrarAcierto();
        }
    }// Método adicional para detectar cuando el jugador está cerca (optimizado)
    void Update()
    {
        if (!yaContado)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                // Solo registrar si está MUY cerca para ser más preciso
                if (distance < 0.8f) // Reducido de 1f a 0.8f para más precisión
                {
                    Debug.Log($"¡Player en bloque {gameObject.name}! Registrando acierto por proximidad");
                    RegistrarAcierto();
                }
            }
        }
    }    void RegistrarAcierto()
    {
        // Verificar si este bloque ya fue contado en la sesión
        if (gameManager != null && gameManager.BloqueYaContado(bloqueID))
        {
            Debug.Log($"❌ Bloque {bloqueID} ya fue contado en la sesión. No suma acierto.");
            yaContado = true;
            MarcarComoUsado(); // Opcional: marcar visualmente aunque no sume
            return;
        }
        yaContado = true;

        // Registrar el bloque como contado en la sesión
        if (gameManager != null)
        {
            gameManager.RegistrarBloqueContado(bloqueID);
        }

        // Registrar acierto en las métricas
        if (gameManager != null)
        {
            gameManager.aciertos++;
            gameManager.interaccionesAcertadas++;
            gameManager.interaccionesTotales++;
            gameManager.saltos_Correctos++;

            // Registrar acierto según el nivel actual
            switch (gameManager.nivelActual)
            {
                case 1:
                    gameManager.nivel1_Saltos_Correctos++;
                    gameManager.nivel1_Saltos_Totales++;
                    gameManager.if_Statements_Correctos++;
                    break;
                case 2:
                    gameManager.nivel2_Saltos_Correctos++;
                    gameManager.nivel2_Saltos_Totales++;
                    gameManager.else_Statements_Correctos++;
                    break;
                case 3:
                    gameManager.nivel3_Saltos_Correctos++;
                    gameManager.nivel3_Saltos_Totales++;
                    gameManager.nested_Statements_Correctos++;
                    break;
            }

            // Aumentar puntaje
            gameManager.puntajeObtenido += 10f; // 10 puntos por acierto

            Debug.Log($"🎯 ¡ACIERTO #{gameManager.aciertos} en {gameObject.name}! Puntaje: {gameManager.puntajeObtenido}");
        }

        // Efecto visual permanente: marcar como "usado"
        MarcarComoUsado();
        
        // Efecto visual temporal
        CrearEfectoAcierto();
    }    void CrearEfectoAcierto()
    {
        // Cambiar el material a uno "brillante" brevemente
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Guardar color original solo la primera vez
            if (colorOriginal == null)
                colorOriginal = renderer.material.color;
            // Cambiar a color de éxito temporalmente
            renderer.material.color = Color.green;
            // Volver al color original después de un tiempo
            StartCoroutine(RestaurarColorOriginal(renderer));
        }

        // Reproducir sonido de éxito
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }

        Debug.Log("✨ Efecto de acierto creado");
    }

    System.Collections.IEnumerator RestaurarColorOriginal(Renderer renderer)
    {
        yield return new WaitForSeconds(0.5f);
        if (renderer != null && colorOriginal != null)
        {
            renderer.material.color = colorOriginal.Value;
        }
    }    void MarcarComoUsado()
    {
        // Cambiar el nombre del objeto para indicar que ya fue usado
        gameObject.name += " ✓USADO";
        
        // Opcional: cambiar ligeramente el color para indicar que ya no dará más aciertos
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color currentColor = renderer.material.color;
            // Hacerlo un poco más opaco/gris para indicar que está "usado"
            renderer.material.color = new Color(currentColor.r * 0.8f, currentColor.g * 0.8f, currentColor.b * 0.8f, currentColor.a);
        }
    }

    // Método para resetear el contador (útil cuando se regeneran bloques)
    public void ResetearContador()
    {
        yaContado = false;
        // Restaurar el nombre original (quitar el "✓USADO")
        if (gameObject.name.Contains(" ✓USADO"))
        {
            gameObject.name = gameObject.name.Replace(" ✓USADO", "");
        }
        // Restaurar color original si fue modificado
        var renderer = GetComponent<Renderer>();
        if (renderer != null && colorOriginal != null)
        {
            renderer.material.color = colorOriginal.Value;
        }
        Debug.Log($"🔄 Contador reseteado en {gameObject.name}");
    }
}
