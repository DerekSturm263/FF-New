using Quantum;
using System;
using UnityEngine;

[System.Serializable]
public class SerializableWrapper<T>
{
    [SerializeField] private string _name;
    public string Name => _name;
    public void SetName(string name) => _name = name;

    [SerializeField][TextArea] private string _description;
    public string Description => _description;
    public void SetDescription(string description) => _description = description;
    
    [SerializeField] private AssetGuid _guid;
    public AssetGuid Guid => _guid;
    
    [SerializeField] private long _creationDate;
    public long CreationDate => _creationDate;
    
    [SerializeField] private long _lastEditedDate;
    public long LastEditedDate => _lastEditedDate;
    public void SetLastEditedDate(long lastEditedDate) => _lastEditedDate = lastEditedDate;

    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    public void SetIcon(Sprite icon) => _icon = icon;

    [SerializeField] private Sprite _preview;
    public Sprite Preview => _preview;
    public void SetPreview(Sprite preview) => _preview = preview;

    [SerializeField] private T _value;
    public ref T Value => ref _value;

    public SerializableWrapper(T value, string name, string description, AssetGuid guid, long creationDate, long lastEditedDate, Sprite icon = null, Sprite preview = null)
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

    public override bool Equals(object obj)
    {
        if (obj is null || obj is not SerializableWrapper<T>)
            return false;

        return (obj as SerializableWrapper<T>)._guid == _guid;
    }
}
