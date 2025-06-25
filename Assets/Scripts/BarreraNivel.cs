using UnityEngine;

/// <summary>
/// Barrera invisible que bloquea el acceso a niveles no desbloqueados
/// Debe colocarse entre los niveles para impedir que el jugador pase sin completar el anterior
/// </summary>
public class BarreraNivel : MonoBehaviour
{
    [Header("Configuración de Barrera")]
    public int nivelRequerido = 2; // Nivel que se necesita haber completado para pasar
    public bool mostrarMensaje = true;
    public string mensajeBloqueo = "Debes completar el nivel anterior para continuar";
    
    [Header("Efectos Visuales")]
    public GameObject efectoBarrera; // Efecto visual cuando está bloqueada
    public Material materialBloqueado;      // Material cuando está bloqueada (rojo)
    public Material materialDesbloqueado;   // Material cuando se puede pasar (verde)
    public Material materialCompletado;     // Material cuando el nivel está completado (azul/dorado)
    
    [Header("Configuración de Retroceso")]
    public bool permitirRetroceso = false;   // Si permite volver a niveles anteriores
    public bool bloquearDespuesDeCompletar = false; // Si bloquea el paso después de completar

    private GameRespawn gameManager;
    private Collider barrierCollider;
    private Renderer barrierRenderer;
    private bool estaDesbloqueada = false;
    private bool nivelCompletado = false;  // Nuevo: tracking de si el nivel está completado
    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        barrierCollider = GetComponent<Collider>();
        barrierRenderer = GetComponent<Renderer>();

        // Si no hay collider, agregar uno automáticamente
        if (barrierCollider == null)
        {
            Debug.LogWarning($"Barrera nivel {nivelRequerido} no tiene Collider. Agregando Box Collider automáticamente.");
            barrierCollider = gameObject.AddComponent<BoxCollider>();

            // Configurar tamaño por defecto
            BoxCollider boxCollider = barrierCollider as BoxCollider;
            if (boxCollider != null)
            {
                boxCollider.size = new Vector3(5f, 3f, 1f); // Ancho, Alto, Profundidad
            }
        }

        // NO configurar como trigger inicialmente - debe ser sólido para bloquear
        if (barrierCollider != null)
        {
            barrierCollider.isTrigger = false; // Sólido por defecto
            barrierCollider.enabled = true;   // Asegurar que esté activado
        }

        // Inicializar como bloqueada por defecto
        estaDesbloqueada = false;
        BloquearBarrera(); // Forzar estado bloqueado al inicio

        // Luego verificar si debería estar desbloqueada
        ActualizarEstadoBarrera();
    }
    void Update()
    {
        // Verificar constantemente si la barrera debe desbloquearse
        ActualizarEstadoBarrera();        // Validación adicional: asegurar que el estado del collider sea correcto
        if (barrierCollider != null)
        {
            if (!estaDesbloqueada && (barrierCollider.enabled == false || barrierCollider.isTrigger == true))
            {
                // La barrera debería estar bloqueada pero el collider está desactivado o es trigger
                Debug.LogWarning("Corrigiendo estado de barrera - reactivando collider bloqueado sólido");
                BloquearBarrera();
            }
            else if (estaDesbloqueada && !nivelCompletado && barrierCollider.isTrigger == false)
            {
                // La barrera debería estar desbloqueada pero aún es sólida
                Debug.LogWarning("Corrigiendo estado de barrera - cambiando a trigger desbloqueado");
                DesbloquearBarrera();
            }
            else if (nivelCompletado && !permitirRetroceso && barrierCollider.isTrigger == true)
            {
                // La barrera completada sin retroceso debería ser sólida pero está como trigger
                Debug.LogWarning("Corrigiendo barrera completada - cambiando a sólida amarilla");
                MarcarComoCompletado();
            }
            else if (nivelCompletado && permitirRetroceso && barrierCollider.isTrigger == false)
            {
                // La barrera completada con retroceso permitido debería ser trigger pero está sólida
                Debug.LogWarning("Corrigiendo barrera completada - cambiando a trigger libre");
                MarcarComoCompletado();
            }
        }
    }
    void ActualizarEstadoBarrera()
    {
        if (gameManager == null)
        {
            Debug.LogWarning($"GameManager no encontrado para barrera nivel {nivelRequerido}");
            return;
        }

        // Verificar si el nivel ANTERIOR está completado
        // Si nivelRequerido = 2, necesita que el nivel 1 esté completado
        int nivelAnterior = nivelRequerido - 1;
        bool deberiaEstarDesbloqueada = false;

        // Verificar si ESTE nivel ya está completado
        bool esteNivelCompletado = false;
        if (nivelRequerido >= 1 && nivelRequerido <= gameManager.maxNivel)
        {
            int indiceEsteNivel = nivelRequerido - 1;
            if (indiceEsteNivel >= 0 && indiceEsteNivel < gameManager.nivelesCompletados.Length)
            {
                esteNivelCompletado = gameManager.nivelesCompletados[indiceEsteNivel];
            }
        }

        // Para la barrera del nivel 1, siempre debería estar desbloqueada (no hay nivel anterior)
        if (nivelRequerido == 1)
        {
            deberiaEstarDesbloqueada = true;
        }
        else if (nivelAnterior >= 1 && nivelAnterior <= gameManager.maxNivel)
        {
            // Verificar que el índice esté dentro del rango válido
            int indice = nivelAnterior - 1;
            if (indice >= 0 && indice < gameManager.nivelesCompletados.Length)
            {
                deberiaEstarDesbloqueada = gameManager.nivelesCompletados[indice];
            }
        }

        // Actualizar estado de completado
        if (esteNivelCompletado != nivelCompletado)
        {
            nivelCompletado = esteNivelCompletado;
            if (nivelCompletado)
            {
                MarcarComoCompletado();
            }
        }

        // Actualizar estado de desbloqueado
        if (deberiaEstarDesbloqueada != estaDesbloqueada)
        {
            estaDesbloqueada = deberiaEstarDesbloqueada;

            if (estaDesbloqueada && !nivelCompletado)
            {
                DesbloquearBarrera();
            }
            else if (!estaDesbloqueada)
            {
                BloquearBarrera();
            }
        }
    }    
    void BloquearBarrera()
    {
        // Activar colisión física sólida para bloquear el paso
        if (barrierCollider != null)
        {
            barrierCollider.isTrigger = false; // SÓLIDO para bloquear
            barrierCollider.enabled = true;
        }

        // Hacer visible el objeto para debug con color rojo intenso
        if (barrierRenderer != null)
        {
            barrierRenderer.enabled = true;

            if (materialBloqueado != null)
            {
                barrierRenderer.material = materialBloqueado;
            }
            else
            {
                // Color rojo intenso y más opaco para que sea bien visible
                Color colorRojo = new Color(1f, 0f, 0f, 0.8f); // Rojo más opaco
                barrierRenderer.material.color = colorRojo;
            }
        }

        // Activar efecto visual
        if (efectoBarrera != null)
        {
            efectoBarrera.SetActive(true);
        }

        Debug.Log($"Barrera del nivel {nivelRequerido} BLOQUEADA (Colisión sólida activada, renderer rojo visible)");
    }    void DesbloquearBarrera()
    {
        // Convertir a trigger para permitir el paso pero detectar cuando el jugador pase
        if (barrierCollider != null)
        {
            barrierCollider.enabled = true;
            barrierCollider.isTrigger = true; // TRIGGER para detectar paso sin bloquear
        }

        // Hacer menos visible o cambiar material
        if (barrierRenderer != null)
        {
            if (materialDesbloqueado != null)
            {
                barrierRenderer.material = materialDesbloqueado;
            }
            else
            {
                // Si no hay material asignado, usar color verde semitransparente
                Color colorVerde = Color.green;
                colorVerde.a = 0.3f;
                barrierRenderer.material.color = colorVerde;
            }
        }

        // Desactivar efecto visual
        if (efectoBarrera != null)
        {
            efectoBarrera.SetActive(false);
        }
        Debug.Log($"Barrera del nivel {nivelRequerido} DESBLOQUEADA (Collider como trigger, material verde)");
    }void MarcarComoCompletado()
    {
        // Cambiar material a completado
        if (barrierRenderer != null)
        {
            barrierRenderer.enabled = true; // Asegurar que sea visible
            
            if (materialCompletado != null)
            {
                barrierRenderer.material = materialCompletado;
            }
            else
            {
                // Si no hay material asignado, usar color dorado/amarillo
                Color colorDorado = new Color(1f, 0.8f, 0f, 0.8f); // Dorado más opaco
                barrierRenderer.material.color = colorDorado;
            }
        }

        // Configurar comportamiento según las opciones
        if (bloquearDespuesDeCompletar)
        {
            // Bloquear completamente el paso (útil para niveles de un solo intento)
            if (barrierCollider != null)
            {
                barrierCollider.enabled = true;
                barrierCollider.isTrigger = false; // Sólido
            }
            Debug.Log($"Barrera del nivel {nivelRequerido} COMPLETADO Y BLOQUEADO (sin retroceso permitido)");
        }        else if (!permitirRetroceso)
        {
            // Hacer la barrera COMPLETAMENTE SÓLIDA para bloquear TODO movimiento
            if (barrierCollider != null)
            {
                barrierCollider.enabled = true;
                barrierCollider.isTrigger = false; // SÓLIDO - bloquea físicamente
                Debug.Log($"🟡 Configurando barrera amarilla SÓLIDA: enabled={barrierCollider.enabled}, isTrigger={barrierCollider.isTrigger}");
            }

            // Hacer la barrera MUY visible con color amarillo intenso
            if (barrierRenderer != null)
            {
                barrierRenderer.enabled = true;
                // Color amarillo más intenso y opaco para que sea más visible
                Color colorAmarillo = new Color(1f, 1f, 0f, 0.9f); // Amarillo muy opaco
                barrierRenderer.material.color = colorAmarillo;
            }

            Debug.Log($"Barrera del nivel {nivelRequerido} COMPLETADO (BARRERA FÍSICA AMARILLA SÓLIDA - completamente bloqueada)");
        }        else
        {
            // Permitir paso libre en ambas direcciones - convertir a trigger para detectar paso
            if (barrierCollider != null)
            {
                barrierCollider.enabled = true;
                barrierCollider.isTrigger = true; // TRIGGER para detectar paso sin bloquear
                Debug.Log($"🟦 Configurando barrera dorada TRIGGER: enabled={barrierCollider.enabled}, isTrigger={barrierCollider.isTrigger}");
            }
            Debug.Log($"Barrera del nivel {nivelRequerido} COMPLETADO (paso libre - modo trigger)");
        }

        // Efecto visual especial para nivel completado
        if (efectoBarrera != null)
        {
            efectoBarrera.SetActive(true);
            // Aquí podrías agregar partículas doradas, brillos, etc.
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Jugador pasó por barrera del nivel {nivelRequerido} (trigger)");

            // Si la barrera está completada y NO permite retroceso, no dejar pasar
            if (nivelCompletado && !permitirRetroceso)
            {
                // Teletransportar al jugador de vuelta al lado correcto
                TeletransportarParaAvance(other);
                MostrarMensajeNoRetroceso();
                Debug.Log($"🟡 Barrera amarilla: retroceso bloqueado por trigger en nivel {nivelRequerido}");
                return;
            }

            // Solo registrar paso si la barrera permite paso libre
            if (estaDesbloqueada && (permitirRetroceso || !nivelCompletado))
            {
                RegistrarPasoDeNivel(other);
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Jugador chocó con barrera del nivel {nivelRequerido}. Bloqueada: {!estaDesbloqueada}, Completada: {nivelCompletado}");

            if (!estaDesbloqueada)
            {
                // El jugador chocó contra la barrera bloqueada (roja)
                MostrarMensajeBloqueo();
                EmpujarJugadorAtras(collision);
            }
            else if (nivelCompletado && !permitirRetroceso)
            {
                // El jugador chocó contra la barrera completada (amarilla) - BLOQUEAR COMPLETAMENTE
                MostrarMensajeNoRetroceso();
                EmpujarJugadorAtras(collision);
                Debug.Log($"🟡 Barrera amarilla completamente bloqueada en nivel {nivelRequerido} - NO SE PUEDE PASAR");
            }
        }
    }

    void EmpujarJugadorAtras(Collision collision)
    {
        // Efecto de rebote o empuje hacia atrás más fuerte
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        CharacterController playerController = collision.gameObject.GetComponent<CharacterController>();

        if (playerRb != null)
        {
            Vector3 direccionEmpuje = -collision.contacts[0].normal;
            playerRb.AddForce(direccionEmpuje * 15f, ForceMode.Impulse); // Más fuerza
        }
        else if (playerController != null)
        {
            // Para Character Controller, mover directamente
            Vector3 direccionEmpuje = -collision.contacts[0].normal * 3f; // Más distancia
            collision.transform.position += direccionEmpuje;
        }
    }

    void TeletransportarParaAvance(Collider player)
    {
        // Teletransportar al jugador al otro lado de la barrera (avance)
        Vector3 direccionAvance = transform.forward;
        Vector3 posicionAvance = transform.position + direccionAvance * 3f;

        player.transform.position = posicionAvance;

        // Detener movimiento
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Registrar paso de nivel
        RegistrarPasoDeNivel(player);

        if (gameManager != null)
        {
            gameManager.teleports_Realizados++;
        }

        Debug.Log($"➡️ Jugador teletransportado hacia adelante en barrera nivel {nivelRequerido}");
    }

    void MostrarMensajeBloqueo()
    {
        if (!mostrarMensaje) return;

        // Registrar intento de acceso no autorizado
        if (gameManager != null)
        {
            gameManager.errores++;
            gameManager.dudas_Expresadas++; // Puede indicar confusión
        }        // Mostrar mensaje en UI
        Debug.Log($"Acceso denegado: {mensajeBloqueo}");
    }
    void MostrarMensajeNoRetroceso()
    {
        string mensajeRetroceso = "No puedes regresar a niveles anteriores una vez completados";

        // Registrar intento de retroceso
        if (gameManager != null)
        {
            gameManager.errores++;
            gameManager.dudas_Expresadas++;
        }

        Debug.Log($"Retroceso bloqueado: {mensajeRetroceso}");

        // Mostrar mensaje en el mundo
        StartCoroutine(MostrarMensajeTemporalPersonalizado(mensajeRetroceso, Color.yellow));
    }

    void TeletransportarJugadorAtras(Collider player)
    {
        // Calcular posición segura hacia atrás
        Vector3 direccionSegura = -transform.forward; // Opuesto a la dirección de avance
        Vector3 posicionSegura = player.transform.position + direccionSegura * 3f; // Más distancia para ser efectivo

        // Teletransportar
        player.transform.position = posicionSegura;

        // Si tiene Rigidbody, detener su movimiento
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Si tiene CharacterController, también funciona
        CharacterController playerController = player.GetComponent<CharacterController>();
        if (playerController != null)
        {
            // CharacterController no necesita ajustes adicionales de velocidad
        }

        if (gameManager != null)
        {
            gameManager.teleports_Realizados++;
        }

        Debug.Log($"🔄 Jugador teletransportado hacia atrás desde barrera nivel {nivelRequerido}");
    }

    void RegistrarPasoDeNivel(Collider player)
    {
        if (gameManager != null)
        {
            // Registrar que el jugador cambió de nivel exitosamente
            gameManager.manipulacionesInteractivas++;
            // Iniciar el timer del nivel exactamente al pasar la barrera SOLO si la barrera está activa
            if (gameObject.activeSelf)
            {
                gameManager.IniciarTiempoNivel();
            }
            Debug.Log($"Jugador pasó al nivel {nivelRequerido} y se inició el timer del nivel");
        }
        // Desactivar la barrera para que no pueda volver a reiniciar el tiempo
        DesactivarBarrera();
    }

    /// <summary>
    /// Desactiva la barrera tras pasarla correctamente
    /// </summary>
    public void DesactivarBarrera()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Reactiva la barrera (llamar desde GameRespawn al reintentar o completar nivel)
    /// </summary>
    public void ActivarBarrera()
    {
        gameObject.SetActive(true);
        // Forzar actualización de estado visual y físico
        ActualizarEstadoBarrera();
        // Si el nivel ya está completado y no permite retroceso, forzar modo amarillo sólido
        if (nivelCompletado && !permitirRetroceso)
        {
            MarcarComoCompletado();
        }
        else if (!estaDesbloqueada)
        {
            BloquearBarrera();
        }
    }

    System.Collections.IEnumerator MostrarMensajeTemporal()
    {
        yield return StartCoroutine(MostrarMensajeTemporalPersonalizado(mensajeBloqueo, Color.red));
    }

    System.Collections.IEnumerator MostrarMensajeTemporalPersonalizado(string mensaje, Color color)
    {
        // Crear mensaje temporal en el mundo VR
        GameObject mensajeObj = new GameObject("MensajeBarrera");
        mensajeObj.transform.position = transform.position + Vector3.up * 2f;
        mensajeObj.transform.LookAt(Camera.main.transform);

        // Agregar texto 3D
        TextMesh texto = mensajeObj.AddComponent<TextMesh>();
        texto.text = mensaje;
        texto.color = color;
        texto.fontSize = 20;
        texto.anchor = TextAnchor.MiddleCenter;

        yield return new WaitForSeconds(3f);

        Destroy(mensajeObj);
    }

    /// <summary>
    /// Método público para forzar verificación de estado
    /// </summary>
    public void VerificarEstado()
    {
        ActualizarEstadoBarrera();
    }    /// <summary>
         /// Método para configurar la barrera desde el inspector o código
         /// </summary>
    public void ConfigurarBarrera(int nivelReq, string mensaje = "")
    {
        nivelRequerido = nivelReq;
        if (!string.IsNullOrEmpty(mensaje))
        {
            mensajeBloqueo = mensaje;
        }

        ActualizarEstadoBarrera();
    }

    /// <summary>
    /// Configurar opciones de retroceso para la barrera
    /// </summary>
    public void ConfigurarRetroceso(bool permitirRetroceso = true, bool bloquearDespuesDeCompletar = false)
    {
        this.permitirRetroceso = permitirRetroceso;
        this.bloquearDespuesDeCompletar = bloquearDespuesDeCompletar;

        // Actualizar inmediatamente si el nivel ya está completado
        if (nivelCompletado)
        {
            MarcarComoCompletado();
        }
    }

    /// <summary>
    /// Forzar que se marque el nivel como completado (útil para testing)
    /// </summary>
    public void ForzarCompletado()
    {
        nivelCompletado = true;
        MarcarComoCompletado();
    }

    /// <summary>
    /// Obtener información del estado actual de la barrera
    /// </summary>
    public string ObtenerEstadoBarrera()
    {
        string estado = $"Barrera Nivel {nivelRequerido}:\n";
        estado += $"- Desbloqueada: {estaDesbloqueada}\n";
        estado += $"- Completada: {nivelCompletado}\n";
        estado += $"- Permite retroceso: {permitirRetroceso}\n";
        estado += $"- Bloquea después de completar: {bloquearDespuesDeCompletar}";
        return estado;
    }
}
