using System;
using System.Collections.Generic;

namespace CSharpToolkit.Testing
{
    internal class FileControlBlockContainer : IDisposable
    {
        public FileControlBlockContainer()
        {
            _collection = new Dictionary<FileIdentifier, FileControlBlock>();
        }

        public bool TryGet(FileIdentifier id, out FileControlBlock block)
        {
            return _collection.TryGetValue(id, out block);
        }

        public FileControlBlock AddOrGet(FileIdentifier id)
        {
            if(!_collection.TryGetValue(id, out var result))
            {
                result = new FileControlBlock(id);
                _collection[id] = result;
                return result;
            }
            return result;
        }

        public bool Remove(FileIdentifier id)
        {
            return _collection.Remove(id);
        }

        public FileControlBlock Change(FileIdentifier from, FileIdentifier to)
        {
            if (_collection.TryGetValue(from, out var ctrlBlock))
            {
                _collection.Remove(from);
            }

            var result = ctrlBlock.Clone(to);
            _collection.Add(to, result);
            return result;
        }

        public void Dispose()
        {
            foreach(var fcb in _collection.Values)
            {
                fcb.Dispose();
            }
        }

        private readonly Dictionary<FileIdentifier, FileControlBlock> _collection;
    }
}
