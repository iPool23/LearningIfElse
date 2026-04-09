using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidad del Mouse")]
    public float mouseSensitivity = 120f;

    [Header("Límites de mirada vertical")]
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    private float xRotation = 0f;

    void Start()
    {
        // Oculta y bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical (arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Aplicar rotación vertical a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (izquierda/derecha) al padre (OVRCameraRig)
        transform.parent.parent.Rotate(Vector3.up * mouseX);
    }
}