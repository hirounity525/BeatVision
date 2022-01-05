using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [SerializeField] private InputProvider playerInput;

    public bool isBarrierBroken;

    public float maxBarrierGauge;
    public float barrierGauge;
    [SerializeField] private float decreaseRate;
    [SerializeField] private float increaseRate;

    private void Awake()
    {
        barrierGauge = maxBarrierGauge;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.left)
        {
            barrierGauge -= decreaseRate * Time.deltaTime;
        }
        else
        {
            if(barrierGauge < maxBarrierGauge)
            {
                barrierGauge += increaseRate * Time.deltaTime;
            }
            else
            {
                barrierGauge = maxBarrierGauge;
            }
        }

        if (barrierGauge > 0)
        {
            isBarrierBroken = false;
        }
        else
        {
            barrierGauge = 0;
            isBarrierBroken = true;
        }
    }
}
