using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager - Gestiona el HUD en tiempo real con los 5 indicadores principales.
/// Incluye temporizador de sesión de 5 minutos con redirección automática a Estadísticas.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ─── PANELES ────────────────────────────────────────────────────────────────
    [Header("=== PANELES PRINCIPALES ===")]
    public GameObject panelHUD;
    public GameObject panelEstadisticas;

    // ─── HUD PRINCIPAL (5 indicadores) ──────────────────────────────────────────
    [Header("=== HUD - GAME MANAGER ===")]
    [Tooltip("Txt_Nivel  → muestra el nivel actual del jugador")]
    public TextMeshProUGUI txtNivelActual;

    [Tooltip("Txt_Saltos → muestra el total de saltos realizados")]
    public TextMeshProUGUI txtSaltosTotales;

    [Tooltip("Txt_Caidas → muestra el total de caídas registradas")]
    public TextMeshProUGUI txtCaidasTotales;

    [Tooltip("Txt_Puntaje → muestra el puntaje acumulado")]
    public TextMeshProUGUI txtPuntaje;

    [Tooltip("Txt_Tiempo → muestra el cronómetro de 0:00 a 5:00")]
    public TextMeshProUGUI txtTiempo;

    // ─── CONFIGURACIÓN DE SESIÓN ────────────────────────────────────────────────
    [Header("=== CONFIGURACIÓN DE SESIÓN ===")]
    [Tooltip("Tiempo máximo de sesión en minutos (por defecto 5)")]
    public float tiempoMaximoMinutos = 5f;

    // ─── CAMPOS LEGACY (panel estadísticas detallado) ───────────────────────────
    [Header("=== ESTADÍSTICAS DETALLADAS (Panel Estadísticas) ===")]
    public TextMeshProUGUI txtTiempoNivel1;
    public TextMeshProUGUI txtTiempoNivel2;
    public TextMeshProUGUI txtTiempoNivel3;
    public TextMeshProUGUI txtTiempoTotal;
    public TextMeshProUGUI txtSaltosCorrectosTotales;
    public TextMeshProUGUI txtSaltosNivel1;
    public TextMeshProUGUI txtSaltosCorrectosNivel1;
    public TextMeshProUGUI txtSaltosNivel2;
    public TextMeshProUGUI txtSaltosCorrectosNivel2;
    public TextMeshProUGUI txtSaltosNivel3;
    public TextMeshProUGUI txtSaltosCorrectosNivel3;
    public TextMeshProUGUI txtCaidasNivel1;
    public TextMeshProUGUI txtCaidasNivel2;
    public TextMeshProUGUI txtCaidasNivel3;

    // ─── ESTADO INTERNO ─────────────────────────────────────────────────────────
    private GameRespawn gameManager;
    private float _tiempoSesion = 0f;          // segundos transcurridos
    private bool  _sesionTerminada = false;     // flag para disparar el evento solo una vez

    // ─── CICLO DE VIDA ──────────────────────────────────────────────────────────
    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        _tiempoSesion = 0f;
        _sesionTerminada = false;
        ActualizarHUD();
    }

    void Update()
    {
        if (!_sesionTerminada)
        {
            // Avanzar cronómetro
            _tiempoSesion += Time.deltaTime;

            float limiteSegundos = tiempoMaximoMinutos * 60f;  // 5 min = 300 s

            if (_tiempoSesion >= limiteSegundos)
            {
                // Clampear para no mostrar "5:01"
                _tiempoSesion = limiteSegundos;
                _sesionTerminada = true;

                // Redirigir automáticamente a Estadísticas
                MostrarEstadisticasFinales();
                Debug.Log("[UIManager] Tiempo de sesión agotado. Redirigiendo a Estadísticas.");
            }

            ActualizarHUD();
        }
    }

    // ─── HUD PRINCIPAL ──────────────────────────────────────────────────────────
    void ActualizarHUD()
    {
        if (gameManager == null) return;

        // 1. Nivel Actual
        if (txtNivelActual != null)
            txtNivelActual.text = $"Nivel: {gameManager.nivelActual}";

        // 2. Saltos Totales (correctos + caídas = todos los intentos)
        if (txtSaltosTotales != null)
            txtSaltosTotales.text = $"Saltos: {gameManager.TotalSaltosTotalesPorNivel}";

        // 3. Caídas Totales
        if (txtCaidasTotales != null)
            txtCaidasTotales.text = $"Caídas: {gameManager.TotalCaidasPorNivel}";

        // 4. Puntaje
        if (txtPuntaje != null)
            txtPuntaje.text = $"Puntaje: {gameManager.puntajeObtenido:F0}";

        // 5. Tiempo de sesión  →  formato  M:SS
        if (txtTiempo != null)
        {
            int minutos  = Mathf.FloorToInt(_tiempoSesion / 60f);
            int segundos = Mathf.FloorToInt(_tiempoSesion % 60f);
            txtTiempo.text = $"Tiempo: {minutos}:{segundos:D2}";
        }

        // ── Estadísticas detalladas (opcionales, panel secundario) ───────────────
        if (txtTiempoNivel1 != null)
            txtTiempoNivel1.text = $"Tiempo N1: {gameManager.nivel1_Tiempo:F2}s";
        if (txtTiempoNivel2 != null)
            txtTiempoNivel2.text = $"Tiempo N2: {gameManager.nivel2_Tiempo:F2}s";
        if (txtTiempoNivel3 != null)
            txtTiempoNivel3.text = $"Tiempo N3: {gameManager.nivel3_Tiempo:F2}s";
        if (txtTiempoTotal != null)
            txtTiempoTotal.text = $"Tiempo Total: {gameManager.tiempoTotal_Sesion:F2}s";
        if (txtSaltosCorrectosTotales != null)
            txtSaltosCorrectosTotales.text = $"Saltos Correctos: {gameManager.TotalSaltosCorrectosPorNivel}";
        if (txtSaltosNivel1 != null)
            txtSaltosNivel1.text = $"Saltos N1: {gameManager.nivel1_Saltos_Totales}";
        if (txtSaltosCorrectosNivel1 != null)
            txtSaltosCorrectosNivel1.text = $"Correctos N1: {gameManager.nivel1_Saltos_Correctos}";
        if (txtSaltosNivel2 != null)
            txtSaltosNivel2.text = $"Saltos N2: {gameManager.nivel2_Saltos_Totales}";
        if (txtSaltosCorrectosNivel2 != null)
            txtSaltosCorrectosNivel2.text = $"Correctos N2: {gameManager.nivel2_Saltos_Correctos}";
        if (txtSaltosNivel3 != null)
            txtSaltosNivel3.text = $"Saltos N3: {gameManager.nivel3_Saltos_Totales}";
        if (txtSaltosCorrectosNivel3 != null)
            txtSaltosCorrectosNivel3.text = $"Correctos N3: {gameManager.nivel3_Saltos_Correctos}";
        if (txtCaidasNivel1 != null)
            txtCaidasNivel1.text = $"Caídas N1: {gameManager.nivel1_Caidas}";
        if (txtCaidasNivel2 != null)
            txtCaidasNivel2.text = $"Caídas N2: {gameManager.nivel2_Caidas}";
        if (txtCaidasNivel3 != null)
            txtCaidasNivel3.text = $"Caídas N3: {gameManager.nivel3_Caidas}";
    }

    // ─── NAVEGACIÓN DE PANELES ──────────────────────────────────────────────────

    /// <summary>
    /// Oculta el HUD y muestra el panel de Estadísticas.
    /// Se llama automáticamente al cumplirse 5 minutos, o manualmente al terminar el juego.
    /// </summary>
    public void MostrarEstadisticasFinales()
    {
        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(true);
        if (panelHUD != null)
            panelHUD.SetActive(false);
    }

    /// <summary>
    /// Muestra el HUD y oculta el panel de Estadísticas.
    /// </summary>
    public void MostrarHUD()
    {
        if (panelHUD != null)
            panelHUD.SetActive(true);
        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(false);
    }

    /// <summary>
    /// Reinicia el cronómetro de sesión (útil para testing o reinicio de partida).
    /// </summary>
    public void ReiniciarSesion()
    {
        _tiempoSesion = 0f;
        _sesionTerminada = false;
        MostrarHUD();
        Debug.Log("[UIManager] Sesión reiniciada.");
    }
}
