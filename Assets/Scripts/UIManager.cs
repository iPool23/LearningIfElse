using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestor de la interfaz de usuario
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("=== PANELES PRINCIPALES ===")]
    public GameObject panelBarrera;
    public GameObject panelHUD;
    public GameObject panelAyuda;
    public GameObject panelEstadisticas;

    [Header("=== HUD ELEMENTOS ===")]
    public TextMeshProUGUI txtNivel;
    public TextMeshProUGUI txtPuntaje;
    public TextMeshProUGUI txtTiempo;
    public TextMeshProUGUI txtAciertos;
    public TextMeshProUGUI txtErrores;    public Slider barraProgreso;

    [Header("=== BARRERA ELEMENTOS ===")]
    public TextMeshProUGUI txtBarrera;
    public Button btnSiguienteNivel;
    public Button btnReintentarNivel;
    public TextMeshProUGUI txtEstadisticasNivel;

    [Header("=== AYUDA ELEMENTOS ===")]
    public TextMeshProUGUI txtAyudaNivel1;
    public TextMeshProUGUI txtAyudaNivel2;
    public TextMeshProUGUI txtAyudaNivel3;
    public Button btnCerrarAyuda;

    [Header("=== CONFIGURACIÓN ===")]
    public float tiempoMostrarMensaje = 3f;
    public Color colorExito = Color.green;
    public Color colorError = Color.red;
    public Color colorNormal = Color.white;

    private GameRespawn gameManager;
    private float tiempoUltimoMensaje;

    // Textos de ayuda por nivel
    private string[] ayudaNivel1 = {
        "NIVEL 1: CONDICIONALES SIMPLES (IF)",
        "",
        "Concepto: IF (condición) THEN (acción)",
        "",
        "En este nivel debes saltar SOLO si el vidrio es VERDE.",
        "SI el vidrio es verde → ENTONCES salta",
        "SI el vidrio es rojo → NO saltes (busca otro camino)",
        "",
        "¡Es como programar! IF (vidrio == verde) { saltar(); }"
    };

    private string[] ayudaNivel2 = {
        "NIVEL 2: CONDICIONALES DOBLES (IF-ELSE)",
        "",
        "Concepto: IF (condición) THEN (acción) ELSE (otra acción)",
        "",
        "Ahora hay DOS opciones en cada fila:",
        "SI el vidrio izquierdo es verde → ENTONCES salta izquierda",
        "SI NO (ELSE) → ENTONCES salta derecha",
        "",
        "¡Como programar! IF (izquierdo == verde) { saltar_izq(); } ELSE { saltar_der(); }"
    };

    private string[] ayudaNivel3 = {
        "NIVEL 3: CONDICIONALES ANIDADAS",
        "",
        "Concepto: IF dentro de otro IF",
        "",
        "Ahora debes evaluar MÚLTIPLES condiciones:",
        "SI el vidrio es verde Y está en posición segura → ENTONCES salta",
        "SI el vidrio es verde PERO está agrietado → busca alternativa",
        "",
        "¡Como programar anidado! IF (verde) { IF (seguro) { saltar(); } }"
    };
    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();

        if (gameManager == null)
        {
            Debug.LogError("No se encontró GameRespawn en la escena");
        }

        ConfigurarEventos();
        InicializarUI();
    }

    void Update()
    {
        ActualizarHUD();
    }    void ConfigurarEventos()
    {
        // Barrera
        if (btnSiguienteNivel != null)
            btnSiguienteNivel.onClick.AddListener(() => gameManager?.PasarSiguienteNivel());

        if (btnReintentarNivel != null)
            btnReintentarNivel.onClick.AddListener(() => gameManager?.ReintentarNivel());

        // Ayuda
        if (btnCerrarAyuda != null)
            btnCerrarAyuda.onClick.AddListener(CerrarAyuda);
    }

    void InicializarUI()
    {
        // Asegurarse de que solo el HUD esté activo al inicio        if (panelHUD != null) panelHUD.SetActive(true);
        if (panelBarrera != null) panelBarrera.SetActive(false);
        if (panelAyuda != null) panelAyuda.SetActive(false);
        if (panelEstadisticas != null) panelEstadisticas.SetActive(false);
    }

    void ActualizarHUD()
    {
        if (gameManager == null) return;

        // Actualizar textos del HUD
        if (txtNivel != null)
            txtNivel.text = $"Nivel {gameManager.nivelActual}";

        if (txtPuntaje != null)
            txtPuntaje.text = $"Puntaje: {gameManager.puntajeObtenido:F0}";

        if (txtTiempo != null)
            txtTiempo.text = $"Tiempo: {gameManager.tiempoActual_Nivel:F1}s";

        if (txtAciertos != null)
            txtAciertos.text = $"Aciertos: {gameManager.aciertos}";

        if (txtErrores != null)
            txtErrores.text = $"Errores: {gameManager.errores}";

        // Actualizar barra de progreso
        if (barraProgreso != null)
        {
            float progreso = (float)gameManager.nivelActual / gameManager.maxNivel;
            barraProgreso.value = progreso;
        }
    }

    public void MostrarBarreraNivel()
    {
        if (panelBarrera == null || gameManager == null) return;

        panelBarrera.SetActive(true);

        // Configurar texto de barrera
        if (txtBarrera != null)
        {
            if (gameManager.nivelActual < gameManager.maxNivel)
            {
                txtBarrera.text = $"¡Nivel {gameManager.nivelActual} Completado!\n\n¿Continuar al Nivel {gameManager.nivelActual + 1}?";
            }
            else
            {
                txtBarrera.text = "¡FELICITACIONES!\n\n¡Has completado todos los niveles!";
                if (btnSiguienteNivel != null) btnSiguienteNivel.gameObject.SetActive(false);
            }
        }

        // Mostrar estadísticas del nivel
        if (txtEstadisticasNivel != null)
        {
            string stats = $"Estadísticas del Nivel {gameManager.nivelActual}:\n";
            stats += $"Saltos Correctos: {GetSaltosCorrectosPorNivel(gameManager.nivelActual)}\n";
            stats += $"Caídas: {GetCaidasPorNivel(gameManager.nivelActual)}\n";
            stats += $"Tiempo: {GetTiempoPorNivel(gameManager.nivelActual):F1}s\n";
            stats += $"Precisión: {CalcularPrecisionNivel(gameManager.nivelActual):F1}%";

            txtEstadisticasNivel.text = stats;
        }
    }

    void CerrarAyuda()
    {
        if (panelAyuda != null)
            panelAyuda.SetActive(false);
    }

    public void MostrarMensajeExito()
    {
        StartCoroutine(MostrarMensajeTemporal("¡Salto Correcto!", colorExito));
    }

    public void MostrarMensajeError()
    {
        StartCoroutine(MostrarMensajeTemporal("¡Cuidado! Vidrio Roto", colorError));
    }

    System.Collections.IEnumerator MostrarMensajeTemporal(string mensaje, Color color)
    {
        // Crear un texto temporal para el mensaje
        GameObject mensajeObj = new GameObject("MensajeTemporal");
        mensajeObj.transform.SetParent(panelHUD.transform);

        TextMeshProUGUI txtMensaje = mensajeObj.AddComponent<TextMeshProUGUI>();
        txtMensaje.text = mensaje;
        txtMensaje.color = color;
        txtMensaje.fontSize = 36;
        txtMensaje.alignment = TextAlignmentOptions.Center;

        RectTransform rect = mensajeObj.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400, 100);

        yield return new WaitForSeconds(tiempoMostrarMensaje);

        Destroy(mensajeObj);
    }

    #region Métodos Auxiliares

    int GetSaltosCorrectosPorNivel(int nivel)
    {
        if (gameManager == null) return 0;

        switch (nivel)
        {
            case 1: return gameManager.nivel1_Saltos_Correctos;
            case 2: return gameManager.nivel2_Saltos_Correctos;
            case 3: return gameManager.nivel3_Saltos_Correctos;
            default: return 0;
        }
    }

    int GetCaidasPorNivel(int nivel)
    {
        if (gameManager == null) return 0;

        switch (nivel)
        {
            case 1: return gameManager.nivel1_Caidas;
            case 2: return gameManager.nivel2_Caidas;
            case 3: return gameManager.nivel3_Caidas;
            default: return 0;
        }
    }

    float GetTiempoPorNivel(int nivel)
    {
        if (gameManager == null) return 0f;

        switch (nivel)
        {
            case 1: return gameManager.nivel1_Tiempo;
            case 2: return gameManager.nivel2_Tiempo;
            case 3: return gameManager.nivel3_Tiempo;
            default: return 0f;
        }
    }

    float CalcularPrecisionNivel(int nivel)
    {
        int correctos = GetSaltosCorrectosPorNivel(nivel);
        int caidas = GetCaidasPorNivel(nivel);
        int total = correctos + caidas;

        return total > 0 ? ((float)correctos / total) * 100f : 0f;
    }

    #endregion

    #region Métodos Públicos para VR

    public void BotonPausa()
    {
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
    }

    public void BotonReiniciar()
    {
        if (gameManager != null)
            gameManager.ReintentarNivel();
    }

    #endregion
}
