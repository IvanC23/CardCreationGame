using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform targetPoint;
    public float jumpDuration = 0.5f;
    public float alignDuration = 0.25f;
    public float height = 1.5f;

    public Transform slot1;
    public Transform slot2;
    public Transform slot3;
    public Transform slot4;

    public float slotShiftDuration = 0.3f;

    public int minNumberOfCardsPerGroup = 2;
    public int maxNumberOfCardsPerGroup = 7;

    public int minNumberOfCardsPerPile = 20;
    public int maxNumberOfCardsPerPile = 40;

    public float zOffsetPerCard = 0.01f;
    public float yOffsetPerCard = 0.01f;

    private List<List<Card>> piles = new List<List<Card>>();
    private List<List<CardGroup>> pileGroups = new List<List<CardGroup>>();

    public static CardManager Instance { get; private set; }

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
        Transform[] slots = { slot1, slot2, slot3, slot4 };
        foreach (Transform slot in slots)
        {
            List<Card> pile = GeneratePile(slot);
            piles.Add(pile);
            pileGroups.Add(BuildGroups(pile));
        }
    }

    private List<Card> GeneratePile(Transform slot)
    {
        List<Card> pile = new List<Card>();

        int totalCards = Random.Range(minNumberOfCardsPerPile, maxNumberOfCardsPerPile + 1);
        List<Utils.Colors> sequence = GenerateColorSequence(totalCards);

        for (int i = 0; i < sequence.Count; i++)
        {
            Vector3 position = slot.position
                - Vector3.up * i * yOffsetPerCard
                - Vector3.forward * i * zOffsetPerCard;

            GameObject go = Instantiate(cardPrefab, position, slot.rotation);
            Card card = go.GetComponent<Card>();
            card.Init(sequence[i]);

            pile.Add(card);
        }

        return pile;
    }

    private List<Utils.Colors> GenerateColorSequence(int totalCards)
    {
        List<Utils.Colors> sequence = new List<Utils.Colors>();

        while (sequence.Count < totalCards)
        {
            int remaining = totalCards - sequence.Count;
            int groupSize = Random.Range(minNumberOfCardsPerGroup, Mathf.Min(maxNumberOfCardsPerGroup, remaining) + 1);

            Utils.Colors color = Utils.RandomColor();

            // Evita due gruppi contigui dello stesso colore
            if (sequence.Count > 0 && color == sequence[sequence.Count - 1])
            {
                color = Utils.RandomColorExcluding(color);
            }

            for (int i = 0; i < groupSize; i++)
                sequence.Add(color);
        }

        return sequence;
    }

    private List<CardGroup> BuildGroups(List<Card> pile)
    {
        List<CardGroup> groups = new List<CardGroup>();
        if (pile.Count == 0) return groups;

        int i = 0;
        while (i < pile.Count)
        {
            Utils.Colors currentColor = pile[i].CardType;
            List<Card> groupCards = new List<Card>();

            while (i < pile.Count && pile[i].CardType == currentColor)
            {
                groupCards.Add(pile[i]);
                i++;
            }

            groups.Add(new CardGroup(groupCards, currentColor));
        }

        return groups;
    }

    // Aggiunte al CardManager

    public IEnumerator SendTopGroup(int pileIndex, System.Action onSent = null, System.Action onComplete = null)
    {

        if (pileIndex < 0 || pileIndex >= piles.Count)
        {
            onComplete?.Invoke();
            AudioManager.Instance.PlayWrongClick();
            yield break;
        }

        List<Card> pile = piles[pileIndex];
        List<CardGroup> groups = pileGroups[pileIndex];

        if (pile.Count == 0 || groups.Count == 0)
        {
            onComplete?.Invoke();
            AudioManager.Instance.PlayWrongClick();
            yield break;
        }

        CardGroup topGroup = groups[0];

        if (!Conveyor.Instance.CanAddCards(topGroup.numberOfCards))
        {
            onComplete?.Invoke();
            AudioManager.Instance.PlayWrongClick();
            yield break;
        }

        float staggerDelay = 0.08f;

        onSent?.Invoke();

        Conveyor.Instance.PlaySmoke();

        for (int i = 0; i < topGroup.cards.Count; i++)
        {
            Card card = topGroup.cards[i];
            card.MoveToSlot(targetPoint, height, jumpDuration, alignDuration);

            AudioManager.Instance.PlayCardJump();

            if (i < topGroup.cards.Count - 1)
                yield return new WaitForSeconds(staggerDelay);
        }

        // Aspetta anche l'ultima carta prima di riposizionare
        yield return new WaitForSeconds(staggerDelay);

        // Rimuovi il gruppo dalla pila
        foreach (Card card in topGroup.cards)
            pile.Remove(card);
        groups.RemoveAt(0);

        // Riposiziona le carte rimaste
        RepositionPile(pileIndex);

        yield return new WaitForSeconds(0.3f);

        onComplete?.Invoke();
    }

    private void RepositionPile(int pileIndex)
    {
        List<Card> pile = piles[pileIndex];
        Transform slot = GetSlot(pileIndex);

        for (int i = 0; i < pile.Count; i++)
        {
            Vector3 targetPosition = slot.position
                - Vector3.up * i * yOffsetPerCard
                - Vector3.forward * i * zOffsetPerCard;

            pile[i].ShiftToSlot(targetPosition, slotShiftDuration);
        }
    }

    private Transform GetSlot(int pileIndex)
    {
        return pileIndex switch
        {
            0 => slot1,
            1 => slot2,
            2 => slot3,
            3 => slot4,
            _ => null
        };
    }
}

public class CardGroup
{
    public List<Card> cards;
    public int numberOfCards;
    public int cardsToDiscard;
    public Utils.Colors color;

    public CardGroup(List<Card> cards, Utils.Colors color)
    {
        this.cards = cards;
        this.color = color;
        this.numberOfCards = cards.Count;
        this.cardsToDiscard = numberOfCards;
    }

    public void OnCardDelivered()
    {
        cardsToDiscard = Mathf.Max(0, cardsToDiscard - 1);
    }

    public bool IsComplete => cardsToDiscard == 0;
}