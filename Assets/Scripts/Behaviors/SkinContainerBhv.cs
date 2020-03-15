using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinContainerBhv : MonoBehaviour
{
    public float SkinContainerYOffset;

    private Shader _shaderGUItext;
    private Shader _originalShader;
    private Color? _originalColor;
    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private Vector3 _hitScale;
    private Vector3 _hitPosition;
    private bool _isResetingHit;

    private void Start()
    {
        _shaderGUItext = Shader.Find("GUI/Text Shader");
        _hitScale = new Vector3(0.8f, 1.15f, 1.0f);
        _hitPosition = new Vector3(0.0f, 0.025f, 0.0f);
    }

    private void Update()
    {
        if (_isResetingHit)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
            transform.position = Vector3.Lerp(transform.position, _originalPosition, 0.2f);
            if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
            {
                transform.localScale = _originalScale;
                transform.position = _originalPosition;
                _isResetingHit = false;
            }
        }
    }

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

    public int GetSkinContainerSortingLayerOrder()
    {
        var spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        var currentOrder = spriteRenderer.sortingOrder;
        int toSubstract = currentOrder / 100;
        int decimals = currentOrder - (toSubstract * 100);
        return decimals;
    }

    public void SetSkinContainerMaskInteraction(SpriteMaskInteraction maskInteraction)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.maskInteraction = maskInteraction;
        }
    }

    public void OnHit()
    {
        _originalShader = null;
        _originalColor = null;
        _originalPosition = transform.position;
        _originalScale = transform.localScale;
        //for (int i = 0; i < transform.childCount; ++i)
        //{
        //    var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
        //    if (_originalShader == null)
        //        _originalShader = spriteRenderer.material.shader;
        //    if (_originalColor == null)
        //        _originalColor = spriteRenderer.color;
        //    spriteRenderer.material.shader = _shaderGUItext;
        //    spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        //}
        transform.localScale = new Vector3(transform.localScale.x * _hitScale.x, _hitScale.y, _hitScale.z);
        transform.position = _originalPosition + _hitPosition;
        _isResetingHit = true;
        //Invoke(nameof(ResetOnHit), 0.05f);
    }

    public void ResetOnHit()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            spriteRenderer.material.shader = _originalShader;
            spriteRenderer.color = _originalColor ?? Constants.ColorPlain;
        }
    }

    public void OrientToTarget(float x)
    {
        if (x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
