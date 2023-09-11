using System.Collections;
using UnityEngine;
using TMPro;

public class ValueChangeAnimation : MonoBehaviour
{
    public TextMeshProUGUI deltaText;
    public float moveSpeed = 1.0f;
    public float fadeDuration = 1.0f;

    void Start() {
        StartCoroutine(Animate());
    }

    IEnumerator Animate() {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Color startColor = deltaText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

            transform.position = startPosition + Vector3.up * (moveSpeed * elapsedTime);
            deltaText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}