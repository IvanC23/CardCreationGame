using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public Card cardPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public float jumpDuration = 0.5f;
    public float alignDuration = 0.25f;
    public float height = 1.5f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCard();
        }
    }

    void SpawnCard()
    {
        Card c = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);

        c.Init(Color.blue);

        c.MoveToSlot(targetPoint, height, jumpDuration, alignDuration);
    }
}
