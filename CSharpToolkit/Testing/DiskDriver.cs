using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace CSharpToolkit.Testing
{
    internal class DiskDriver : IDiskDriver, IDisposable
    {
        public Func<string, IDiskDriver, File> FileFactoryMethod { get => _factory.FileFactoryMethod; set => _factory.FileFactoryMethod = value; }

        public Func<string, IDiskDriver, Directory> DirectoryFactoryMethod { get => _factory.DirectoryFactoryMethod; set => _factory.DirectoryFactoryMethod = value; }

        public DiskDriver()
        {
            _factory = new IOFactory();
            _directories = new DirectoryControlBlockContainer();
            _files = new FileControlBlockContainer();
        }

        public File CreateOrGetFile(string path)
        {
            var id = FileIdentifier.FromPath(path);
            if (!_files.TryGet(id, out _))
            {
                var isNewDirectory = _directories.AddOrGet(id.OwningDirectory, out var blocks);
                if (isNewDirectory)
                {
                    LinkDirectoryControlBlocks(blocks);
                }

                var now = DateTime.Now;
                UpdateTimes(blocks, now, true, true, isNewDirectory);

                // add file id to owning control block
                var owningCtrlBlock = blocks[blocks.Count - 1];
                owningCtrlBlock.Files.Add(id);
                
                _files.AddOrGet(id);
            }
            return _factory.CreateFile(path, this);
        }

        public Directory CreateOrGetDirectory(string path)
        {
            if (Exists(FileIdentifier.FromPath(path)))
            {
                throw new IOException($"Cannot create \"{path}\" because a file or directory with the same name already exists.");
            }

            if (_directories.AddOrGet(new DirectoryIdentifier(path), out var added))
            {
                LinkDirectoryControlBlocks(added);
                var now = DateTime.Now;
                UpdateTimes(added, now, true, true, true);
            }
            return _factory.CreateDirectory(path, this);
        }

        public Directory CreateSubdirectory(DirectoryIdentifier id, string path)
        {
            if (IsAbsolute(path) || IsUnc(path))
            {
                throw new ArgumentException("Path must not be a drive or UNC name.");
            }

            var subdirAbsolutePath = Path.GetFullPath(Path.Combine(id.FullName, path));

            if (!subdirAbsolutePath.StartsWith(id.FullName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The directory specified, '{path}', is not a subdirectory of '{id.FullName}'.");
            }

            return CreateOrGetDirectory(subdirAbsolutePath);
        }

        public File GetFile(string path)
        {
            return _factory.CreateFile(path, this);
        }

        public Directory GetDirectory(string path)
        {
            return _factory.CreateDirectory(path, this);
        }


        public void Dispose()
        {
            _directories.Dispose();
            _files.Dispose();
        }

        public bool Exists(FileIdentifier file)
        {
            return _files.TryGet(file, out _);
        }

        public bool Exists(DirectoryIdentifier dir)
        {
            return _directories.TryGet(dir, out _);
        }

        public Stream OpenFile(FileIdentifier id, FileMode mode, FileAccess access, FileShare share)
        {
            if (mode == FileMode.Append && access != FileAccess.Write)
            {
                throw new ArgumentException("Append access can be requested only in write-only mode.");
            }

            var path = IdentifierHelper.ToPath(id);
            if (!_directories.TryGet(id.OwningDirectory, out var dirCtrlBlock))
            {
                throw new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
            }

            if (Exists(IdentifierHelper.ConvertToDirId(id)))
            {
                throw new UnauthorizedAccessException($"'Access to the path '{IdentifierHelper.ToPath(id)}' is denied.'");
            }

            // file exists
            if (_files.TryGet(id, out var ctrlBlock))
            {
                if (mode == FileMode.CreateNew)
                {
                    throw new IOException($"The file '{path}' already exists.");
                }
                if (mode == FileMode.Create || mode == FileMode.Truncate)
                {
                    ctrlBlock.Stream.Seek(0, SeekOrigin.Begin);
                    ctrlBlock.Stream.SetLength(0);
                }

                var result = new MemoryStreamProxy(ctrlBlock.Stream, id, this);

                if (mode == FileMode.Append)
                {
                    result.Seek(0, SeekOrigin.End);
                }

                return result;
            }

            // file doesn't exist
            if (mode == FileMode.Open || mode == FileMode.Truncate)
            {
                throw new FileNotFoundException($"Could not find file '{path}'.");
            }

            var fileCtrlBlock = _files.AddOrGet(id);
            dirCtrlBlock.Files.Add(id);

            var timeStamp = DateTime.Now;
            SetLastWriteTime(id.OwningDirectory, timeStamp);
            SetLastAccessTime(id.OwningDirectory, timeStamp);

            return new MemoryStreamProxy(fileCtrlBlock.Stream, id, this);
        }

        public void SetIsReadOnly(FileIdentifier id, bool value)
        {
            var fCtrlBlock = FindFileControlBlock(id);
            fCtrlBlock.IsReadOnly = value;
        }

        public bool GetIsReadOnly(FileIdentifier id)
        {
            if (!_files.TryGet(id, out var fCtrlBlock))
            {
                return true;
            }
            return fCtrlBlock.IsReadOnly;
        }

        public long GetLength(FileIdentifier id)
        {
            var fCtrlBlock = FindFileControlBlock(id);
            return fCtrlBlock.Stream.Length;
        }

        public FileAttributes GetAttributes(FileIdentifier id)
        {
            var ctrlBlock = FindFileControlBlock(id, false, false);
            return ctrlBlock == null ? (FileAttributes)(-1) : ctrlBlock.Attributes;
        }

        public FileAttributes GetAttributes(DirectoryIdentifier id)
        {
            var ctrlBlock = FindDirControlBlock(id, false);
            return ctrlBlock == null ? (FileAttributes)(-1) : ctrlBlock.Attributes;
        }

        public void SetAttributes(FileIdentifier id, FileAttributes attr)
        {
            var ctrlBlock = FindFileControlBlock(id);
            ctrlBlock.Attributes = attr;
        }

        public void SetAttributes(DirectoryIdentifier id, FileAttributes attr)
        {
            var ctrlBlock = FindDirControlBlock(id);
            ctrlBlock.Attributes = attr;
        }

        public void Delete(FileIdentifier id)
        {
            if (Exists(IdentifierHelper.ConvertToDirId(id)))
            {
                throw new UnauthorizedAccessException($"'Access to the path '{IdentifierHelper.ToPath(id)}' is denied.'");
            }

            if (FindFileControlBlock(id, false) == null)
            {
                return;
            }

            var dirCtrlBlock = FindDirControlBlock(id.OwningDirectory, false);
            if(dirCtrlBlock == null)
            {
                throw new InvalidOperationException($"control block container is in corrupted state. cannot find {id.OwningDirectory}");
            }

            _files.TryGet(id, out var fCtrlBlock);
            _files.Remove(id);

            dirCtrlBlock.Files.Remove(id);
            var now = DateTime.Now;
            SetLastAccessTime(id.OwningDirectory, now);
            SetLastWriteTime(id.OwningDirectory, now);

            fCtrlBlock.Dispose();
        }

        public void Delete(DirectoryIdentifier id, bool recurse)
        {
            var dir = FindDirControlBlock(id);
            var parentId = IdentifierHelper.Parent(dir.Idendifier);
            if (parentId == null)
            {
                throw new InvalidOperationException("removing drive? srsly?");
            }

            var parent = FindDirControlBlock(parentId, false);
            if (parent == null)
            {
                throw new InvalidOperationException($"control block container is in corrupted state. failed to find control block for {parentId.FullName}");
            }

            var fIds = new List<FileIdentifier>();
            var dIds = new List<DirectoryIdentifier>();

            if (!recurse)
            {
                if (dir.Directories.Any() || dir.Files.Any())
                {
                    throw new IOException(@"The directory is not empty.");
                }
                else
                {
                    dIds.Add(id);
                }
            }
            else
            {
                var it = new ControlBlockIterator(_directories);
                dIds.Add(id);
                it.ForEach(dir, i =>
                {
                    fIds.AddRange(i.Files);
                    dIds.AddRange(i.Directories);
                });
            }

            parent.Directories.Remove(id);

            var now = DateTime.Now;
            SetLastWriteTime(parentId, now);
            SetLastAccessTime(parentId, now);

            foreach (var i in fIds)
            {
                _files.Remove(i);
            }
            foreach (var i in dIds)
            {
                _directories.Remove(i);
            }
        }

        public File CopyTo(FileIdentifier source, FileIdentifier target, bool overwrite)
        {
            var srcFileCtrlBlock = FindFileControlBlock(source);
            var targFileCtrlBlock = FindFileControlBlock(target, false);

            if (Exists(IdentifierHelper.ConvertToDirId(target)))
            {
                throw new IOException($"The target file \"{IdentifierHelper.ToPath(target)}\" is a directory, not a file.");
            }

            if (targFileCtrlBlock == null)
            {
                targFileCtrlBlock = _files.AddOrGet(target);
            }
            else
            {
                if (overwrite == false)
                {
                    throw new IOException($"The file '{IdentifierHelper.ToPath(target)}' already exists.");
                }
            }

            targFileCtrlBlock.Stream.Seek(0, SeekOrigin.Begin);
            targFileCtrlBlock.Stream.SetLength(0);

            var copier = new StreamCopier { ResetTarget = true };
            copier.Copy(srcFileCtrlBlock.Stream, targFileCtrlBlock.Stream);

            _directories.TryGet(target.OwningDirectory, out var targetDirBlock);
            targetDirBlock.Files.Add(target);

            targFileCtrlBlock.LastAccessTime =
                targFileCtrlBlock.LastWriteTime =
                targFileCtrlBlock.CreationTime =
                DateTime.Now;

            return _factory.CreateFile(IdentifierHelper.ToPath(target), this);
        }

        public void MoveTo(FileIdentifier source, FileIdentifier target)
        {
            FindFileControlBlock(source);
            var targFileCtrlBlock = FindFileControlBlock(target, false);

            if (targFileCtrlBlock != null || Exists(IdentifierHelper.ConvertToDirId(target)))
            {
                throw new IOException($"Cannot create a file when that file already exists.");
            }

            _files.Change(source, target);

            _directories.TryGet(target.OwningDirectory, out var targetDirBlock);
            targetDirBlock.Files.Add(target);

            _directories.TryGet(source.OwningDirectory, out var srcDirBlock);
            srcDirBlock.Files.Remove(source);

            _factory.OnFileMoved(source, target);
        }

        private class MoveChange
        {
            public List<KeyValuePair<FileIdentifier, FileIdentifier>> FilesToChange { get; set; }
            public List<KeyValuePair<DirectoryIdentifier, DirectoryIdentifier>> DirsToChange { get; set; }
            public DirectoryIdentifier NewDirId { get; set; }
        }

        public void MoveTo(DirectoryIdentifier source, DirectoryIdentifier target)
        {
            if(source == target)
            {
                return;
            }

            var srcCtrlBlock = FindDirControlBlock(source);
            var targParentId = IdentifierHelper.Parent(target);
            var targParentCtrlBlock = FindDirControlBlock(targParentId);
            var targDirCtrlBlock = FindDirControlBlock(target, false);
            if(targDirCtrlBlock != null || Exists(IdentifierHelper.ConvertToFileId(target)))
            {
                throw new IOException($"Cannot create a file when that file already exists.");
            }

            var srcParentId = IdentifierHelper.Parent(source);
            if (srcParentId == null)
            {
                throw new InvalidOperationException(@"move drive? srlsy?");
            }


            // compute changes for control blocks
            var changes = new Dictionary<DirectoryControlBlock, MoveChange>();
            var iterator = new ControlBlockIterator(_directories);
            iterator.ForEach(srcCtrlBlock, d =>
            {
                // files to update
                var changedFiles = d.Files.Select(i =>
                {
                    if (!IdentifierHelper.GetRelative(srcParentId, i, out var rel))
                    {
                        var msg = $"Illegal path convertion from {source.FullName} to {IdentifierHelper.ToPath(i)}";
                        throw new InvalidOperationException(msg);
                    }
                    var newId = IdentifierHelper.CombineFileId(targParentId, rel);
                    return new KeyValuePair<FileIdentifier, FileIdentifier>(i, newId);
                })
                .ToList();

                // directories to update
                var changedDirs = d.Directories.Select(i =>
                {
                    if (!IdentifierHelper.GetRelative(srcParentId, i, out var rel))
                    {
                        var msg = $"Illegal path convertion from {source.FullName} to {i.FullName}";
                        throw new InvalidOperationException(msg);
                    }
                    var newId = IdentifierHelper.CombineDirId(targParentId, rel);
                    return new KeyValuePair<DirectoryIdentifier, DirectoryIdentifier>(i, newId);
                })
                .ToList();

                if (!changes.TryGetValue(d, out var c))
                {
                    c = new MoveChange();
                    changes.Add(d, c);
                }
                c.DirsToChange = changedDirs;
                c.FilesToChange = changedFiles;
                changes[d] = c;
            });


            // remove src from its parent
            var srcParentCtrlBlock = FindDirControlBlock(srcParentId, false);
            if (srcParentCtrlBlock == null)
            {
                throw new InvalidOperationException($"control block container is in corrupted state, cannot find {srcParentId}");
            }
            srcParentCtrlBlock.Directories.Remove(source);


            // update control blocks with new childs
            foreach(var change in changes)
            {
                var ctrlBlock = change.Key;

                var newDirIdsInCtrlBlock = change.Value.DirsToChange.Select(i => i.Value);
                ctrlBlock.Directories.Clear();
                ctrlBlock.Directories.AddRange(newDirIdsInCtrlBlock);

                var newFileIdsInCtrlBlock = change.Value.FilesToChange.Select(i => i.Value);
                ctrlBlock.Files.Clear();
                ctrlBlock.Files.AddRange(newFileIdsInCtrlBlock);
            }


            // update dir and file collections
            _directories.Change(srcCtrlBlock.Idendifier, target);
            foreach(var change in changes)
            {
                foreach(var d in change.Value.DirsToChange)
                {
                    _directories.Change(d.Key, d.Value);
                    _factory.OnDirectoryMoved(d.Key, d.Value);
                }

                foreach(var f in change.Value.FilesToChange)
                {
                    _files.Change(f.Key, f.Value);
                    _factory.OnFileMoved(f.Key, f.Value);
                }
            }

            targParentCtrlBlock.Directories.Add(target);
        }

        public DateTime GetLastWriteTime(FileIdentifier id)
        {
            var ctrlBlock = FindFileControlBlock(id, false);
            return ctrlBlock?.LastWriteTime ?? DateTime.MinValue;
        }

        public void SetLastWriteTime(FileIdentifier id, DateTime time)
        {
            var ctrlBlock = FindFileControlBlock(id);
            ctrlBlock.LastWriteTime = time;
        }

        public DateTime GetCreationTime(FileIdentifier id)
        {
            var ctrlBlock = FindFileControlBlock(id, false);
            return ctrlBlock?.CreationTime ?? DateTime.MinValue;
        }

        public void SetCreationTime(FileIdentifier id, DateTime time)
        {
            var ctrlBlock = FindFileControlBlock(id);
            ctrlBlock.CreationTime = time;
        }

        public DateTime GetLastAccessTime(FileIdentifier id)
        {
            var ctrlBlock = FindFileControlBlock(id, false);
            return ctrlBlock?.LastAccessTime ?? DateTime.MinValue;
        }

        public void SetLastAccessTime(FileIdentifier id, DateTime time)
        {
            var ctrlBlock = FindFileControlBlock(id);
            ctrlBlock.LastAccessTime = time;
        }

        public DateTime GetLastWriteTime(DirectoryIdentifier id)
        {
            var ctrlBlock = FindDirControlBlock(id, false);
            return ctrlBlock?.LastWriteTime ?? DateTime.MinValue;
        }

        public void SetLastWriteTime(DirectoryIdentifier id, DateTime time)
        {
            var ctrlBlock = FindDirControlBlock(id);
            ctrlBlock.LastWriteTime = time;
        }

        public DateTime GetCreationTime(DirectoryIdentifier id)
        {
            var ctrlBlock = FindDirControlBlock(id, false);
            return ctrlBlock?.CreationTime ?? DateTime.MinValue;
        }

        public void SetCreationTime(DirectoryIdentifier id, DateTime time)
        {
            var ctrlBlock = FindDirControlBlock(id);
            ctrlBlock.CreationTime = time;
        }

        public DateTime GetLastAccessTime(DirectoryIdentifier id)
        {
            var ctrlBlock = FindDirControlBlock(id, false);
            return ctrlBlock?.LastAccessTime ?? DateTime.MinValue;
        }

        public void SetLastAccessTime(DirectoryIdentifier id, DateTime time)
        {
            var ctrlBlock = FindDirControlBlock(id);
            ctrlBlock.LastAccessTime = time;
        }

        public File[] GetFiles(DirectoryIdentifier id, string searchPattern, SearchOption searchOptions)
        {
            var dirCtrlBlock = FindDirControlBlock(id);
            ControlBlockIterator it = new ControlBlockIterator(_directories);
            var ids = it.GetFiles(dirCtrlBlock, searchPattern, searchOptions);
            var result = ids.Select(i => _factory.CreateFile(IdentifierHelper.ToPath(i), this)).ToArray();
            return result;
        }

        public Directory[] GetDirectories(DirectoryIdentifier id, string searchPattern, SearchOption searchOptions)
        {
            var block = FindDirControlBlock(id);
            ControlBlockIterator it = new ControlBlockIterator(_directories);
            var ids = it.GetDirectories(block, searchPattern, searchOptions);
            var result = ids.Select(i => _factory.CreateDirectory(i.FullName, this)).ToArray();
            return result;
        }

        public Dictionary<object, object> GetCustomData(FileIdentifier id)
        {
            var ctrlBlock = FindFileControlBlock(id);
            return ctrlBlock.CustomData;
        }

        private FileControlBlock FindFileControlBlock(FileIdentifier id,
            bool throwIfFileNotFound = true,
            bool throwIfDirNotFound = true)
        {
            var path = IdentifierHelper.ToPath(id);
            if (FindDirControlBlock(id.OwningDirectory, false) == null)
            {
                if (throwIfDirNotFound)
                {
                    throw new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
                }
                return null;
            }

            if (_files.TryGet(id, out var fCtrlBlock))
            {
                return fCtrlBlock;
            }

            if (throwIfFileNotFound)
            {
                throw new FileNotFoundException($"Could not find file '{path}'.");
            }
            return null;
        }

        private DirectoryControlBlock FindDirControlBlock(DirectoryIdentifier id, bool throwIfNotFound = true)
        {
            if (!_directories.TryGet(id, out var result) && throwIfNotFound)
            {
                var path = id.FullName;
                throw new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
            }
            return result;
        }

        private static void LinkDirectoryControlBlocks(List<DirectoryControlBlock> blocks)
        {
            if (blocks.Count < 2)
            {
                return;
            }
            for (var i = 1; i < blocks.Count; ++i)
            {
                var previousIndex = i - 1;
                blocks[previousIndex].Directories.Add(blocks[i].Idendifier);
            }
        }

        private static bool IsAbsolute(string path)
        {
            if (path.Length < 3) return false;
            return path.Substring(1, 2) == @":\";
        }

        private static bool IsUnc(string path)
        {
            if (path.Length < 2) return false;
            return path.StartsWith(@"\\");
        }

        private void UpdateTimes(List<DirectoryControlBlock> ctrlBlocks,
            DateTime time,
            bool lastAccess,
            bool lastWrite,
            bool creation)
        {
            foreach (var block in ctrlBlocks)
            {
                if (lastAccess) SetLastAccessTime(block.Idendifier, time);
                if (lastWrite) SetLastWriteTime(block.Idendifier, time);
                if (creation) SetCreationTime(block.Idendifier, time);
            }
        }

        private readonly DirectoryControlBlockContainer _directories;
        private readonly FileControlBlockContainer _files;

        private readonly IOFactory _factory;
    }
}
