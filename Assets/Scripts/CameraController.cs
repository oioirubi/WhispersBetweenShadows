using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Referência para Cristal
    public float followDistance = 5f; // Distância fixa entre Cristal e a câmera
    public float cameraHeight = 2f; // Altura fixa da câmera em relação a Cristal
    public float followSpeed = 2f; // Velocidade com que a câmera segue Cristal
    public float minFollowDistance = 3f; // Distância mínima que a câmera pode chegar perto de Cristal
    public float proximityZoomFOV = 30f; // Zoom ao se aproximar de um presente
    public float zoomInFOV = 15f; // Zoom maior durante a interação
    public float zoomOutFOV = 60f; // FOV original da câmera
    public float zoomSpeed = 2f; // Velocidade do zoom
    public Transform[] presents; // Referência para todos os presentes no ambiente
    public float proximityDistance = 2f; // Distância para detectar a proximidade de Cristal com os presentes

    private Vector3 offset; // Posição relativa entre Cristal e a câmera
    private bool isZoomingForInteraction = false; // Para controlar o zoom de interação
    private bool isNearPresent = false; // Para verificar se Cristal está perto de um presente
    private CinemachineVirtualCamera virtualCamera; // Referência para a câmera virtual

    void Start()
    {
        // Inicializa o offset com a altura e distância desejadas
        offset = new Vector3(0, cameraHeight, -followDistance);

        // Tenta encontrar a CinemachineVirtualCamera automaticamente
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Tenta encontrar uma câmera virtual na cena
        }
    }

    void LateUpdate()
    {
        // Se a câmera virtual não foi atribuída, não prosseguir
        if (virtualCamera == null)
        {
            return;
        }

        // Verifica a distância atual entre Cristal e a câmera
        float currentDistance = Vector3.Distance(player.position, transform.position);

        // Se a distância for menor que a mínima permitida, ajusta a posição da câmera
        if (currentDistance < minFollowDistance)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            transform.position = player.position + direction * minFollowDistance;
        }

        // Calcula a nova posição da câmera com base na posição de Cristal
        Vector3 targetPosition = player.position + offset;

        // Movimenta a câmera suavemente para seguir Cristal
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // Mantém a rotação da câmera apenas no plano horizontal
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Verifica a proximidade com os presentes
        CheckProximityToPresents();

        // Aplicar zoom baseado na proximidade ou na interação
        if (isZoomingForInteraction)
        {
            // Zoom durante a interação com o presente
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomInFOV, Time.deltaTime * zoomSpeed);
        }
        else if (isNearPresent)
        {
            // Zoom ao se aproximar de um presente
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, proximityZoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // Reverter o zoom para o FOV original quando não estiver perto de um presente ou interagindo
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);
        }
    }

    // Função para ativar o zoom durante a interação
    public void StartZoom()
    {
        isZoomingForInteraction = true;
    }

    // Função para desativar o zoom após a interação
    public void StopZoom()
    {
        isZoomingForInteraction = false;
    }

    // Função para verificar se Cristal está perto de um presente
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
