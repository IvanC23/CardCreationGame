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
        Conveyor.Instance?.RemoveCard(card);
        card.MoveToContainerSlot(slot, slotJumpHeight, slotMoveDuration);
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
