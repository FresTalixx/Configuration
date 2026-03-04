using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration;

internal class directoryWatcher
{
    private string _dirPath;
    public directoryWatcher(string dirPath)
    {
        _dirPath = dirPath;
    }
    public void WatchDirectory()
    {
        var previousFiles = Directory.GetFiles(_dirPath, "*.*", SearchOption.AllDirectories);
        var previousDirs = Directory.GetDirectories(_dirPath, "*.*", SearchOption.AllDirectories);
        Dictionary<string, long> fileNamesAndSizes = new();

        foreach (var file in previousFiles)
        {
            long length = new System.IO.FileInfo(file).Length;
            fileNamesAndSizes.Add(file, length);
        }
        
        while (true)
        {
            var currentFiles = Directory.GetFiles(_dirPath, "*.*", SearchOption.AllDirectories);
            var newFiles = currentFiles.Except(previousFiles).ToList();
            var deletedFiles = previousFiles.Except(currentFiles).ToList();

            var currentDirs = Directory.GetDirectories(_dirPath, "*.*", SearchOption.AllDirectories);
            var newDirs = currentDirs.Except(previousDirs).ToList();
            var deletedDirs = previousDirs.Except(currentDirs).ToList();

            foreach (var file in currentFiles)
            {

                long newLength = new System.IO.FileInfo(file).Length;
                if (File.Exists(file))
                {
                    fileNamesAndSizes.TryGetValue(file, out long value);
                    if (newLength != value)
                    {
                        LoggerUpdateFileInfo.UpdateFileInfo("files");
                        Log.Information($"File: {file} has been modified");
                    }
                }
            }

            foreach (var dir in newDirs)
            {
                LoggerUpdateFileInfo.UpdateFileInfo("dirs");
                Log.Information($"New directory: {dir}");
            }

            foreach (var dir in deletedDirs)
            {
                LoggerUpdateFileInfo.UpdateFileInfo("dirs");
                Log.Information($"Deleted directory: {dir}");
            }


            foreach (var file in newFiles)
            {
                LoggerUpdateFileInfo.UpdateFileInfo("files");
                Log.Information($"New file: {file}");

            }

            foreach (var file in deletedFiles)
            {
                LoggerUpdateFileInfo.UpdateFileInfo("files");
                Log.Information($"Deleted file: {file}");
            }


            previousFiles = currentFiles;
            previousDirs = currentDirs;

            foreach (var file in currentFiles)
            {
                long length = new System.IO.FileInfo(file).Length;
                fileNamesAndSizes[file] = length;
            }

            Thread.Sleep(1000);

        }
    }
}
