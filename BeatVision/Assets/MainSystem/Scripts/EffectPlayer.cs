using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [SerializeField] ObjectPool excellentPool;
    [SerializeField] ObjectPool goodPool;

    [SerializeField] Transform playTrans;

    public void PlayExcellentEffect()
    {
        GameObject effect = excellentPool.GetObject();
        effect.transform.position = playTrans.position;
        effect.GetComponent<ParticleSystem>().Play();
    }

    public void PlayGoodEffect()
    {
        GameObject effect = goodPool.GetObject();
        effect.transform.position = playTrans.position;
        effect.GetComponent<ParticleSystem>().Play();
    }
}
