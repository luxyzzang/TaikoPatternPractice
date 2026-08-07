using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;

    [Header("Hit Sound")]
    public AudioClip donSound;
    public AudioClip katSound;
    public AudioClip beatSound;

    private void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }


    public void PlayDon() => audioSource.PlayOneShot(donSound);


    public void PlayKat() => audioSource.PlayOneShot(katSound);

    public void PlayBeatSound() => audioSource.PlayOneShot(beatSound);
}