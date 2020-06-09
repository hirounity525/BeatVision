using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPDamager : MonoBehaviour
{
    [SerializeField] Image HPberG;
    [SerializeField] Image HPberR;

    [SerializeField] private float damageRate;

    private bool startsDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
