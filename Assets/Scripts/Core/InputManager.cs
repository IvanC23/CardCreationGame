using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask slotLayer;
    public RectTransform handCursor;
    public float clickScaleMultiplier = 1.3f;
    public float clickScaleDuration = 0.15f;

    private HashSet<int> lockedPiles = new HashSet<int>();
    private Vector3 handOriginalScale;
    private Coroutine handAnimCoroutine;

    void Start()
    {
        Cursor.visible = false;
        if (handCursor != null)
            handOriginalScale = handCursor.localScale;
    }

    void Update()
    {
        if (handCursor != null)
            handCursor.position = Input.mousePosition;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayer))
        {
            Slot slot = hit.collider.GetComponent<Slot>();

            if (slot == null || lockedPiles.Count != 0)
            {
                AudioManager.Instance.PlayWrongClick();
                return;
            }


            lockedPiles.Add(slot.pileIndex);
            StartCoroutine(CardManager.Instance.SendTopGroup(
                slot.pileIndex,
                onSent: () =>
                {
                    if (handAnimCoroutine != null) StopCoroutine(handAnimCoroutine);
                    handAnimCoroutine = StartCoroutine(PunchHand());
                },
                onComplete: () => lockedPiles.Remove(slot.pileIndex)
            ));
        }
    }

    private IEnumerator PunchHand()
    {
        Vector3 bigScale = handOriginalScale * clickScaleMultiplier;

        float t = 0f;
        while (t < clickScaleDuration)
        {
            float normalized = t / clickScaleDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            handCursor.localScale = Vector3.Lerp(handOriginalScale, bigScale, eased);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < clickScaleDuration)
        {
            float normalized = t / clickScaleDuration;
            float eased = normalized * normalized * (3f - 2f * normalized);
            handCursor.localScale = Vector3.Lerp(bigScale, handOriginalScale, eased);
            t += Time.deltaTime;
            yield return null;
        }

        handCursor.localScale = handOriginalScale;
        handAnimCoroutine = null;
    }
}