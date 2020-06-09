using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    ParticleSystem effect;

    private void Awake()
    {
        effect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!effect.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
