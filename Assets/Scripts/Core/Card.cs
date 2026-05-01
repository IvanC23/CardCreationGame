using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Renderer rend;
    private Color cardColor;
    private bool onConveyor = false;

    public void Init(Color color)
    {
        cardColor = color;
        ApplyColor();
    }

    void ApplyColor()
    {
        rend.material = new Material(rend.material)
        {
            color = cardColor
        };
    }

    public void MoveToSlot(Transform slot, float jumpHeight, float jumpDuration = 0.5f, float alignDuration = 0.25f)
    {
        Vector3 midPoint = slot.position + Vector3.up * jumpHeight;

        StartCoroutine(MoveSequence(midPoint, slot, jumpDuration, alignDuration));
    }

    private IEnumerator MoveSequence(Vector3 mid, Transform slot, float jumpDuration = 0.5f, float alignDuration = 0.25f)
    {
        yield return StartCoroutine(JumpPhase(mid, jumpDuration));
        yield return StartCoroutine(AlignPhase(slot, alignDuration));
    }

    public void MoveToDiscardSlot(Vector3 slotPosition, float duration = 0.35f)
    {
        StopAllCoroutines();
        StartCoroutine(DiscardTransition(slotPosition, duration));
    }

    private IEnumerator DiscardTransition(Vector3 slotPosition, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startX = transform.eulerAngles.x;
        Quaternion targetRot = Quaternion.Euler(startX, 90f, 0f);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPos, slotPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(slotPosition, targetRot);
    }

    private IEnumerator JumpPhase(Vector3 target, float duration)
    {
        Vector3 start = transform.position;

        float height = 1.5f;

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * height;

            transform.position = pos;

            float arc = Mathf.Sin(t * Mathf.PI);

            // 🔥 curva reale (salita → discesa)
            float tiltX = Mathf.Lerp(-20f, 50f, t * t);
            float tiltZ = arc * 5f;

            transform.rotation = Quaternion.Euler(tiltX, 0f, tiltZ);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator AlignPhase(Transform slot, float duration)
    {
        Vector3 start = transform.position;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = slot.rotation;

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            // 🔥 rotazione anticipata (più veloce della posizione)
            float rotT = Mathf.Pow(t, 0.5f);

            transform.position = Vector3.Lerp(start, slot.position, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, rotT);

            time += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(slot.position, targetRot);

        // 🔥 AGGANCIO
        onConveyor = true;

        Conveyor.Instance.AddCard(this);
    }

    public void SetDistance(float d)
    {
        onConveyor = true;
    }

    public void SetConveyorTransform(Vector3 pos, Vector3 tangent)
    {
        if (!onConveyor) return;

        transform.position = pos + Vector3.up;

        if (tangent.sqrMagnitude < 0.0001f)
            return;

        Quaternion flowRot = Quaternion.LookRotation(tangent);

        Quaternion baseTilt = Quaternion.Euler(90f, 0f, 0f);

        transform.rotation = flowRot * baseTilt;
    }
}