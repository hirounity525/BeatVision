using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraEffectPlayer : MonoBehaviour
{
    public ObjectPool upperAura;
    public ObjectPool strikeAura;
    public ObjectPool lowerAura;

    GameObject aura;

    public void PlayEffect(Action action)
    {
        switch (action)
        {
            case Action.UPPER:
                aura = upperAura.GetObject();
                aura.transform.position = upperAura.transform.position;
                aura.SetActive(true);
                break;
            case Action.STRIKE:
                aura = strikeAura.GetObject();
                aura.transform.position = upperAura.transform.position;
                aura.SetActive(true);
                break;
            case Action.LOWER:
                aura = lowerAura.GetObject();
                aura.transform.position = upperAura.transform.position;
                aura.SetActive(true);
                break;
        }
    }
}
