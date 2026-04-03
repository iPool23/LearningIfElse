using UnityEngine;

public class BlockSpawner_Nested : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab;
    public int rows = 5;
    public int columns = 2;
    public float spacing = 2.7f;    [Header("Texture Logic - Nested IF")]
    public Texture redCrystalTexture;      // Rojo + símbolo = SEGURO
    public Texture redNoSymbolTexture;     // Rojo sin símbolo = PELIGROSO (única peligrosa)
    public Texture blueCrystalTexture;     // Azul = SEGURO
    public Texture greenCrystalTexture;    // Verde = SEGURO

    [Header("Texture Settings")]
    public Vector2 textureScale = new Vector2(2f, 2f); // Escala de repetición de textura

    [Header("Nivel y Dificultad")]
    public int nivelAsociado = 3; // Para registrar métricas por nivel

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
            Debug.Log($"BlockSpawner_Nested iniciado. Nivel asociado: {nivelAsociado}, GameManager nivel actual: {gameManager.nivelActual}");
        }

        GenerateNestedConditionalBlocks();
    }    void GenerateNestedConditionalBlocks()
    {
        // Si no hay contenedor especificado, usar este objeto
        Transform container = blockContainer != null ? blockContainer : transform;

        // Array de texturas seguras (3 de 4)
        Texture[] safeTextures = { redCrystalTexture, blueCrystalTexture, greenCrystalTexture };
        // Única textura peligrosa
        Texture dangerTexture = redNoSymbolTexture;

        // Lista para garantizar que todas las texturas aparezcan al menos una vez
        System.Collections.Generic.List<Texture> texturesUsed = new System.Collections.Generic.List<Texture>();
        
        for (int row = 0; row < rows; row++)
        {
            // Garantizar que al menos uno por fila sea seguro
            int safeColumnIndex = Random.Range(0, columns);
            // Garantizar que al menos uno por fila sea peligroso
            int dangerColumnIndex = (safeColumnIndex == 0) ? 1 : 0; // La otra columna

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
                var collider = block.GetComponent<Collider>();

                Texture selectedTexture;

                // LÓGICA PARA GARANTIZAR QUE TODAS LAS TEXTURAS APAREZCAN:
                if (col == safeColumnIndex)
                {
                    // BLOQUE GARANTIZADO SEGURO
                    // En las primeras 3 filas, forzar que aparezcan las 3 texturas seguras
                    if (row < 3)
                    {
                        selectedTexture = safeTextures[row]; // Fila 0->Roja, Fila 1->Azul, Fila 2->Verde
                        texturesUsed.Add(selectedTexture);
                    }
                    else
                    {
                        // En las filas restantes, usar cualquier textura segura
                        selectedTexture = safeTextures[Random.Range(0, safeTextures.Length)];
                    }

                    if (renderer != null)
                    {
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = selectedTexture;
                        newMaterial.mainTextureScale = textureScale;
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = false; // Sólido
                    block.name = $"SafeBlock_Row{row}_Col{col}";

                    if (block.GetComponent<CountOnCorrect>() == null)
                    {
                        block.AddComponent<CountOnCorrect>();
                    }
                }
                else if (col == dangerColumnIndex)
                {
                    // BLOQUE GARANTIZADO PELIGROSO: En cada fila debe haber al menos uno
                    selectedTexture = dangerTexture;
                    texturesUsed.Add(dangerTexture);

                    if (renderer != null)
                    {
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = selectedTexture;
                        newMaterial.mainTextureScale = textureScale;
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = true;

                    if (block.GetComponent<DestroyOnTrigger>() == null)
                    {
                        block.AddComponent<DestroyOnTrigger>();
                    }

                    block.name = $"DangerBlock_Row{row}_Col{col}";
                }

                // Agregar etiqueta de Parent para organización
                block.transform.SetParent(transform);
            }
        }

        // Verificar que todas las texturas hayan aparecido al menos una vez
        Debug.Log($"Texturas utilizadas en el nivel: {texturesUsed.Count}");
        foreach (var texture in texturesUsed)
        {
            Debug.Log($"- Textura usada: {texture.name}");
        }

        // Registrar que se generaron los bloques
        if (gameManager != null)
        {
            gameManager.comandosIniciados++;
            Debug.Log($"Nivel {nivelAsociado} (Anidado) generado: {rows} filas, {columns} columnas");
            Debug.Log("REGLA ANIDADA: Rojo con símbolo + Azul + Verde = SEGURO | Rojo sin símbolo = PELIGROSO");
            Debug.Log("GARANTÍA: Las 3 texturas seguras aparecen al menos 1 vez Y la peligrosa aparece en TODAS las filas");
        }
    }/// <summary>
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
        GenerateNestedConditionalBlocks();

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
                                    (nivelAsociado == 3 && (gameManager.nivelActual == 2 || gameManager.nivelActual == 3)));

                if (puedeCompletar)
                {
                    nivelCompletado = true; // Marcar como completado para evitar múltiples llamadas
                    // ¡Nivel completado!
                    Debug.Log($"¡Nivel {nivelAsociado} (Anidado) completado!");

                    // Teletransportar al jugador y completar el nivel (esto terminará el juego)
                    StartCoroutine(TeletransportarYCompletarJuego(player));
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
    }
    /// <summary>
    /// Teletransportar y completar el juego mostrando todos los datos recolectados
    /// </summary>
    System.Collections.IEnumerator TeletransportarYCompletarJuego(GameObject player)
    {
        Debug.Log("Iniciando teletransporte final del juego...");
        yield return new WaitForSeconds(0.2f); // Delay más corto

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

            Debug.Log($"Jugador teletransportado exitosamente a {player.transform.position}");            // AHORA SÍ completar el nivel
            yield return new WaitForSeconds(0.3f); // Pequeño delay adicional

            if (gameManager != null)
            {
                Debug.Log("Completando nivel final y terminando juego...");
                gameManager.CompletarNivelActual();
            }
        }
    }
}
