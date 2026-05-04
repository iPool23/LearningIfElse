using UnityEngine;
using LearningIfElse.Framework.PlayerSystems;

/// <summary>
/// PlayerAnimatorController - Maneja las animaciones del personaje visible.
/// Lee el estado de VRLocomotion para sincronizar Speed y Jump con el Animator.
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public OVRCameraRig cameraRig;

    [Header("Configuración")]
    public float smoothTime = 0.1f;

    // Hashes para optimización (evitar string lookups en Update)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash  = Animator.StringToHash("Jump");

    private VRLocomotion _locomotion;
    private float _smoothSpeed;
    private float _smoothVelocity;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Buscar el VRLocomotion en el jugador o en sus padres
        _locomotion = GetComponent<VRLocomotion>();
        if (_locomotion == null)
            _locomotion = GetComponentInParent<VRLocomotion>();

        if (_locomotion == null)
            Debug.LogWarning("[PlayerAnimatorController] No se encontró VRLocomotion. Las animaciones no se actualizarán.");
    }

    void Update()
    {
        if (animator == null) return;

        // ── Velocidad suavizada ───────────────────────────────────────────────────
        float targetSpeed = _locomotion != null ? _locomotion.CurrentSpeed : 0f;

        // Normalizar: walkSpeed ≈ 3 → 0.3, runSpeed ≈ 6 → 1.0
        float normalizedSpeed = Mathf.Clamp01(targetSpeed / 6f);

        _smoothSpeed = Mathf.SmoothDamp(_smoothSpeed, normalizedSpeed, ref _smoothVelocity, smoothTime);
        animator.SetFloat(SpeedHash, _smoothSpeed);

        // ── Salto ─────────────────────────────────────────────────────────────────
        // Escuchar el botón A (VRLocomotion lo procesa, aquí solo disparamos la animación)
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(JumpHash);
        }
    }

    void LateUpdate()
    {
        if (cameraRig == null) return;

        // El mesh del personaje sigue al OVRCameraRig
        Vector3 pos = cameraRig.transform.position;
        pos.y = transform.position.y; // Mantener Y controlada por el CharacterController
        transform.position = pos;

        // Solo rotar en el eje Y
        float camY = cameraRig.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, camY, 0f);
    }
}