using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

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

    private List<Card> cards = new List<Card>();
    private List<float> cardAddTimes = new List<float>();

    public static Conveyor Instance { get; private set; }

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

        discardContainer?.TryPlaceCard(card);
    }

    public bool RemoveCard(Card card)
    {
        int index = cards.IndexOf(card);
        if (index < 0)
            return false;

        cards.RemoveAt(index);
        cardAddTimes.RemoveAt(index);
        return true;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        cardAddTimes.Add(headDistance);

        card.transform.localScale *= cardRescaleFactor;

        card.SetDistance(0f);
    }
}