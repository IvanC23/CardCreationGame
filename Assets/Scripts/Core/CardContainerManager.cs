using System.Collections;
using UnityEngine;

public class CardContainerManager : MonoBehaviour
{
    public static CardContainerManager Instance { get; private set; }

    public GameObject containerPrefab;

    public Transform leftContainerSlot;
    public Transform rightContainerSlot;

    [Header("Exit Animation")]
    public float exitRiseHeight = 0.5f;
    public float exitScaleMultiplier = 1.3f;
    public float exitDuration = 1.5f;

    [Header("Enter Animation")]
    public float enterZOffset = 1.5f;
    public float enterSlideDuration = 0.5f;
    public float enterScaleMultiplier = 1.2f;
    public float enterScaleDuration = 0.5f;

    private CardContainer leftContainer;
    private CardContainer rightContainer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        leftContainer = SpawnContainer(leftContainerSlot, animate: false);
        rightContainer = SpawnContainer(rightContainerSlot, animate: false);
    }

    private CardContainer SpawnContainer(Transform slot, bool animate = true)
    {
        Vector3 spawnPosition = animate
            ? slot.position + Vector3.forward * enterZOffset
            : slot.position;

        GameObject go = Instantiate(containerPrefab, spawnPosition, slot.rotation);
        CardContainer container = go.GetComponent<CardContainer>();

        container.OnContainerFull += OnContainerFull;

        if (animate)
            StartCoroutine(EnterAnimation(container, slot.position));

        return container;
    }

    private void OnContainerFull(CardContainer container)
    {
        container.OnContainerFull -= OnContainerFull;

        bool isLeft = container == leftContainer;
        Transform slot = isLeft ? leftContainerSlot : rightContainerSlot;

        StartCoroutine(ReplaceContainer(container, slot, isLeft));
    }

    private IEnumerator ReplaceContainer(CardContainer container, Transform slot, bool isLeft)
    {
        yield return StartCoroutine(ExitAnimation(container, slot, isLeft));

        foreach (var card in container.GetHostedCards())
            if (card != null)
                Destroy(card.gameObject);

        Destroy(container.gameObject);
    }

    private IEnumerator ExitAnimation(CardContainer container, Transform slot, bool isLeft)
    {
        Vector3 startPos = container.transform.position;
        Vector3 endPos = startPos + Vector3.up * exitRiseHeight;
        Vector3 startScale = container.transform.localScale;
        Vector3 endScale = startScale * exitScaleMultiplier;

        var cards = container.GetHostedCards();
        var cardOffsets = new System.Collections.Generic.List<Vector3>();
        var cardOriginalScales = new System.Collections.Generic.List<Vector3>();
        foreach (var card in cards)
        {
            cardOffsets.Add(card != null ? card.transform.position - startPos : Vector3.zero);
            cardOriginalScales.Add(card != null ? card.transform.localScale : Vector3.one);
        }

        bool spawnedNew = false;

        float t = 0f;
        while (t < exitDuration)
        {
            float normalized = t / exitDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);

            container.transform.position = Vector3.Lerp(startPos, endPos, eased);
            container.transform.localScale = Vector3.Lerp(startScale, endScale, eased);

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == null) continue;
                cards[i].transform.position = Vector3.Lerp(startPos, endPos, eased) + cardOffsets[i];
                cards[i].transform.localScale = cardOriginalScales[i];
            }

            if (normalized >= 0.5f && !spawnedNew)
            {
                spawnedNew = true;
                CardContainer newContainer = SpawnContainer(slot, animate: true);
                if (isLeft) leftContainer = newContainer;
                else rightContainer = newContainer;
            }

            if (normalized > 0.7f)
            {
                float fadeT = (normalized - 0.7f) / 0.3f;
                SetContainerAlpha(container, 1f - fadeT);
                foreach (var card in cards)
                    if (card != null)
                        SetCardAlpha(card, 1f - fadeT);
            }

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator EnterAnimation(CardContainer container, Vector3 targetPosition)
    {
        Vector3 startPos = container.transform.position;
        Vector3 originalScale = container.transform.localScale;
        Vector3 bigScale = originalScale * enterScaleMultiplier;
        container.boxCollider.enabled = false;

        // Fase 1: scivolata in avanti
        float t = 0f;
        while (t < enterSlideDuration)
        {
            float normalized = t / enterSlideDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            container.transform.position = Vector3.Lerp(startPos, targetPosition, eased);
            t += Time.deltaTime;
            yield return null;
        }
        container.transform.position = targetPosition;

        // Fase 2: scala su
        t = 0f;
        while (t < enterScaleDuration)
        {
            float normalized = t / enterScaleDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            container.transform.localScale = Vector3.Lerp(originalScale, bigScale, eased);
            t += Time.deltaTime;
            yield return null;
        }

        // Fase 3: scala giù
        t = 0f;
        while (t < enterScaleDuration)
        {
            float normalized = t / enterScaleDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            container.transform.localScale = Vector3.Lerp(bigScale, originalScale, eased);
            t += Time.deltaTime;
            yield return null;
        }

        container.transform.localScale = originalScale;

        yield return new WaitForSecondsRealtime(0.5f);
        container.boxCollider.enabled = true;
    }

    private void SetContainerAlpha(CardContainer container, float alpha)
    {
        foreach (var rend in container.renderers)
        {
            Color c = rend.material.color;
            c.a = alpha;
            rend.material.color = c;
        }
    }

    private void SetCardAlpha(Card card, float alpha)
    {
        Color c = card.rend.material.color;
        c.a = alpha;
        card.rend.material.color = c;
    }
}