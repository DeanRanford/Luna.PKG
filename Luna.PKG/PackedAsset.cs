namespace Luna.PKG;

public class PackedAsset(string path, long start, int size)
{
    public string Path { get; set; } = path;
    public long Start { get; set; } = start;
    public int Size { get; set; } = size;
}
