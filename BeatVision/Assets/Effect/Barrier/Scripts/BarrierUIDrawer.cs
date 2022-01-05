using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierUIDrawer : MonoBehaviour
{
    [SerializeField] private BarrierController barrierController;

    [SerializeField] private Image gaugeImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gaugeImage.fillAmount = barrierController.barrierGauge / barrierController.maxBarrierGauge;
    }
}
