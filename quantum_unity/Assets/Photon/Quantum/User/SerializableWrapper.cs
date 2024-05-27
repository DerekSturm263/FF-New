using UnityEngine;

[System.Serializable]
public class SerializableWrapper<T>
{
    [SerializeField] private Texture2D _icon;
    public Sprite Icon
    {
        get
        {
            if (_icon)
            {
                int size = Mathf.Min(_icon.width, _icon.height);
                return Sprite.Create(_icon, new(0, 0, size, size), new(size / 2, size / 2));
            }
            else
                return null;
        }
    }

    [SerializeField] private Texture2D _preview;
    public Sprite Preview
    {
        get
        {
            if (_preview)
                return Sprite.Create(_icon, new(0, 0, _preview.width, _preview.height), new(_preview.width / 2, _preview.height / 2));
            else
                return null;
        }
    }

    [SerializeField] private T _value;
    public T Value => _value;

    public SerializableWrapper(T value, Texture2D icon = null, Texture2D preview = null)
    {
        _value = value;
        _icon = icon;
        _preview = preview;
    }

    public static implicit operator T(SerializableWrapper<T> lhs)
    {
        return lhs._value;
    }
}
