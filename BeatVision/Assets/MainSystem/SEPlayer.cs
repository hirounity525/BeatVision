using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct AudioData
{
    public string audioName;
    public AudioClip SEClip;
}

[RequireComponent(typeof(AudioSource))]
public class SEPlayer : MonoBehaviour
{
    [SerializeField] private AudioData[] audioDatas;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SEPlay(string audioName)
    {
        AudioClip audioClip = audioDatas.FirstOrDefault(clip => clip.audioName == audioName).SEClip;

        if(audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
