using System.Diagnostics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            process.WaitForExit(10);
            return await process.StandardOutput.ReadToEndAsync();
           
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
        public async Task<string> UnzipCommand(string zipname) => @"$exitStatus = 0; try { Expand-Archive -Path " + zipname + " -DestinationPath C:\\inetpub\\wwwroot\\ -ErrorAction stop; } catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; } ";
        public async Task<string> DeleteCommand(string pathname) => @$"Remove-Item '{pathname}' -Recurse";

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


        public async Task<string> poolGetState(string name)
            => await ExecuteCommand(@"$exitStatus = 0; try { $State = Get-Website -Name " + name + " -ErrorAction stop;} catch { echo \"An error occurred: \"; echo $_. ;  $exitStatus = 20;} finally { $result = $State.value + \":\" + $exitStatus; echo $result}");
        /* Add this to WebGetState
        ## Dev log number 01:51. Sanity? what is sanity. 
        ## Script works its not pretty. but i will start fighting if anyone comments on it
        ## also null values are a bitch. funny enough I DIDNT HAVE THIS PROBLEM BEFORE!!!!!
        $exitStatus = 0; 
        try { 
        $State = Get-Website -Name pub -ErrorAction stop; 
        } 
        catch {
        echo "An error occurred:"; echo $_. ;  $exitStatus = 20; 
        } finally 
        { 
        if($State -eq $null )
        {
        $StateResult = "null"; 
        }
        else
        { 
        $StateResult = $State.GetAttributeValue("State")
        } 
        $result = $StateResult.ToString() + ":" + $exitStatus; echo $result 
        }
        */

        /// <summary>
        ///  Gets the sate of a website
        ///  1(started),3(stopped),null(Doesnt exist)
        ///  exit code: 0 ok, 20 failure
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string, string </returns>
        public async Task<string> WebGetState(string name) => await ExecuteCommand(@"$exitStatus = 0;  try { $State = Get-Website -Name "+ name+" -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_. ;  $exitStatus = 20; } finally {if ($State - eq $null ) { $StateResult = \"null\";} else{ $StateResult = $State.GetAttributeValue(\"State\")} $result = $StateResult.ToString() + \":\" + $exitStatus; echo $result }");

        #region Deprecated or bad
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
        #endregion
    }
}
