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
                return Sprite.Create(_icon, new(0, 0, 256, 256), new(128, 128));
            else
                return null;
        }
    }

    public Sprite Preview
    {
        get
        {
            if (_icon)
                return Sprite.Create(_icon, new(0, 0, _icon.width, _icon.height), new(_icon.width / 2, _icon.height / 2));
            else
                return null;
        }
    }

    [SerializeField] private T _value;
    public T Value => _value;

    public SerializableWrapper(T value, Texture2D icon = null)
    {
        _value = value;
        _icon = icon;
    }

    public static implicit operator T(SerializableWrapper<T> lhs)
    {
        return lhs._value;
    }
}
