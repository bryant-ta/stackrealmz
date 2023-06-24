using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetZoomIn;
    public float zoomInFOV = 5f;
    public float zoomDuration = 2f;

    Camera mainCamera;
    Vector3 initialPosition;
    float initialFOV;

    bool isZoomed;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initialPosition = transform.position;
        initialFOV = mainCamera.fieldOfView;
    }

    public void ZoomToggle() {
        if (isZoomed) {
            isZoomed = false;
            ZoomOut();
        } else {
            isZoomed = true;
            ZoomIn();
        }
    }

    void ZoomIn()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(targetZoomIn.position, zoomInFOV));
    }

    void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(initialPosition, initialFOV));
    }

    IEnumerator ZoomRoutine(Vector3 targetPosition, float targetScale)
    {
        float t = 0f;
        Vector3 startPosition = transform.position;
        float startScale = mainCamera.fieldOfView;

        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.fieldOfView = Mathf.Lerp(startScale, targetScale, t);

            yield return null;
        }
    }
}
