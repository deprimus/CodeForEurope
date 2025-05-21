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

    public void Play(AudioClip clip)
    {
        _src.PlayOneShot(clip);
    }
}
