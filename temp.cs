
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
