using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.BuildTasks
{
    public class RunAndroidTests : AbstractTask
    {

        public RunAndroidTests()
        {
            this.StartEmulatorArgumentsFormatString = "-avd {0} -no-boot-anim";
            this.InstallApkArgumentsFormatString = "install -r \"{0}\"";
            this.RunTestsArgumentsFormatString  = "shell am instrument -w {0}/{1}";         

        }

        /// <summary>
        /// The full path to the emulator exe.
        /// </summary>
        [Required]
        public string EmulatorExePath { get; set; }

        /// <summary>
        /// The full path to adb.exe.
        /// </summary>
        [Required]
        public string AdbExePath { get; set; }

        /// <summary>
        /// The full path to your build APK file for your android tests.
        /// </summary>
        [Required]
        public string ApkPath { get; set; }

        /// <summary>
        /// The name of the AVD image to launch in the emulator.
        /// </summary>
        [Required]
        public string AvdName { get; set; }

        /// <summary>
        /// The name of your android package as per the manifest.
        /// </summary>
        [Required]
        public string ApkPackageName { get; set; }

        /// <summary>
        /// The class path for your android test instruemntation class.
        /// </summary>
        [Required]
        public string TestInstrumentationClassPath { get; set; }

        /// <summary>
        /// The number of seconds to wait for the emulator to startup.
        /// </summary>
        [Required]
        public int EmulatorStartupWaitTimeInSeconds { get; set; }

        /// <summary>
        /// The arguments passed to the emulator.
        /// </summary>     
        public string StartEmulatorArgumentsFormatString { get; set; }

        /// <summary>
        /// The arguments passed to adb to install the tests apk package..
        /// </summary>     
        public string InstallApkArgumentsFormatString { get; set; }

        /// <summary>
        /// The arguments passed to adb to run your tests using your test instrumentation class.
        /// </summary>     
        public string RunTestsArgumentsFormatString { get; private set; }

        /// <summary>
        /// The result output.
        /// </summary>
        [Output]
        public string Output { get; set; }
          

        public override bool ExecuteTask()
        {

            string name = System.IO.Path.GetFileNameWithoutExtension(EmulatorExePath);
            this.LogMessage(string.Format("Starting emulator: {0}...", name));

            string emulatorArgs = GetEmulatorArgs();
            var emulatorProcess = new System.Diagnostics.ProcessStartInfo(EmulatorExePath, emulatorArgs);
            emulatorProcess.UseShellExecute = true;
            using (var process = System.Diagnostics.Process.Start(emulatorProcess))
            {

                // wait for emulator to load.
                var sleepTime = new TimeSpan(0, 0, 0, EmulatorStartupWaitTimeInSeconds, 0);
                LogMessage(string.Format("Waiting for emulator to load up. {0} (hh:mm:ss) ", sleepTime), MessageImportance.Normal);
                System.Threading.Thread.Sleep(sleepTime);

                var installApkArgs = GetInstallApkArgs();
                var adbProcess = new System.Diagnostics.ProcessStartInfo(AdbExePath, installApkArgs);
                adbProcess.UseShellExecute = true;
                using (var installApkProcess = System.Diagnostics.Process.Start(adbProcess))
                {

                    installApkProcess.WaitForExit();
                    if(installApkProcess.ExitCode != 0)
                    {
                        LogMessage(string.Format("Unable to install test apk. Adb args was: {0}", installApkArgs), MessageImportance.High);
                        return false;
                    }                

                }

                string runTestsArgs = GetRunTestsArgs();
                adbProcess = new System.Diagnostics.ProcessStartInfo(AdbExePath, runTestsArgs);
                using (var runTestsProcess = System.Diagnostics.Process.Start(adbProcess))
                {

                    runTestsProcess.WaitForExit();

                    var output = runTestsProcess.StandardOutput.ReadToEnd();
                    this.Output = output;

                    if (runTestsProcess.ExitCode != 0)
                    {
                        LogMessage(string.Format("Tests failed: {0}", runTestsArgs), MessageImportance.High);
                        return false;
                    }

                    return true;       


                }

            }          

        }

        private string GetRunTestsArgs()
        {
            return string.Format(RunTestsArgumentsFormatString, ApkPackageName, TestInstrumentationClassPath);
        }

        protected virtual string GetEmulatorArgs()
        {
            return string.Format(StartEmulatorArgumentsFormatString, AvdName);
        }

        protected virtual string GetInstallApkArgs()
        {
            return string.Format(InstallApkArgumentsFormatString, ApkPath);
        }
    }

}
