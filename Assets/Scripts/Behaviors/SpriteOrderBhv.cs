using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderBhv : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteSortingLayerOrder(Constants.InputLayer);
    }

    private void SetSpriteSortingLayerOrder(int hundred)
    {
        if (_spriteRenderer != null)
        {
            var currentOrder = _spriteRenderer.sortingOrder;
            int toSubstract = currentOrder / 100;
            int decimals = currentOrder - (toSubstract * 100);
            _spriteRenderer.sortingOrder = (hundred * 100) + decimals;
        }
    }
}
