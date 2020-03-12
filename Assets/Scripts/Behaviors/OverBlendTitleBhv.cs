using UnityEngine;

public class OverBlendTitleBhv : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _backgroundRenderer;
    private TMPro.TextMeshPro _title;
    private System.Func<bool, object> _resultAction;

    private Vector3 _startPosition;
    private Vector3 _slideStartPosition;
    private Vector3 _slideEndPosition;
    private Vector3 _endPosition;
    private int _state; // 0:Start | 1:SlideStart | 2: SlideEnd | 3:End
    private Color _backgroundPlain;
    private Color _backgroundTransparent;

    private float _moveSpeed;
    private float _slideSpeed;
    private float _fadeSpeed;

    public void SetPrivates(string title, System.Func<bool, object> resultAction, Direction mainDirection, Direction secondaryDirection)
    {
        _startPosition = new Vector3(0.0f, 0.0f, 0.0f);
        AddDirection(mainDirection);
        if (secondaryDirection != Direction.None)
            AddDirection(secondaryDirection);
        _endPosition = new Vector3(-_startPosition.x, -_startPosition.y, 0.0f);
        var slidingBit = 0.01f;
        _slideStartPosition = new Vector3(_startPosition.x * slidingBit, -_startPosition.y * slidingBit, 0.0f);
        _slideEndPosition = new Vector3(_endPosition.x * slidingBit, -_endPosition.y * slidingBit, 0.0f);
        _state = 0;
        _resultAction = resultAction;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Constants.ColorPlainTransparent;
        _backgroundRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        _backgroundPlain = _backgroundRenderer.color;
        _backgroundTransparent = new Color(_backgroundPlain.r, _backgroundPlain.g, _backgroundPlain.b, 0.0f);
        _backgroundRenderer.color = _backgroundTransparent;
        _title = transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _title.text = title;
        _moveSpeed = 0.15f;
        _slideSpeed = 0.05f;
        _fadeSpeed = 0.2f;
        transform.position = _startPosition;
    }

    private void AddDirection(Direction direction)
    {
        if (direction == Direction.Up)
            _startPosition += new Vector3(0.0f, -6.0f, 0.0f);
        else if (direction == Direction.Down)
            _startPosition += new Vector3(0.0f, 6.0f, 0.0f);
        if (direction == Direction.Right)
            _startPosition += new Vector3(-6.0f, 0.0f, 0.0f);
        else if (direction == Direction.Left)
            _startPosition += new Vector3(6.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        if (_state == 0)
        {
            transform.position = Vector3.Lerp(transform.position, _slideStartPosition, _moveSpeed);
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlain, _fadeSpeed);
            _backgroundRenderer.color = Color.Lerp(_backgroundRenderer.color, _backgroundPlain, _fadeSpeed);
            if (Helper.VectorEqualsPrecision(transform.position, _slideStartPosition, 0.01f))
            {
                transform.position = _slideStartPosition;
                _spriteRenderer.color = Constants.ColorPlain;
                _backgroundRenderer.color = _backgroundPlain;
                _state = 1;
            }
        }
        else if (_state == 1)
        {
            transform.position = Vector3.Lerp(transform.position, _slideEndPosition, _slideSpeed);
            if (Helper.VectorEqualsPrecision(transform.position, _slideEndPosition, 0.05f))
            {
                transform.position = _slideEndPosition;
                _state = 2;
            }
        }
        else if (_state == 2)
        {
            transform.position = Vector3.Lerp(transform.position, _endPosition, _moveSpeed);
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainTransparent, _fadeSpeed);
            _backgroundRenderer.color = Color.Lerp(_backgroundRenderer.color, _backgroundTransparent, _fadeSpeed);
            if (Helper.FloatEqualsPrecision(transform.position.x, _endPosition.x, 0.01f))
            {
                transform.position = _endPosition;
                _spriteRenderer.color = Constants.ColorPlainTransparent;
                _backgroundRenderer.color = _backgroundTransparent;
                _state = 3;
            }
        }
        else if (_state == 3)
        {
            ExitOverBlendTitle();
        }
    }

    public virtual void ExitOverBlendTitle()
    {
        _state = -1;
        Constants.InputLocked = false;
        _resultAction?.Invoke(true);
        Destroy(gameObject);
    }
}
