using System;
using UnityEngine;
// --- INICIO: Firebase ---
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
using Firebase;
using Firebase.Database;
#endif
// --- FIN: Firebase ---

namespace LearningIfElse.Cloud.FirebaseSync
{
    /// <summary>
    /// Encargado de la sincronización de datos con Firebase.
    /// Senior Note: Esta clase es el único puente con la nube, facilitando el cambio de proveedor de backend si es necesario.
    /// </summary>
    public class CloudGateway : MonoBehaviour
    {
        [Header("Firebase Config")]
        public string sessionId = "";
        private bool firebaseReady = false;

        void Awake()
        {
            sessionId = Guid.NewGuid().ToString();
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    firebaseReady = true;
                    Debug.Log("[Cloud] Firebase inicializado con éxito.");
                }
                else
                {
                    Debug.LogError($"[Cloud] Error en dependencias Firebase: {dependencyStatus}");
                }
            });
#endif
        }

        public void SubirEstadisticas(string jsonPayload)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            if (!firebaseReady)
            {
                Debug.LogWarning("[Cloud] Firebase no está listo aún. La subida podría fallar.");
            }

            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("estadisticas").Child(sessionId).SetRawJsonValueAsync(jsonPayload)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                        Debug.Log("[Cloud] Estadísticas sincronizadas con éxito.");
                    else
                        Debug.LogError("[Cloud] Error al sincronizar con Firebase: " + task.Exception);
                });
#else
            Debug.Log("[Cloud] Plataforma no soportada para Firebase. Simulando subida...");
#endif
        }
    }

    [Serializable]
    public class EstadisticasSesion
    {
        public string sessionId;
        public string username;
        public string timestamp;
        public float puntaje;
        public int aciertos;
        public int errores;
        public float tiempoTotal;
        public NivelStats nivel1;
        public NivelStats nivel2;
        public NivelStats nivel3;
    }

    [Serializable]
    public class NivelStats
    {
        public int saltos;
        public int saltosCorrectos;
        public int caidas;
        public float tiempo;
    }
}
