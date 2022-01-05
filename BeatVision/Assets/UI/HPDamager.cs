using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPDamager : MonoBehaviour
{
    [SerializeField] PlayerHPController playerHP;

    [SerializeField] Image HPberG;
    [SerializeField] Image HPberR;

    [SerializeField] private float damageRate;

    private bool startsDamage;

    private float nowHP;

    // Start is called before the first frame update
    void Start()
    {
        nowHP = playerHP.hitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHP.hitPoints < nowHP)
        {
            nowHP = playerHP.hitPoints;
            HPberG.fillAmount = (float)playerHP.hitPoints / playerHP.maxHP;
            startsDamage = true;
        }

        if (startsDamage)
        {
            HPberR.fillAmount -= damageRate * Time.deltaTime;

            if(HPberR.fillAmount <= HPberG.fillAmount)
            {
                HPberR.fillAmount = HPberG.fillAmount;
                startsDamage = false;
            }
        }
    }

    public void Damage()
    {
        HPberG.fillAmount -= damageRate;
        startsDamage = true;
    }

    public void smallDamage()
    {
        HPberG.fillAmount -= damageRate * 0.5f;
        startsDamage = true;
    }
}
