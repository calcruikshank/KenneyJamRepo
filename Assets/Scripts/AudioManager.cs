using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton;

    [SerializeField] public List<AudioClip> steps;
    [SerializeField] public List<AudioClip> impact;

    [SerializeField] AudioSource soundFXAudioSource;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
    }

    public void PlayRandomCarpetStep()
    {
        soundFXAudioSource.clip = GetRandomClipFromGivenList(steps);

        soundFXAudioSource.Play();
    }
    public void PlayRandomImpact()
    {
        soundFXAudioSource.clip = GetRandomClipFromGivenList(impact);

        soundFXAudioSource.Play();
    }

    internal AudioClip GetRandomClipFromGivenList(List<AudioClip> clipsSent)
    {
        return clipsSent[UnityEngine.Random.Range(0, clipsSent.Count)];
    }
}
