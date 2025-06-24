using UnityEngine;

public class BlockSpawner_Simple : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab;
    public int rows = 5;
    public int columns = 2;
    public float spacing = 2.7f;    [Header("Texture Logic - Simple IF")]
    public Texture correctTexture; // La textura correcta
    public Texture wrongTexture;   // La textura incorrecta

    [Header("Texture Settings")]
    public Vector2 textureScale = new Vector2(2f, 2f); // Escala de repetición de textura

    [Header("Nivel y Dificultad")]
    public int nivelAsociado = 1; // Para registrar métricas por nivel

    [Header("Contenedor de Bloques")]
    public Transform blockContainer; // Para aplicar rotaciones y transformaciones
    private GameRespawn gameManager;
    private bool nivelCompletado = false; // Para evitar múltiples detecciones

    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();
        GenerateSimpleConditionalBlocks();
    }
    void GenerateSimpleConditionalBlocks()
    {
        // Si no hay contenedor especificado, usar este objeto
        Transform container = blockContainer != null ? blockContainer : transform;

        for (int row = 0; row < rows; row++)
        {
            // Decidir aleatoriamente cuál columna tendrá la textura correcta
            int correctColumnIndex = Random.Range(0, columns);

            for (int col = 0; col < columns; col++)
            {
                // Calcular posición local
                Vector3 localPosition = new Vector3(
                    col * spacing - (columns - 1) * spacing / 2f,
                    0,
                    row * spacing
                );

                // Crear bloque como hijo del contenedor
                GameObject block = Instantiate(blockPrefab, container);
                block.transform.localPosition = localPosition;

                var renderer = block.GetComponent<Renderer>();
                var collider = block.GetComponent<Collider>();                if (col == correctColumnIndex)
                {
                    // BLOQUE CORRECTO: SI tiene la textura correcta → Es seguro
                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = correctTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = false; // Sólido para caminar encima
                    block.name = $"CorrectBlock_Row{row}_Col{col}";

                    // Agregar componente para contar aciertos
                    if (block.GetComponent<CountOnCorrect>() == null)
                    {
                        block.AddComponent<CountOnCorrect>();
                    }

                    // Crear un trigger adicional para detectar cuando el jugador está encima
                    GameObject triggerZone = new GameObject("TriggerZone");
                    triggerZone.transform.SetParent(block.transform);
                    triggerZone.transform.localPosition = Vector3.up * 0.6f; // Ligeramente arriba del bloque
                    triggerZone.transform.localScale = Vector3.one;
                    
                    // Agregar BoxCollider como trigger
                    BoxCollider triggerCollider = triggerZone.AddComponent<BoxCollider>();
                    triggerCollider.isTrigger = true;
                    triggerCollider.size = new Vector3(2f, 0.5f, 2f); // Zona de detección
                    
                    // Agregar el componente CountOnCorrect al trigger también
                    triggerZone.AddComponent<CountOnCorrect>();

                }                else
                {
                    // BLOQUE INCORRECTO: NO tiene la textura correcta → Se destruye
                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = wrongTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = true;

                    // Asegurar que tiene el componente DestroyOnTrigger                    if (block.GetComponent<DestroyOnTrigger>() == null)
                    {
                        block.AddComponent<DestroyOnTrigger>();
                    }

                    block.name = $"WrongBlock_Row{row}_Col{col}";

                }

                // Agregar etiqueta de Parent para organización
                block.transform.SetParent(transform);
            }
        }

        // Registrar que se generaron los bloques
        if (gameManager != null)
        {
            gameManager.comandosIniciados++;
            Debug.Log($"Nivel {nivelAsociado} generado: {rows} filas, {columns} columnas");
        }
    }
    /// <summary>
    /// Método para regenerar los bloques (útil para reintentos)
    /// </summary>
    public void RegenerarBloques()
    {        // Destruir bloques existentes
        Transform container = blockContainer != null ? blockContainer : transform;

        foreach (Transform child in container)
        {
            if (child != container)
            {
                // Resetear contadores si existe el componente
                var correctCounter = child.GetComponent<CountOnCorrect>();
                if (correctCounter != null)
                {
                    correctCounter.ResetearContador();
                }

                var destroyTrigger = child.GetComponent<DestroyOnTrigger>();
                if (destroyTrigger != null)
                {
                    // También podríamos resetear contadores aquí si fuera necesario
                }

                Destroy(child.gameObject);
            }
        }
        // Generar nuevos bloques
        GenerateSimpleConditionalBlocks();

        // Resetear bandera de completado
        nivelCompletado = false;

        if (gameManager != null)
        {
            gameManager.reintentos_Nivel++;
        }
    }
    /// <summary>
    /// Verificar si el jugador ha llegado al final del nivel
    /// </summary>
    void Update()
    {
        if (gameManager != null)
        {
            // Verificar si el jugador llegó al final
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Verificar si el jugador está sobre el último bloque (última fila)
                bool playerOnFinalBlock = false;

                if (blockContainer != null)
                {
                    Vector3 localPlayerPos = blockContainer.transform.InverseTransformPoint(player.transform.position);
                    float finalRowZ = (rows - 1) * spacing;

                    // Verificar si está en la última fila (con un pequeño margen de tolerancia)
                    if (localPlayerPos.z >= finalRowZ - 1.0f && localPlayerPos.z <= finalRowZ + 1.5f)
                    {
                        playerOnFinalBlock = true;
                    }
                }
                else
                {
                    // Usar coordenadas globales
                    float finalRowZ = transform.position.z + (rows - 1) * spacing;

                    if (player.transform.position.z >= finalRowZ - 1.0f && player.transform.position.z <= finalRowZ + 1.5f)
                    {
                        playerOnFinalBlock = true;
                    }
                }
                // Condición más flexible para completar el nivel
                bool puedeCompletar = playerOnFinalBlock && !nivelCompletado &&
                                    (gameManager.nivelActual == nivelAsociado ||
                                    (nivelAsociado == 1 && gameManager.nivelActual <= 1));

                if (puedeCompletar)
                {
                    nivelCompletado = true; // Marcar como completado para evitar múltiples llamadas
                    // ¡Nivel completado!
                    Debug.Log($"¡Nivel {nivelAsociado} (IF Simple) completado!");

                    // Registrar completion del nivel ANTES del teletransporte
                    gameManager.CompletarNivelActual();

                    // Teletransportar al jugador a las coordenadas especificadas con delay
                    StartCoroutine(TeletransportarConDelay(player));
                }
            }
            else
            {
                Debug.LogWarning("No se encontró jugador con tag 'Player'");
            }
        }
    }
    /// <summary>
    /// Teletransportar con delay para evitar problemas
    /// </summary>
    System.Collections.IEnumerator TeletransportarConDelay(GameObject player)
    {
        Debug.Log("Iniciando teletransporte...");
        yield return new WaitForSeconds(0.5f); // Pequeño delay

        if (player != null)
        {
            Debug.Log("Teletransportando jugador a spawn point...");

            // Desactivar física temporalmente
            Rigidbody rb = player.GetComponent<Rigidbody>();
            CharacterController cc = player.GetComponent<CharacterController>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // Temporalmente kinematic
            }

            if (cc != null)
            {
                cc.enabled = false; // Desactivar temporalmente
            }

            // Teletransportar
            player.transform.position = new Vector3(-11.804f, 1.022f, -0.238f);
            player.transform.rotation = Quaternion.identity; // Resetear rotación también

            yield return new WaitForEndOfFrame(); // Esperar un frame

            // Reactivar física
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (cc != null)
            {
                cc.enabled = true;
            }

            Debug.Log($"Jugador teletransportado exitosamente a {player.transform.position}");
        }
    }
}