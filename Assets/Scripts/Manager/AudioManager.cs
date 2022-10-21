using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource clickBtnAudioSource;
    [SerializeField] AudioSource mergeAudioSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayClickBtnAudioSource()
    {
        clickBtnAudioSource.Play();
    }

    public void PlayMergeAudioSource()
    {
        mergeAudioSource.Play();
    }

}
