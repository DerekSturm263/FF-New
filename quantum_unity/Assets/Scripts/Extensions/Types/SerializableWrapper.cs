using Extensions.Miscellaneous;
using Extensions.Types;
using Quantum;
using System.IO;
using UnityEngine;

public static class SerializableWrapperHelper
{
    public static RenderTexture IconRT, PreviewRT;

    public static void SetRTs(RenderTexture icon, RenderTexture preview)
    {
        IconRT = icon;
        PreviewRT = preview;
    }
}

[System.Serializable]
public struct SerializableWrapper<T>
{
    public static readonly Vector2Int IconDimensions = new(512, 512);
    public static readonly Vector2Int PreviewDimensions = new(1920, 1080);

    public T value;

    [SerializeField] private string _directory;

    [SerializeField] private string _name;
    public readonly string Name => _name;
    public void SetName(string name) => _name = name;

    [SerializeField][TextArea] private string _description;
    public readonly string Description => _description;
    public void SetDescription(string description) => _description = description;

    [SerializeField] private long _creationDate;
    public readonly System.DateTime CreationDate => new(_creationDate);

    [SerializeField] private long _lastEditedDate;
    public readonly System.DateTime LastEditedDate => new(_lastEditedDate);
    public void SetLastEditedDate(long lastEditedDate) => _lastEditedDate = lastEditedDate;

    [SerializeField] private AssetGuid _fileID;
    public readonly AssetGuid FileID => _fileID;

    [SerializeField] private bool _madeByPlayer;
    public readonly bool MadeByPlayer => _madeByPlayer;

    [SerializeField] private Sprite _iconOverride;

    private Sprite _icon;
    public Sprite Icon
    {
        get
        {
            if (_iconOverride)
                return _iconOverride;

            if (!_icon)
                _icon = Helper.SpriteFromScreenshot($"{_directory}/{FileID}_ICON.png", IconDimensions.x, IconDimensions.y, TextureFormat.RGBA32, true);
            else
                Debug.Log("Icon already exists!");

            return _icon;
        }
    }

    [SerializeField] private Sprite _previewOverride;

    private Sprite _preview;
    public Sprite Preview
    {
        get
        {
            if (_previewOverride)
                return _previewOverride;

            if (!_preview)
                _preview = Helper.SpriteFromScreenshot($"{_directory}/{FileID}_PREVIEW.png", PreviewDimensions.x, PreviewDimensions.y, TextureFormat.RGBA32, true);
            else
                Debug.Log("Preview already exists!");

            return _preview;
        }
    }

    [SerializeField] private string[] _filterTags;
    public readonly string[] FilterTags => _filterTags;

    [SerializeField] private Tuple<string, string>[] _groupTags;
    public readonly Tuple<string, string>[] GroupTags => _groupTags;

    public readonly bool IsValid => _fileID != AssetGuid.Invalid;

    public SerializableWrapper(T value, string directory, string name, string description, AssetGuid fileID, string[] filterTags, Tuple<string, string>[] groupTags)
    {
        this.value = value;
        _directory = directory;
        _name = name;
        _description = description;
        _creationDate = System.DateTime.Now.Ticks;
        _lastEditedDate = System.DateTime.Now.Ticks;
        _fileID = fileID;
        _madeByPlayer = true;
        _iconOverride = null;
        _icon = null;
        _previewOverride = null;
        _preview = null;
        _filterTags = filterTags;
        _groupTags = groupTags;
    }

    public readonly void CreateIcon(Camera camera, Shader shader = null, RenderTexture output = null)
    {
        camera.RenderToScreenshot($"{_directory}/{FileID}_ICON.png", output ?? SerializableWrapperHelper.IconRT, Helper.ImageType.PNG, TextureFormat.RGBA32, true, shader);
    }

    public readonly void CreatePreview(Camera camera, Shader shader = null, RenderTexture output = null)
    {
        camera.RenderToScreenshot($"{_directory}/{FileID}_PREVIEW.png", output ?? SerializableWrapperHelper.PreviewRT, Helper.ImageType.PNG, TextureFormat.RGBA32, true, shader);
    }

    public readonly void Save()
    {
        FusionFighters.Serializer.Save(this, _fileID, _directory);
    }

    public readonly void Delete()
    {
        FusionFighters.Serializer.Delete($"{_directory}/{_fileID}.json", _directory);

        if (File.Exists($"{_directory}/{_fileID}_ICON.png"))
            File.Delete($"{_directory}/{_fileID}_ICON.png");

        if (File.Exists($"{_directory}/{_fileID}_PREVIEW.png"))
            File.Delete($"{_directory}/{_fileID}_PREVIEW.png");
    }

    public static SerializableWrapper<T> LoadAs(string directory, AssetGuid fileID)
    {
        return FusionFighters.Serializer.LoadAs<SerializableWrapper<T>>($"{directory}/{fileID}.json", directory);
    }

    public static implicit operator T(SerializableWrapper<T> lhs)
    {
        return lhs.value;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is not SerializableWrapper<T>)
            return false;

        return ((SerializableWrapper<T>)obj)._fileID.Equals(_fileID);
    }

    public override readonly int GetHashCode()
    {
        System.HashCode hash = new();

        hash.Add(_fileID);

        return hash.ToHashCode();
    }

    public override readonly string ToString() => value.ToString();
}
