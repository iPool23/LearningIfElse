# Sistema de Barreras - Juego VR Condicionales

## 📋 Resumen del Sistema

Este proyecto implementa un sistema completo de aprendizaje de condicionales IF-ELSE en VR con:

- **Barreras entre niveles** que bloquean el progreso hasta completar el nivel anterior
- **Métricas completas de aprendizaje** según tu investigación
- **UI adaptada para VR**

## 🎯 Sobre los Desaciertos

**Respuesta a tu pregunta**: En Unity VR, los "desaciertos" se registran principalmente cuando:

1. **El jugador cae** (ya está implementado en `GameRespawn.RegistrarCaida()`)
2. **Toca un vidrio incorrecto** (registrado en `DestroyOnTrigger`)
3. **Intenta pasar una barrera bloqueada** (registrado en `BarreraNivel`)
4. **Toma más tiempo del esperado** (pausas largas)

Los **errores** se incrementan automáticamente cuando:
- `transform.position.y < threshold` (caída)
- `OnTriggerEnter` con bloque incorrecto
- Intento de acceso no autorizado a nivel

## 🚀 Scripts Principales

### 1. GameRespawn.cs (ACTUALIZADO)
- **Función**: Gestor principal de métricas y progreso
- **Incluye**: Todas las variables de tu investigación
- **Nuevas funciones**: Sistema de barreras, cálculo de métricas

### 2. BlockInteraction.cs (NUEVO)
- **Función**: Detecta cuando el jugador toca bloques
- **Registra**: Aciertos/desaciertos por nivel específico
- **Métricas**: Comprensión conceptual por tipo de condicional

### 3. UIManager.cs (NUEVO)
- **Función**: Gestiona toda la interfaz de usuario
- **Incluye**: ayuda contextual, estadísticas
- **VR Ready**: Textos 3D y botones adaptados para VR

### 4. BarreraNivel.cs (NUEVO)
- **Función**: Bloquea acceso a niveles no completados
- **Características**: Visual feedback, mensajes informativos
- **Integración**: Se comunica con GameRespawn para verificar progreso

### 5. GameController.cs (NUEVO)
- **Función**: Coordinador general del sistema
- **Incluye**: Modo debug, cambio de niveles, guardado automático

## ⚙️ Configuración en Unity

### Paso 1: Configurar la Escena
1. Crear un GameObject vacío llamado "GameManager"
2. Agregar el script `GameController.cs`
3. Crear Canvas para UI (WorldSpace para VR)

### Paso 2: Configurar el Player
1. Asegurar que tenga el tag "Player"
2. Agregar `GameRespawn.cs` al jugador
3. Configurar la posición de respawn

### Paso 3: Configurar Bloques
1. En cada prefab de bloque, agregar `BlockInteraction.cs`
2. Los bloques incorrectos deben tener `DestroyOnTrigger.cs`
3. Actualizar `BlockSpawner.cs` (ya modificado)

### Paso 4: Configurar Barreras
1. Crear GameObjects invisibles entre niveles
2. Agregar `BarreraNivel.cs`
3. Configurar `nivelRequerido` en cada barrera

### Paso 5: Configurar UI
1. Crear paneles: Barrera, HUD, Ayuda
2. Agregar `UIManager.cs` al Canvas
3. Conectar referencias en el Inspector

## 📊 Métricas Implementadas

### Variables Independientes
- **IIU** (Índice de Interacción): comandos, gestos, manipulaciones
- **TPI** (Precisión): aciertos vs total de interacciones  
- **IFN** (Fluidez): velocidad de movimiento medida

### Variables Dependientes
- **Rendimiento**: puntaje basado en precisión
- **IDC** (Dominio Conceptual): evaluación por tipo de condicional
- **ISU** (Satisfacción): métricas de uso y ayuda solicitada

### Variables Específicas del Juego
- **Por Nivel**: saltos, caídas, tiempo, reintentos
- **Comprensión**: IF simples, IF-ELSE, condicionales anidadas
- **Comportamiento**: pausas, dudas, acceso a ayuda

## 🎮 Flujo del Juego

1. **Inicio**: Esta en el spawnpoint
2. **Nivel 1**: Condicionales simples (IF)
3. **Barrera**: Solo se abre al completar Nivel 1
4. **Nivel 2**: Condicionales dobles (IF-ELSE)  
5. **Barrera**: Solo se abre al completar Nivel 2
6. **Nivel 3**: Condicionales anidadas
7. **Final**: Estadísticas completas y guardado

## 🔧 Personalización

### Modificar Tiempos Límite
```csharp
// En GameRespawn.cs
public float[] tiemposLimite = { 120f, 180f, 240f }; // segundos por nivel
```

### Cambiar Criterios de Éxito
```csharp
// Modificar en GameRespawn.CalcularPuntaje()
float precision = interaccionesTotales > 0 ? (float)interaccionesAcertadas / interaccionesTotales : 0f;
```

## 📈 Exportación de Datos - PROXIMAMENTE

Los datos se guardan en `PlayerPrefs` y se pueden exportar a:
- JSON para análisis estadístico
- CSV para Excel/SPSS
- Base de datos para estudios longitudinales

## ✅ Errores Corregidos

### Problemas Resueltos:
1. **FindObjectOfType Obsoleto** → Cambiado a `FindFirstObjectByType<T>()`
2. **Métodos Inaccesibles** → Agregados métodos públicos necesarios
3. **Referencias Faltantes** → Corregidas todas las referencias entre scripts
4. **Duplicación de Métodos** → Eliminados métodos duplicados

### Cambios Realizados:
- ✅ Todos los `FindObjectOfType<T>()` → `FindFirstObjectByType<T>()`
- ✅ `GuardarEstadisticas()` ahora son públicos
- ✅ Referencias correctas entre GameController y GameRespawn
- ✅ Sin errores de compilación

### Estado Actual:
🟢 **Todos los scripts compilados correctamente**
🟢 **Sin warnings de métodos obsoletos**
🟢 **Sistema listo para usar**
