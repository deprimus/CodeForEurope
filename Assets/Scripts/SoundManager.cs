// -----------------------------------------------------------------------------
// SoundManager.cs
//
// Centralizes audio playback for UI and game events, ensuring consistent sound effects and music.
// Implements a singleton pattern for global access and persists across scenes.
//
// Main Functions:
// - Play(AudioClip clip): Plays a sound effect.
//
// Fields:
// - yes, hmm, no: Arrays of audio clips for different responses.
// - select, flip, appear, appear2: UI sound effects.
// - _src: AudioSource for playback.
// -----------------------------------------------------------------------------

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

    public void Play(AudioClip clip, float volume = 1f)
    {
        _src.PlayOneShot(clip, volume);
    }
}
