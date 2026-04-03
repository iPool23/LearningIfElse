using UnityEngine;

public class BlockSpawner_Double : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab;
    public int rows = 5;
    public int columns = 2;
    public float spacing = 2.7f;    [Header("Texture Logic - IF-ELSE")]
    public Texture safeTexture;     // Si hay gato → seguro
    public Texture dangerTexture;   // Si hay perro → peligroso

    [Header("Texture Settings")]
    public Vector2 textureScale = new Vector2(2f, 2f); // Escala de repetición de textura

    [Header("Nivel y Dificultad")]
    public int nivelAsociado = 2; // Para registrar métricas por nivel

    [Header("Contenedor de Bloques")]
    public Transform blockContainer; // Para aplicar rotaciones y transformaciones

    private GameRespawn gameManager;
    private bool nivelCompletado = false; // Para evitar múltiples detecciones
    void Start()
    {
        gameManager = FindFirstObjectByType<GameRespawn>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado! El teletransporte no funcionará.");
        }
        else
        {
            Debug.Log($"BlockSpawner_Double iniciado. Nivel asociado: {nivelAsociado}, GameManager nivel actual: {gameManager.nivelActual}");
        }

        GenerateDoubleConditionalBlocks();
    }
    void GenerateDoubleConditionalBlocks()
    {
        // Si no hay contenedor especificado, usar este objeto
        Transform container = blockContainer != null ? blockContainer : transform;

        for (int row = 0; row < rows; row++)
        {
            // En IF-ELSE, garantizar que haya exactamente UN bloque correcto por fila
            // Decidir aleatoriamente cuál columna será la segura (gato)
            int safeColumnIndex = Random.Range(0, columns);

            for (int col = 0; col < columns; col++)
            {
                // Calcular posición local
                Vector3 localPosition = new Vector3(
                    col * spacing - (columns - 1) * spacing / 2f,
                    0,
                    row * spacing
                );                // Crear bloque como hijo del contenedor
                GameObject block = Instantiate(blockPrefab, container);
                block.transform.localPosition = localPosition;

                var renderer = block.GetComponent<Renderer>();
                var collider = block.GetComponent<Collider>();

                // LÓGICA IF-ELSE CORRECTA:
                // IF (col == safeColumnIndex) THEN bloque_seguro (gato)
                // ELSE bloque_peligroso (perro)
                if (col == safeColumnIndex)
                {
                    // IF: Es la columna segura → Poner gato (seguro)
                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = safeTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }                    collider.isTrigger = false; // Sólido
                    block.name = $"SafeBlock_Cat_Row{row}_Col{col}";

                    // Agregar componente para contar aciertos
                    if (block.GetComponent<CountOnCorrect>() == null)
                    {
                        block.AddComponent<CountOnCorrect>();
                    }
                }
                else
                {
                    // ELSE: No es la columna segura → Poner perro (peligroso)
                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = dangerTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = true;

                    // Asegurar que tiene el componente DestroyOnTrigger
                    if (block.GetComponent<DestroyOnTrigger>() == null)
                    {
                        block.AddComponent<DestroyOnTrigger>();
                    }

                    block.name = $"DangerBlock_Dog_Row{row}_Col{col}";

                }
                // Agregar etiqueta de Parent para organización
                block.transform.SetParent(transform);
            }
        }

        // Registrar que se generaron los bloques
        if (gameManager != null)
        {
            gameManager.comandosIniciados++;
            Debug.Log($"Nivel {nivelAsociado} (IF-ELSE) generado: {rows} filas, {columns} columnas");
        }
    }    /// <summary>
         /// Método para regenerar los bloques (útil para reintentos)
         /// </summary>
    public void RegenerarBloques()
    {
        // Destruir bloques existentes
        Transform container = blockContainer != null ? blockContainer : transform;

        foreach (Transform child in container)
        {
            if (child != container)
            {
                Destroy(child.gameObject);
            }
        }

        // Generar nuevos bloques
        GenerateDoubleConditionalBlocks();

        // Resetear bandera de completado
        nivelCompletado = false;

        if (gameManager != null)
        {
            gameManager.reintentos_Nivel++;
        }
    }    /// <summary>
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
                                    (nivelAsociado == 2 && (gameManager.nivelActual == 1 || gameManager.nivelActual == 2)));                if (puedeCompletar)
                {
                    nivelCompletado = true; // Marcar como completado para evitar múltiples llamadas
                    // ¡Nivel completado!
                    Debug.Log($"¡Nivel {nivelAsociado} (IF-ELSE) completado!");

                    // Teletransportar PRIMERO, luego completar nivel
                    StartCoroutine(TeletransportarYCompletarNivel(player));
                }
                else if (playerOnFinalBlock)
                {
                    // Debug adicional para ver por qué no se completa
                    Debug.Log($"Player on final block but not completing: GameManager nivel={gameManager.nivelActual}, nivel asociado={nivelAsociado}, completado={nivelCompletado}");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró jugador con tag 'Player'");
            }
        }
    }    /// <summary>
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
    }    /// <summary>
         /// Teletransportar y luego completar el nivel
         /// </summary>
    System.Collections.IEnumerator TeletransportarYCompletarNivel(GameObject player)
    {
        Debug.Log("Iniciando teletransporte y completado de nivel...");
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

            // AHORA completar el nivel DESPUÉS del teletransporte
            yield return new WaitForSeconds(0.3f);

            if (gameManager != null)
            {
                Debug.Log("Completando nivel después del teletransporte...");
                gameManager.CompletarNivelActual();

                // Completar la transición del nivel
                yield return new WaitForSeconds(0.5f);
                gameManager.CompletarTransicionNivel();
            }
        }
    }
}