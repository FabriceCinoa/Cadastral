using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Logger;

public class FileLogger : ILogger
{
    private IConfiguration _configuration;

    private string LogFileName => $"log-{DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy")}.log";
    private LogLevel  LogLevel  { get; init; }

    private string DirectoryPath{ get; init; } 

    private void WriteLog(LogLevel level, string? message)
    {
        try
        {
         
        var filePath = Path.Combine(this.DirectoryPath, LogFileName); 
            
           using( var fs = new StreamWriter(filePath, true))
            {
                fs.WriteLine($"{DateTime.Now.ToLongTimeString()} -  Level : { level} ->  Message :  {message}"); 
            }
        }

        catch (Exception ex)
        {
            return;
        }
    }

    public FileLogger(IConfiguration configuration)
    {
        this._configuration = configuration;
        this.LogLevel = LogLevel.Debug;
        if( _configuration != null )
        {
            var level = this._configuration["Logging:LogLevel:Default"];
            if( level != null)
            {
                try
                {
                    if( Enum.TryParse<LogLevel>(level, out var res )) this.LogLevel = res;  
                }
                catch  {}
            }
            this.DirectoryPath = this._configuration["Logging:Path"] == null ? AppDomain.CurrentDomain.BaseDirectory : this._configuration["Logging:Path"];
            this.DirectoryPath = this.DirectoryPath.Replace("{ApplicationPath}", DropBoxUserApp.Common.Constants.StorageLocation);
            if( !Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            else
            {
                foreach( var file in Directory.EnumerateFiles(this.DirectoryPath))
                {
                    var _fileInfo = new FileInfo(file);
                    var d = (DateTime.Now - _fileInfo.LastWriteTime).Days;
                    if (d >= 30)
                    {
                        File.Delete(file);
                    }
                }
            }

        }
    }
    private string FileName { get; set; }
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (this.LogLevel == LogLevel.Debug  ||  this.LogLevel  <= logLevel )
        {
            
            if (exception == null && state != null)
            {
                WriteLog(logLevel,state.ToString());
            }
            else if (exception != null)
            {
                if (state != null)
                {
                    WriteLog(logLevel, state.ToString());
                }
                WriteLog(logLevel, exception.ToString());
              
            }
        }
        else
        {
            if (exception == null && state != null)
            {
                Console.WriteLine(state.ToString());
            }
            else if (exception != null)
            {
                if (state != null)
                {
                    Console.WriteLine(state.ToString());
                }
                Console.WriteLine(exception.ToString());
            }
        }

    }


}
