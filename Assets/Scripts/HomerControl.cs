using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HomerControl : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Salto")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Animación")]
    [SerializeField] private string parametroRunning = "isRunning";

    private Vector3 velocity;
    private float speedMultiplier = 1f;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cam == null)
            cam = Camera.main;
    }

    private void Update()
    {
        if (controller == null || !controller.enabled || !controller.gameObject.activeInHierarchy)
            return;

        Mover();
        AplicarGravedadYSalto();
    }

    private void Mover()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direccion = new Vector3(x, 0f, z);

        if (direccion.magnitude > 1f)
            direccion.Normalize();

        Vector3 movimientoFinal = direccion;

        if (cam != null)
        {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            movimientoFinal = camForward * z + camRight * x;
        }

        controller.Move(movimientoFinal * speed * speedMultiplier * Time.deltaTime);

        if (movimientoFinal.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimientoFinal.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                rotationSpeed * Time.deltaTime
            );
        }

        if (animator != null && !string.IsNullOrEmpty(parametroRunning))
        {
            animator.SetBool(parametroRunning, movimientoFinal.magnitude > 0.1f);
        }
    }

    private void AplicarGravedadYSalto()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}