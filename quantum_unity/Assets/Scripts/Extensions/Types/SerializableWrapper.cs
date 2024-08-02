using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public unsafe struct SerializableWrapper<T> where T : unmanaged
{
    [SerializeField] private string _name;
    public readonly string Name => _name;
    public void SetName(string name) => _name = name;

    [SerializeField][TextArea] private string _description;
    public readonly string Description => _description;
    public void SetDescription(string description) => _description = description;

    [SerializeField] private long _creationDate;
    public readonly long CreationDate => _creationDate;

    [SerializeField] private long _lastEditedDate;
    public readonly long LastEditedDate => _lastEditedDate;
    public void SetLastEditedDate(long lastEditedDate) => _lastEditedDate = lastEditedDate;

    [SerializeField] private AssetGuid _fileID;
    public readonly AssetGuid FileID => _fileID;

    [SerializeField] private bool _madeByPlayer;
    public readonly bool MadeByPlayer => _madeByPlayer;

    [SerializeField] private Sprite _icon;
    public readonly Sprite Icon => _icon;

    [SerializeField] private Sprite _preview;
    public readonly Sprite Preview => _preview;

    [FormerlySerializedAs("_value")] public T value;

    public SerializableWrapper(T value, string name, string description, long creationDate, long lastEditedDate, AssetGuid fileID, bool madeByPlayer, Sprite icon = null, Sprite preview = null)
    {
        this.value = value;
        _name = name;
        _description = description;
        _creationDate = creationDate;
        _lastEditedDate = lastEditedDate;
        _fileID = fileID;
        _madeByPlayer = madeByPlayer;
        _icon = icon;
        _preview = preview;
    }

    public static implicit operator T(SerializableWrapper<T> lhs)
    {
        return lhs.value;
    }

    public readonly void Save(string directory)
    {
        FusionFighters.Serializer.Save(this, _fileID, directory);
    }

    public readonly void Delete(string directory)
    {
        FusionFighters.Serializer.Delete($"{directory}/{FileID}.json", directory);
    }

    public static SerializableWrapper<T> LoadAs(string directory, AssetGuid fileID)
    {
        return FusionFighters.Serializer.LoadAs<SerializableWrapper<T>>($"{directory}/{fileID}.json", directory);
    }
}
