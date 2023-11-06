using System.Diagnostics;
using System.Xml.Linq;

namespace testPowershell
{
    public class Powershell
    {


        #region powershell
        string ExecuteCommand(string command)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }

        bool ModuleCheck(string name)
        {
            return ExecuteCommand(@$"Get-Module -ListAvailable -Name {name}").Contains($@"{name}");
        }

        public string CreateWebCommand(string name, int port)
            => @$"New-WebSite -Name {name} -Port {port} -IpAddress * -PhysicalPath C:\inetpub\wwwroot\{name} -ApplicationPool {name}";
        public string StopWebCommand(string name) => @$"Stop-Website -Name {name}";

        public string CreatePoolCommand(string name) => @$"New-WebAppPool -Name {name}";
        public string StartWebCommand(string name) => @$"Start-Website -Name {name}";
        public string StopPoolCommand(string name) => @$"Stop-WebAppPool -Name {name}";
        public string StartPoolCommand(string name) => @$"Start-WebAppPool -Name {name}";
        public string UnzipCommand(string zipname, string name) => @$"Expand-Archive -Path {zipname} -DestinationPath C:\Users\maxx0696\Desktop\{name}";
        public string DeleteCommand(string name) => @$"Remove-Item 'C:\inetpub\wwwroot\{name}' -Recurse";

        public List<string> commands = new(){
@"Test-Path IIS:\AppPools\;",
@"Get-Website -Name ;",
@"Expand-Archive -Path ;.zip -DestinationPath C:\inetpub\wwwroot\;",
@"Start-Website -Name ;",
@"Stop-Website -Name ;",
@"Start-WebAppPool -Name ;",
@"Remove-Item 'C:\inetpub\wwwroot\;",
@"New-WebSite -Name ; -Port ; -IpAddress ; -PhysicalPath C:\inetpub\wwwroot\; -ApplicationPool ;"
};



        public string CommandWithArgument(string command, string[] args)
        {
            string commandbuild = null;
            for (int i = 0; i < args.Count(); i++)
            {
                commandbuild += command.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()[i] + args[i];
            }
            return commandbuild;
        }



        public string CheckIfExist(string name)
        {
            string error = "";
            if (ExecuteCommand(name).Contains("True"))
                return error = @$"{name} already exist";
            else
                return error;
        }

    }
}
#endregion