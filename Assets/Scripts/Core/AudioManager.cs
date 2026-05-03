using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource cardJumpSource;
    public AudioSource wrongClickSource;
    public AudioSource containerFullSource;
    public AudioSource levelEndSource;
    public AudioSource bgMusicSource;
    public AudioSource sfxSource;


    [Header("Clips")]
    public AudioClip cardJumpClip;
    public AudioClip wrongClickClip;
    public AudioClip containerFullClip;
    public AudioClip levelEndClip;
    public AudioClip bgMusicClip;
    public AudioClip[] sfxClips;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgMusicSource != null && bgMusicClip != null)
        {
            bgMusicSource.clip = bgMusicClip;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }
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

    public void PlaySFX(int index = -1)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[index]);
        }
    }
}