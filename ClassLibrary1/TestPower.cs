using System.Diagnostics;

namespace ClassLibrary1
{
    public class TestPower
    {


        //public async Task<string> ExecuteCommand(string Command)
        // {
        //     var tcs = new TaskCompletionSource<string>();

        //     var process = new Process
        //     {
        //         StartInfo = {    FileName = "powershell.exe",
        //         Arguments = $"-Command \"{Command}\"",
        //         UseShellExecute = false,
        //         RedirectStandardOutput = true }
        //     };

        //     process.Exited += (sender, args) =>
        //     {
        //         tcs.SetResult(process.StandardOutput.ReadToEnd().ToString());
        //         process.Dispose();
        //     };
        //     process.Start();

        //     return tcs.Task.Result;
        // }

        public string ExecuteCommand(string command)
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
            process.WaitForExit(10);
            return process.StandardOutput.ReadToEnd().ToString();
        }

        //async bool ModuleCheck(string name) => await ExecuteCommand(@$"Get-Module -ListAvailable -Name {name}").Contains($@"{name}");

        public async Task<string> CreateWebCommand(string name, int port, string ipAddr = "*")
            => @$"New-WebSite -Name {name}-{port} -Port {port} -IpAddress {ipAddr} -PhysicalPath C:\inetpub\wwwroot\{name} -ApplicationPool {name}";
        public async Task<string> StopWebCommand(string name) => @"try {Stop-Website -Name " + name + " -ErrorAction stop; echo finished} catch {echo failed}";
        public async Task<string> RenamefolderCommand(string name) => @"try {Rename-Item" + name + " -ErrorAction stop; echo finished} catch {echo failed}";

        public async Task<string> CreatePoolCommand(string name) => @"try {New-WebAppPool -Name " + name + " -ErrorAction stop; echo finished} catch {echo failed}";
        public async Task<string> StartWebCommand(string name) => @"try {Start-WebSite -Name " + name + " -ErrorAction stop; echo finished} catch {echo failed}";
        public async Task<string> StopPoolCommand(string name) => @"try {Stop-WebAppPool -Name " + name + " -ErrorAction stop; echo finished} catch {echo failed}";
        public async Task<string> StartPoolCommand(string name) => @"try {Start-WebAppPool -Name " + name + " -ErrorAction stop; echo finished} catch {echo failed}";


        //public async Task<string> UnzipCommand(string zipname) => @$"Expand-Archive -Path {zipname} -DestinationPath C:\inetpub\wwwroot\";
        public async Task<string> UnzipCommand(string zipname) => @"try { Expand-Archive -Path " + zipname + @" -DestinationPath C:\inetpub\wwwroot\ -ErrorAction stop; echo Finished  } catch {echo Failed}";
        public async Task<string> DeleteCommand(string pathname) => @"try { Remove-Item " + pathname + " -Recurse -ErrorAction stop; echo Finished } catch {echo Failed}";

        //        public List<string> commands = new(){
        //@"Test-Path IIS:\AppPools\;",
        //@"Get-Website -Name ;",
        //@"Expand-Archive -Path ;.zip -DestinationPath C:\inetpub\wwwroot\;",
        //@"Start-Website -Name ;",
        //@"Stop-Website -Name ;",
        //@"Start-WebAppPool -Name ;",
        //@"Remove-Item 'C:\inetpub\wwwroot\;",
        //@"New-WebSite -Name ; -Port ; -IpAddress ; -PhysicalPath C:\inetpub\wwwroot\; -ApplicationPool ;"
        //};




        public async Task<string> poolCheck(string name) => ExecuteCommand(@"$result = Get-WebAppPoolState -Name " + name + "; echo $result");
        public async Task<string> webCheck(string name) => ExecuteCommand(@"$result = Get-Website -Name " + name + ";  echo $result.GetAttributeValue(\"State\")");


        //public async Task<string> CommandCheck(string name)
        //{
        //    string existing = ExecuteCommand(name);
        //    return existing;
        //}
        //public async Task<string> CommandWithArgument(string command, string[] args)
        //{
        //    string commandbuild = null;
        //    for (int i = 0; i < args.Count(); i++)
        //    {
        //        commandbuild += command.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()[i] + args[i];
        //    }
        //    return commandbuild;

        //}


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
