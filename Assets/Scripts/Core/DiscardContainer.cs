using System.Collections.Generic;
using UnityEngine;

public class DiscardContainer : MonoBehaviour
{
    public List<Transform> discardSlots;
    public float slotShiftDuration = 0.3f;
    public float verticalOffsetOnDiscard = 0.15f;
    public float horizontalOffsetOnDiscard = 0.15f;

    private Dictionary<int, Card> slotOccupancy = new Dictionary<int, Card>();
    private Dictionary<Utils.Colors, int> lastSlotByColor = new Dictionary<Utils.Colors, int>();

    void Start()
    {
        InitializeColorTracking();
    }

    private void InitializeColorTracking()
    {
        foreach (Utils.Colors color in System.Enum.GetValues(typeof(Utils.Colors)))
        {
            lastSlotByColor[color] = -1;
        }
    }

    public bool TryPlaceCard(Card card)
    {
        if (discardSlots == null || discardSlots.Count == 0)
            return false;

        Utils.Colors cardColor = card.CardType;
        int lastColorSlot = lastSlotByColor.ContainsKey(cardColor) ? lastSlotByColor[cardColor] : -1;

        int targetSlot;
        if (lastColorSlot == -1)
        {
            targetSlot = -1;
            for (int i = 0; i < discardSlots.Count; i++)
            {
                if (!slotOccupancy.ContainsKey(i))
                {
                    targetSlot = i;
                    break;
                }
            }
            if (targetSlot == -1)
                return false;
        }
        else
        {
            targetSlot = lastColorSlot + 1;
            if (targetSlot >= discardSlots.Count)
                return false;

            if (slotOccupancy.ContainsKey(targetSlot))
            {
                if (!HasRoomToShift(targetSlot))
                    return false;

                ShiftCardsFromSlot(targetSlot);
            }
        }

        // Registra e muovi nella stessa operazione atomica
        slotOccupancy[targetSlot] = card;
        lastSlotByColor[cardColor] = targetSlot;

        card.MoveToDiscardSlot(
            discardSlots[targetSlot].position + Vector3.up * verticalOffsetOnDiscard - Vector3.right * horizontalOffsetOnDiscard,
            slotShiftDuration
        );

        return true;
    }

    private void ShiftCardsFromSlot(int startSlot)
    {
        List<int> slotsToShift = new List<int>();
        foreach (var kvp in slotOccupancy)
        {
            if (kvp.Key >= startSlot)
                slotsToShift.Add(kvp.Key);
        }

        slotsToShift.Sort((a, b) => b.CompareTo(a));

        foreach (int slotIdx in slotsToShift)
        {
            int nextSlot = slotIdx + 1;

            if (nextSlot >= discardSlots.Count)
            {
                Debug.LogWarning("Non c'è spazio per spostare la carta");
                continue;
            }

            Card card = slotOccupancy[slotIdx];
            slotOccupancy.Remove(slotIdx);
            slotOccupancy[nextSlot] = card;

            // Aggiorna lastSlotByColor per questo colore se questa carta era l'ultima
            if (lastSlotByColor.ContainsKey(card.CardType) && lastSlotByColor[card.CardType] == slotIdx)
            {
                lastSlotByColor[card.CardType] = nextSlot;
            }

            card.ShiftToSlot(discardSlots[nextSlot].position, slotShiftDuration, verticalOffsetOnDiscard, horizontalOffsetOnDiscard);
        }
    }


    public void PlaceCardInSlot(Card card, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= discardSlots.Count)
            return;

        slotOccupancy[slotIndex] = card;
        lastSlotByColor[card.CardType] = slotIndex;
    }

    public void RemoveCardFromSlot(Card card)
    {
        int slotIndex = -1;
        foreach (var kvp in slotOccupancy)
        {
            if (kvp.Value == card)
            {
                slotIndex = kvp.Key;
                break;
            }
        }

        if (slotIndex >= 0)
        {
            slotOccupancy.Remove(slotIndex);
        }
    }

    private bool HasRoomToShift(int fromSlot)
    {
        // Trova il blocco contiguo di slot occupati a partire da fromSlot
        int last = fromSlot;
        while (slotOccupancy.ContainsKey(last))
            last++;

        return last < discardSlots.Count;
    }

    public void ResetSlots()
    {
        slotOccupancy.Clear();
        InitializeColorTracking();
    }
}
