using Quantum;
using UnityEngine;

[System.Serializable]
public class SerializableWrapper<T>
{
    [SerializeField] private string _name;
    public string Name => _name;
    public void SetName(string name) => _name = name;

    [SerializeField] private string _description;
    public string Description => _description;
    public void SetDescription(string description) => _description = description;
    
    [SerializeField] private AssetGuid _guid;
    public AssetGuid Guid => _guid;
    
    [SerializeField] private long _creationDate;
    public long CreationDate => _creationDate;
    
    [SerializeField] private long _lastEditedDate;
    public long LastEditedDate => _lastEditedDate;
    public void SetLastEditedDate(long lastEditedDate) => _lastEditedDate = lastEditedDate;

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
    public void SetIcon(Texture2D icon) => _icon = icon;

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
    public void SetPreview(Texture2D preview) => _preview = preview;

    [SerializeField] private T _value;
    public T Value => _value;

    public SerializableWrapper(T value, string name, string description, AssetGuid guid, long creationDate, long lastEditedDate, Texture2D icon = null, Texture2D preview = null)
    {
        _value = value;
        _name = name;
        _description = description;
        _guid = guid;
        _creationDate = creationDate;
        _lastEditedDate = lastEditedDate;
        _icon = icon;
        _preview = preview;
    }

    public static implicit operator T(SerializableWrapper<T> lhs)
    {
        return lhs._value;
    }
}
