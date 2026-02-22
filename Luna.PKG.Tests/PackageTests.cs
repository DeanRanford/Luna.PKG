namespace Luna.PKG.Tests;

[TestClass]
public sealed class PackageTests
{
    [TestMethod]
    public void TestMakePackageAndRead()
    {
        PackageWriter.MakePkg("assets.pkg", "Assets");
        Assert.IsTrue(File.Exists("assets.pkg"));
        PackageReader pkg = new("assets.pkg");
        Assert.IsTrue(pkg.Assets.Count > 0);
        var txtFile = pkg.GetAsset("Assets/test.txt");
        var spriteFile = pkg.GetAsset("Assets/SprItes/sprite.png");
        Assert.IsTrue(spriteFile.Length == 796);
        _ = Assert.ThrowsException<KeyNotFoundException>(() => pkg.GetAsset("Assets/doesnotexist.txt"));
        _ = Assert.ThrowsException<FileNotFoundException>(() => new PackageReader("doesnotexist.pkg"));
    }
}
