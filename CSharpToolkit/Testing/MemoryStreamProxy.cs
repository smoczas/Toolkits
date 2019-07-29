using System;
using System.IO;

namespace CSharpToolkit.Testing
{
    public class MemoryStreamProxy : Stream
    {
        public MemoryStreamProxy(MemoryStream ms, FileIdentifier id, IDiskDriver driver)
        {
            _id = id;
            _driver = driver;

            _sink = ms;
            _local = new MemoryStream();

            var copier = new StreamCopier { ResetTarget = true };
            copier.Copy(_sink, _local);
        }

        public override bool CanRead => _local.CanRead;

        public override bool CanSeek => _local.CanSeek;

        public override bool CanWrite => _local.CanWrite;

        public override long Length => _local.Length;

        public override long Position { get => _local.Position; set => _local.Position = value; }

        public override void Flush()
        {
            var sinkPos = _sink.Position;
            _sink.Seek(0, SeekOrigin.Begin);
            _sink.SetLength(0);

            var copier = new StreamCopier();
            copier.Copy(_local, _sink);

            _sink.Seek(Math.Min(sinkPos, _sink.Length), SeekOrigin.Begin);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _local.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _local.SetLength(value);
            _isModified = true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _local.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _local.Write(buffer, offset, count);
            _isModified = true;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }

            if (disposing)
            {
                Flush();
                if (_isModified)
                {
                    var now = DateTime.Now;
                    _driver.SetLastWriteTime(_id, now);
                    _driver.SetLastAccessTime(_id, now);
                }
                _local.Dispose();
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private readonly MemoryStream _sink;
        private readonly MemoryStream _local;

        private readonly IDiskDriver _driver;
        private readonly FileIdentifier _id;

        private bool disposed = false;

        private bool _isModified;
    }
}
