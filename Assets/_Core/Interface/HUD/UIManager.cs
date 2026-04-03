using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestor de la interfaz de usuario completo y educativo
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("=== PANELES PRINCIPALES ===")]
    public GameObject panelHUD;
    public GameObject panelEstadisticas;

    [Header("=== HUD ELEMENTOS ===")]
    public TextMeshProUGUI txtPuntaje;

    [Header("=== TIEMPOS ===")]
    public TextMeshProUGUI txtTiempoNivel1;
    public TextMeshProUGUI txtTiempoNivel2;
    public TextMeshProUGUI txtTiempoNivel3;
    public TextMeshProUGUI txtTiempoTotal;

    [Header("=== SALTOS ===")]
    public TextMeshProUGUI txtSaltosTotales;
    public TextMeshProUGUI txtSaltosCorrectosTotales;
    public TextMeshProUGUI txtSaltosNivel1;
    public TextMeshProUGUI txtSaltosCorrectosNivel1;
    public TextMeshProUGUI txtSaltosNivel2;
    public TextMeshProUGUI txtSaltosCorrectosNivel2;
    public TextMeshProUGUI txtSaltosNivel3;
    public TextMeshProUGUI txtSaltosCorrectosNivel3;

    [Header("=== CAIDAS ===")]
    public TextMeshProUGUI txtCaidasTotales;
    public TextMeshProUGUI txtCaidasNivel1;
    public TextMeshProUGUI txtCaidasNivel2;
    public TextMeshProUGUI txtCaidasNivel3;

    private GameRespawn gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        ActualizarHUD();
    }

    void Update()
    {
        ActualizarHUD();
    }

    void ActualizarHUD()
    {
        if (gameManager == null) return;

        // Solo mostrar puntaje en el HUD principal
        if (txtPuntaje != null)
            txtPuntaje.text = $"Puntaje: {gameManager.puntajeObtenido}";

        // El resto de los campos pueden usarse en paneles de estadísticas o detalles
        if (txtTiempoNivel1 != null)
            txtTiempoNivel1.text = $"Tiempo: {gameManager.nivel1_Tiempo:F2}s";
        if (txtTiempoNivel2 != null)
            txtTiempoNivel2.text = $"Tiempo: {gameManager.nivel2_Tiempo:F2}s";
        if (txtTiempoNivel3 != null)
            txtTiempoNivel3.text = $"Tiempo: {gameManager.nivel3_Tiempo:F2}s";
        if (txtTiempoTotal != null)
            txtTiempoTotal.text = $"Tiempo Total: {gameManager.tiempoTotal_Sesion:F2}s";
        if (txtSaltosTotales != null)
            txtSaltosTotales.text = $"Saltos Totales: {gameManager.TotalSaltosTotalesPorNivel}";
        if (txtSaltosCorrectosTotales != null)
            txtSaltosCorrectosTotales.text = $"Saltos Correctos Totales: {gameManager.TotalSaltosCorrectosPorNivel}";
        if (txtSaltosNivel1 != null)
            txtSaltosNivel1.text = $"Saltos: {gameManager.nivel1_Saltos_Totales}";
        if (txtSaltosCorrectosNivel1 != null)
            txtSaltosCorrectosNivel1.text = $"Saltos Correctos: {gameManager.nivel1_Saltos_Correctos}";
        if (txtSaltosNivel2 != null)
            txtSaltosNivel2.text = $"Saltos: {gameManager.nivel2_Saltos_Totales}";
        if (txtSaltosCorrectosNivel2 != null)
            txtSaltosCorrectosNivel2.text = $"Saltos Correctos: {gameManager.nivel2_Saltos_Correctos}";
        if (txtSaltosNivel3 != null)
            txtSaltosNivel3.text = $"Saltos: {gameManager.nivel3_Saltos_Totales}";
        if (txtSaltosCorrectosNivel3 != null)
            txtSaltosCorrectosNivel3.text = $"Saltos Correctos: {gameManager.nivel3_Saltos_Correctos}";
        if (txtCaidasTotales != null)
            txtCaidasTotales.text = $"Caídas Totales: {gameManager.TotalCaidasPorNivel}";
        if (txtCaidasNivel1 != null)
            txtCaidasNivel1.text = $"Caídas: {gameManager.nivel1_Caidas}";
        if (txtCaidasNivel2 != null)
            txtCaidasNivel2.text = $"Caídas: {gameManager.nivel2_Caidas}";
        if (txtCaidasNivel3 != null)
            txtCaidasNivel3.text = $"Caídas: {gameManager.nivel3_Caidas}";
    }

    public void MostrarEstadisticasFinales()
    {
        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(true);
        if (panelHUD != null)
            panelHUD.SetActive(false);
    }

    public void MostrarHUD()
    {
        if (panelHUD != null)
            panelHUD.SetActive(true);
        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(false);
    }
}
