using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LearningIfElse.Framework.Analysis;
using LearningIfElse.Framework.StateManagement;
using LearningIfElse.Cloud.FirebaseSync;
using LearningIfElse.Framework.PlayerSystems;

/// <summary>
/// GameRespawn - Coordinator (Senior Architecture).
/// Actúa como el centro neurálgico que delega el trabajo a componentes especializados.
/// </summary>
public class GameRespawn : MonoBehaviour
{
    [Header("=== COMPONENTES DE DOMINIO ===")]
    private AcademicAnalysis _analysis;
    private ProgressController _progress;
    private CloudGateway _cloud;
    private PlayerInteraction _playerSystems;

    [Header("=== CONFIGURACIÓN DE SESIÓN ===")]
    public string username = "Estudiante_VR";
    public string sessionId => _cloud != null ? _cloud.sessionId : "";

    [Header("=== VARIABLES DE SESIÓN (TRACKING) ===")]
    public int aciertos = 0;
    public int errores = 0;
    public int interaccionesTotales => aciertos + errores;
    public float puntajeObtenido = 0f;

    [Header("=== TIEMPOS POR NIVEL ===")]
    public float nivel1_Tiempo = 0f;
    public float nivel2_Tiempo = 0f;
    public float nivel3_Tiempo = 0f;
    private float _tiempoInicioNivel;

    [Header("=== ESTADÍSTICAS POR NIVEL ===")]
    public int nivel1_Saltos_Correctos = 0;
    public int nivel1_Caidas = 0;
    public int nivel1_Saltos_Totales => nivel1_Saltos_Correctos + nivel1_Caidas;
    public int nivel2_Saltos_Correctos = 0;
    public int nivel2_Caidas = 0;
    public int nivel2_Saltos_Totales => nivel2_Saltos_Correctos + nivel2_Caidas;
    public int nivel3_Saltos_Correctos = 0;
    public int nivel3_Caidas = 0;
    public int nivel3_Saltos_Totales => nivel3_Saltos_Correctos + nivel3_Caidas;

    [Header("=== RETOS Y ESTADO ===")]
    public int comandosIniciados = 0;
    public int reintentos_Nivel = 0;
    public float tiempoTotal_Sesion => nivel1_Tiempo + nivel2_Tiempo + nivel3_Tiempo;

    [Header("=== UI REFS ===")]
    public TextMeshProUGUI uiNivelActual;
    public TextMeshProUGUI uiPuntaje;
    public GameObject panelBarrera;

    [Header("=== INTERACCIONES VR (COMPATIBILIDAD) ===")]
    public int teleports_Realizados = 0;
    public int dudas_Expresadas = 0;
    public int manipulacionesInteractivas = 0;

    // Propiedades para compatibilidad con scripts antiguos
    public int nivelActual => _progress != null ? _progress.nivelActual : 1;
    public int maxNivel => _progress != null ? _progress.maxNivel : 3;

    public bool IsNivelCompletado(int nivel) => _progress != null && _progress.IsNivelCompletado(nivel);

    // Bloques ya contados en la sesión (Para evitar duplicados)
    private HashSet<string> _bloquesContados = new HashSet<string>();

    void Awake()
    {
        // Inicialización de componentes (pueden estar en el mismo objeto o diferentes)
        _analysis = GetComponent<AcademicAnalysis>() ?? gameObject.AddComponent<AcademicAnalysis>();
        _progress = GetComponent<ProgressController>() ?? gameObject.AddComponent<ProgressController>();
        _cloud = GetComponent<CloudGateway>() ?? gameObject.AddComponent<CloudGateway>();
        _playerSystems = GetComponent<PlayerInteraction>() ?? gameObject.AddComponent<PlayerInteraction>();
    }

    void Start()
    {
        _tiempoInicioNivel = Time.time;
        ActualizarUI();
    }

    void FixedUpdate()
    {
        // Delegar física de respawn
        if (transform.position.y < _playerSystems.threshold)
        {
            RegistrarCaida();
            _playerSystems.Respawn(gameObject);
        }
    }

    void Update()
    {
        ActualizarUI();
    }

    #region Lógica de Progresión

    public void IniciarTiempoNivel()
    {
        _tiempoInicioNivel = Time.time;
        Debug.Log($"[GameMaster] Timer de nivel {nivelActual} iniciado.");
    }

    public bool BloqueYaContado(string id) => _bloquesContados.Contains(id);
    public void RegistrarBloqueContado(string id) => _bloquesContados.Add(id);

    public void RegistrarSaltoExitoso()
    {
        aciertos++;
        RegistrarEstadisticaNivel(true);
        CalcularPuntaje();
        Debug.Log($"[GameMaster] Salto exitoso. Total: {aciertos}");
    }

    public void RegistrarCaida()
    {
        errores++;
        RegistrarEstadisticaNivel(false);
        _playerSystems.ReproducirSonidoCaida();
        Debug.Log($"[GameMaster] Caída registrada. Total: {errores}");
    }

    private void RegistrarEstadisticaNivel(bool esAcierto)
    {
        switch (nivelActual)
        {
            case 1: if (esAcierto) nivel1_Saltos_Correctos++; else nivel1_Caidas++; break;
            case 2: if (esAcierto) nivel2_Saltos_Correctos++; else nivel2_Caidas++; break;
            case 3: if (esAcierto) nivel3_Saltos_Correctos++; else nivel3_Caidas++; break;
        }
    }

    public void CompletarNivelActual()
    {
        float tiempoGastado = Time.time - _tiempoInicioNivel;
        RegistrarTiempoNivel(nivelActual, tiempoGastado);
        
        _progress.CompletarNivel(nivelActual);

        if (!_progress.JuegoTerminado)
        {
            _progress.SetNivelActual(nivelActual + 1);
            _tiempoInicioNivel = Time.time;
        }
        else
        {
            FinalizarSesión();
        }
    }

    private void RegistrarTiempoNivel(int nivel, float tiempo)
    {
        switch (nivel)
        {
            case 1: nivel1_Tiempo = tiempo; break;
            case 2: nivel2_Tiempo = tiempo; break;
            case 3: nivel3_Tiempo = tiempo; break;
        }
    }

    private void FinalizarSesión()
    {
        Debug.Log("[GameMaster] Sesión finalizada. Sincronizando con la nube...");
        PrepararYEnviarDatos();
    }

    #endregion

    #region Cálculos y Sincronización

    void CalcularPuntaje()
    {
        puntajeObtenido = _analysis.CalcularRendimiento(aciertos, interaccionesTotales);
    }

    void PrepararYEnviarDatos()
    {
        var data = new LearningIfElse.Cloud.FirebaseSync.EstadisticasSesion {
            sessionId = sessionId,
            username = username,
            timestamp = DateTime.UtcNow.ToString("o"),
            puntaje = puntajeObtenido,
            aciertos = aciertos,
            errores = errores,
            tiempoTotal = nivel1_Tiempo + nivel2_Tiempo + nivel3_Tiempo,
            nivel1 = new LearningIfElse.Cloud.FirebaseSync.NivelStats { saltosCorrectos = nivel1_Saltos_Correctos, caidas = nivel1_Caidas, tiempo = nivel1_Tiempo },
            nivel2 = new LearningIfElse.Cloud.FirebaseSync.NivelStats { saltosCorrectos = nivel2_Saltos_Correctos, caidas = nivel2_Caidas, tiempo = nivel2_Tiempo },
            nivel3 = new LearningIfElse.Cloud.FirebaseSync.NivelStats { saltosCorrectos = nivel3_Saltos_Correctos, caidas = nivel3_Caidas, tiempo = nivel3_Tiempo }
        };

        _cloud.SubirEstadisticas(JsonUtility.ToJson(data));
    }

    #endregion

    void ActualizarUI()
    {
        if (uiNivelActual != null) uiNivelActual.text = $"Nivel: {nivelActual}";
        if (uiPuntaje != null) uiPuntaje.text = $"Aciertos: {aciertos} | Puntaje: {puntajeObtenido:F0}";
    }

    // Métodos de compatibilidad con Barreras y Spawners
    public bool PuedeAccederNivel(int nivel) => _progress.PuedeAccederNivel(nivel);

    // --- PROPIEDADES DE SUMA GLOBAL POR NIVELES ---
    public int TotalSaltosTotalesPorNivel
    {
        get { return RegistrarSaltoCorrectoTotal(); }
    }
    public int TotalSaltosCorrectosPorNivel => nivel1_Saltos_Correctos + nivel2_Saltos_Correctos + nivel3_Saltos_Correctos;
    public int TotalCaidasPorNivel => nivel1_Caidas + nivel2_Caidas + nivel3_Caidas;

    private int RegistrarSaltoCorrectoTotal()
    {
        // En la versión simplificada, sumamos los contadores locales que ya tenemos
        return nivel1_Saltos_Correctos + nivel2_Saltos_Correctos + nivel3_Saltos_Correctos;
    }

    /// <summary>
    /// Teletransporta al jugador al área final.
    /// </summary>
    public void TeletransportarAFinal()
    {
        _playerSystems.Teletransportar(gameObject, new Vector3(-11.804f, 17.612f, -5.92f));
        Debug.Log("[GameMaster] Teletransporte a zona final completado.");
    }
}
