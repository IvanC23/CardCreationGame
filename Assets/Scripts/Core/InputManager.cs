using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask slotLayer;

    private HashSet<int> lockedPiles = new HashSet<int>();

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayer))
        {
            Slot slot = hit.collider.GetComponent<Slot>();
            if (slot == null || lockedPiles.Count != 0)
                return;

            lockedPiles.Add(slot.pileIndex);
            StartCoroutine(CardManager.Instance.SendTopGroup(slot.pileIndex, () => lockedPiles.Remove(slot.pileIndex)));
        }
    }
}