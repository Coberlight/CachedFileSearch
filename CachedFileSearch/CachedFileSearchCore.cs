using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CachedFileSearch
{
    internal class CachedFileSearchCore
    {
        public delegate void SearchLog(string message);
        public static string MakeCache(string[] paths, int pathDepth, SearchLog logger)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                sb.Append(paths[i]);
                ProcessFolderCache(new DirectoryInfo(paths[i]), sb, pathDepth, logger);
            }
            return sb.ToString();
        }
        public static string MakeCache(string path, int pathDepth, SearchLog logger)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            ProcessFolderCache(new DirectoryInfo(path), sb, pathDepth, logger);
            return sb.ToString();
        }
        private static void ProcessFolderCache(DirectoryInfo directoryInfo, StringBuilder sb, int pathDepth, SearchLog logger)
        {
            logger?.Invoke(directoryInfo.FullName);
            FileInfo[] files = directoryInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                sb.Append(files[i].Name);
                sb.Append(Path.PathSeparator);
            }
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                sb.Append(directories[i].FullName);
                sb.Append(Path.PathSeparator);
                if (pathDepth != 1)
                {
                    ProcessFolderCache(directories[i], sb, pathDepth == 0 ? 0 : pathDepth - 1, logger);
                }
            }
        }
        /// <summary>
        /// Метод поиска файлов и папок в кэше.
        /// logger - делегат, вызывающийся с названием текущей обрабатываемой директории для отображения информации о поиске.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<FileSystemInfo> FindMatches(string query, ref string cache, bool enableCase, SearchLog logger)
        {
            List<FileSystemInfo> matchedFSIs = new List<FileSystemInfo>();
            int currentIndex = 0;
            int cacheLength = cache.Length;
            string currentDirectory = GetNextPath(ref cache, ref currentIndex);
            logger?.Invoke(currentDirectory);
            if (!enableCase) query = query.ToLower();
            while (currentIndex < cacheLength)
            {
                string nextLine = GetNextPath(ref cache, ref currentIndex);
                string nextLineProcessed = enableCase ? nextLine : nextLine.ToLower();
                if (Path.IsPathRooted(nextLine))
                {
                    currentDirectory = nextLine;
                    if (Path.GetFileName(nextLineProcessed).IndexOf(query) != -1)
                    {
                        matchedFSIs.Add(new DirectoryInfo(currentDirectory));
                        logger?.Invoke(currentDirectory);
                    }
                } else
                {
                    if (nextLineProcessed.IndexOf(query) != -1)
                    {
                        matchedFSIs.Add(new FileInfo(Path.Combine(currentDirectory, nextLine)));
                    }
                }
            }
            return matchedFSIs;
        }

        public static string GetNextPath(ref string cache, ref int index)
        {
            int oldIndex = index;
            index = cache.IndexOf(Path.PathSeparator, index);
            return cache.Substring(oldIndex, index++ - oldIndex);
        }
    }
}
