using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Renderer rend;

    private Color cardColor;

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
    }
}