using UnityEngine;

public class BlockSpawner_Nested : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab;
    public int rows = 5;
    public int columns = 2;
    public float spacing = 2.7f;    [Header("Texture Logic - Nested IF")]
    public Texture redCrystalTexture;      // Rojo + símbolo = SEGURO
    public Texture redNoSymbolTexture;     // Rojo sin símbolo = PELIGROSO
    public Texture blueCrystalTexture;     // Azul = PELIGROSO
    public Texture greenCrystalTexture;    // Verde = PELIGROSO

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
    }
    void GenerateNestedConditionalBlocks()
    {
        // Si no hay contenedor especificado, usar este objeto
        Transform container = blockContainer != null ? blockContainer : transform;

        for (int row = 0; row < rows; row++)
        {
            // Garantizar que al menos uno por fila sea seguro
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

                // LÓGICA ANIDADA CORRECTA:
                // IF (col == safeColumnIndex) THEN bloque_seguro (rojo con símbolo)
                // ELSE bloque_peligroso (rojo sin símbolo, azul o verde)
                if (col == safeColumnIndex)
                {
                    // BLOQUE SEGURO: Siempre rojo con símbolo
                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = redCrystalTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }                    collider.isTrigger = false; // Sólido
                    block.name = $"SafeBlock_RedSymbol_Row{row}_Col{col}";

                    // Agregar componente para contar aciertos
                    if (block.GetComponent<CountOnCorrect>() == null)
                    {
                        block.AddComponent<CountOnCorrect>();
                    }

                }
                else
                {
                    // BLOQUES PELIGROSOS: Pueden ser rojo sin símbolo, azul o verde
                    Texture[] dangerTextures = { redNoSymbolTexture, blueCrystalTexture, greenCrystalTexture };
                    Texture selectedTexture = dangerTextures[Random.Range(0, dangerTextures.Length)];

                    if (renderer != null)
                    {
                        // Crear una instancia del material para evitar modificar el original
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.mainTexture = selectedTexture;
                        newMaterial.mainTextureScale = textureScale; // Aplicar escala de textura
                        renderer.material = newMaterial;
                    }

                    collider.isTrigger = true;

                    // Asegurar que tiene el componente DestroyOnTrigger
                    if (block.GetComponent<DestroyOnTrigger>() == null)
                    {                        block.AddComponent<DestroyOnTrigger>();
                    }

                    block.name = $"DangerBlock_Row{row}_Col{col}";

                }

                // Agregar etiqueta de Parent para organización
                block.transform.SetParent(transform);
            }
        }

        // Registrar que se generaron los bloques
        if (gameManager != null)
        {
            gameManager.comandosIniciados++;
            Debug.Log($"Nivel {nivelAsociado} (Anidado) generado: {rows} filas, {columns} columnas");
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

            Debug.Log($"Jugador teletransportado exitosamente a {player.transform.position}");

            // AHORA SÍ completar el nivel
            yield return new WaitForSeconds(0.3f); // Pequeño delay adicional

            if (gameManager != null)
            {
                Debug.Log("Completando nivel final y terminando juego...");
                gameManager.CompletarNivelActual();

                // Mostrar todos los datos recolectados
                yield return new WaitForSeconds(1f); // Esperar un segundo antes de mostrar los datos
                MostrarDatosFinales();
            }
        }
    }

    /// <summary>
    /// Mostrar todos los datos recolectados durante el juego
    /// </summary>
    void MostrarDatosFinales()
    {
        if (gameManager == null) return;

        Debug.Log("=== JUEGO COMPLETADO - RESUMEN FINAL DE DATOS ===");

        // Variables independientes
        Debug.Log("=== VARIABLES INDEPENDIENTES ===");
        Debug.Log($"ÍNDICE DE INTERACCIÓN DEL USUARIO (IIU):");
        Debug.Log($"  - Comandos iniciados: {gameManager.comandosIniciados}");
        Debug.Log($"  - Gestos interpretados: {gameManager.gestosInterpretados}");
        Debug.Log($"  - Manipulaciones interactivas: {gameManager.manipulacionesInteractivas}");

        Debug.Log($"PRECISIÓN DE INTERACCIÓN (TPI):");
        Debug.Log($"  - Interacciones acertadas: {gameManager.interaccionesAcertadas}");
        Debug.Log($"  - Interacciones totales: {gameManager.interaccionesTotales}");
        Debug.Log($"  - Errores: {gameManager.errores}");
        Debug.Log($"  - Aciertos: {gameManager.aciertos}");

        Debug.Log($"FLUIDEZ DE NAVEGACIÓN (IFN):");
        Debug.Log($"  - Velocidad medida: {gameManager.velocidadMovimientoMedida:F2}");
        Debug.Log($"  - Velocidad inicial esperada: {gameManager.velocidadInicialEsperada:F2}");
        Debug.Log($"  - Velocidad estándar objetivo: {gameManager.velocidadEstandarObjetivo:F2}");

        // Variables dependientes
        Debug.Log("=== VARIABLES DEPENDIENTES ===");
        Debug.Log($"RENDIMIENTO:");
        Debug.Log($"  - Puntaje obtenido: {gameManager.puntajeObtenido:F2}");
        Debug.Log($"  - Puntaje máximo posible: {gameManager.puntajeMaximoPosible:F2}");
        Debug.Log($"  - Porcentaje de rendimiento: {gameManager.porcentajeRendimiento:F2}%");

        Debug.Log($"DOMINIO CONCEPTUAL:");
        Debug.Log($"  - Evaluación conceptual: {gameManager.evaluacionConceptual:F2}");
        Debug.Log($"  - Aplicación práctica: {gameManager.aplicacionPractica:F2}");
        Debug.Log($"  - Resolución de casos: {gameManager.resolucionCasos:F2}");

        Debug.Log($"SATISFACCIÓN DEL USUARIO:");
        Debug.Log($"  - Suma scores SUS: {gameManager.sumaScoresSUS:F2}");
        Debug.Log($"  - Número total preguntas: {gameManager.numeroTotalPreguntas}");

        // Variables específicas del juego
        Debug.Log("=== MECÁNICA DE SALTOS ===");
        Debug.Log($"  - Saltos correctos: {gameManager.saltos_Correctos}");
        Debug.Log($"  - Saltos incorrectos: {gameManager.saltos_Incorrectos}");
        Debug.Log($"  - Reintentos de nivel: {gameManager.reintentos_Nivel}");

        // Datos por nivel
        Debug.Log("=== NIVEL 1 (IF SIMPLE) ===");
        Debug.Log($"  - Saltos totales: {gameManager.nivel1_Saltos_Totales}");
        Debug.Log($"  - Saltos correctos: {gameManager.nivel1_Saltos_Correctos}");
        Debug.Log($"  - Caídas: {gameManager.nivel1_Caidas}");
        Debug.Log($"  - Tiempo: {gameManager.nivel1_Tiempo:F2} segundos");
        Debug.Log($"  - Instrucciones leídas: {gameManager.nivel1_Instrucciones_Leidas}");

        Debug.Log("=== NIVEL 2 (IF-ELSE) ===");
        Debug.Log($"  - Saltos totales: {gameManager.nivel2_Saltos_Totales}");
        Debug.Log($"  - Saltos correctos: {gameManager.nivel2_Saltos_Correctos}");
        Debug.Log($"  - Caídas: {gameManager.nivel2_Caidas}");
        Debug.Log($"  - Tiempo: {gameManager.nivel2_Tiempo:F2} segundos");
        Debug.Log($"  - Instrucciones leídas: {gameManager.nivel2_Instrucciones_Leidas}");

        Debug.Log("=== NIVEL 3 (CONDICIONALES ANIDADAS) ===");
        Debug.Log($"  - Saltos totales: {gameManager.nivel3_Saltos_Totales}");
        Debug.Log($"  - Saltos correctos: {gameManager.nivel3_Saltos_Correctos}");
        Debug.Log($"  - Caídas: {gameManager.nivel3_Caidas}");
        Debug.Log($"  - Tiempo: {gameManager.nivel3_Tiempo:F2} segundos");
        Debug.Log($"  - Instrucciones leídas: {gameManager.nivel3_Instrucciones_Leidas}");

        // Comprensión de condicionales
        Debug.Log("=== COMPRENSIÓN DE CONDICIONALES ===");
        Debug.Log($"  - IF statements correctos: {gameManager.if_Statements_Correctos}");
        Debug.Log($"  - ELSE statements correctos: {gameManager.else_Statements_Correctos}");
        Debug.Log($"  - Nested statements correctos: {gameManager.nested_Statements_Correctos}");

        // Patrones de comportamiento
        Debug.Log("=== PATRONES DE COMPORTAMIENTO ===");
        Debug.Log($"  - Dudas expresadas: {gameManager.dudas_Expresadas}");
        Debug.Log($"  - Pausas largas: {gameManager.pausas_Largas}");
        Debug.Log($"  - Veces menú ayuda: {gameManager.veces_Menu_Ayuda}");

        // Tiempo y progreso
        Debug.Log("=== TIEMPO Y PROGRESO ===");
        Debug.Log($"  - Tiempo total sesión: {gameManager.tiempoTotal_Sesion:F2} segundos");
        Debug.Log($"  - Tiempo promedio por nivel: {gameManager.tiempoPromedio_PorNivel:F2} segundos");
        Debug.Log($"  - Nivel actual: {gameManager.nivelActual}");
        Debug.Log($"  - Máximo nivel: {gameManager.maxNivel}");

        // Interacciones VR específicas
        Debug.Log("=== INTERACCIONES VR ===");
        Debug.Log($"  - Teleports realizados: {gameManager.teleports_Realizados}");
        Debug.Log($"  - Objetos agarrados: {gameManager.objetos_Agarrados}");
        Debug.Log($"  - Menús abiertos: {gameManager.menus_Abiertos}");
        Debug.Log($"  - Ayudas solicitadas: {gameManager.ayudas_Solicitadas}");

        Debug.Log("=== FIN DEL RESUMEN ===");
        Debug.Log("¡Gracias por completar el juego de aprendizaje de condicionales!");
    }    /// <summary>
         /// Teletransportar con delay para evitar problemas (método de respaldo)
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