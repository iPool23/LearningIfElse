using UnityEngine;

/// <summary>
/// CountOnCorrect - Registra un acierto cuando el jugador pisa un bloque correcto.
///
/// NOTA DE ARQUITECTURA (CharacterController):
///   El player usa CharacterController, que NO dispara OnCollisionEnter ni OnTriggerEnter
///   a menos que el otro objeto tenga un Rigidbody. Por eso usamos Update() con
///   distancia como método principal de detección, y los eventos de física como
///   respaldo adicional.
///
///   BUGS CORREGIDOS vs versión anterior:
///   1. bloqueID usa el nombre del bloque PADRE (no "TriggerZone" que era igual en todos).
///   2. bloqueRenderer busca en el padre y en los hijos con GetComponentInChildren.
///   3. El Update ya no llama FindGameObjectWithTag cada frame (cacheado en Start).
/// </summary>
public class CountOnCorrect : MonoBehaviour
{
    private GameRespawn gameManager;
    private bool yaContado = false;
    private Color? colorOriginal = null;

    // ID único: siempre apunta al bloque padre (no al TriggerZone)
    private string bloqueID;

    // Renderer del bloque visible (buscado en padre o en hijos)
    private Renderer bloqueRenderer;

    // Referencia cacheada al jugador para no buscarla cada frame
    private Transform playerTransform;

    // Distancia de detección (ajustable)
    [Tooltip("Distancia máxima para detectar que el jugador está sobre el bloque")]
    public float distanciaDeteccion = 1.2f;

    // ─── CICLO DE VIDA ───────────────────────────────────────────────────────────

    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();

        // Cachear referencia al jugador desde el inicio
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
            playerTransform = playerGO.transform;
        else
            Debug.LogWarning("[CountOnCorrect] No se encontró GameObject con tag 'Player'");

        // ── Resolver bloqueID y bloqueRenderer ──────────────────────────────────
        // Si este objeto es el TriggerZone hijo, el bloque real es el padre
        bool esTriggerZone = gameObject.name.StartsWith("TriggerZone") && transform.parent != null;

        if (esTriggerZone)
        {
            // Usar el nombre y Renderer del bloque padre
            bloqueID       = transform.parent.name;
            bloqueRenderer = ObtenerRenderer(transform.parent.gameObject);
        }
        else
        {
            bloqueID       = gameObject.name;
            bloqueRenderer = ObtenerRenderer(gameObject);
        }

        Debug.Log($"[CountOnCorrect] Init '{gameObject.name}' → id='{bloqueID}' renderer={(bloqueRenderer != null ? bloqueRenderer.gameObject.name : "NULL")}");
    }

    /// <summary>
    /// Busca un Renderer en el objeto o en sus hijos (para prefabs con mesh en child).
    /// </summary>
    Renderer ObtenerRenderer(GameObject go)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r == null) r = go.GetComponentInChildren<Renderer>();
        return r;
    }

    // ─── DETECCIÓN PRINCIPAL (Update) ────────────────────────────────────────────
    // Necesario porque CharacterController no genera eventos de física estándar
    // a menos que el bloque tenga un Rigidbody.

    void Update()
    {
        if (yaContado || playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < distanciaDeteccion)
        {
            Debug.Log($"[CountOnCorrect] Jugador a {dist:F2}m de '{gameObject.name}' → Registrando acierto");
            RegistrarAcierto();
        }
    }

    // ─── DETECCIÓN SECUNDARIA (Física) ───────────────────────────────────────────
    // Solo funcionan si el player tiene Rigidbody, pero los dejamos como respaldo.

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaContado)
        {
            Debug.Log($"[CountOnCorrect] OnTriggerEnter en '{gameObject.name}'");
            RegistrarAcierto();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !yaContado)
        {
            Debug.Log($"[CountOnCorrect] OnCollisionEnter en '{gameObject.name}'");
            RegistrarAcierto();
        }
    }

    // ─── LÓGICA DE ACIERTO ───────────────────────────────────────────────────────

    void RegistrarAcierto()
    {
        // El HashSet en GameRespawn evita doble conteo aunque tanto el bloque
        // como su TriggerZone llamen aquí con el mismo bloqueID
        if (gameManager != null && gameManager.BloqueYaContado(bloqueID))
        {
            Debug.Log($"[CountOnCorrect] '{bloqueID}' ya contado en sesión. Skip.");
            yaContado = true;
            return;
        }

        yaContado = true;

        if (gameManager != null)
        {
            gameManager.RegistrarBloqueContado(bloqueID);
            gameManager.RegistrarSaltoExitoso();
            gameManager.manipulacionesInteractivas++;
            Debug.Log($"[CountOnCorrect] ✅ Acierto registrado → '{bloqueID}'");
        }

        MostrarEfectoVerde();
        MarcarComoUsado();
    }

    // ─── EFECTOS VISUALES ────────────────────────────────────────────────────────

    void MostrarEfectoVerde()
    {
        if (bloqueRenderer == null)
        {
            Debug.LogWarning($"[CountOnCorrect] No hay Renderer para efecto verde en '{bloqueID}'");
            return;
        }

        if (colorOriginal == null)
            colorOriginal = bloqueRenderer.material.color;

        bloqueRenderer.material.color = Color.green;
        StartCoroutine(RestaurarColor());

        AudioSource audio = bloqueRenderer.GetComponent<AudioSource>();
        if (audio != null) audio.Play();

        Debug.Log($"[CountOnCorrect] ✨ Verde en '{bloqueRenderer.gameObject.name}'");
    }

    System.Collections.IEnumerator RestaurarColor()
    {
        yield return new WaitForSeconds(0.8f);
        if (bloqueRenderer != null && colorOriginal != null)
            bloqueRenderer.material.color = colorOriginal.Value;
    }

    void MarcarComoUsado()
    {
        if (bloqueRenderer == null) return;

        if (!bloqueRenderer.gameObject.name.Contains("✓"))
            bloqueRenderer.gameObject.name += " ✓USADO";

        // Oscurecer para indicar que ya fue usado
        Color c = bloqueRenderer.material.color;
        bloqueRenderer.material.color = new Color(c.r * 0.75f, c.g * 0.75f, c.b * 0.75f, c.a);
    }

    // ─── RESET ───────────────────────────────────────────────────────────────────

    public void ResetearContador()
    {
        yaContado = false;

        if (bloqueRenderer != null)
        {
            bloqueRenderer.gameObject.name = bloqueRenderer.gameObject.name.Replace(" ✓USADO", "");
            if (colorOriginal != null)
                bloqueRenderer.material.color = colorOriginal.Value;
        }

        Debug.Log($"[CountOnCorrect] 🔄 Reseteado '{bloqueID}'");
    }
}
