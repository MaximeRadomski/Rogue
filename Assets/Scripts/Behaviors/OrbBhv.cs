using UnityEngine;

public class OrbBhv : MonoBehaviour
{
    public bool PopText;

    private TMPro.TextMeshPro _textMesh;
    private GameObject _content;
    private GameObject _subContent;
    private float _height;

    GameObject _instantChange;
    GameObject _delayedChange;
    private bool _isDelayingContent;
    private float _delayingSpeed;

    void Start()
    {
        _textMesh = transform.Find(gameObject.name + "Text").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find(gameObject.name + "Mask").Find(gameObject.name + "Content").gameObject;
        _subContent = transform.Find(gameObject.name + "Mask").Find(gameObject.name + "SubContent").gameObject;
        _height = _content.GetComponent<SpriteRenderer>().sprite.rect.size.y * Constants.Pixel;
    }

    private void Update()
    {
        if (_isDelayingContent)
        {
            _delayedChange.transform.position = Vector3.Lerp(_delayedChange.transform.position, _instantChange.transform.position, _delayingSpeed);
            _delayingSpeed *= 1.2f;
            if (Helper.VectorEqualsPrecision(_delayedChange.transform.position, _instantChange.transform.position, 0.005f))
            {
                _delayedChange.transform.position = _instantChange.transform.position;
                _isDelayingContent = false;
            }
        }

        if (_content.transform.position.x + _height  <= transform.position.x)
        {
            _content.transform.position = new Vector3(transform.position.x + _height, _content.transform.position.y, 0.0f);
            _subContent.transform.position = new Vector3(transform.position.x + _height, _subContent.transform.position.y, 0.0f);
        }
        else
        {
            _content.transform.position += new Vector3(-0.005f, 0.0f, 0.0f);
            _subContent.transform.position += new Vector3(-0.005f, 0.0f, 0.0f);
        }
    }

    public void UpdateContent(int current, int max, Instantiator instantiator, TextType textType, int? changingAmount = null, Direction direction = Direction.None)
    {
        if (changingAmount != null && changingAmount != 0 && PopText)
        {
            instantiator.PopText((changingAmount >= 0 ? "+" : "") + changingAmount, transform.position + new Vector3(0.0f, 0.5f, 0.0f), textType);
        }
        _textMesh.text = current.ToString();

        bool isDelaying;
        if (direction == Direction.Up)
        {
            _instantChange = _subContent;
            _delayedChange = _content;
            isDelaying = true;
        }
        else if (direction == Direction.Down)
        {
            _instantChange = _content;
            _delayedChange = _subContent;
            isDelaying = true;
        }
        else
        {
            _instantChange = _content;
            _delayedChange = _subContent;
            isDelaying = false;
        }

        if (isDelaying)
        {
            _delayedChange.transform.position = _instantChange.transform.position;
            _isDelayingContent = true;
            _delayingSpeed = 0.0001f;
        }
        float ratio = (float)current / max;
        _instantChange.transform.position = new Vector3(_instantChange.transform.position.x,
            transform.position.y + (_height * ratio) - _height,
            0.0f);
        if (!isDelaying)
        {
            _delayedChange.transform.position = _instantChange.transform.position;
        }
    }
}
