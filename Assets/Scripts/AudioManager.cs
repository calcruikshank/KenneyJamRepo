using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton;

    [SerializeField] public List<AudioClip> carpetSteps;

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
        soundFXAudioSource.clip = GetRandomClipFromGivenList(carpetSteps);

        soundFXAudioSource.Play();
    }

    internal AudioClip GetRandomClipFromGivenList(List<AudioClip> clipsSent)
    {
        return clipsSent[UnityEngine.Random.Range(0, clipsSent.Count)];
    }
}
