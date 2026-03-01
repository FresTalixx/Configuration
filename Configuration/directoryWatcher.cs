using Serilog;
using System;
using System.Collections.Generic;
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

        while (true)
        {
            var currentFiles = Directory.GetFiles(_dirPath, "*.*", SearchOption.AllDirectories);
            var newFiles = currentFiles.Except(previousFiles).ToList();
            var deletedFiles = previousFiles.Except(currentFiles).ToList();


            foreach (var file in newFiles)
            {
                Log.Information($"New file: {file}");
            }

            foreach (var file in deletedFiles)
            {
                Log.Information($"Deleted file: {file}");
            }

           previousFiles = currentFiles;

            Thread.Sleep(1000);

        }
    }
}
