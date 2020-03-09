using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowBhv : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float LowestIntensity;
    public float Speed;

    private Color _lowestColor;
    private bool _isGoingLow;

    void Start()
    {
        _lowestColor = new Color(LowestIntensity, LowestIntensity, LowestIntensity);
    }

    void Update()
    {
        if (_isGoingLow)
        {
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, _lowestColor, Speed);
            if (Helper.FloatEqualsPrecision(SpriteRenderer.color.r, LowestIntensity, 0.2f))
                _isGoingLow = false;
        }
        else
        {
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Constants.ColorPlain, Speed);
            if (Helper.FloatEqualsPrecision(SpriteRenderer.color.r, 1.0f, 0.2f))
                _isGoingLow = true;
        }
    }
}
