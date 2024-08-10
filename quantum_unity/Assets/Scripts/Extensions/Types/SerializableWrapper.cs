using Extensions.Types;
using Quantum;
using System.IO;
using UnityEngine;

[System.Serializable]
public unsafe struct SerializableWrapper<T> where T : struct
{
    public T value;

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

    [SerializeField] private string _iconFilePath;
    public readonly string IconFilePath => _iconFilePath;

    [SerializeField] private Sprite _iconOverride;

    private Sprite _icon;
    public Sprite Icon
    {
        get
        {
            if (_iconOverride)
                return _iconOverride;

            if (!_icon)
            {
                if (File.Exists(_iconFilePath))
                {
                    byte[] fileData = File.ReadAllBytes(_iconFilePath);

                    Texture2D iconTexture = new(512, 512, TextureFormat.RGBA32, false, true);
                    iconTexture.LoadImage(fileData);

                    _icon = Sprite.Create(iconTexture, new(0, 0, iconTexture.width, iconTexture.height), Vector2.one);
                }
                else
                {
                    _icon = Sprite.Create(Texture2D.whiteTexture, new(0, 0, Texture2D.whiteTexture.width, -Texture2D.whiteTexture.height), Vector2.one);
                }
            }

            return _icon;
        }
    }

    [SerializeField] private Sprite _preview;
    public readonly Sprite Preview => _preview;

    [SerializeField] private string[] _filterTags;
    public readonly string[] FilterTags => _filterTags;

    [SerializeField] private Tuple<string, string>[] _groupTags;
    public readonly Tuple<string, string>[] GroupTags => _groupTags;

    public SerializableWrapper(T value, string name, string description, long creationDate, long lastEditedDate, AssetGuid fileID, string[] filterTags, Tuple<string, string>[] groupTags, string iconFilePath, Sprite preview)
    {
        this.value = value;
        _name = name;
        _description = description;
        _creationDate = creationDate;
        _lastEditedDate = lastEditedDate;
        _fileID = fileID;
        _madeByPlayer = true;
        _iconFilePath = iconFilePath;
        _iconOverride = null;
        _icon = null;
        _preview = preview;
        _filterTags = filterTags;
        _groupTags = groupTags;
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
