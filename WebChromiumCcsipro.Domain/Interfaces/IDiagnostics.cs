using System;

namespace WebChromiumCcsipro.Domain.Interfaces
{
    public interface IDiagnostics
    {
        string CommandLine { get; }
        string Version { get; }
        string VersionDeploy { get; }
        bool IsNetworkDeployed { get; }
        string CurrentDirectory { get; }
        string UserDomainName { get; }
        string MachineName { get; }
        string UserName { get; }
        OperatingSystem OSVersion { get; }
        string SystemDirectory { get; }
        bool Is64BitOperatingSystem { get; }
        bool Is64BitProcess { get; }
        int ProcessorCount { get; }
        TimeSpan UpTime { get; }
        string CLRVersion { get; }
        string WorkingSet { get; }
        string StackTrace { get; }
    }
}
