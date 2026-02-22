namespace Luna.PKG
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class PackageReader : IDisposable
    {
        public List<PackedAsset> Assets { get; private set; } = new List<PackedAsset>();
        private readonly string path;
        private readonly BinaryReader reader;
        private bool disposed;

        public PackageReader(string path)
        {
            this.path = path;

            try
            {
                this.reader = new BinaryReader(File.Open(path, FileMode.Open));
            }
            catch (FileNotFoundException fnfEx)
            {
                throw new FileNotFoundException($"The package file at path '{path}' could not be found.", fnfEx);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                throw new UnauthorizedAccessException($"Access to the package file at path '{path}' is denied.", uaEx);
            }

            var magic = this.reader.ReadBytes(3);
            if (Encoding.ASCII.GetString(magic) != "PKG")
            {
                throw new InvalidOperationException($"Invalid file format: Expected 'PKG' magic header in file '{path}', but found '{Encoding.ASCII.GetString(magic)}'.");
            }

            var index = this.reader.ReadInt64();
            var size = this.reader.ReadInt32();

            _ = this.reader.BaseStream.Seek(index, SeekOrigin.Begin);
            for (var i = 0; i < size; i++)
            {
                PackedAsset asset = new(this.reader.ReadString(), this.reader.ReadInt64(), this.reader.ReadInt32());
                this.Assets.Add(asset);
            }
        }

        public byte[] GetAsset(string path)
        {
            var asset = this.Assets.FirstOrDefault(a => a.Path.Equals(path, StringComparison.OrdinalIgnoreCase)) ?? throw new KeyNotFoundException($"Asset with path '{path}' was not found in the package at '{this.path}'.");

            _ = this.reader.BaseStream.Seek(asset.Start, SeekOrigin.Begin);
            return this.reader.ReadBytes(asset.Size);
        }

        ~PackageReader()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.reader?.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}