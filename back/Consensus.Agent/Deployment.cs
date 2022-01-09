using Consensus.ApiContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace Consensus.Agent
{
    public class Deployment
    {
        private const string AppName = "Consensus.Agent";
        private const string ExeName = $"{AppName}.exe";

        /// <summary>
        /// CLI param which indicates that the app restarted itself in "background" mode - with invisible window.
        /// </summary>
        private const string BackgroundArg = "background";

        private static string AppDataLocalPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string DeployDir => Path.Combine(AppDataLocalPath, AppName);
        private static string UpdateDir => Path.Combine(DeployDir, "Update");
        
        private static string MainExePath => Path.Combine(DeployDir, ExeName);
        private static string UpdateExePath => Path.Combine(UpdateDir, ExeName);
        private static string CurrentExePath => Path.Combine(AppContext.BaseDirectory, ExeName);
        public static Version CurrentVersion => typeof(Deployment).Assembly.GetName().Version;

        public bool IsDeployed => !_deploymentEnabled || CurrentExePath == MainExePath || CurrentExePath == UpdateExePath;

        private readonly bool _startOnBoot;
        private readonly bool _deploymentEnabled;
        private readonly bool _updateEnabled;
        private readonly IAgentApi _agentApi;

        public Deployment(IConfiguration config, IAgentApi agentApi)
        {
            _startOnBoot = config.GetValue<bool>("StartOnBoot");
            _deploymentEnabled = config.GetValue<bool>("DeploymentEnabled");
            _updateEnabled = config.GetValue<bool>("UpdateEnabled");
            _agentApi = agentApi;
        }

        /// <summary>
        /// Returns true if application is already running in background mode and does not need restart.
        /// Current process should keep running.
        /// Returns false if application was not started in the background and has just been restarted properly.
        /// Current process should exit.
        /// Applies only to deployed instance of the app (binary in deployment location). Otherwise method returns true.
        /// </summary>
        public bool EnsureProperlyStarted(string[] args)
        {
            if (CurrentExePath == MainExePath)
            {
                if (args.FirstOrDefault() != BackgroundArg)
                {
                    StartExe(MainExePath);
                    return false;
                }
            }

            return true;
        }

        public async Task DeployAndStart()
        {
            if (!_deploymentEnabled) return;

            Log.Information("Deploying application version {Version}", CurrentVersion);

            Directory.CreateDirectory(DeployDir);
            TerminateMainExe();
            await CopyExe(CurrentExePath, MainExePath);

            if (_startOnBoot)
            {
                using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue(AppName, $"\"{MainExePath}\"");
            }

            StartExe(MainExePath);
        }

        public async Task<bool> CheckForUpdates()
        {
            if (!_updateEnabled) return false;

            var versionFromApi = await _agentApi.GetVersion();
            var newVersion = Version.Parse(versionFromApi.Version);

            return newVersion > CurrentVersion;
        }

        /// <summary>
        /// Downloads update from the API to the update folder and runs it.
        /// Current process should exit in order to let the updated one to complete.
        /// </summary>
        public async Task Update()
        {
            if (!_updateEnabled) return;

            Log.Debug("Agent is being updated");

            Directory.CreateDirectory(UpdateDir);
            using (var sourceStream = await _agentApi.DownloadExe())
            using (var destinationStream = File.Create(UpdateExePath))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }

            StartExe(UpdateExePath);

            Log.Debug("Agent has been updated and restarting");
        }

        /// <summary>
        /// If current process is a process running from the update folder -
        /// copies itself to the main location and returns true which means that the current process should exit.
        /// Otherwise returns false which means that the current process is running from the main location and can keep executing.
        /// </summary>
        public async Task<bool> CompleteUpdate()
        {
            if (!_updateEnabled) return false;

            if (CurrentExePath == UpdateExePath)
            {
                Log.Debug("Agent is completing update");

                await CopyExe(CurrentExePath, MainExePath);
                StartExe(MainExePath);

                Log.Debug("Agent has been completed update and restarting");

                return true;
            }

            return false;
        }

        private static void StartExe(string exe)
        {
            var startInfo = new ProcessStartInfo(exe)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = BackgroundArg,
            };
            Process.Start(startInfo);
        }

        private static async Task CopyExe(string source, string destination)
        {
            for (var i = 0; i < 10; ++i)
            {
                try
                {
                    File.Copy(source, destination, true);
                    return;
                }
                catch (Exception)
                {
                    if (i == 9) throw;

                    // Main process has not exited after update yet
                    // Or process has exited but OS file lock is not cleared yet
                    // It's usually a matter of a second but give it more time just in case
                    await Task.Delay(2_000);
                }
            }
        }

        private static void TerminateMainExe()
        {
            var processes = Process.GetProcessesByName(AppName).Where(x => x.Id != Environment.ProcessId);
            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
