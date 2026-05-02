using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    public int maxCards = 6;
    public List<Transform> cardSlots;
    public List<Renderer> renderers;
    public float slotJumpHeight = 0.4f;
    public float slotMoveDuration = 0.35f;

    private Utils.Colors containerColor;
    private int nextSlotIndex = 0;

    public event System.Action<CardContainer> OnContainerFull;

    private List<Card> hostedCards = new List<Card>();

    // Chiamato da CardContainerManager durante l'animazione di uscita
    public List<Card> GetHostedCards() => hostedCards;

    private int cardsAnimatingCount = 0;
    private bool isFull = false;


    void Awake()
    {
        containerColor = Utils.RandomColor();

        Color containerC = Utils.ToColor(containerColor);

        renderers.ForEach(r => r.material = new Material(r.material)
        {
            color = containerC
        });
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Card"))
            return;

        Card card = other.GetComponent<Card>();
        if (card == null)
            return;

        if (card.CardType != containerColor)
            return;

        Transform slot = GetNextSlot();
        if (slot == null)
            return;

        card.LeaveConveyor();
        hostedCards.Add(card);
        cardsAnimatingCount++;
        card.OnContainerSlotReached += OnCardAnimationComplete;

        Conveyor.Instance?.RemoveCard(card);
        card.MoveToContainerSlot(slot, slotJumpHeight, slotMoveDuration);

        if (nextSlotIndex >= Mathf.Min(maxCards, cardSlots.Count))
            isFull = true;
    }

    private void OnCardAnimationComplete(Card card)
    {
        card.OnContainerSlotReached -= OnCardAnimationComplete;
        cardsAnimatingCount--;

        if (isFull && cardsAnimatingCount == 0)
            OnContainerFull?.Invoke(this);
    }

    public Transform GetNextSlot()
    {
        if (cardSlots == null || cardSlots.Count == 0)
            return null;

        if (nextSlotIndex >= Mathf.Min(maxCards, cardSlots.Count))
            return null;

        return cardSlots[nextSlotIndex++];
    }

    public void ResetSlots()
    {
        nextSlotIndex = 0;
    }
}
