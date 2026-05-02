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

    public float valueForPrimaryLight = 0.2f;
    public float valueForSecondaryLight = 36f;


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

        inputManager.enabled = false;
        inputManager.handCursor.gameObject.SetActive(false);

        StartCoroutine(EndGameSequence());
        StartCoroutine(ManageLights());
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
}