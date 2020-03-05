using UnityEngine;

class ButtonFightBhv : MonoBehaviour
{
    private InventoryItemType _type;
    private Character _character;
    private ButtonBhv _button;
    private bool _enabled;
    private int _id;

    public void SetPrivates(InventoryItemType type, Character character, int id)
    {
        _type = type;
        _character = character;
        _id = id;
        _button = GetComponent<ButtonBhv>();
        _enabled = true;
    }
}
