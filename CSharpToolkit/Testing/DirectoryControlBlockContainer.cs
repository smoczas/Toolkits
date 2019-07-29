using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpToolkit.Testing
{
    internal class DirectoryControlBlockContainer : IDisposable
    {
        public DirectoryControlBlockContainer()
        {
            _collection = new Dictionary<DirectoryIdentifier, DirectoryControlBlock>();
        }

        public void Dispose()
        {
            foreach(var v in _collection.Values)
            {
                v.Dispose();
            }
        }

        public bool TryGet(DirectoryIdentifier id, out DirectoryControlBlock result)
        {
            return _collection.TryGetValue(id, out result);
        }

        public bool AddOrGet(DirectoryIdentifier id, out List<DirectoryControlBlock> result)
        {
            var keys = DirectoryIdentifier.Split(id);

            var controlBlocks = new List<KeyValuePair<bool, DirectoryControlBlock>>();
            foreach(var i in keys)
            {
                var isNew = AddOrGetSingle(i, out var block);
                controlBlocks.Add(new KeyValuePair<bool, DirectoryControlBlock>(isNew, block));
            }

            bool added = false;
            int index = controlBlocks.FindLastIndex(i => !i.Key);
            if (index == controlBlocks.Count - 1)
            {
                // no new item
                result = new List<DirectoryControlBlock> { controlBlocks[controlBlocks.Count - 1].Value };
            }
            else
            {
                index = Math.Max(0, index);
                result = controlBlocks.GetRange(index, controlBlocks.Count - index).Select(i => i.Value).ToList();
                added = true;
            }
            return added;
        }

        public bool Remove(DirectoryIdentifier id)
        {
            return _collection.Remove(id);
        }

        public DirectoryControlBlock Change(DirectoryIdentifier from, DirectoryIdentifier to)
        {
            if (_collection.TryGetValue(from, out var ctrlBlock))
            {
                _collection.Remove(from);
            }

            var result = ctrlBlock.Clone(to);
            _collection.Add(to, result);
            return result;
        }

        private bool AddOrGetSingle(DirectoryIdentifier key, out DirectoryControlBlock result)
        {
            var added = false;
            if (!_collection.TryGetValue(key, out result))
            {
                result = new DirectoryControlBlock(key);
                _collection[key] = result;
                added = true;
            }
            return added;
        }

        private readonly Dictionary<DirectoryIdentifier, DirectoryControlBlock> _collection; 
    }
}
