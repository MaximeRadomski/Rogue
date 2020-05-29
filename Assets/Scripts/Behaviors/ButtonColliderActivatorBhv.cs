using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonColliderActivatorBhv : MonoBehaviour
{
    public float Xmin, Xmax;
    public float Ymin, Ymax;

    private BoxCollider2D _boxCollider;

    void Start()
    {
        SetPrivates();
    }

    private void SetPrivates()
    {
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var test = Screen.width;
        if (_boxCollider.GetComponent<SpriteRenderer>().isVisible)
        {
            _boxCollider.enabled = true;
        }
        else
        {
            _boxCollider.enabled = false;
        }
        //if (transform.position.x >= Xmin && transform.position.y >= Ymin && transform.position.x <= Xmax && transform.position.y <= Ymax)
        //{
        //    _boxCollider.enabled = true;
        //}
        //else
        //{
        //    _boxCollider.enabled = false;
        //}
    }
}
