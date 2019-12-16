﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinContainerBhv : MonoBehaviour
{
    public void SetSkinContainerSortingLayer(string sortingLayerName)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.sortingLayerName = sortingLayerName;
        }
    }

    public void SetSkinContainerSortingLayerOrder(int hundred)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var currentOrder = spriteRenderer.sortingOrder;
                int toSubstract = currentOrder / 100;
                int decimals = currentOrder - (toSubstract * 100);
                spriteRenderer.sortingOrder = (hundred * 100) + decimals;
            }
        }
    }
}
