using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioData[] audioDatas;
    [SerializeField] private float maxVolume;
    [SerializeField] private float fadeTime;

    private AudioSource audioSource;

    private Tweener tweener;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(string audioName)
    {
        tweener.Kill();

        audioSource.Stop();
        audioSource.volume = 0;

        AudioClip audioClip = audioDatas.FirstOrDefault(clip => clip.audioName == audioName).SEClip;

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            tweener = audioSource.DOFade(maxVolume, fadeTime).SetEase(Ease.InQuad);
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
