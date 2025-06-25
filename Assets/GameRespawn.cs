using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRespawn : MonoBehaviour
{
    [Header("=== CONFIGURACIÓN BÁSICA ===")]
    public float threshold = -10f;
    public Vector3 respawnPosition = new Vector3(-11.804f, 1.022f, -0.238f);
    
    [Header("=== VARIABLES INDEPENDIENTES ===")]
    
    // 1. ÍNDICE DE INTERACCIÓN DEL USUARIO (IIU)
    [Header("Interacción del Usuario")]
    public int comandosIniciados = 0;           // CI - Comandos iniciados
    public int gestosInterpretados = 0;         // GI - Gestos interpretados  
    public int manipulacionesInteractivas = 0;  // MI - Manipulaciones interactivas
    
    // 2. PRECISIÓN DE INTERACCIÓN (TPI)
    [Header("Precisión de Interacción")]
    public int interaccionesAcertadas = 0;      // IA - Interacciones acertadas
    public int interaccionesTotales = 0;        // IT - Interacciones totales
    public int errores = 0;                     // Para tracking adicional
    public int aciertos = 0;                    // Para tracking adicional
    
    // 3. FLUIDEZ DE NAVEGACIÓN (IFN)
    [Header("Fluidez de Navegación")]
    public float velocidadMovimientoMedida = 0f;    // VM - Velocidad medida
    public float velocidadInicialEsperada = 1.5f;   // VI - Velocidad inicial
    public float velocidadEstandarObjetivo = 3.0f;  // VE - Velocidad estándar
    
    [Header("=== VARIABLES DEPENDIENTES ===")]
    
    // 4. RENDIMIENTO ACADÉMICO (Para estudio transversal)
    [Header("Rendimiento en Sesión Única")]
    public float puntajeObtenido = 0f;          // Puntaje total obtenido
    public float puntajeMaximoPosible = 100f;   // PM - Puntaje máximo
    public float porcentajeRendimiento = 0f;    // Porcentaje de rendimiento
    
    // 5. ÍNDICE DE DOMINIO CONCEPTUAL (IDC)
    [Header("Dominio Conceptual")]
    public float evaluacionConceptual = 0f;     // EC - Evaluación conceptual (0-1)
    public float aplicacionPractica = 0f;      // AP - Aplicación práctica (0-1)
    public float resolucionCasos = 0f;         // RC - Resolución de casos (0-1)
    
    // 6. SATISFACCIÓN DEL USUARIO (ISU)
    [Header("Satisfacción del Usuario")]
    public float sumaScoresSUS = 0f;           // SS - Suma de scores SUS
    public int numeroTotalPreguntas = 10;      // NT - Número total preguntas SUS
    
    [Header("=== VARIABLES ESPECÍFICAS JUEGO SALTO DE VIDRIOS ===")]
    
    // Variables del juego tipo "Calamar" - Salto de vidrios
    [Header("Mecánica de Saltos")]
    public int saltos_Correctos = 0;            // Vidrios seguros pisados
    public int saltos_Incorrectos = 0;          // Vidrios que se rompieron
    public int reintentos_Nivel = 0;            // Veces que reinició nivel
    
    [Header("Nivel 1: Condicionales Simples (IF)")]
    public int nivel1_Saltos_Totales = 0;
    public int nivel1_Saltos_Correctos = 0;
    public int nivel1_Caidas = 0;
    public float nivel1_Tiempo = 0f;
    public int nivel1_Instrucciones_Leidas = 0; // Cuántas veces leyó las instrucciones
    
    [Header("Nivel 2: Condicionales Dobles (IF-ELSE)")]
    public int nivel2_Saltos_Totales = 0;
    public int nivel2_Saltos_Correctos = 0;
    public int nivel2_Caidas = 0;
    public float nivel2_Tiempo = 0f;
    public int nivel2_Instrucciones_Leidas = 0;
    
    [Header("Nivel 3: Condicionales Anidadas")]
    public int nivel3_Saltos_Totales = 0;
    public int nivel3_Saltos_Correctos = 0;
    public int nivel3_Caidas = 0;
    public float nivel3_Tiempo = 0f;
    public int nivel3_Instrucciones_Leidas = 0;
    
    [Header("Comprensión de Condicionales")]
    public int if_Statements_Correctos = 0;     // Entendió cuándo usar IF
    public int else_Statements_Correctos = 0;   // Entendió cuándo usar ELSE
    public int nested_Statements_Correctos = 0; // Entendió condicionales anidadas
      [Header("Patrones de Comportamiento")]
    public int dudas_Expresadas = 0;            // Cuántas veces pidió ayuda
    public int pausas_Largas = 0;               // Pausas > 5 segundos antes de saltar
    public int veces_Menu_Ayuda = 0;           // Acceso al menú de ayuda

    [Header("Tiempo y Progreso")]
    public float tiempoTotal_Sesion = 0f;
    public float tiempoPromedio_PorNivel = 0f;
    public float tiempoInicio_Sesion;
    public float tiempoActual_Nivel;
    
    // Tiempos precisos por nivel (medidos desde que inicia hasta que completa)
    [Header("Tiempos Precisos por Nivel")]
    public float tiempoReal_Nivel1 = 0f;      // Tiempo exacto gastado solo en nivel 1
    public float tiempoReal_Nivel2 = 0f;      // Tiempo exacto gastado solo en nivel 2  
    public float tiempoReal_Nivel3 = 0f;      // Tiempo exacto gastado solo en nivel 3
    public float tiempoTotal_Pausas = 0f;     // Tiempo total en barreras/menús
    public float tiempoInicioPausa = 0f;      // Para medir pausas en barreras
    
    [Header("Interacciones VR Específicas")]
    public int teleports_Realizados = 0;
    public int objetos_Agarrados = 0;
    public int menus_Abiertos = 0;
    public int ayudas_Solicitadas = 0;
    
    [Header("=== SISTEMA DE NIVELES Y BARRERAS ===")]
    public int nivelActual = 1;
    public int maxNivel = 3;    public bool[] nivelesCompletados = new bool[3];
    public float[] tiemposLimite = { 120f, 180f, 240f }; // Tiempo límite por nivel
      [Header("=== UI REFERENCIAS ===")]
    public TextMeshProUGUI uiNivelActual;
    public TextMeshProUGUI uiPuntaje;
    public GameObject panelBarrera;
    public Button botonSiguienteNivel;
    public Button botonReintentar;

    [Header("UI Tiempos por Nivel")]
    public TextMeshProUGUI uiTiempoNivel1;    // Para mostrar tiempo del Nivel 1
    public TextMeshProUGUI uiTiempoNivel2;    // Para mostrar tiempo del Nivel 2
    public TextMeshProUGUI uiTiempoNivel3;    // Para mostrar tiempo del Nivel 3
    public TextMeshProUGUI uiTiempoTotal;     // Para mostrar tiempo total de sesión

    // === NUEVAS REFERENCIAS UI PARA TOTALES GLOBALES ===
    [Header("UI Totales Globales")]
    public TextMeshProUGUI uiTotalSaltosTotales;
    public TextMeshProUGUI uiTotalSaltosCorrectos;
    public TextMeshProUGUI uiTotalCaidas;
      // Variables privadas para control interno
    private Vector3 ultimaPosicion;
    private float tiempoUltimoPausa;
    private bool enPausa = false;
    private float tiempoInicioNivel;
    private bool enBarrera = false;           // Para controlar si está en barrera/menú
    private float tiempoInicioBarrera = 0f;   // Cuándo empezó la barrera
    private bool juegoTerminado = false;
    private bool nivelEnCurso = false;        // NUEVO: para controlar si el nivel está corriendo

    // Nuevo: para controlar si el nivel está en curso
    private bool[] nivelEnCursoArray = new bool[3];
      // === BLOQUES CONTADOS POR SESIÓN ===
    private System.Collections.Generic.HashSet<string> bloquesContados = new System.Collections.Generic.HashSet<string>();

    /// <summary>
    /// Devuelve true si el bloque con este ID ya fue contado como acierto en la sesión actual.
    /// </summary>
    public bool BloqueYaContado(string id)
    {
        return bloquesContados.Contains(id);
    }

    /// <summary>
    /// Registra el bloque con este ID como ya contado en la sesión actual.
    /// </summary>
    public void RegistrarBloqueContado(string id)
    {
        bloquesContados.Add(id);
    }

    void Start()
    {
        // Validar que el array de niveles completados tenga el tamaño correcto
        if (nivelesCompletados == null || nivelesCompletados.Length != maxNivel)
        {
            Debug.LogWarning($"Redimensionando array nivelesCompletados a {maxNivel} elementos");
            nivelesCompletados = new bool[maxNivel];
        }
        tiempoInicio_Sesion = Time.time;
        // NO iniciar el timer del nivel aquí, solo cuando se pase la barrera
        // tiempoInicioNivel = Time.time;
        nivelEnCurso = false; // El nivel comienza cuando se pasa la barrera
        ultimaPosicion = transform.position;
        ActualizarUI();
    }
    
    void FixedUpdate()
    {
        // Respawn básico
        if (transform.position.y < threshold)
        {
            RegistrarCaida();
            transform.position = respawnPosition;
        }
        
        // Medición de velocidad
        MedirVelocidad();
        
        // Control de tiempo
        ActualizarTiempo();
        
        // Detectar pausas largas
        DetectarPausasLargas();
    }
    
    void Update()
    {
        ActualizarUI();
        // Actualizar tiempo de nivel en tiempo real en su UI SOLO si el nivel está en curso y el juego no ha terminado
        if (!juegoTerminado && nivelEnCurso)
        {
            if (uiTiempoNivel1 != null && nivelActual == 1)
                uiTiempoNivel1.text = $"Nivel 1: {Time.time - tiempoInicioNivel:F1}s";
            if (uiTiempoNivel2 != null && nivelActual == 2)
                uiTiempoNivel2.text = $"Nivel 2: {Time.time - tiempoInicioNivel:F1}s";
            if (uiTiempoNivel3 != null && nivelActual == 3)
                uiTiempoNivel3.text = $"Nivel 3: {Time.time - tiempoInicioNivel:F1}s";
        }
        // Actualizar tiempo total solo si el juego no ha terminado
        if (uiTiempoTotal != null && !juegoTerminado)
            uiTiempoTotal.text = $"Total: {Time.time - tiempoInicio_Sesion:F1}s";
    }
    
    #region Sistema de Respawn y Métricas
    
    void RegistrarCaida()
    {
        saltos_Incorrectos++;
        errores++;
        interaccionesTotales++;
        
        // Registrar por nivel específico
        switch (nivelActual)
        {
            case 1:
                nivel1_Caidas++;
                break;
            case 2:
                nivel2_Caidas++;
                break;
            case 3:
                nivel3_Caidas++;
                break;
        }
        
        Debug.Log($"Caída registrada. Total errores: {errores}");
    }
      public void RegistrarSaltoExitoso()
    {
        saltos_Correctos++;
        aciertos++;
        interaccionesAcertadas++;
        interaccionesTotales++;
        
        // Registrar por nivel específico
        switch (nivelActual)
        {
            case 1:
                nivel1_Saltos_Correctos++;
                nivel1_Saltos_Totales++;
                break;
            case 2:
                nivel2_Saltos_Correctos++;
                nivel2_Saltos_Totales++;
                break;
            case 3:
                nivel3_Saltos_Correctos++;
                nivel3_Saltos_Totales++;
                break;
        }
        
        // Calcular puntaje
        CalcularPuntaje();
        
        // Actualizar UI inmediatamente
        ActualizarUI();
        
        Debug.Log($"Salto exitoso registrado! Aciertos totales: {aciertos}, Saltos correctos: {saltos_Correctos}, Nivel {nivelActual}: {GetSaltosCorrectosPorNivel()} aciertos");
    }
    
    void MedirVelocidad()
    {
        float distancia = Vector3.Distance(transform.position, ultimaPosicion);
        velocidadMovimientoMedida = distancia / Time.fixedDeltaTime;
        ultimaPosicion = transform.position;
    }
    
    void DetectarPausasLargas()
    {
        if (velocidadMovimientoMedida < 0.1f) // Prácticamente inmóvil
        {
            if (!enPausa)
            {
                tiempoUltimoPausa = Time.time;
                enPausa = true;
            }
            else if (Time.time - tiempoUltimoPausa > 5f) // Pausa de más de 5 segundos
            {
                pausas_Largas++;
                enPausa = false; // Reset para no contar la misma pausa múltiples veces
            }
        }
        else
        {
            enPausa = false;
        }
    }
    
    #endregion
    
    #region Sistema de Niveles y Barreras
      public bool PuedeAccederNivel(int nivel)
    {
        if (nivel <= 1) return true; // Siempre puede acceder al nivel 1 o menos
        
        // Validar que el nivel esté dentro del rango válido
        if (nivel > maxNivel) return false;
        
        // Para acceder a un nivel, debe haber completado el anterior
        int indiceNivelAnterior = nivel - 2; // Nivel 2 necesita completar nivel 1 (índice 0)
        
        // Verificar que el índice esté dentro de los límites del array
        if (indiceNivelAnterior >= 0 && indiceNivelAnterior < nivelesCompletados.Length)
        {
            return nivelesCompletados[indiceNivelAnterior];
        }
        
        return false;
    }    public void CompletarNivelActual()
    {
        // Validar que el nivel actual esté dentro del rango válido
        if (nivelActual < 1 || nivelActual > maxNivel) 
        {
            Debug.LogError($"Nivel actual inválido: {nivelActual}");
            return;
        }
        
        // CALCULAR TIEMPO REAL DEL NIVEL (sin contar pausas en barreras)
        float tiempoRealNivel = Time.time - tiempoInicioNivel;
        nivelEnCurso = false; // DETENER el timer del nivel
        
        // Detener el tiempo en curso para este nivel
        int idx = nivelActual - 1;
        if (idx >= 0 && idx < nivelEnCursoArray.Length) nivelEnCursoArray[idx] = false;
        
        // Validar que el índice esté dentro de los límites del array
        int indiceNivel = nivelActual - 1;
        if (indiceNivel >= 0 && indiceNivel < nivelesCompletados.Length)
        {
            nivelesCompletados[indiceNivel] = true;
        }
        else
        {
            Debug.LogError($"Índice de nivel fuera de rango: {indiceNivel}");
            return;
        }
        
        // INCREMENTAR ACIERTOS POR COMPLETAR EL NIVEL
        aciertos++;
        interaccionesAcertadas++;
        interaccionesTotales++;
        
        // Registrar tiempos REALES y de REFERENCIA del nivel
        switch (nivelActual)
        {
            case 1:
                tiempoReal_Nivel1 = tiempoRealNivel;  // Tiempo real preciso
                nivel1_Tiempo = tiempoRealNivel;      // Mantener compatibilidad
                Debug.Log($"NIVEL 1 COMPLETADO - Tiempo real: {tiempoReal_Nivel1:F2}s");
                break;
            case 2:
                tiempoReal_Nivel2 = tiempoRealNivel;
                nivel2_Tiempo = tiempoRealNivel;
                Debug.Log($"NIVEL 2 COMPLETADO - Tiempo real: {tiempoReal_Nivel2:F2}s");
                break;
            case 3:
                tiempoReal_Nivel3 = tiempoRealNivel;
                nivel3_Tiempo = tiempoRealNivel;
                Debug.Log($"NIVEL 3 COMPLETADO - Tiempo real: {tiempoReal_Nivel3:F2}s");
                break;
        }

        // --- NUEVO: Reactivar la barrera amarilla al completar el nivel ---
        // Buscar todas las barreras y activar la correspondiente
        var barreras = GameObject.FindObjectsOfType<BarreraNivel>();
        foreach (var barrera in barreras)
        {
            if (barrera.nivelRequerido == nivelActual)
            {
                barrera.ActivarBarrera();
                break;
            }
        }
        // --- FIN NUEVO ---

        // Recalcular puntaje con el nuevo acierto
        CalcularPuntaje();
        
        // Actualizar UI inmediatamente
        ActualizarUI();
          Debug.Log($"¡Nivel {nivelActual} completado en {tiempoRealNivel:F2} segundos! Aciertos totales: {aciertos}");
        
        // Avanzar automáticamente al siguiente nivel si no es el último
        if (nivelActual < maxNivel)
        {
            // NO MOSTRAR BARRERA INMEDIATAMENTE - Dejar que el BlockSpawner maneje el teletransporte
            // La barrera se mostrará después del teletransporte si es necesario
            Debug.Log($"Nivel {nivelActual} completado. Preparando transición al nivel {nivelActual + 1}");
        }
        else
        {
            // Juego completado
            juegoTerminado = true; // Asegura que no se siga actualizando el tiempo
            CompletarJuego();
        }
    }
      void MostrarBarreraNivel()
    {
        if (panelBarrera != null)
        {
            panelBarrera.SetActive(true);
            
            // INICIAR MEDICIÓN DE TIEMPO EN BARRERA
            enBarrera = true;
            tiempoInicioBarrera = Time.time;
            
            Time.timeScale = 0; // Pausar el juego
            
            Debug.Log($"Entrando en barrera nivel {nivelActual} -> {nivelActual + 1}");
        }
    }
      public void PasarSiguienteNivel()
    {
        if (PuedeAccederNivel(nivelActual + 1))
        {
            // CALCULAR TIEMPO GASTADO EN LA BARRERA
            if (enBarrera)
            {
                float tiempoEnBarrera = Time.time - tiempoInicioBarrera;
                tiempoTotal_Pausas += tiempoEnBarrera;
                enBarrera = false;
                
                Debug.Log($"Tiempo en barrera: {tiempoEnBarrera:F2}s. Total pausas: {tiempoTotal_Pausas:F2}s");
            }
            
            // AVANZAR AL SIGUIENTE NIVEL
            nivelActual++;
            // tiempoInicioNivel = Time.time;  // REINICIAR TIMER DEL NUEVO NIVEL (ahora lo hace IniciarTiempoNivel)
            reintentos_Nivel = 0;
            nivelEnCurso = false; // El timer del nivel se inicia al pasar la barrera
            
            if (panelBarrera != null)
            {
                panelBarrera.SetActive(false);
                Time.timeScale = 1; // Reanudar el juego
            }
            
            Debug.Log($"Accediendo al nivel {nivelActual} - Timer reiniciado en {Time.time}");
        }
        else
        {
            Debug.Log("No puedes acceder a este nivel aún");
        }
    }
      public void ReintentarNivel()
    {
        // CALCULAR TIEMPO GASTADO EN LA BARRERA (si estaba en una)
        if (enBarrera)
        {
            float tiempoEnBarrera = Time.time - tiempoInicioBarrera;
            tiempoTotal_Pausas += tiempoEnBarrera;
            enBarrera = false;
            
            Debug.Log($"Tiempo en barrera (reintento): {tiempoEnBarrera:F2}s");
        }
        
        reintentos_Nivel++;
        // tiempoInicioNivel = Time.time;  // REINICIAR TIMER DEL NIVEL ACTUAL (ahora lo hace IniciarTiempoNivel)
        nivelEnCurso = false; // El timer del nivel se inicia al pasar la barrera
        transform.position = respawnPosition;
        
        // Reactivar la barrera correspondiente al reintentar el nivel
        var barreras = GameObject.FindObjectsOfType<BarreraNivel>();
        foreach (var barrera in barreras)
        {
            if (barrera.nivelRequerido == nivelActual)
            {
                barrera.ActivarBarrera();
                break;
            }
        }
        
        if (panelBarrera != null)
        {
            panelBarrera.SetActive(false);
            Time.timeScale = 1;
        }
        
        Debug.Log($"Reintentando nivel {nivelActual} - Timer reiniciado en {Time.time}");
    }
    void CompletarJuego()
    {
        tiempoTotal_Sesion = Time.time - tiempoInicio_Sesion;
        // Teletransportar al jugador al finalizar el juego
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 destino = new Vector3(-11.804f, 17.612f, -5.92f);
            Rigidbody rb = player.GetComponent<Rigidbody>();
            CharacterController cc = player.GetComponent<CharacterController>();
            // Desactivar componentes de movimiento temporalmente
            if (cc != null) cc.enabled = false;
            if (rb != null) rb.isKinematic = true;
            // Forzar posición
            player.transform.position = destino;
            player.transform.rotation = Quaternion.identity;
            // Reactivar componentes
            if (cc != null) cc.enabled = true;
            if (rb != null) rb.isKinematic = false;
        }
        
        // CALCULAR TIEMPO PROMEDIO USANDO LOS TIEMPOS REALES (sin pausas)
        float tiempoJuegoEfectivo = tiempoReal_Nivel1 + tiempoReal_Nivel2 + tiempoReal_Nivel3;
        tiempoPromedio_PorNivel = tiempoJuegoEfectivo / 3f;
        
        // Mostrar resumen de tiempos
        Debug.Log("=== RESUMEN DE TIEMPOS ===");
        Debug.Log($"Tiempo total sesión: {tiempoTotal_Sesion:F2}s");
        Debug.Log($"Tiempo total en pausas/barreras: {tiempoTotal_Pausas:F2}s");
        Debug.Log($"Tiempo efectivo jugando: {tiempoJuegoEfectivo:F2}s");
        Debug.Log($"Nivel 1 - Tiempo real: {tiempoReal_Nivel1:F2}s");
        Debug.Log($"Nivel 2 - Tiempo real: {tiempoReal_Nivel2:F2}s");
        Debug.Log($"Nivel 3 - Tiempo real: {tiempoReal_Nivel3:F2}s");
        Debug.Log($"Tiempo promedio por nivel: {tiempoPromedio_PorNivel:F2}s");
        
        // Calcular métricas finales
        CalcularMetricasFinales();
        
        Debug.Log("¡Juego completado! Guardando estadísticas...");
        GuardarEstadisticas();
    }
    
    #endregion
    
    #region Cálculos y Métricas
    
    void CalcularPuntaje()
    {
        // Puntaje basado en aciertos vs errores
        float precision = interaccionesTotales > 0 ? (float)interaccionesAcertadas / interaccionesTotales : 0f;
        puntajeObtenido = precision * puntajeMaximoPosible;
        porcentajeRendimiento = precision * 100f;
    }
    
    void CalcularMetricasFinales()
    {
        // Índice de Interacción del Usuario (IIU)
        float iiu = (comandosIniciados + gestosInterpretados + manipulacionesInteractivas) / 3f;
        
        // Precisión de Interacción (TPI)
        float tpi = interaccionesTotales > 0 ? (float)interaccionesAcertadas / interaccionesTotales : 0f;
        
        // Fluidez de Navegación (IFN)
        float ifn = velocidadMovimientoMedida / velocidadEstandarObjetivo;
        
        // Índice de Dominio Conceptual (IDC)
        float idc = (evaluacionConceptual + aplicacionPractica + resolucionCasos) / 3f;
        
        // Satisfacción del Usuario (ISU)
        float isu = sumaScoresSUS / numeroTotalPreguntas;
        
        Debug.Log($"Métricas finales - IIU: {iiu:F2}, TPI: {tpi:F2}, IFN: {ifn:F2}, IDC: {idc:F2}, ISU: {isu:F2}");
    }
    
    void ActualizarTiempo()
    {
        tiempoActual_Nivel = Time.time - tiempoInicioNivel;
        
        // Verificar límite de tiempo
        if (tiempoActual_Nivel > tiemposLimite[nivelActual - 1])
        {
            Debug.Log("Tiempo límite excedido para el nivel " + nivelActual);
            // Opcional: forzar reinicio o mostrar mensaje
        }
    }    void ActualizarUI()
    {
        if (uiNivelActual != null)
            uiNivelActual.text = $"Nivel: {nivelActual}";
        if (uiPuntaje != null)
            uiPuntaje.text = $"Aciertos: {aciertos} | Errores: {errores} | Puntaje: {puntajeObtenido:F0}";
        // Actualizar tiempos por nivel solo si el nivel ya fue completado
        if (uiTiempoNivel1 != null && tiempoReal_Nivel1 > 0)
            uiTiempoNivel1.text = $"Nivel 1: {tiempoReal_Nivel1:F1}s";
        if (uiTiempoNivel2 != null && tiempoReal_Nivel2 > 0)
            uiTiempoNivel2.text = $"Nivel 2: {tiempoReal_Nivel2:F1}s";
        if (uiTiempoNivel3 != null && tiempoReal_Nivel3 > 0)
            uiTiempoNivel3.text = $"Nivel 3: {tiempoReal_Nivel3:F1}s";
        if (uiTiempoTotal != null && juegoTerminado)
            uiTiempoTotal.text = $"Total: {tiempoTotal_Sesion:F1}s";
        
        // --- NUEVO: ACTUALIZAR TOTALES GLOBALES EN LA UI ---
        if (uiTotalSaltosTotales != null)
            uiTotalSaltosTotales.text = $"Total Saltos: {TotalSaltosTotalesPorNivel}";
        if (uiTotalSaltosCorrectos != null)
            uiTotalSaltosCorrectos.text = $"Saltos Correctos: {TotalSaltosCorrectosPorNivel}";
        if (uiTotalCaidas != null)
            uiTotalCaidas.text = $"Total Caídas: {TotalCaidasPorNivel}";
        // --- FIN NUEVO ---
    }
    
    #endregion
    
    #region Métodos Públicos para Interacciones VR
    
    public void RegistrarTeleport()
    {
        teleports_Realizados++;
        comandosIniciados++;
    }
    
    public void RegistrarObjetoAgarrado()
    {
        objetos_Agarrados++;
        manipulacionesInteractivas++;
    }
    
    public void RegistrarMenuAbierto()
    {
        menus_Abiertos++;
        gestosInterpretados++;
    }
    
    public void RegistrarGesto()
    {
        gestosInterpretados++;
    }
    
    #endregion
      #region Métodos Públicos para GameController
    
    /// <summary>
    /// Guardar estadísticas público para GameController
    /// </summary>
    public void GuardarEstadisticas()
    {
        // Aquí puedes implementar el guardado de estadísticas
        // Por ejemplo, en PlayerPrefs, archivo JSON, o base de datos
        
        PlayerPrefs.SetInt("SaltosCorrectos", saltos_Correctos);
        PlayerPrefs.SetInt("SaltosIncorrectos", saltos_Incorrectos);
        PlayerPrefs.SetFloat("TiempoTotal", tiempoTotal_Sesion);
        PlayerPrefs.SetFloat("PuntajeFinal", puntajeObtenido);
          // Guardar datos por nivel (TIEMPOS PRECISOS)
        PlayerPrefs.SetFloat("TiempoReal_Nivel1", tiempoReal_Nivel1);
        PlayerPrefs.SetFloat("TiempoReal_Nivel2", tiempoReal_Nivel2);
        PlayerPrefs.SetFloat("TiempoReal_Nivel3", tiempoReal_Nivel3);
        PlayerPrefs.SetFloat("TiempoTotal_Pausas", tiempoTotal_Pausas);
        
        PlayerPrefs.SetInt("Nivel1_Saltos", nivel1_Saltos_Correctos);
        PlayerPrefs.SetInt("Nivel1_Caidas", nivel1_Caidas);
        PlayerPrefs.SetFloat("Nivel1_Tiempo", nivel1_Tiempo);
        
        PlayerPrefs.SetInt("Nivel2_Saltos", nivel2_Saltos_Correctos);
        PlayerPrefs.SetInt("Nivel2_Caidas", nivel2_Caidas);
        PlayerPrefs.SetFloat("Nivel2_Tiempo", nivel2_Tiempo);
        
        PlayerPrefs.SetInt("Nivel3_Saltos", nivel3_Saltos_Correctos);
        PlayerPrefs.SetInt("Nivel3_Caidas", nivel3_Caidas);
        PlayerPrefs.SetFloat("Nivel3_Tiempo", nivel3_Tiempo);
        
        // Guardar métricas de comprensión
        PlayerPrefs.SetInt("IF_Correctos", if_Statements_Correctos);
        PlayerPrefs.SetInt("ELSE_Correctos", else_Statements_Correctos);
        PlayerPrefs.SetInt("Nested_Correctos", nested_Statements_Correctos);
        
        PlayerPrefs.Save();        
        Debug.Log("Estadísticas guardadas exitosamente");
    }
    
    #endregion

    public void RegistrarError()
    {
        errores++;
        interaccionesTotales++;
        
        // Registrar por nivel específico
        switch (nivelActual)
        {
            case 1:
                nivel1_Saltos_Totales++;
                break;
            case 2:
                nivel2_Saltos_Totales++;
                break;
            case 3:
                nivel3_Saltos_Totales++;
                break;
        }
        
        // Actualizar UI inmediatamente
        ActualizarUI();
        
        Debug.Log($"Error registrado. Total errores: {errores}, Nivel: {nivelActual}, Saltos totales nivel: {GetSaltosTotalesPorNivel()}");
    }
    
    // Métodos auxiliares para obtener estadísticas por nivel
    public int GetSaltosCorrectosPorNivel()
    {
        switch (nivelActual)
        {
            case 1: return nivel1_Saltos_Correctos;
            case 2: return nivel2_Saltos_Correctos;
            case 3: return nivel3_Saltos_Correctos;
            default: return 0;
        }
    }
    
    public int GetSaltosTotalesPorNivel()
    {
        switch (nivelActual)
        {            case 1: return nivel1_Saltos_Totales;
            case 2: return nivel2_Saltos_Totales;
            case 3: return nivel3_Saltos_Totales;
            default: return 0;
        }
    }
    
    /// <summary>
    /// Método llamado por BlockSpawner después del teletransporte para completar la transición
    /// </summary>
    public void CompletarTransicionNivel()
    {
        if (nivelActual < maxNivel)
        {
            // AVANZAR AL SIGUIENTE NIVEL
            int siguienteNivel = nivelActual + 1;
            nivelActual = siguienteNivel;
            tiempoInicioNivel = Time.time;  // REINICIAR TIMER DEL NUEVO NIVEL
            reintentos_Nivel = 0;
            
            Debug.Log($"Transición completada al nivel {nivelActual} - Timer reiniciado");
            
            // Opcional: Mostrar barrera después del teletransporte si se desea
            // MostrarBarreraNivel();
        }
    }
    
    /// <summary>
    /// Método para mostrar barrera manualmente cuando sea necesario
    /// </summary>
    public void MostrarBarreraManual()
    {
        MostrarBarreraNivel();
    }

    /// <summary>
    /// Método para ser llamado por BarreraNivel cuando el jugador pasa la barrera
    /// </summary>
    public void IniciarTiempoNivel()
    {
        tiempoInicioNivel = Time.time;
        nivelEnCurso = true;
        Debug.Log($"⏱️ Timer de nivel {nivelActual} iniciado en {tiempoInicioNivel}");
    }
    
    // --- PROPIEDADES DE SUMA GLOBAL POR NIVELES ---
    public int TotalSaltosTotalesPorNivel
    {
        get { return nivel1_Saltos_Totales + nivel2_Saltos_Totales + nivel3_Saltos_Totales; }
    }
    public int TotalSaltosCorrectosPorNivel
    {
        get { return nivel1_Saltos_Correctos + nivel2_Saltos_Correctos + nivel3_Saltos_Correctos; }
    }
    public int TotalCaidasPorNivel
    {
        get { return nivel1_Caidas + nivel2_Caidas + nivel3_Caidas; }
    }
    // --- FIN PROPIEDADES DE SUMA GLOBAL ---

    /// <summary>
    /// Teletransporta al jugador al área final (0, -450, 945)
    /// </summary>
    public void TeletransportarAFinal()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0f, -450f, 945f);
            player.transform.rotation = Quaternion.identity;
            Debug.Log("[GameRespawn] Jugador teletransportado al área final (0, -450, 945)");
        }
    }
}
