using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EndGameManager : MonoBehaviour
{
    public List<SplineAnimate> animatedObjects;
    public InputManager inputManager;
    public List<Transform> objectsToScaleToZero;
    public List<Transform> objectsToScaleToOne;
    public Light primaryLight;
    public Light secondaryLight;
    public Light tertiaryLight;

    public float valueForPrimaryLight = 0.2f;
    public float valueForSecondaryLight = 36f;
    public float valueForTertiaryLight = 7.5f;
    public RectTransform endGameUI;


    public static EndGameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EndGame()
    {
        foreach (var anim in animatedObjects)
            anim.enabled = false;

        AudioManager.Instance.PlayLevelEnd();

        inputManager.enabled = false;
        inputManager.handCursor.gameObject.SetActive(false);

        StartCoroutine(ManageLights());
        StartCoroutine(EndGameSequence());
        if (endGameUI != null)
            StartCoroutine(TranslateUI(endGameUI, new Vector3(50, -550f, 0f), 0.3f));
    }

    private IEnumerator EndGameSequence()
    {
        yield return StartCoroutine(ScaleObjects(objectsToScaleToZero, Vector3.zero, .3f));
        yield return StartCoroutine(ScaleObjects(objectsToScaleToOne, Vector3.one, .3f));
    }

    private IEnumerator ManageLights()
    {
        if (primaryLight != null)
            primaryLight.intensity = valueForPrimaryLight;

        if (secondaryLight != null)
            secondaryLight.intensity = valueForSecondaryLight;

        if (tertiaryLight != null)
            tertiaryLight.intensity = valueForTertiaryLight;
        yield return null;
    }

    private IEnumerator ScaleObjects(List<Transform> objects, Vector3 targetScale, float duration)
    {
        var startScales = new List<Vector3>();
        foreach (var obj in objects)
            startScales.Add(obj.localScale);

        float t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            float eased = normalized * normalized * (3f - 2f * normalized);

            for (int i = 0; i < objects.Count; i++)
                if (objects[i] != null)
                    objects[i].localScale = Vector3.Lerp(startScales[i], targetScale, eased);

            t += Time.deltaTime;
            yield return null;
        }

        foreach (var obj in objects)
            if (obj != null)
                obj.localScale = targetScale;
    }

    private IEnumerator TranslateUI(RectTransform uiElement, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = uiElement.anchoredPosition;

        float t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            uiElement.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, eased);
            t += Time.deltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition;
    }   
}