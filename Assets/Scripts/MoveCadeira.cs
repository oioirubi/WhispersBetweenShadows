using System.Collections;
using UnityEngine;

public class MoveCadeira : MonoBehaviour
{
    public Transform player; // Refer�ncia para Cristal
    public GameObject otherChair; // Cadeira que desaparecer�
    public Vector3[] randomPositions; // Posi��es aleat�rias para reaparecer
    private bool isNear = false; // Verifica se Cristal est� perto
    private bool isHolding = false; // Verifica se Cristal est� segurando a cadeira
    private static bool isAnyChairHeld = false; // Certifica que s� uma cadeira pode ser segurada
    public float holdDistance = 1f; // Dist�ncia da cadeira para a frente de Cristal
    public float tiltAngle = 30f; // �ngulo de inclina��o da cadeira

    private Rigidbody chairRigidbody;
    private Collider chairCollider;
    private int originalLayer; // Armazenar a camada original da cadeira
    private Quaternion originalRotation; // Armazenar a rota��o original da cadeira

    void Start()
    {
        chairRigidbody = GetComponent<Rigidbody>();
        chairCollider = GetComponent<Collider>();
        originalLayer = gameObject.layer; // Armazena a camada original
        originalRotation = transform.rotation; // Armazena a rota��o original da cadeira
    }

    void Update()
    {
        // Verifica se Cristal est� perto da cadeira para peg�-la
        if (Vector3.Distance(transform.position, player.position) < 1f)
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }

        // Intera��o com a tecla E
        if (isNear && Input.GetKeyDown(KeyCode.E) && !isHolding && !isAnyChairHeld)
        {
            isHolding = true; // Cristal pega a cadeira
            isAnyChairHeld = true; // Marca que uma cadeira est� sendo segurada

            // Desativa a f�sica da cadeira enquanto ela � arrastada
            if (chairRigidbody != null)
            {
                chairRigidbody.isKinematic = true;
            }

            gameObject.layer = LayerMask.NameToLayer("CarregandoCadeira"); // Altera a camada para evitar colis�es
        }
        else if (isHolding && Input.GetKeyDown(KeyCode.E))
        {
            isHolding = false; // Cristal solta a cadeira
            isAnyChairHeld = false;

            // Volta � camada original e reativa a f�sica
            gameObject.layer = originalLayer;

            if (chairRigidbody != null)
            {
                chairRigidbody.isKinematic = false;
            }

            // Ao soltar a cadeira, ela volta para a rota��o original
            transform.rotation = originalRotation;

            // Faz a outra cadeira desaparecer e reaparecer
            if (otherChair != null)
            {
                StartCoroutine(FadeOutAndReappear(otherChair));
            }
        }

        // Se Cristal estiver segurando a cadeira, ela arrasta a cadeira inclinada
        if (isHolding)
        {
            Vector3 holdPosition = player.position + player.forward * holdDistance;
            holdPosition.y = transform.position.y; // Mant�m a cadeira na altura correta do ch�o

            // Inclinando a cadeira de forma controlada
            transform.position = holdPosition;

            // A parte superior da cadeira sempre aponta para Cristal
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer); // A cadeira sempre "olha" para Cristal
            Quaternion tiltRotation = Quaternion.Euler(tiltAngle, lookRotation.eulerAngles.y, 0); // Inclina apenas no eixo X

            transform.rotation = tiltRotation; // Aplica a rota��o inclinada
        }
    }

    // Corrotina para fazer a cadeira desaparecer e reaparecer suavemente
    IEnumerator FadeOutAndReappear(GameObject chair)
    {
        Renderer chairRenderer = chair.GetComponent<Renderer>();
        Material chairMaterial = chairRenderer.material;
        Color originalColor = chairMaterial.color;
        float fadeDuration = 1f;

        // Desaparecer suavemente
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            chairMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        chair.SetActive(false);

        // Reaparecer em posi��o aleat�ria
        int randomIndex = Random.Range(0, randomPositions.Length);
        chair.transform.position = randomPositions[randomIndex];
        chair.SetActive(true);

        // Fade in para reaparecer suavemente
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            chairMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}
