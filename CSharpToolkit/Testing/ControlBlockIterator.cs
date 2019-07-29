using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpToolkit.Testing
{
    internal class ControlBlockIterator
    {
        public ControlBlockIterator(DirectoryControlBlockContainer dirs)
        {
            _dirs = dirs;
        }

        public FileIdentifier[] GetFiles(DirectoryControlBlock dir, string searchPattern, SearchOption searchOptions)
        {
            var result = new List<FileIdentifier>();

            if (searchPattern.Contains(Path.DirectorySeparatorChar) ||
                searchPattern.Contains(Path.AltDirectorySeparatorChar))
            {
                var segments = searchPattern.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (HasIllegalChars(segments))
                {
                    throw new ArgumentException("Illegal characters in path");
                }

                result = GetFilesBySegments(dir, segments, 0);
                return result.ToArray();
            }


            result = dir.Files.Where(f => NamePredicate(f.Name.Name, searchPattern)).ToList();
            if (searchOptions == SearchOption.TopDirectoryOnly)
            {
                return result.ToArray();
            }

            foreach (var d in dir.Directories)
            {
                if (!_dirs.TryGet(d, out var b))
                {
                    continue;
                }
                result.AddRange(GetFiles(b, searchPattern, searchOptions));
            }

            return result.ToArray();
        }

        public DirectoryIdentifier[] GetDirectories(DirectoryControlBlock dir, string searchPattern, SearchOption searchOptions)
        {
            List<DirectoryIdentifier> result;
            if(searchPattern.Contains(Path.DirectorySeparatorChar) ||
                searchPattern.Contains(Path.AltDirectorySeparatorChar))
            {
                var segments = searchPattern.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if(HasIllegalChars(segments))
                {
                    throw new ArgumentException("Illegal characters in path");
                }

                result = GetDirectoryBySegments(dir, segments, 0);
                return result.ToArray();
            }

            result = dir.Directories.Where(d => NamePredicate(d.Name, searchPattern)).ToList();
            if(searchOptions == SearchOption.TopDirectoryOnly)
            {
                return result.ToArray();
            }

            foreach(var d in dir.Directories)
            {
                if(!_dirs.TryGet(d, out var b))
                {
                    continue;
                }
                result.AddRange(GetDirectories(b, searchPattern, searchOptions));
            }

            return result.ToArray();
        }

        public void ForEach(DirectoryControlBlock dir, Action<DirectoryControlBlock> action)
        {
            action(dir);

            foreach(var d in dir.Directories)
            {
                if (!_dirs.TryGet(d, out var b))
                {
                    throw new InvalidOperationException($"control block container is in corrupted state. failed to find control block for {d.FullName}");
                }

                ForEach(b, action);
            }
        }

        private bool HasIllegalChars(string[] segments)
        {
            // wildchar is only allowed inside the last segment
            var illegal = new char[] { '*', '?' };
            for(var i = 0; i < segments.Length - 1; ++i)
            {
                if(illegal.Any(segments[i].Contains))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool NamePredicate(string name, string pattern)
        {
            var regexFilter = pattern
                .Replace(".", @"\.")
                .Replace("?", ".{0,1}")
                .Replace("*", ".*?");
            regexFilter = $"^{regexFilter}$";

            return Regex.IsMatch(name, regexFilter);
        }

        private List<DirectoryIdentifier> GetDirectoryBySegments(DirectoryControlBlock dir, string[] segments, int level)
        {
            var result = new List<DirectoryIdentifier>();
            var current = dir;
            while(true)
            {
                if(level < 0 || level >= segments.Length)
                {
                    break;
                }

                var found = GetDirectories(current, segments[level], SearchOption.TopDirectoryOnly).ToList();

                if (level == segments.Length - 1)
                {
                    return found;
                }

                if (!found.Any())
                {
                    throw new DirectoryNotFoundException($"Could not find part of the path '{current.Idendifier.FullName}'");
                }

                var nextDirId = found.First();
                if (!_dirs.TryGet(nextDirId, out current))
                {
                    throw new InvalidOperationException($"control block container is in corrupted state. failed to find control block for {nextDirId.FullName}");
                }

                ++level;
            }
            return result;
        }

        private List<FileIdentifier> GetFilesBySegments(DirectoryControlBlock dir, string[] segments, int level)
        {
            var result = new List<FileIdentifier>();
            var current = dir;
            while (true)
            {
                if (level < 0 || level >= segments.Length)
                {
                    break;
                }

                if (level == segments.Length - 1)
                {
                    return GetFiles(current, segments[level], SearchOption.TopDirectoryOnly).ToList();
                }

                var found = GetDirectories(current, segments[level], SearchOption.TopDirectoryOnly).ToList();
                if (!found.Any())
                {
                    throw new DirectoryNotFoundException($"Could not find part of the path '{current.Idendifier.FullName}'");
                }

                var nextDirId = found.First();
                if (!_dirs.TryGet(nextDirId, out current))
                {
                    throw new InvalidOperationException($"control block container is in corrupted state. failed to find control block for {nextDirId.FullName}");
                }

                ++level;
            }
            return result;
        }

        private readonly DirectoryControlBlockContainer _dirs;
    }
}
