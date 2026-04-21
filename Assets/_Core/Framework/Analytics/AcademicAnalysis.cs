using UnityEngine;

namespace LearningIfElse.Framework.Analysis
{
    /// <summary>
    /// Encargado de los cálculos matemáticos y métricas académicas del proyecto.
    /// Senior Note: Separamos la lógica de cálculo de la persistencia y del flujo de juego.
    /// </summary>
    public class AcademicAnalysis : MonoBehaviour
    {
        [Header("Configuración de Velocidad")]
        public float velocidadInicialEsperada = 1.0f;   // VI
        public float velocidadEstandarObjetivo = 4.0f;  // VE

        [Header("Configuración de Puntaje")]
        public float puntajeMaximoPosible = 100f;

        /// <summary>
        /// Calcula el Índice de Fluidez de Navegación (IFN) normalizado.
        /// Fórmula: ((VM - VI) / (VE - VI)) * 100
        /// </summary>
        public float CalcularIFN(float velocidadMedida)
        {
            float denominador = velocidadEstandarObjetivo - velocidadInicialEsperada;
            if (denominador <= 0.001f) return 0f;
            
            return ((velocidadMedida - velocidadInicialEsperada) / denominador) * 100f;
        }

        /// <summary>
        /// Calcula la Precisión de Interacción (TPI).
        /// Fórmula: (IA / IT) * 100
        /// </summary>
        public float CalcularTPI(int interaccionesAcertadas, int interaccionesTotales)
        {
            if (interaccionesTotales <= 0) return 0f;
            return ((float)interaccionesAcertadas / interaccionesTotales) * 100f;
        }

        /// <summary>
        /// Calcula el Índice de Interacción del Usuario (IIU).
        /// </summary>
        public float CalcularIIU(float sumaScoresSUS, int numeroTotalPreguntas)
        {
            if (numeroTotalPreguntas <= 0) return 0f;
            return (sumaScoresSUS / numeroTotalPreguntas) * 10f;
        }

        /// <summary>
        /// Calcula el porcentaje de rendimiento académico.
        /// </summary>
        public float CalcularRendimiento(int acertadas, int totales)
        {
            if (totales <= 0) return 0f;
            return ((float)acertadas / totales) * 100f;
        }

        /// <summary>
        /// Calcula el Índice de Dominio Conceptual (IDC).
        /// </summary>
        public float CalcularIDC(float conceptual, float practica, float resolucion)
        {
            return (conceptual + practica + resolucion) / 3f;
        }
    }
}
