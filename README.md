# LearningIfElse v1 - Guía de Configuración Rápida

Este proyecto es un videojuego educativo en VR desarrollado en Unity 6000.4.1f1. Para que el proyecto funcione correctamente, debes seguir estos pasos de configuración.

> **Nota:** La documentación detallada sobre la arquitectura, métricas y diseño se encuentra en el archivo `Analysis.md` (ubicado en `Assets/_Core/Analysis.md`).

## 🛠️ Requisitos Previos

- **Unity Editor:** Versión `6000.4.1f1`.
- **Plataforma objetivo:** VR (Android / iOS / Editor).

## 📦 1. Instalación de Dependencias

Al abrir el proyecto por primera vez, asegúrate de tener instalados los paquetes necesarios:

1. Ve a `Window > Package Manager`.
2. Verifica que estén instalados:
   - **XR Plugin Management** (incluyendo el proveedor VR como Oculus XR Plugin).
   - **TextMeshPro** (Si los textos no se ven, ve a `Window > TextMeshPro > Import TMP Essential Resources`).

## 🔥 2. Configuración del Firebase SDK

El proyecto guarda estadísticas de los jugadores en la nube, por lo que requiere Firebase.

1. Descarga el [Firebase SDK para Unity](https://firebase.google.com/download/unity).
2. Descomprime el .zip y arrastra el paquete **`FirebaseDatabase.unitypackage`** (de la carpeta `dotnet4`) a la ventana de Project en Unity para importarlo.
3. Permite que el *External Dependency Manager* resuelva las dependencias. Si falla, ve a `Assets > External Dependency Manager > Android Resolver > Force Resolve`.

## 🔑 3. Archivo de Configuración de Firebase (google-services.json)

**Este paso es obligatorio** para evitar errores de inicialización ("Firebase app creation failed"):

1. Ve a la [Consola de Firebase](https://console.firebase.google.com/).
2. Entra a "Configuración del proyecto" (ícono de engranaje).
3. Selecciona tu app (o añade una si no existe) y descarga el archivo **`google-services.json`**.
4. Arrastra y suelta este archivo **dentro de la carpeta `Assets/StreamingAssets/`** (o en la raíz de `Assets/`) de tu proyecto en Unity.

## ▶️ 4. Listo para Jugar

1. Abre la escena principal del juego (dentro de `Assets/Scenes/`).
2. ¡Presiona Play! El sistema conectará automáticamente con Firebase y el manejo de niveles VR funcionará correctamente.
