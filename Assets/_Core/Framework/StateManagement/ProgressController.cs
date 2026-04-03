using UnityEngine;
using System.Collections.Generic;

namespace LearningIfElse.Framework.StateManagement
{
    /// <summary>
    /// Gestión de la progresión de niveles y estado de la sesión.
    /// Senior Note: Separamos el "qué toca hacer ahora" de los cálculos de las métricas.
    /// </summary>
    public class ProgressController : MonoBehaviour
    {
        [Header("Configuración de Niveles")]
        public int nivelActual = 1;
        public int maxNivel = 3;
        private bool[] nivelesCompletados = new bool[3];
        private bool juegoTerminado = false;

        public bool JuegoTerminado => juegoTerminado;

        /// <summary>
        /// Verifica si el usuario puede acceder a un nivel específico.
        /// </summary>
        public bool PuedeAccederNivel(int nivel)
        {
            if (nivel <= 1) return true;
            if (nivel > maxNivel) return false;

            int indiceAnterior = nivel - 2;
            if (indiceAnterior >= 0 && indiceAnterior < nivelesCompletados.Length)
            {
                return nivelesCompletados[indiceAnterior];
            }
            return false;
        }

        /// <summary>
        /// Marca un nivel como completado y avanza el estado.
        /// </summary>
        public void CompletarNivel(int nivel)
        {
            int indice = nivel - 1;
            if (indice >= 0 && indice < nivelesCompletados.Length)
            {
                nivelesCompletados[indice] = true;
                Debug.Log($"[Progress] Nivel {nivel} completado.");
            }

            if (nivel >= maxNivel)
            {
                juegoTerminado = true;
                Debug.Log("[Progress] ¡Juego finalizado!");
            }
        }

        public void SetNivelActual(int nivel)
        {
            nivelActual = nivel;
        }

        public bool IsNivelCompletado(int nivel)
        {
            int idx = nivel - 1;
            if (idx >= 0 && idx < nivelesCompletados.Length)
                return nivelesCompletados[idx];
            return false;
        }
    }
}
