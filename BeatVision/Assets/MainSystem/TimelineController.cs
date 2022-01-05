using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    PlayableDirector playableDirector;

    public bool isFinish;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (!isFinish)
        {
            if (playableDirector.time >= playableDirector.duration - 0.1f)
            {
                playableDirector.Stop();
                isFinish = true;
            }
        }
    }

    public void PlayTimeline()
    {
        playableDirector.Play();
    }

    public void SkipTimeline(float skipTime)
    {
        playableDirector.time = skipTime;
    }

    public float timelineTime()
    {
        return (float)playableDirector.time;
    }
}
