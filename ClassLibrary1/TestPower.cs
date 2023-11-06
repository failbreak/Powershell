using System.Diagnostics;

namespace ClassLibrary1
{
    public class TestPower
    {

        public async Task<string> ExecuteCommand(string command)
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
            return process.StandardOutput.ReadToEnd().ToString();
        }

        //async bool ModuleCheck(string name) => await ExecuteCommand(@$"Get-Module -ListAvailable -Name {name}").Contains($@"{name}");

        public async Task<string> CreateWebCommand(string name, int port, string ipAddr = "*")
            => @$"New-WebSite -Name {name} -Port {port} -IpAddress {ipAddr} -PhysicalPath C:\inetpub\wwwroot\{name} -ApplicationPool {name}";
        public async Task<string> StopWebCommand(string name) => @$"Stop-Website -Name {name}";
        public async Task<string> RenamefolderCommand(string name) => @$"Rename-Item {name}";

        public async Task<string> CreatePoolCommand(string name) => @$"New-WebAppPool -Name {name}";
        public async Task<string> StartWebCommand(string name) => @$"Start-Website -Name {name}";
        public async Task<string> StopPoolCommand(string name) => @$"Stop-WebAppPool -Name {name}";
        public async Task<string> StartPoolCommand(string name) => @$"Start-WebAppPool -Name {name}";
        public async Task<string> UnzipCommand(string zipname) => @$"Expand-Archive -Path {zipname} -DestinationPath C:\inetpub\wwwroot\";
        public async Task<string> DeleteCommand(string pathname) => @$"Remove-Item '{pathname}' -Recurse";

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




        public async Task<bool> poolCheck(string name)
        {
            string existing = await ExecuteCommand(@$"Test-Path IIS:\AppPools\{name}");
            return existing.Contains("True");
        }
        public async Task<bool> webCheck(string name)
        {
            string existing = await ExecuteCommand(@$"Get-Website -Name {name}");
            return existing.Contains("True");
        }
        public async Task<string> CommandWithArgument(string command, string[] args)
        {
            string commandbuild = null;
            for (int i = 0; i < args.Count(); i++)
            {
                commandbuild += command.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()[i] + args[i];
            }
            return commandbuild;
        }



        //public async Task<string> CheckIfExist(string name, byte index)
        //{
        //string[] command = { @$"Test -Path IIS:\AppPools\{name}", @$"Get-Website -Name {name}" };
        //    string error = "";
        //string test = await ExecuteCommand(command[index]).Contains("True");

        //        return error = @$"{name} already exist";
        //    else
        //        return error;
        //}

    }
}
