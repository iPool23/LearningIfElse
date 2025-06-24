using UnityEngine;

/// <summary>
/// Script principal que inicializa y coordina todos los sistemas del juego
/// Debe ir en un GameObject vacío en la escena llamado "GameManager"
/// </summary>
public class GameController : MonoBehaviour
{    [Header("=== REFERENCIAS DEL SISTEMA ===")]
    public GameRespawn gameRespawn;
    public GameObject player;

    [Header("=== SPAWNERS POR NIVEL ===")]
    public BlockSpawner_Simple spawnerNivel1;
    public GameObject spawnerNivel2; // BlockSpawner_Double
    public GameObject spawnerNivel3; // BlockSpawner_Nested

    [Header("=== BARRERAS ENTRE NIVELES ===")]
    public BarreraNivel barreraNivel2;
    public BarreraNivel barreraNivel3;

    [Header("=== CONFIGURACIÓN INICIAL ===")]
    public bool iniciarConTutorial = true;
    public bool modoDebug = false;

    void Start()
    {
        InicializarSistemas();
        ConfigurarNiveles();

        if (modoDebug)
        {
            MostrarInfoDebug();
        }
    }    void InicializarSistemas()
    {        // Verificar que todas las referencias estén asignadas
        if (gameRespawn == null)
            gameRespawn = FindFirstObjectByType<GameRespawn>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("Sistemas inicializados correctamente");
    }

    void ConfigurarNiveles()
    {
        // Configurar spawners por nivel
        if (spawnerNivel1 != null)
        {
            // El nivel 1 siempre está activo
            spawnerNivel1.gameObject.SetActive(true);
        }

        if (spawnerNivel2 != null)
        {
            // El nivel 2 inicialmente inactivo
            spawnerNivel2.SetActive(false);
        }

        if (spawnerNivel3 != null)
        {
            // El nivel 3 inicialmente inactivo
            spawnerNivel3.SetActive(false);
        }

        // Configurar barreras
        if (barreraNivel2 != null)
        {
            barreraNivel2.ConfigurarBarrera(2, "Completa el Nivel 1 para continuar");
        }

        if (barreraNivel3 != null)
        {
            barreraNivel3.ConfigurarBarrera(3, "Completa el Nivel 2 para continuar");
        }
    }

    void Update()
    {
        // Manejar cambios de nivel
        if (gameRespawn != null)
        {
            ManejarCambioNivel();
        }

        // Inputs para debug (solo en editor)
        if (modoDebug && Application.isEditor)
        {
            ManejarInputsDebug();
        }
    }

    void ManejarCambioNivel()
    {
        // Activar/desactivar spawners según el nivel actual
        if (spawnerNivel1 != null)
            spawnerNivel1.gameObject.SetActive(gameRespawn.nivelActual == 1);

        if (spawnerNivel2 != null)
            spawnerNivel2.SetActive(gameRespawn.nivelActual == 2);

        if (spawnerNivel3 != null)
            spawnerNivel3.SetActive(gameRespawn.nivelActual == 3);
    }

    void ManejarInputsDebug()
    {
        // Teclas para debug (solo en editor)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CambiarANivel(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CambiarANivel(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CambiarANivel(3);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarNivelActual();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (gameRespawn != null)
            {
                gameRespawn.IniciarTutorial();
            }
        }
    }

    public void CambiarANivel(int nivel)
    {
        if (gameRespawn != null && nivel >= 1 && nivel <= gameRespawn.maxNivel)
        {
            if (modoDebug || gameRespawn.PuedeAccederNivel(nivel))
            {
                gameRespawn.nivelActual = nivel;
                Debug.Log($"Cambiando a nivel {nivel}");
            }
            else
            {
                Debug.Log($"No se puede acceder al nivel {nivel}");
            }
        }
    }

    public void ReiniciarNivelActual()
    {
        if (gameRespawn != null)
        {
            gameRespawn.ReintentarNivel();
        }
    }

    void MostrarInfoDebug()
    {
        Debug.Log("=== MODO DEBUG ACTIVADO ===");
        Debug.Log("Controles de debug:");
        Debug.Log("1, 2, 3 - Cambiar nivel");
        Debug.Log("R - Reiniciar nivel");
        Debug.Log("T - Mostrar tutorial");
        Debug.Log("========================");
    }

    #region Métodos Públicos para VR/UI

    public void BotonNivel1()
    {
        CambiarANivel(1);
    }

    public void BotonNivel2()
    {
        CambiarANivel(2);
    }

    public void BotonNivel3()
    {
        CambiarANivel(3);
    }    public void MostrarEstadisticas()
    {
        if (gameRespawn != null)
        {
            Debug.Log("Mostrando estadísticas - delegando a GameRespawn");
            // Las estadísticas se muestran a través del panel personalizado del usuario
            // que está conectado directamente con GameRespawn
        }
    }

    public void GuardarProgreso()
    {
        if (gameRespawn != null)
        {
            gameRespawn.GuardarEstadisticas();
        }
    }

    #endregion

    void OnApplicationPause(bool pauseStatus)
    {
        // Guardar automáticamente cuando la aplicación se pausa
        if (pauseStatus)
        {
            GuardarProgreso();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Guardar cuando la aplicación pierde foco
        if (!hasFocus)
        {
            GuardarProgreso();
        }
    }
}
