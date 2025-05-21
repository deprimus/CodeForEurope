using NaughtyAttributes;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource _src;

    public AudioClip[] yes;
    public AudioClip[] hmm;
    public AudioClip[] no;

    public AudioClip select;
    public AudioClip flip;
    public AudioClip appear;
    public AudioClip appear2;

    public static SoundManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        _src.PlayOneShot(clip);
    }
}
