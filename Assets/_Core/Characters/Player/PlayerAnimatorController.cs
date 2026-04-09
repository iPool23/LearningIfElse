using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public OVRCameraRig cameraRig;

    [Header("Configuración")]
    public float walkSpeed = 0.3f;
    public float runSpeed = 0.6f;
    public float smoothTime = 0.1f;

    // Variables internas
    private float currentSpeed = 0f;
    private float smoothVelocity;
    private Vector3 lastPosition;
    private bool isVR;

    // Hashes para optimización
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Detectar si estamos en VR
        isVR = OVRManager.isHmdPresent;

        lastPosition = transform.position;
    }

    void Update()
    {
        float targetSpeed = 0f;

        if (isVR)
        {
            targetSpeed = GetVRSpeed();
        }
        else
        {
            targetSpeed = GetKeyboardSpeed();
        }

        // Si hay input de teclado aunque estemos en VR, usarlo también
        float keyboardSpeed = GetKeyboardSpeed();
        if (keyboardSpeed > targetSpeed)
            targetSpeed = keyboardSpeed;

        // Suavizar la transición de velocidad
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity, smoothTime);

        // Enviar al Animator
        animator.SetFloat(SpeedHash, currentSpeed);

        // Jump con teclado (Space) o botón A del Quest
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.One))
        {
            animator.SetTrigger(JumpHash);
        }

        lastPosition = transform.position;
    }

    float GetKeyboardSpeed()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float inputMagnitude = new Vector2(h, v).magnitude;

        if (inputMagnitude > 0.1f)
        {
            // Shift para correr
            if (Input.GetKey(KeyCode.LeftShift))
                return 1f;
            else
                return 0.3f;
        }

        return 0f;
    }

    float GetVRSpeed()
    {
        // Velocidad basada en el movimiento real del personaje
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f; // Ignorar movimiento vertical
        float speed = delta.magnitude / Time.deltaTime;

        // Normalizar: caminar ~1.5 m/s, correr ~3 m/s
        return Mathf.Clamp01(speed / 3f);
    }

    void LateUpdate()
    {
        // Posición — seguir al OVRCameraRig
        Vector3 pos = cameraRig.transform.position;
        pos.y = 0; // o ajusta según necesites
        transform.position = pos;

        // Rotación — solo eje Y
        float camY = cameraRig.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, camY, 0);
    }
}