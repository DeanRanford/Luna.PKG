namespace Luna.PKG;

using System.Text;

public class PackageReader : IDisposable
{
    public List<PackedAsset> Assets { get; private set; } = [];
    private readonly string _path;
    private readonly BinaryReader _reader;

    public PackageReader(string path)
    {
        this._path = path;
        this._reader = new BinaryReader(File.Open(path, FileMode.Open));
        var magic = this._reader.ReadBytes(3);
        if (Encoding.ASCII.GetString(magic) != "PKG")
        {
            throw new Exception("Invalid package file");
        }

        var index = this._reader.ReadInt64(); //index to file table
        var size = this._reader.ReadInt32(); //size of file table

        _ = this._reader.BaseStream.Seek(index, SeekOrigin.Begin);
        for (var i = 0; i < size; i++)
        {
            PackedAsset asset = new(this._reader.ReadString(), this._reader.ReadInt64(), this._reader.ReadInt32());
            this.Assets.Add(asset);
        }
    }

    public byte[] GetAsset(string path)
    {
        var asset = this.Assets.FirstOrDefault(a => a.Path.Equals(path, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Asset {path} not found in package {this._path}");
        _ = this._reader.BaseStream.Seek(asset.Start, SeekOrigin.Begin);
        return this._reader.ReadBytes(asset.Size);
    }

    ~PackageReader()
    {
        this._reader?.Dispose();
    }

    public void Dispose() => throw new NotImplementedException();
}
