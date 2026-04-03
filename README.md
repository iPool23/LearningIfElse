# 🎮 LearningIfElse v1 — Juego Educativo VR de Condicionales

> **Proyecto Unity 6000.0.36f1** | Plataforma: VR (Android/iOS/Editor) | Lenguaje: C#

---

## 📋 Descripción General

**LearningIfElse** es un videojuego educativo en realidad virtual (VR) diseñado para enseñar el concepto de **estructuras condicionales en programación** (`if`, `if-else`, y condicionales anidadas) mediante mecánicas de juego inmersivas. Inspirado en la dinámica del "puente de vidrio" del Juego del Calamar, el jugador debe avanzar saltando sobre plataformas de vidrio, eligiendo el bloque correcto según la lógica condicional que se le presenta.

El proyecto también funciona como herramienta de **investigación académica**: registra métricas detalladas de comportamiento del usuario y las sincroniza con **Firebase Realtime Database** para análisis transversal post-sesión.

---

## 🏗️ Arquitectura del Proyecto

```
LearningIfElsev1/
└── Assets/
    ├── GameRespawn.cs              ← Game Manager central (raíz de Assets)
    └── Scripts/
        ├── BlockSpawner.cs         ← Generador de bloques – Nivel 1 (IF simple)
        ├── BlockSpawner_Double.cs  ← Generador de bloques – Nivel 2 (IF-ELSE)
        ├── BlockSpawner_Nested.cs  ← Generador de bloques – Nivel 3 (Anidadas)
        ├── BarreraNivel.cs         ← Barrera de acceso entre niveles
        ├── CountOnCorrect.cs       ← Detección y registro de aciertos en bloque correcto
        ├── DestroyOnTrigger.cs     ← Destrucción de bloque peligroso al pisarlo
        └── UIManager.cs            ← Gestión de la interfaz de usuario (HUD y estadísticas)
```

---

## 🎯 Mecánica de Juego

### Flujo General

```
[Spawn / Lobby]
      │
      ▼
[Barrera Nivel 1] ──desbloqueada siempre──► [Nivel 1: IF Simple]
      │                                            │ (completa fila final)
      ▼                                            ▼
[Teletransporte al Spawn]               [CompletarNivelActual()]
      │                                            │
      ▼                                            ▼
[Barrera Nivel 2] ──requiere N1 completo──► [Nivel 2: IF-ELSE]
      │                                            │
      ▼                                            ▼
[Teletransporte al Spawn]               [CompletarNivelActual()]
      │                                            │
      ▼                                            ▼
[Barrera Nivel 3] ──requiere N2 completo──► [Nivel 3: Anidadas]
                                                   │
                                                   ▼
                                          [CompletarJuego()]
                                          [Teletransporte Final]
                                          [Firebase Upload]
```

### Lógica de Bloques por Nivel

| Nivel | Concepto | Bloques por fila | Regla de selección | Textura segura | Textura peligrosa |
|-------|----------|------------------|--------------------|----------------|-------------------|
| 1 | `IF` simple | 2 columnas | Aleatorio: 1 correcto, 1 incorrecto | `correctTexture` | `wrongTexture` |
| 2 | `IF-ELSE` | 2 columnas | Aleatorio: 1 gato (seguro), 1 perro (peligroso) | `safeTexture` (gato) | `dangerTexture` (perro) |
| 3 | Anidadas | 2 columnas | Garantizado: 1 seguro (rojo+símbolo, azul o verde), 1 peligroso (rojo sin símbolo) | `redCrystalTexture`, `blueCrystalTexture`, `greenCrystalTexture` | `redNoSymbolTexture` |

- **Bloque seguro** → `collider.isTrigger = false` (sólido, se puede pisar) + componente `CountOnCorrect`
- **Bloque peligroso** → `collider.isTrigger = true` (trigger, se destruye al tocarlo) + componente `DestroyOnTrigger`

---

## 📁 Scripts — Documentación Detallada

---

### `GameRespawn.cs`
**Ruta:** `Assets/GameRespawn.cs`  
**Rol:** Game Manager central. Es el núcleo del sistema; todos los demás scripts lo referencian.

#### Responsabilidades
- **Respawn:** Detecta si el jugador cae por debajo del umbral `threshold` (default `-10f`) y lo teletransporta a `respawnPosition`.
- **Sistema de niveles:** Controla el nivel actual (`nivelActual`), el array de niveles completados (`nivelesCompletados[]`) y el acceso secuencial entre niveles.
- **Métricas de investigación:** Almacena y calcula todas las variables del estudio.
- **Persistencia:** Guarda estadísticas locales con `PlayerPrefs` y las sube a Firebase Realtime Database.
- **Control de tiempo:** Mide tiempos reales por nivel (excluye pausas en barreras), tiempo total de sesión y detecta pausas largas (>5 s).
- **UI:** Actualiza en tiempo real los textos del HUD (nivel, puntaje, tiempos, saltos, caídas).
- **Audio:** Reproduce sonido de caída mediante `AudioSource`.

#### Variables Principales

**Variables Independientes (estudio):**
| Variable | Tipo | Descripción |
|----------|------|-------------|
| `comandosIniciados` | `int` | CI — Comandos iniciados (IIU) |
| `gestosInterpretados` | `int` | GI — Gestos interpretados (IIU) |
| `manipulacionesInteractivas` | `int` | MI — Manipulaciones interactivas (IIU) |
| `interaccionesAcertadas` | `int` | IA — Interacciones acertadas (TPI) |
| `interaccionesTotales` | `int` | IT — Interacciones totales (TPI) |
| `velocidadMovimientoMedida` | `float` | VM — Velocidad medida (IFN) |
| `velocidadInicialEsperada` | `float` | VI = 1.0f — Velocidad inicial base (IFN) |
| `velocidadEstandarObjetivo` | `float` | VE = 4.0f — Velocidad estándar objetivo (IFN) |

**Variables Dependientes (estudio):**
| Variable | Tipo | Descripción |
|----------|------|-------------|
| `puntajeObtenido` | `float` | Puntaje total acumulado |
| `puntajeMaximoPosible` | `float` | PM = 100f |
| `porcentajeRendimiento` | `float` | % rendimiento = (IA/IT) × 100 |
| `evaluacionConceptual` | `float` | EC — Dominio conceptual (0–1) |
| `aplicacionPractica` | `float` | AP — Aplicación práctica (0–1) |
| `resolucionCasos` | `float` | RC — Resolución de casos (0–1) |
| `sumaScoresSUS` | `float` | SS — Suma de scores SUS (satisfacción usuario) |

**Variables de Mecánica de Juego:**
| Variable | Tipo | Descripción |
|----------|------|-------------|
| `saltos_Correctos` | `int` | Vidrios seguros pisados |
| `saltos_Incorrectos` | `int` | Vidrios que se rompieron / caídas por trigger |
| `reintentos_Nivel` | `int` | Reintentos del nivel en curso |
| `nivel1/2/3_Saltos_Totales` | `int` | Saltos totales por nivel |
| `nivel1/2/3_Saltos_Correctos` | `int` | Saltos correctos por nivel |
| `nivel1/2/3_Caidas` | `int` | Caídas por nivel |
| `nivel1/2/3_Tiempo` | `float` | Tiempo real gastado por nivel |
| `if_Statements_Correctos` | `int` | Aciertos en nivel de IF simple |
| `else_Statements_Correctos` | `int` | Aciertos en nivel de IF-ELSE |
| `nested_Statements_Correctos` | `int` | Aciertos en nivel de anidadas |
| `dudas_Expresadas` | `int` | Intentos de acceso bloqueado / retroceso |
| `pausas_Largas` | `int` | Pausas >5 s antes de saltar |
| `veces_Menu_Ayuda` | `int` | Veces que abrió el menú de ayuda |
| `tiempoLimiteGlobal` | `float` | Límite máximo de sesión (default: 600 s) |
| `teleports_Realizados` | `int` | Teletransportes totales realizados |

**Propiedades calculadas:**
- `TotalSaltosTotalesPorNivel` → suma `nivel1 + nivel2 + nivel3`
- `TotalSaltosCorrectosPorNivel` → suma correctos
- `TotalCaidasPorNivel` → suma caídas

#### Métodos Clave
| Método | Descripción |
|--------|-------------|
| `CompletarNivelActual()` | Marca el nivel como completado, registra tiempo real, reactiva barrera amarilla, llama a `CalcularPuntaje()` |
| `CompletarTransicionNivel()` | Avanza `nivelActual++` y reinicia el timer (llamado por BlockSpawner post-teletransporte) |
| `IniciarTiempoNivel()` | Inicia el temporizador del nivel (llamado por BarreraNivel al pasar) |
| `PuedeAccederNivel(int nivel)` | Retorna `true` si el nivel anterior está completado |
| `ReintentarNivel()` | Resetea posición, incrementa `reintentos_Nivel`, desactiva barrera |
| `PasarSiguienteNivel()` | Avanza al siguiente nivel si tiene acceso |
| `GuardarEstadisticas()` | Guarda en `PlayerPrefs` + sube a Firebase Realtime Database |
| `BloqueYaContado(string id)` | Consulta el HashSet de bloques contados (anti-duplicado) |
| `RegistrarBloqueContado(string id)` | Registra un bloque como ya contado en la sesión |
| `RegistrarCaida()` | Registra caída y actualiza métricas por nivel |
| `RegistrarSaltoExitoso()` | Registra acierto y actualiza métricas |
| `CalcularMetricasFinales()` | Calcula IIU, TPI, IFN, IDC, ISU al terminar el juego |
| `TeletransportarAFinal()` | Mueve al jugador a `(-11.804, 17.612, -5.92)` (área final/lobby) |
| `CompletarJuego()` | Teletransporta al área final, calcula métricas, llama a `GuardarEstadisticas()` |

#### Integración Firebase
- Solo activo bajo `#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS`
- Inicializa Firebase en `Start()` con `CheckAndFixDependenciesAsync()`
- Al completar el juego, serializa `EstadisticasSesion` con `JsonUtility.ToJson()` y lo sube a `estadisticas/{sessionId}` en Realtime Database
- Clases serializables: `EstadisticasSesion` y `NivelStats`

---

### `BlockSpawner.cs` (clase: `BlockSpawner_Simple`)
**Ruta:** `Assets/Scripts/BlockSpawner.cs`  
**Nivel:** 1 — Condicionales Simples (`IF`)

#### Responsabilidades
- Genera una cuadrícula de `rows × columns` bloques proceduralmente al inicio.
- En cada fila, elige **aleatoriamente** qué columna tiene la textura correcta.
- Bloque correcto: `isTrigger = false` + `CountOnCorrect` + zona trigger adicional encima.
- Bloque incorrecto: `isTrigger = true` + `DestroyOnTrigger`.
- Detecta en `Update()` si el jugador llegó a la última fila y dispara `TeletransportarYCompletarNivel()`.

#### Inspector Fields
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `blockPrefab` | `GameObject` | Prefab del bloque |
| `rows` | `int` | Filas de bloques (default: 5) |
| `columns` | `int` | Columnas (default: 2) |
| `spacing` | `float` | Separación entre bloques (default: 2.7) |
| `correctTexture` | `Texture` | Textura del bloque correcto |
| `wrongTexture` | `Texture` | Textura del bloque incorrecto |
| `textureScale` | `Vector2` | Escala de repetición de textura (default: 2,2) |
| `nivelAsociado` | `int` | ID del nivel (1) |
| `blockContainer` | `Transform` | Contenedor de bloques (opcional) |

#### Métodos Clave
| Método | Descripción |
|--------|-------------|
| `GenerateSimpleConditionalBlocks()` | Genera la cuadrícula con lógica IF |
| `RegenerarBloques()` | Destruye y regenera bloques (reintentos) |
| `TeletransportarYCompletarNivel()` | Coroutine: teletransporta jugador → llama `CompletarNivelActual()` → `CompletarTransicionNivel()` |

---

### `BlockSpawner_Double.cs`
**Ruta:** `Assets/Scripts/BlockSpawner_Double.cs`  
**Nivel:** 2 — Condicionales Dobles (`IF-ELSE`)

#### Responsabilidades
- Genera cuadrícula con lógica `IF-ELSE`: exactamente **1 bloque seguro (gato)** y **1 bloque peligroso (perro)** por fila.
- La columna segura se elige aleatoriamente por fila con `Random.Range(0, columns)`.
- Bloque gato → `CountOnCorrect`; bloque perro → `DestroyOnTrigger`.
- Detecta llegada a la última fila y llama a `TeletransportarYCompletarNivel()`.
- Permite nivel anterior (nivel 1 o 2) como válido para mayor flexibilidad.

#### Inspector Fields
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `safeTexture` | `Texture` | Textura del gato (seguro) |
| `dangerTexture` | `Texture` | Textura del perro (peligroso) |
| `nivelAsociado` | `int` | ID del nivel (2) |
| (resto igual que BlockSpawner) | | |

---

### `BlockSpawner_Nested.cs`
**Ruta:** `Assets/Scripts/BlockSpawner_Nested.cs`  
**Nivel:** 3 — Condicionales Anidadas

#### Responsabilidades
- Genera cuadrícula con lógica de **condicionales anidadas**: 3 texturas seguras (cristal rojo+símbolo, azul, verde) y 1 textura peligrosa (rojo sin símbolo).
- **Garantía pedagógica:** En las primeras 3 filas, fuerza que cada textura segura aparezca exactamente una vez (fila 0 → rojo, fila 1 → azul, fila 2 → verde). Las filas restantes usan cualquier textura segura aleatoria.
- Siempre hay **exactamente 1 bloque peligroso** por fila (rojo sin símbolo).
- Al completar la última fila llama a `TeletransportarYCompletarJuego()`.
- `MostrarDatosFinales()` imprime un resumen completo de todas las métricas en consola al terminar.

#### Inspector Fields
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `redCrystalTexture` | `Texture` | Cristal rojo con símbolo (SEGURO) |
| `redNoSymbolTexture` | `Texture` | Rojo sin símbolo (PELIGROSO) |
| `blueCrystalTexture` | `Texture` | Cristal azul (SEGURO) |
| `greenCrystalTexture` | `Texture` | Cristal verde (SEGURO) |
| `nivelAsociado` | `int` | ID del nivel (3) |
| (resto igual que BlockSpawner) | | |

#### Métodos Exclusivos
| Método | Descripción |
|--------|-------------|
| `TeletransportarYCompletarJuego()` | Coroutine: teletransporta → `CompletarNivelActual()` → `CompletarTransicionNivel()` → `MostrarDatosFinales()` |
| `MostrarDatosFinales()` | Imprime en consola todo el resumen de métricas (IIU, TPI, IFN, rendimiento, tiempos, saltos, caídas, patrones) |

---

### `BarreraNivel.cs`
**Ruta:** `Assets/Scripts/BarreraNivel.cs`

#### Responsabilidades
Controla el acceso entre niveles mediante una barrera física con 3 estados visuales y físicos:

| Estado | Color | Collider | Condición |
|--------|-------|----------|-----------|
| **Bloqueada** | 🔴 Rojo | Sólido (`isTrigger = false`) | Nivel anterior NO completado |
| **Desbloqueada** | 🟢 Verde semitransparente | Trigger | Nivel anterior completado, nivel actual NO completado |
| **Completada** | 🟡 Amarillo | Sólido (si `!permitirRetroceso`) / Trigger (si `permitirRetroceso`) | Nivel actual YA completado |

#### Inspector Fields
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `nivelRequerido` | `int` | Número del nivel que esta barrera protege |
| `mostrarMensaje` | `bool` | Si muestra mensaje al ser bloqueado |
| `mensajeBloqueo` | `string` | Texto del mensaje de bloqueo |
| `efectoBarrera` | `GameObject` | Efecto visual (partículas, etc.) |
| `materialBloqueado` | `Material` | Material rojo (bloqueada) |
| `materialDesbloqueado` | `Material` | Material verde (desbloqueada) |
| `materialCompletado` | `Material` | Material dorado (completada) |
| `permitirRetroceso` | `bool` | Si permite volver a niveles anteriores |
| `bloquearDespuesDeCompletar` | `bool` | Si bloquea absolutamente tras completar |

#### Lógica de Estados
- `Start()` → Arranca en estado bloqueado, luego llama a `ActualizarEstadoBarrera()`.
- `Update()` → Verifica constantemente el estado y corrige inconsistencias.
- `ActualizarEstadoBarrera()` → Consulta `gameManager.nivelesCompletados[]` para determinar si debe desbloquearse o marcarse como completada.
- `OnCollisionEnter()` → Si bloqueada: muestra mensaje + empuja al jugador. Si amarilla sin retroceso: empuja también.
- `OnTriggerEnter()` → Si completada sin retroceso: teletransporta al jugador hacia adelante. Si abierta: registra paso.
- `RegistrarPasoDeNivel()` → Incrementa `manipulacionesInteractivas`, llama a `IniciarTiempoNivel()` y desactiva la barrera.

#### Métodos Públicos
| Método | Descripción |
|--------|-------------|
| `ActivarBarrera()` | Reactiva la barrera (llamado por GameRespawn en reintentos / completar nivel) |
| `DesactivarBarrera()` | Desactiva el GameObject tras ser cruzada |
| `VerificarEstado()` | Fuerza actualización de estado |
| `ConfigurarBarrera(int, string)` | Configura nivel requerido y mensaje desde código |
| `ConfigurarRetroceso(bool, bool)` | Configura opciones de retroceso |
| `ForzarCompletado()` | Marca como completado (útil para testing) |
| `ObtenerEstadoBarrera()` | Devuelve string con estado actual |

#### Comportamiento Especial: Barrera Nivel 1
- `nivelRequerido == 1` → Siempre desbloqueada (no hay nivel previo requerido).

#### Mensajes en Mundo VR
- Los mensajes de bloqueo/retroceso se crean como objetos 3D (`TextMesh`) flotando sobre la barrera durante 3 segundos.

---

### `CountOnCorrect.cs`
**Ruta:** `Assets/Scripts/CountOnCorrect.cs`

#### Responsabilidades
Se adjunta a los **bloques seguros**. Registra un acierto cuando el jugador los pisa, con sistema anti-duplicado por sesión.

#### Detección (triple mecanismo)
1. `OnTriggerEnter()` — para zonas trigger sobre el bloque
2. `OnCollisionEnter()` — para colisión directa sólida
3. `Update()` — detección por proximidad (`< 0.8f`) como respaldo

#### Lógica de Registro
- Verifica `gameManager.BloqueYaContado(bloqueID)` antes de registrar (evita duplicados entre regeneraciones).
- Si no está contado: incrementa `aciertos`, `interaccionesAcertadas`, `interaccionesTotales`, `saltos_Correctos`.
- Registra aciertos por nivel: `nivel1_Saltos_Correctos/Totales`, `if_Statements_Correctos`, etc.
- Suma `+10f` a `puntajeObtenido`.
- Efectos: cambia material a verde 0.5 s, luego restaura color original; reproduce `AudioSource` si existe.
- `MarcarComoUsado()`: añade `" ✓USADO"` al nombre y oscurece ligeramente el color.

#### Métodos Públicos
| Método | Descripción |
|--------|-------------|
| `ResetearContador()` | Resetea `yaContado`, restaura nombre y color (para regeneración) |

---

### `DestroyOnTrigger.cs`
**Ruta:** `Assets/Scripts/DestroyOnTrigger.cs`

#### Responsabilidades
Se adjunta a los **bloques peligrosos**. Cuando el jugador los toca:
1. Registra `errores++`, `interaccionesTotales++`, `saltos_Incorrectos++` en `GameRespawn`.
2. Llama a `CrearEfectoVidroRoto()`: cambia material a rojo + reproduce audio.
3. Destruye el GameObject del bloque con `Destroy(gameObject)`.

> **Nota:** La caída física (respawn) la maneja `GameRespawn.FixedUpdate()` cuando `y < threshold`. `DestroyOnTrigger` solo destruye el vidrio y pre-registra el error.

---

### `UIManager.cs`
**Ruta:** `Assets/Scripts/UIManager.cs`

#### Responsabilidades
Gestiona la interfaz de usuario del juego, actualizando todos los elementos de texto en tiempo real desde `GameRespawn`.

#### Paneles
- `panelHUD` — HUD principal durante el juego
- `panelEstadisticas` — Panel de estadísticas finales (se muestra al terminar)

#### Campos de Texto (TextMeshProUGUI)
| Campo | Muestra |
|-------|---------|
| `txtPuntaje` | Puntaje actual (`puntajeObtenido`) |
| `txtTiempoNivel1/2/3` | Tiempo de cada nivel (`:F2`s) |
| `txtTiempoTotal` | Tiempo total de la sesión |
| `txtSaltosTotales` | Total de saltos acumulados |
| `txtSaltosCorrectosTotales` | Total de saltos correctos |
| `txtSaltosNivel1/2/3` | Saltos por nivel |
| `txtSaltosCorrectosNivel1/2/3` | Saltos correctos por nivel |
| `txtCaidasTotales` | Total de caídas |
| `txtCaidasNivel1/2/3` | Caídas por nivel |

#### Métodos Públicos
| Método | Descripción |
|--------|-------------|
| `MostrarEstadisticasFinales()` | Activa panel estadísticas, desactiva HUD |
| `MostrarHUD()` | Activa HUD, desactiva panel estadísticas |

> `ActualizarHUD()` se llama en cada `Update()`, actualizando todos los campos si el GameManager está disponible.

---

## 🔗 Dependencias entre Scripts

```
GameRespawn (Game Manager)
    ├── BarreraNivel ──────────────► GameRespawn.IniciarTiempoNivel()
    │                                GameRespawn.nivelesCompletados[]
    │                                GameRespawn.errores / dudas_Expresadas
    │
    ├── BlockSpawner_Simple ───────► GameRespawn.CompletarNivelActual()
    ├── BlockSpawner_Double ───────► GameRespawn.CompletarTransicionNivel()
    ├── BlockSpawner_Nested ───────► GameRespawn.CompletarNivelActual()
    │                                GameRespawn.comandosIniciados
    │                                GameRespawn.reintentos_Nivel
    │
    ├── CountOnCorrect ────────────► GameRespawn.aciertos / puntajeObtenido
    │                                GameRespawn.BloqueYaContado()
    │                                GameRespawn.RegistrarBloqueContado()
    │
    ├── DestroyOnTrigger ──────────► GameRespawn.errores
    │                                GameRespawn.saltos_Incorrectos
    │
    └── UIManager ─────────────────► GameRespawn.* (solo lectura, todos los campos)
```

---

## 📊 Sistema de Métricas de Investigación

El proyecto implementa un modelo de variables para investigación académica:

### Índices Calculados
| Índice | Fórmula | Descripción |
|--------|---------|-------------|
| **IIU** (Índice Interacción Usuario) | `(CI + GI + MI) / 3` | Nivel de engagement con la interfaz |
| **TPI** (Tasa Precisión Interacción) | `(IA / IT) × 100` | Porcentaje de aciertos sobre total de interacciones |
| **IFN** (Índice Fluidez Navegación) | `((VM - VI) / (VE - VI)) × 100` | Normalización de velocidad en rango [VI=1.0, VE=4.0] u/s |
| **IDC** (Índice Dominio Conceptual) | `(EC + AP + RC) / 3` | Comprensión de los conceptos enseñados |
| **ISU** (Índice Satisfacción Usuario) | `(SS / NT) × 10` | Basado en escala SUS (System Usability Scale) |

### Persistencia de Datos
- **Local:** `PlayerPrefs` (19+ claves guardadas al finalizar)
- **Remoto:** Firebase Realtime Database en rama `estadisticas/{sessionId}`
- **Sesión única:** `sessionId` generado con `Guid.NewGuid()` al inicio

---

## ⚙️ Configuración de Niveles

### Tiempos Límite por Nivel
```csharp
float[] tiemposLimite = { 120f, 180f, 240f }; // Nivel 1: 2 min, N2: 3 min, N3: 4 min
```

### Tiempo Límite Global
```csharp
float tiempoLimiteGlobal = 600f; // 10 minutos totales de sesión
```
Al alcanzarlo, se fuerza `CompletarJuego()` automáticamente.

### Posición de Respawn
```csharp
Vector3 respawnPosition = new Vector3(-11.804f, 1.022f, -0.238f);
```

### Posición Final (Lobby/Área de resultados)
```csharp
new Vector3(-11.804f, 17.612f, -5.92f) // Destino al completar el juego
```

---

## 🚀 Cómo Configurar en Unity

### Requerimientos
- Unity **6000.0.36f1**
- TextMeshPro (para UIManager y GameRespawn)
- Firebase SDK para Unity (Firebase.Database)
- XR Plugin Management (para VR)

### Setup GameRespawn
1. Adjuntar `GameRespawn.cs` al GameObject del jugador (o a un objeto vacío de escena).
2. Configurar `respawnPosition`, `threshold` y `tiempoLimiteGlobal` en el Inspector.
3. Asignar las referencias UI (`uiNivelActual`, `uiPuntaje`, paneles de tiempo, etc.).
4. Asignar `fallSound` si se usa audio de caída.

### Setup por Nivel (BlockSpawner)
1. Crear un GameObject vacío en la escena por nivel.
2. Adjuntar el script correspondiente (`BlockSpawner_Simple`, `_Double`, `_Nested`).
3. Asignar `blockPrefab` (prefab con Collider y Renderer).
4. Asignar las texturas correspondientes.
5. Opcionalmente asignar un `blockContainer` para rotaciones del grupo.

### Setup BarreraNivel
1. Crear un GameObject con mesh visible entre niveles.
2. Adjuntar `BarreraNivel.cs`.
3. Configurar `nivelRequerido` (el nivel que protege esta barrera).
4. Asignar materiales de colores (rojo, verde, dorado/amarillo) opcionales.
5. Configurar `permitirRetroceso = false` para modo estándar.

### Setup UIManager
1. Crear un Canvas con los paneles `panelHUD` y `panelEstadisticas`.
2. Adjuntar `UIManager.cs` al Canvas.
3. Asignar todos los campos `TextMeshProUGUI` desde el Inspector.

### Tags Requeridos
- El jugador **debe** tener el tag `"Player"` para que todos los sistemas funcionen.

---

## 🧪 Flujo de Datos Completo (al completar el juego)

```
BlockSpawner_Nested.TeletransportarYCompletarJuego()
    └── GameRespawn.CompletarNivelActual()          ← registra tiempo nivel 3
        └── GameRespawn.CompletarJuego()
            ├── tiempoTotal_Sesion = Time.time - tiempoInicio_Sesion
            ├── tiempoPromedio_PorNivel = (N1 + N2 + N3) / 3
            ├── CalcularMetricasFinales()            ← IIU, TPI, IFN, IDC, ISU
            └── GuardarEstadisticas()
                ├── PlayerPrefs.Save()               ← local
                └── Firebase.SetRawJsonValueAsync()  ← remoto
    └── BlockSpawner_Nested.MostrarDatosFinales()    ← consola completa
```

---

## 📝 Notas de Diseño

- **Anti-duplicado de aciertos:** Se usa un `HashSet<string>` (`bloquesContados`) en `GameRespawn` para evitar que un bloque sume puntos más de una vez por sesión, incluso si el jugador regresa al mismo bloque.
- **Timer preciso:** El timer de cada nivel solo corre mientras el jugador está activamente jugando (`nivelEnCurso = true`). Se inicia al pasar la barrera y se detiene al completar el nivel, excluyendo tiempo en barreras/menús.
- **Garantía pedagógica Nivel 3:** Las primeras 3 filas del nivel 3 garantizan que el jugador vea las 3 texturas seguras al menos una vez antes de encontrarse con repeticiones aleatorias.
- **Barrera auto-correctiva:** En cada `Update()`, `BarreraNivel` verifica que el estado del Collider sea consistente con el estado lógico y lo corrige si hay discrepancias.

---

*Proyecto desarrollado en Unity 6000.0.36f1 como herramienta educativa de investigación en VR para la enseñanza de estructuras condicionales en programación.*
