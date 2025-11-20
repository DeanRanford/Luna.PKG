namespace Luna.PKG;

using System.Text;

public class PackageWriter
{
    public List<PackedAsset> Assets { get; } = [];

    public static void MakePkg(string outputPath, string assetPath)
    {

        PackageWriter pkg = new();
        var files = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var path = file.Replace("\\", "/").Replace("//", "/");
            Console.WriteLine($"Adding {path}");
            pkg.AddAsset(path);
        }
        pkg.Write(outputPath);
    }

    public void Write(string path)
    {
        using BinaryWriter writer = new(File.Open(path, FileMode.Create));
        writer.Write(Encoding.ASCII.GetBytes("PKG"));
        writer.Write((long)0); //index to file table
        writer.Write((long)0); //size of file table

        foreach (var asset in this.Assets)
        {
            var data = LoadData(asset);
            asset.Size = data.Length;
            asset.Start = writer.BaseStream.Position;
            writer.Write(data);
        }

        var index = (int)writer.BaseStream.Position;
        foreach (var asset in this.Assets)
        {
            writer.Write(asset.Path);
            writer.Write(asset.Start);
            writer.Write(asset.Size);
        }

        _ = writer.BaseStream.Seek(3, SeekOrigin.Begin);
        writer.Write((long)index);
        writer.Write((long)this.Assets.Count);
    }

    public void AddAsset(string path)
    {
        if (this.Assets.Any(a => a.Path == path))
        {
            return;
        }

        this.Assets.Add(new PackedAsset(path, 0, 0));
    }

    public static byte[] LoadData(PackedAsset asset)
    {
        using BinaryReader reader = new(File.Open(asset.Path, FileMode.Open));
        return reader.ReadBytes((int)reader.BaseStream.Length);
    }
}
