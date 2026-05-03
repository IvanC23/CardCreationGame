using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PromoManager : MonoBehaviour
{
    public RectTransform promoUI;
    public float targetScaleMultiplier = 1.2f;
    public float scaleDuration = 0.5f;
    public float finalScaleTargetMultiplier = 1.0f;
    public float finalScaleDuration = 0.5f;
    public float timerBeforeNextSceneLoad = 15.0f;

    public int nextSceneIndex = 1;

    void Start()
    {
        if (promoUI != null)
            StartCoroutine(PlayPromoSequence());
    }

    private IEnumerator PlayPromoSequence()
    {
        Vector3 startScale = promoUI.localScale; // scala iniziale (0,0,0)
        Vector3 bigScale = startScale + Vector3.one * targetScaleMultiplier; // (1.2, 1.2, 1.2)
        Vector3 finalScale = Vector3.one * finalScaleTargetMultiplier;       // (1.0, 1.0, 1.0)

        yield return StartCoroutine(ScaleUI(promoUI, startScale, bigScale, scaleDuration));
        yield return StartCoroutine(ScaleUI(promoUI, bigScale, finalScale, finalScaleDuration));

        if (nextSceneIndex == 0)
            yield break;

        yield return new WaitForSeconds(timerBeforeNextSceneLoad);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private IEnumerator ScaleUI(RectTransform uiElement, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            uiElement.localScale = Vector3.Lerp(from, to, eased);
            t += Time.deltaTime;
            yield return null;
        }
        uiElement.localScale = to;
    }
}
