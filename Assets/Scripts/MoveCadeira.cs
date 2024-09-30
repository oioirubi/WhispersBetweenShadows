using System.Collections;
using UnityEngine;

public class MoveCadeira : MonoBehaviour
{
    public Transform player; // Referência para Cristal
    public GameObject otherChair; // Cadeira que desaparecerá
    public Vector3[] randomPositions; // Posições aleatórias para reaparecer
    private bool isNear = false; // Verifica se Cristal está perto
    private bool isHolding = false; // Verifica se Cristal está segurando a cadeira
    private static bool isAnyChairHeld = false; // Certifica que só uma cadeira pode ser segurada
    public float holdDistance = 1f; // Distância da cadeira para a frente de Cristal
    public float tiltAngle = 30f; // Ângulo de inclinação da cadeira

    private Rigidbody chairRigidbody;
    private Collider chairCollider;
    private int originalLayer; // Armazenar a camada original da cadeira
    private Quaternion originalRotation; // Armazenar a rotação original da cadeira

    void Start()
    {
        chairRigidbody = GetComponent<Rigidbody>();
        chairCollider = GetComponent<Collider>();
        originalLayer = gameObject.layer; // Armazena a camada original
        originalRotation = transform.rotation; // Armazena a rotação original da cadeira
    }

    void Update()
    {
        // Verifica se Cristal está perto da cadeira para pegá-la
        if (Vector3.Distance(transform.position, player.position) < 1f)
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }

        // Interação com a tecla E
        if (isNear && Input.GetKeyDown(KeyCode.E) && !isHolding && !isAnyChairHeld)
        {
            isHolding = true; // Cristal pega a cadeira
            isAnyChairHeld = true; // Marca que uma cadeira está sendo segurada

            // Desativa a física da cadeira enquanto ela é arrastada
            if (chairRigidbody != null)
            {
                chairRigidbody.isKinematic = true;
            }

            gameObject.layer = LayerMask.NameToLayer("CarregandoCadeira"); // Altera a camada para evitar colisões
        }
        else if (isHolding && Input.GetKeyDown(KeyCode.E))
        {
            isHolding = false; // Cristal solta a cadeira
            isAnyChairHeld = false;

            // Volta à camada original e reativa a física
            gameObject.layer = originalLayer;

            if (chairRigidbody != null)
            {
                chairRigidbody.isKinematic = false;
            }

            // Ao soltar a cadeira, ela volta para a rotação original
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
            holdPosition.y = transform.position.y; // Mantém a cadeira na altura correta do chão

            // Inclinando a cadeira de forma controlada
            transform.position = holdPosition;

            // A parte superior da cadeira sempre aponta para Cristal
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer); // A cadeira sempre "olha" para Cristal
            Quaternion tiltRotation = Quaternion.Euler(tiltAngle, lookRotation.eulerAngles.y, 0); // Inclina apenas no eixo X

            transform.rotation = tiltRotation; // Aplica a rotação inclinada
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

        // Reaparecer em posição aleatória
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
