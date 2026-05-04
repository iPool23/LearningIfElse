using UnityEngine;

namespace LearningIfElse.Framework.PlayerSystems
{
    /// <summary>
    /// VRLocomotion - Sistema de locomoción para VR sentado (Oculus Quest).
    /// El usuario está sentado con los lentes puestos y se mueve con los controles.
    ///
    /// CONTROLES:
    ///   Joystick IZQUIERDO  → Mover (adelante / atrás / laterales)
    ///   Botón B (derecho)   → Correr (mantener presionado)
    ///   Botón A (derecho)   → Saltar
    ///   Joystick DERECHO    → Girar en snap (45° por paso)
    ///
    /// TECLADO (debug en editor):
    ///   WASD               → Mover
    ///   Shift              → Correr
    ///   Espacio            → Saltar
    ///   Q / E              → Girar
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class VRLocomotion : MonoBehaviour
    {
        // ─── VELOCIDADES ────────────────────────────────────────────────────────────
        [Header("=== VELOCIDADES ===")]
        [Tooltip("Velocidad al caminar (m/s)")]
        public float walkSpeed = 3f;

        [Tooltip("Velocidad al correr (m/s)")]
        public float runSpeed = 6f;

        [Tooltip("Fuerza del salto")]
        public float jumpForce = 5f;

        [Tooltip("Gravedad aplicada al personaje")]
        public float gravity = -15f;

        // ─── SNAP TURN ──────────────────────────────────────────────────────────────
        [Header("=== GIRO ===")]
        [Tooltip("Ángulo de cada snap turn en grados")]
        public float snapTurnAngle = 45f;

        [Tooltip("Zona muerta del joystick derecho para el snap turn")]
        public float snapTurnDeadzone = 0.7f;

        // ─── REFERENCIAS ────────────────────────────────────────────────────────────
        [Header("=== REFERENCIAS ===")]
        [Tooltip("Transform de la cámara principal (cabeza del jugador en VR)")]
        public Transform cameraTransform;

        [Tooltip("Referencia al OVRCameraRig (opcional, para soporte adicional)")]
        public OVRCameraRig ovrCameraRig;

        // ─── ESTADO INTERNO ─────────────────────────────────────────────────────────
        private CharacterController _cc;
        private Vector3 _verticalVelocity;
        private bool _snapTurnReady = true;   // evita girar múltiples veces con un solo input

        // ─── PROPIEDADES PÚBLICAS (leídas por PlayerAnimatorController) ─────────────
        public float CurrentSpeed { get; private set; }
        public bool IsGrounded   { get; private set; }
        public bool IsRunning    { get; private set; }

        // ─── CICLO DE VIDA ──────────────────────────────────────────────────────────

        void Awake()
        {
            _cc = GetComponent<CharacterController>();

            // Si no se asignó cámara, buscar la principal
            if (cameraTransform == null)
                cameraTransform = Camera.main != null ? Camera.main.transform : transform;
        }

        void Update()
        {
            IsGrounded = _cc.isGrounded;

            // Resetear velocidad vertical cuando toca el suelo
            if (IsGrounded && _verticalVelocity.y < 0f)
                _verticalVelocity.y = -2f;  // pequeño valor negativo para mantener pegado al suelo

            // ── 1. Leer input ────────────────────────────────────────────────────────
            Vector2 moveInput  = GetMoveInput();
            bool    wantsRun   = GetRunInput();
            bool    wantsJump  = GetJumpInput();
            float   snapInput  = GetSnapTurnInput();

            // ── 2. Snap Turn (joystick derecho) ──────────────────────────────────────
            AplicarSnapTurn(snapInput);

            // ── 3. Movimiento horizontal ─────────────────────────────────────────────
            // La dirección se basa en hacia dónde mira la cámara (no el rig)
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight   = cameraTransform.right;
            camForward.y = 0f;
            camRight.y   = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

            IsRunning = wantsRun && moveDir.magnitude > 0.1f;
            float speed = IsRunning ? runSpeed : walkSpeed;
            CurrentSpeed = moveDir.magnitude * speed;

            // ── 4. Salto ─────────────────────────────────────────────────────────────
            if (wantsJump && IsGrounded)
            {
                // v = sqrt(h * -2 * g)  →  versión simplificada con jumpForce directo
                _verticalVelocity.y = jumpForce;
            }

            // ── 5. Gravedad ───────────────────────────────────────────────────────────
            _verticalVelocity.y += gravity * Time.deltaTime;

            // ── 6. Aplicar movimiento al CharacterController ──────────────────────────
            Vector3 finalMove = moveDir * speed * Time.deltaTime
                              + _verticalVelocity * Time.deltaTime;
            _cc.Move(finalMove);
        }

        // ─── INPUT ──────────────────────────────────────────────────────────────────

        /// <summary>Joystick izquierdo del Oculus + WASD como fallback.</summary>
        Vector2 GetMoveInput()
        {
            // OVR: joystick izquierdo = PrimaryThumbstick
            Vector2 vrInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            // Teclado (editor/debug)
            float kbH = Input.GetAxis("Horizontal");
            float kbV = Input.GetAxis("Vertical");
            Vector2 kbInput = new Vector2(kbH, kbV);

            // Usar whichever es mayor (no se suman)
            return vrInput.magnitude > kbInput.magnitude ? vrInput : kbInput;
        }

        /// <summary>Botón B (mando derecho) para correr + Shift como fallback.</summary>
        bool GetRunInput()
        {
            return OVRInput.Get(OVRInput.Button.Two)          // Botón B derecho
                || Input.GetKey(KeyCode.LeftShift);
        }

        /// <summary>Botón A (mando derecho) para saltar + Espacio como fallback.</summary>
        bool GetJumpInput()
        {
            return OVRInput.GetDown(OVRInput.Button.One)      // Botón A derecho
                || Input.GetKeyDown(KeyCode.Space);
        }

        /// <summary>Joystick derecho horizontal para snap turn + Q/E como fallback.</summary>
        float GetSnapTurnInput()
        {
            float vrAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            float kbAxis = Input.GetKey(KeyCode.E) ?  1f
                         : Input.GetKey(KeyCode.Q) ? -1f
                         : 0f;

            return Mathf.Abs(vrAxis) > Mathf.Abs(kbAxis) ? vrAxis : kbAxis;
        }

        // ─── SNAP TURN ──────────────────────────────────────────────────────────────

        void AplicarSnapTurn(float axisX)
        {
            if (Mathf.Abs(axisX) >= snapTurnDeadzone && _snapTurnReady)
            {
                float angulo = Mathf.Sign(axisX) * snapTurnAngle;
                transform.Rotate(Vector3.up, angulo);
                _snapTurnReady = false;

                Debug.Log($"[VRLocomotion] Snap turn {angulo}°");
            }
            else if (Mathf.Abs(axisX) < snapTurnDeadzone * 0.5f)
            {
                // Resetear cuando el joystick vuelve al centro
                _snapTurnReady = true;
            }
        }

        // ─── GIZMOS DE DEBUG ────────────────────────────────────────────────────────

        void OnDrawGizmosSelected()
        {
            if (cameraTransform == null) return;

            // Mostrar dirección de movimiento relativa a la cámara
            Gizmos.color = Color.cyan;
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            Gizmos.DrawRay(transform.position, forward.normalized * 2f);
        }
    }
}
