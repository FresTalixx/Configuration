using Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.Json;
using Serilog;

//Консольний застосунок
//який моніторить директорію (з конфігурації)
//моніториться як сама директорія так і усі піддерикторії в ньому

//Якщо з'явилася нова директорія то пише в лог
//Якщо видалили директорію яка була, то пише в лог
//Якщо з'явився новий файл то пише в лог
//Якщо видалився файл то пише в лог

//Для логу використовуємо serilog
//Інформація про запуск та зупинку програми пишемо в файл log-20260301.log
//Інформацію про зміни файлів пишемо в файл files-20260301.log
//Інформацію про зміни директорій пишемо в файл dirs-20260301.log

LoggerUpdateFileInfo.UpdateFileInfo("log");
Log.Information($"Application started at {DateTime.Now}");

Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: $"logs/log-.log",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug
    )
    .CreateLogger();


IConfiguration congenfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
    .Build();


//output dir files and sub folders to console
// get dir path from appsettings.json file
var dirPath = congenfiguration.GetSection("DirectoryPath").Value;

var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);

//foreach (var file in files)
//{
//    Log.Information(file);
//}

var watcher = new directoryWatcher(dirPath);

watcher.WatchDirectory();

LoggerUpdateFileInfo.UpdateFileInfo("log");
Log.Information($"Application ended at {DateTime.Now}");

