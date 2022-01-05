using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPController : MonoBehaviour
{
    public int maxHP;
    public int hitPoints;
    public int excellentDamagePoint;
    public int goodDamagePoint;

    private void Awake()
    {
        hitPoints = maxHP;
    }

    public void ExcellentDamage()
    {
        hitPoints -= excellentDamagePoint;
    }

    public void GoodDamage()
    {
        hitPoints -= goodDamagePoint;
    }
}
