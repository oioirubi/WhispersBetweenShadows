using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform cam;

    private Vector3 velocity;
    private bool isGrounded;
    public float gravity = -9.81f;
    private Vector3 moveDir;
    public float turnSmoothTime = 0.1f; // Tempo para suavizar a rota��o
    private float turnSmoothVelocity;

    void Update()
    {
        // Checando se est� no ch�o
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Para garantir que a gravidade mantenha a personagem no ch�o
        }

        // Captura o input de movimento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Pega a dire��o baseada na c�mera, mas sem alterar o eixo Y
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Apenas move se houver entrada de movimento
        if (direction.magnitude >= 0.1f)
        {
            // Calcula o �ngulo para rotacionar a personagem em dire��o ao movimento
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // Suaviza a rota��o da personagem
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f); // Rotaciona suavemente

            // Define a dire��o de movimento
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
        }
        else
        {
            // Se n�o houver input, zera o movimento
            moveDir = Vector3.zero;
            controller.Move(Vector3.zero);
        }

        // Aplicando gravidade (apenas no eixo Y)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
