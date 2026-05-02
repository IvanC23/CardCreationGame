using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class Conveyor : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 2f;
    public float spacing = 0.5f;
    public float verticalOffset = 0.1f;
    public float cardRescaleFactor = 0.8f;

    public DiscardContainer discardContainer;
    public float discardTransitionDuration = 0.35f;
    public float exitThreshold = 0.98f;
    public float verticalOffsetOnDiscard = 0.15f;
    public float horizontalOffsetOnDiscard = 0.15f;

    private float headDistance;
    private float splineLength;

    public int MaxCardsOnConveyor = 20;
    public TMP_Text cardCountText;
    public Slider cardCountSlider;

    private List<Card> cards = new List<Card>();
    private List<float> cardAddTimes = new List<float>();

    public static Conveyor Instance { get; private set; }

    private bool isFlashingWarning = false;

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
        splineLength = Mathf.Max(0.0001f, spline.CalculateLength());

        UpdateCardCountUI();
    }

    void Update()
    {
        headDistance += speed * Time.deltaTime;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            float distSinceAdded = headDistance - cardAddTimes[i];
            float dist = distSinceAdded - i * spacing;

            if (dist < 0f)
                continue; // ❗ evita schiacciare tutto all'inizio

            if (dist >= splineLength * exitThreshold)
            {
                EjectCard(i);
                continue;
            }

            dist = Mathf.Min(dist, splineLength);
            float t = dist / splineLength;

            Vector3 localPos = spline.Spline.EvaluatePosition(t);
            Vector3 localTan = spline.Spline.EvaluateTangent(t);

            Vector3 worldPos = spline.transform.TransformPoint(localPos) + Vector3.up * verticalOffset;
            Vector3 worldTan = spline.transform.TransformDirection(localTan);

            cards[i].SetConveyorTransform(worldPos, worldTan);
        }
    }

    private void EjectCard(int index)
    {
        Card card = cards[index];
        cards.RemoveAt(index);
        cardAddTimes.RemoveAt(index);

        UpdateCardCountUI();

        discardContainer?.TryPlaceCard(card);
    }

    public bool RemoveCard(Card card)
    {
        int index = cards.IndexOf(card);
        if (index < 0)
            return false;

        cards.RemoveAt(index);
        cardAddTimes.RemoveAt(index);
        UpdateCardCountUI();
        return true;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        cardAddTimes.Add(headDistance);

        card.transform.localScale *= cardRescaleFactor;

        card.SetDistance(0f);
        UpdateCardCountUI();
    }

    private void UpdateCardCountUI()
    {
        if (cardCountText != null)
            cardCountText.text = $"{cards.Count} / {MaxCardsOnConveyor}";

        if (cardCountSlider != null)
            cardCountSlider.value = (float)cards.Count / MaxCardsOnConveyor;

    }

    internal bool CanAddCards(int count)
    {
        if (cards.Count + count > MaxCardsOnConveyor)
            FlashFullWarning();
        return cards.Count + count <= MaxCardsOnConveyor;
    }

    public void FlashFullWarning()
    {
        if (isFlashingWarning) return;
        isFlashingWarning = true;
        StartCoroutine(nameof(PulseText));
    }

    private IEnumerator PulseText()
    {
        if (cardCountText == null) yield break;

        int pulses = 1;
        float pulseDuration = 0.25f;
        Vector3 originalScale = cardCountText.transform.localScale;
        Color originalColor = cardCountText.color;
        Color warningColor = Color.red;
        float scaleMultiplier = 1.25f;

        for (int i = 0; i < pulses; i++)
        {
            // Espandi e colora di rosso
            float t = 0f;
            while (t < pulseDuration)
            {
                float normalized = t / pulseDuration;
                cardCountText.transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, normalized);
                cardCountText.color = Color.Lerp(originalColor, warningColor, normalized);
                t += Time.deltaTime;
                yield return null;
            }

            // Ritorna alla normalità
            t = 0f;
            while (t < pulseDuration)
            {
                float normalized = t / pulseDuration;
                cardCountText.transform.localScale = Vector3.Lerp(originalScale * scaleMultiplier, originalScale, normalized);
                cardCountText.color = Color.Lerp(warningColor, originalColor, normalized);
                t += Time.deltaTime;
                yield return null;
            }
        }

        cardCountText.transform.localScale = originalScale;
        cardCountText.color = originalColor;
        isFlashingWarning = false;
    }
}