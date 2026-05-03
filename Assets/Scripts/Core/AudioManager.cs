using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource cardJumpSource;
    public AudioSource wrongClickSource;
    public AudioSource containerFullSource;
    public AudioSource levelEndSource;

    [Header("Clips")]
    public AudioClip cardJumpClip;
    public AudioClip wrongClickClip;
    public AudioClip containerFullClip;
    public AudioClip levelEndClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayCardJump()
    {
        cardJumpSource.PlayOneShot(cardJumpClip);
    }

    public void PlayWrongClick()
    {
        wrongClickSource.PlayOneShot(wrongClickClip);
    }

    public void PlayContainerFull()
    {
        containerFullSource.PlayOneShot(containerFullClip);
    }

    public void PlayLevelEnd()
    {
        levelEndSource.PlayOneShot(levelEndClip);
    }
}