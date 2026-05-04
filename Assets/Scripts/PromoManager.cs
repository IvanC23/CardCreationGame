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
        /*Vector3 startScale = promoUI.localScale;
        Vector3 bigScale = startScale + Vector3.one * targetScaleMultiplier;
        Vector3 finalScale = Vector3.one * finalScaleTargetMultiplier;

        yield return StartCoroutine(ScaleUI(promoUI, startScale, bigScale, scaleDuration * 0.5f));

        AudioManager.Instance.PlaySFX(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(ScaleUI(promoUI, promoUI.localScale, bigScale, scaleDuration * 0.5f));
        yield return StartCoroutine(ScaleUI(promoUI, bigScale, finalScale, finalScaleDuration));

         AudioManager.Instance.PlaySFX(SceneManager.GetActiveScene().buildIndex);

         if (promoUI != null)
             StartCoroutine(ScaleUI(promoUI, promoUI.localScale, Vector3.one * finalScaleTargetMultiplier, finalScaleDuration));
*/
        if (nextSceneIndex == 0)
        {
            yield return new WaitForSeconds(timerBeforeNextSceneLoad);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            yield break;
        }


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
