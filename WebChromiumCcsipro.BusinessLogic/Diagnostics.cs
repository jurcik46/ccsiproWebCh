using System;

namespace WebChromiumCcsipro.BusinessLogic
{
    public class Diagnostics : IDiagnostics
    {
        public string CommandLine { get; set; }
        public string Version { get; set; }
        public string VersionDeploy { get; set; }
        public bool IsNetworkDeployed { get; set; }
        public string CurrentDirectory { get; set; }
        public string UserDomainName { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public OperatingSystem OSVersion { get; set; }
        public string SystemDirectory { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
        public bool Is64BitProcess { get; set; }
        public int ProcessorCount { get; set; }
        public TimeSpan UpTime { get; set; }
        public string CLRVersion { get; set; }
        public string WorkingSet { get; set; }
        public string StackTrace { get; set; }
    }
}
