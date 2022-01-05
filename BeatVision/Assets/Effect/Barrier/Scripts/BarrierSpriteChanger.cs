using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpriteChanger : MonoBehaviour
{
    [SerializeField] private BarrierController barrierController;

    [SerializeField] private Sprite[] barrierSprites;

    SpriteRenderer spriteRenderer;

    private int spritesCount; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spritesCount = barrierSprites.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        int spriteNum = spritesCount - Mathf.CeilToInt((barrierController.barrierGauge / barrierController.maxBarrierGauge) * spritesCount);
        spriteRenderer.sprite = barrierSprites[spriteNum];
    }
}
