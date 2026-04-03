using UnityEngine;

namespace LearningIfElse.Framework.PlayerSystems
{
    /// <summary>
    /// Encargado de la interacción física y respawn del jugador.
    /// Senior Note: Separamos la lógica de colisión y teletransporte de la lógica de sesión del juego.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Configuración de Respawn")]
        public float threshold = -10f;
        public Vector3 respawnPosition = new Vector3(-11.804f, 1.022f, -0.238f);

        [Header("Audio")]
        public AudioClip fallSound;
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void ReproducirSonidoCaida()
        {
            if (fallSound != null)
            {
                audioSource.PlayOneShot(fallSound);
            }
        }

        public void Teletransportar(GameObject player, Vector3 destino)
        {
            if (player == null) return;
            
            Rigidbody rb = player.GetComponent<Rigidbody>();
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            if (rb != null) rb.isKinematic = true;

            player.transform.position = destino;
            player.transform.rotation = Quaternion.identity;

            if (cc != null) cc.enabled = true;
            if (rb != null) rb.isKinematic = false;
        }

        public void Respawn(GameObject player)
        {
            Teletransportar(player, respawnPosition);
        }
    }
}
