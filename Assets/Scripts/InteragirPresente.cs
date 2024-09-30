using System.Collections;
using UnityEngine;

public class InteragirPresente : MonoBehaviour
{
    public bool isRealPresent = false; // Define se este presente � o "real"
    public Transform player; // Refer�ncia para Cristal
    private bool isNear = false; // Verifica se Cristal est� perto o suficiente para interagir
    private bool isOpened = false; // Verifica se o presente j� foi aberto
    private static bool isAnyPresentOpened = false; // Garante que apenas um presente seja aberto por vez

    public float tremorDuration = 2f; // Dura��o do tremor (ajustado para mais tempo)
    public float tremorIntensity = 0.05f; // Intensidade do tremor
    public CameraController cameraController; // Refer�ncia ao CameraController

    void Start()
    {
        // Verifica se o CameraController foi atribu�do
        if (cameraController == null)
        {
            cameraController = Camera.main.GetComponent<CameraController>();
        }
    }

    void Update()
    {
        // Verifica se Cristal est� perto o suficiente
        if (Vector3.Distance(transform.position, player.position) < 2f && IsPresentInFront())
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }

        // Se Cristal est� perto, pressiona E, e nenhum outro presente foi aberto
        if (isNear && Input.GetKeyDown(KeyCode.E) && !isOpened && !isAnyPresentOpened)
        {
            isOpened = true; // Marca o presente como aberto
            isAnyPresentOpened = true; // Garante que outro presente n�o possa ser aberto ao mesmo tempo
            OpenPresent();
        }
    }

    // Fun��o para abrir o presente
    void OpenPresent()
    {
        cameraController.StartZoom(); // Ativa o zoom de intera��o via CameraController
        StartCoroutine(ShakeAndDisappear());
    }

    IEnumerator ShakeAndDisappear()
    {
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;

        // Tremor do presente
        while (elapsedTime < tremorDuration)
        {
            transform.position = originalPosition + Random.insideUnitSphere * tremorIntensity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restaurar a posi��o original
        transform.position = originalPosition;

        // Fade out (desaparecer)
        StartCoroutine(Disappear());
    }

    // Corrotina para fazer o presente desaparecer
    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1f); // Tempo antes de reverter o zoom
        cameraController.StopZoom(); // Reverte o zoom via CameraController

        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        float fadeDuration = 1f;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false); // Desativa o presente ap�s o fade out
        isAnyPresentOpened = false; // Libera para outro presente ser aberto
    }

    bool IsPresentInFront()
    {
        Vector3 directionToPresent = (transform.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, directionToPresent);
        return angle < 30f; // Define o �ngulo de intera��o
    }
}
