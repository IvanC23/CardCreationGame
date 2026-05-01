using System.Collections.Generic;
using UnityEngine;

public class DiscardContainer : MonoBehaviour
{
    public List<Transform> discardSlots;

    private int nextSlotIndex = 0;

    public Transform GetNextSlot()
    {
        if (discardSlots == null || discardSlots.Count == 0)
            return null;

        if (nextSlotIndex >= discardSlots.Count)
            return null;

        return discardSlots[nextSlotIndex++];
    }

    public void ResetSlots()
    {
        nextSlotIndex = 0;
    }
}
