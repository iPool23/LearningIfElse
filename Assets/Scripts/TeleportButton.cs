using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform playerArmature; // Arrastra aquí el PlayerArmature desde la jerarquía
    public Vector3 teleportPosition = new Vector3(0, 1, 0); // Coordenadas X, Y, Z de destino

    [Header("Auto Find Player (Optional)")]
    public bool autoFindPlayer = true; // Buscar automáticamente el PlayerArmature

    void Start()
    {
        // Si está marcado para buscar automáticamente y no hay referencia asignada
        if (autoFindPlayer && playerArmature == null)
        {
            // Buscar el PlayerArmature en la escena
            GameObject playerObj = GameObject.Find("PlayerArmature");
            if (playerObj != null)
            {
                playerArmature = playerObj.transform;
                Debug.Log("PlayerArmature encontrado automáticamente!");
            }
            else
            {
                Debug.LogWarning("No se pudo encontrar PlayerArmature en la escena!");
            }
        }
    }

    // Método que se llama cuando se presiona el botón
    public void TeleportPlayer()
    {
        // Verificar que el PlayerArmature esté asignado
        if (playerArmature == null)
        {
            Debug.LogWarning("PlayerArmature no está asignado!");
            return;
        }

        // Teletransportar el PlayerArmature
        playerArmature.position = teleportPosition;

        Debug.Log($"PlayerArmature teletransportado a: {teleportPosition}");
    }

    // Método alternativo con coordenadas específicas
    public void TeleportPlayerToPosition(float x, float y, float z)
    {
        if (playerArmature == null)
        {
            Debug.LogWarning("PlayerArmature no está asignado!");
            return;
        }

        Vector3 newPosition = new Vector3(x, y, z);
        playerArmature.position = newPosition;

        Debug.Log($"PlayerArmature teletransportado a: {newPosition}");
    }
}