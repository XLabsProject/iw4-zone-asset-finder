using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder
{
    public static class Helper
    {
        public static string GetProgramOutput(string programName, string commandLineArguments, out int exitCode)
        {
            Process process = new Process();
            process.StartInfo.FileName = programName;
            process.StartInfo.Arguments = commandLineArguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            exitCode = process.ExitCode;

            return output;
        }

        public static string GetProgramOutput(string programName, string commandLineArguments)
        {
            return GetProgramOutput(programName, commandLineArguments, out _);
        }
    }
}
