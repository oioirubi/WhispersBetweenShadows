using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Refer�ncia para Cristal
    public float followDistance = 5f; // Dist�ncia fixa entre Cristal e a c�mera
    public float cameraHeight = 2f; // Altura fixa da c�mera em rela��o a Cristal
    public float followSpeed = 2f; // Velocidade com que a c�mera segue Cristal
    public float minFollowDistance = 3f; // Dist�ncia m�nima que a c�mera pode chegar perto de Cristal
    public float proximityZoomFOV = 30f; // Zoom ao se aproximar de um presente
    public float zoomInFOV = 15f; // Zoom maior durante a intera��o
    public float zoomOutFOV = 60f; // FOV original da c�mera
    public float zoomSpeed = 2f; // Velocidade do zoom
    public Transform[] presents; // Refer�ncia para todos os presentes no ambiente
    public float proximityDistance = 2f; // Dist�ncia para detectar a proximidade de Cristal com os presentes

    private Vector3 offset; // Posi��o relativa entre Cristal e a c�mera
    private bool isZoomingForInteraction = false; // Para controlar o zoom de intera��o
    private bool isNearPresent = false; // Para verificar se Cristal est� perto de um presente
    private CinemachineVirtualCamera virtualCamera; // Refer�ncia para a c�mera virtual

    void Start()
    {
        // Inicializa o offset com a altura e dist�ncia desejadas
        offset = new Vector3(0, cameraHeight, -followDistance);

        // Tenta encontrar a CinemachineVirtualCamera automaticamente
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Tenta encontrar uma c�mera virtual na cena
        }
    }

    void LateUpdate()
    {
        // Se a c�mera virtual n�o foi atribu�da, n�o prosseguir
        if (virtualCamera == null)
        {
            return;
        }

        // Verifica a dist�ncia atual entre Cristal e a c�mera
        float currentDistance = Vector3.Distance(player.position, transform.position);

        // Se a dist�ncia for menor que a m�nima permitida, ajusta a posi��o da c�mera
        if (currentDistance < minFollowDistance)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            transform.position = player.position + direction * minFollowDistance;
        }

        // Calcula a nova posi��o da c�mera com base na posi��o de Cristal
        Vector3 targetPosition = player.position + offset;

        // Movimenta a c�mera suavemente para seguir Cristal
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // Mant�m a rota��o da c�mera apenas no plano horizontal
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Verifica a proximidade com os presentes
        CheckProximityToPresents();

        // Aplicar zoom baseado na proximidade ou na intera��o
        if (isZoomingForInteraction)
        {
            // Zoom durante a intera��o com o presente
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomInFOV, Time.deltaTime * zoomSpeed);
        }
        else if (isNearPresent)
        {
            // Zoom ao se aproximar de um presente
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, proximityZoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // Reverter o zoom para o FOV original quando n�o estiver perto de um presente ou interagindo
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);
        }
    }

    // Fun��o para ativar o zoom durante a intera��o
    public void StartZoom()
    {
        isZoomingForInteraction = true;
    }

    // Fun��o para desativar o zoom ap�s a intera��o
    public void StopZoom()
    {
        isZoomingForInteraction = false;
    }

    // Fun��o para verificar se Cristal est� perto de um presente
    void CheckProximityToPresents()
    {
        isNearPresent = false; // Reseta o estado de proximidade

        foreach (Transform present in presents)
        {
            float distanceToPresent = Vector3.Distance(player.position, present.position);
            if (distanceToPresent < proximityDistance)
            {
                isNearPresent = true;
                break; // Interrompe o loop assim que um presente estiver perto o suficiente
            }
        }
    }
}
