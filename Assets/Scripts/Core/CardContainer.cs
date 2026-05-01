using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    public int maxCards = 6;
    public List<Transform> cardSlots;
    public List<Renderer> renderers;
    private Utils.Colors containerColor;

    void Awake()
    {
        containerColor = Utils.Colors.Red;

        Color containerC = containerColor switch
        {
            Utils.Colors.Red => Color.red,
            Utils.Colors.Green => Color.green,
            Utils.Colors.Blue => Color.blue,
            Utils.Colors.Yellow => Color.yellow,
            _ => Color.white
        };

        renderers.ForEach(r => r.material = new Material(r.material)
        {
            color = containerC
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Card"))
        {
            Card card = other.GetComponent<Card>();

        }
    }
}
