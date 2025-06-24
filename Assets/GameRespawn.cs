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
    public bool completo_Tutorial = false;      // Si completó el tutorial inicial
    public int veces_Menu_Ayuda = 0;           // Acceso al menú de ayuda
    
    [Header("Tiempo y Progreso")]
    public float tiempoTotal_Sesion = 0f;
    public float tiempoPromedio_PorNivel = 0f;
    public float tiempoInicio_Sesion;
    public float tiempoActual_Nivel;
    
    [Header("Interacciones VR Específicas")]
    public int teleports_Realizados = 0;
    public int objetos_Agarrados = 0;
    public int menus_Abiertos = 0;
    public int ayudas_Solicitadas = 0;
    
    [Header("=== SISTEMA DE NIVELES Y BARRERAS ===")]
    public int nivelActual = 1;
    public int maxNivel = 3;
    public bool[] nivelesCompletados = new bool[3];
    public float[] tiemposLimite = { 120f, 180f, 240f }; // Tiempo límite por nivel
    
    [Header("=== SISTEMA DE TUTORIAL ===")]
    public bool tutorialActivo = false;
    public int pasoTutorialActual = 0;
    public int totalPasosTutorial = 5;
    
    [Header("=== UI REFERENCIAS ===")]
    public TextMeshProUGUI uiNivelActual;
    public TextMeshProUGUI uiPuntaje;
    public TextMeshProUGUI uiTiempo;
    public TextMeshProUGUI uiTutorial;
    public GameObject panelTutorial;
    public GameObject panelBarrera;
    public Button botonSiguienteNivel;
    public Button botonReintentar;
    
    // Variables privadas para control interno
    private Vector3 ultimaPosicion;
    private float tiempoUltimoPausa;
    private bool enPausa = false;
    private float tiempoInicioNivel;
      void Start()
    {
        // Validar que el array de niveles completados tenga el tamaño correcto
        if (nivelesCompletados == null || nivelesCompletados.Length != maxNivel)
        {
            Debug.LogWarning($"Redimensionando array nivelesCompletados a {maxNivel} elementos");
            nivelesCompletados = new bool[maxNivel];
        }
        
        tiempoInicio_Sesion = Time.time;
        tiempoInicioNivel = Time.time;
        ultimaPosicion = transform.position;
        
        // Inicializar tutorial si es la primera vez
        if (!completo_Tutorial)
        {
            IniciarTutorial();
        }
        
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
        
        // Registrar tiempo final del nivel
        float tiempoNivel = Time.time - tiempoInicioNivel;
        switch (nivelActual)
        {
            case 1:
                nivel1_Tiempo = tiempoNivel;
                break;
            case 2:
                nivel2_Tiempo = tiempoNivel;
                break;
            case 3:
                nivel3_Tiempo = tiempoNivel;
                break;
        }
        
        // Recalcular puntaje con el nuevo acierto
        CalcularPuntaje();
        
        // Actualizar UI inmediatamente
        ActualizarUI();
        
        Debug.Log($"¡Nivel {nivelActual} completado en {tiempoNivel:F2} segundos! Aciertos totales: {aciertos}");
        
        // Avanzar automáticamente al siguiente nivel si no es el último
        if (nivelActual < maxNivel)
        {
            int siguienteNivel = nivelActual + 1;
            nivelActual = siguienteNivel;
            tiempoInicioNivel = Time.time;
            reintentos_Nivel = 0;
            
            Debug.Log($"Avanzando automáticamente al nivel {nivelActual}");
            
            // Opcional: Mostrar mensaje de transición
            //MostrarBarreraNivel();
        }
        else
        {
            // Juego completado
            CompletarJuego();
        }
    }
    
    void MostrarBarreraNivel()
    {
        if (panelBarrera != null)
        {
            panelBarrera.SetActive(true);
            Time.timeScale = 0; // Pausar el juego
        }
    }
    
    public void PasarSiguienteNivel()
    {
        if (PuedeAccederNivel(nivelActual + 1))
        {
            nivelActual++;
            tiempoInicioNivel = Time.time;
            reintentos_Nivel = 0;
            
            if (panelBarrera != null)
            {
                panelBarrera.SetActive(false);
                Time.timeScale = 1; // Reanudar el juego
            }
            
            Debug.Log($"Accediendo al nivel {nivelActual}");
        }
        else
        {
            Debug.Log("No puedes acceder a este nivel aún");
        }
    }
    
    public void ReintentarNivel()
    {
        reintentos_Nivel++;
        tiempoInicioNivel = Time.time;
        transform.position = respawnPosition;
        
        if (panelBarrera != null)
        {
            panelBarrera.SetActive(false);
            Time.timeScale = 1;
        }
    }
    
    void CompletarJuego()
    {
        tiempoTotal_Sesion = Time.time - tiempoInicio_Sesion;
        tiempoPromedio_PorNivel = tiempoTotal_Sesion / 3f;
        
        // Calcular métricas finales
        CalcularMetricasFinales();
        
        Debug.Log("¡Juego completado! Guardando estadísticas...");
        GuardarEstadisticas();
    }
    
    #endregion
      #region Sistema de Tutorial
    
    public void IniciarTutorial()
    {
        tutorialActivo = true;
        pasoTutorialActual = 0;
        
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(true);
        }
        
        MostrarPasoTutorial();
    }
    
    void MostrarPasoTutorial()
    {
        string[] pasosTutorial = {
            "¡Bienvenido al juego de condicionales IF-ELSE! Aprenderás programación saltando vidrios.",
            "Objetivo: Llegar al final saltando solo sobre los vidrios SEGUROS (verdes).",
            "Los vidrios ROJOS se romperán y caerás. ¡Observa bien antes de saltar!",
            "Usa los controles VR para moverte y saltar. Recuerda: IF (vidrio verde) THEN (saltar).",
            "¡Perfecto! Ahora comienza el nivel 1. ¡Buena suerte!"
        };
        
        if (uiTutorial != null && pasoTutorialActual < pasosTutorial.Length)
        {
            uiTutorial.text = pasosTutorial[pasoTutorialActual];
        }
    }
    
    public void SiguientePasoTutorial()
    {
        pasoTutorialActual++;
        
        if (pasoTutorialActual < totalPasosTutorial)
        {
            MostrarPasoTutorial();
        }
        else
        {
            FinalizarTutorial();
        }
    }
    
    void FinalizarTutorial()
    {
        tutorialActivo = false;
        completo_Tutorial = true;
        
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(false);
        }
        
        Debug.Log("Tutorial completado");
    }
    
    public void MostrarAyuda()
    {
        veces_Menu_Ayuda++;
        ayudas_Solicitadas++;
        dudas_Expresadas++;
        
        // Aquí puedes mostrar un panel de ayuda
        Debug.Log("Mostrando ayuda - Veces solicitada: " + veces_Menu_Ayuda);
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
    }
      void ActualizarUI()
    {
        if (uiNivelActual != null)
            uiNivelActual.text = $"Nivel: {nivelActual}";
            
        if (uiPuntaje != null)
            uiPuntaje.text = $"Aciertos: {aciertos} | Errores: {errores} | Puntaje: {puntajeObtenido:F0}";
              if (uiTiempo != null)
            uiTiempo.text = $"Tiempo: {tiempoActual_Nivel:F1}s";
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
        
        // Guardar datos por nivel
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
}
