using System.Diagnostics;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
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
            try
            {
            
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
            finally {
                process.Dispose();
            }
           
        }

        //async bool ModuleCheck(string name) => await ExecuteCommand(@$"Get-Module -ListAvailable -Name {name}").Contains($@"{name}");
        #region Deployment Crud
        public string CreateWebCommand(string name, int port, string ipAddr = "*")
            => @$"New-WebSite -Name {name}-{port} -Port {port} -IpAddress {ipAddr} -PhysicalPath C:\inetpub\wwwroot\{name} -ApplicationPool {name}";
        public string CreatePoolCommand(string name) => @"$exitStatus = 0; try {New-WebAppPool -Name " + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus }";
        public string StartWebCommand(string name) => @"$exitStatus = 0; try {Start-WebSite -Name " + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus }";
        public string StartPoolCommand(string name) => @"exitStatus = 0; try {Start-WebAppPool -Name " + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string StopPoolCommand(string name) => @"exitStatus = 0; try {Stop-WebAppPool -Name " + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string StopWebCommand(string name) => @"exitStatus = 0; try {Stop-Website -Name " + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string RenamefolderCommand(string name) => @"exitStatus = 0; try {Rename-Item" + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string UnzipCommand(string zipname) => @"$exitStatus = 0; try { Expand-Archive -Path " + zipname + " -DestinationPath C:\\inetpub\\wwwroot\\ -ErrorAction stop; } catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; } ";
        public string DeleteCommand(string pathname) => @"exitStatus = 0; try {Remove-Item "+pathname + " -Recurse;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string DeleteWebCommand(string name) => @"exitStatus = 0; try { Remove-WebAppPool -Name " + name +"} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public string DeletePoolCommand(string name) => @"exitStatus = 0; try { Remove-WebApplication -Name "+name+"} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        
        #endregion



        #region GetStates
    public enum State
        {
            Pool,
            Web,
            Server
        }
        public async Task<string> GetState(string name, State state)
        {
            if (state == State.Pool || state == State.Web)
                return await ExecuteCommand(@"$exitStatus = 0; try { $State = " + (state == State.Pool ? "Get-WebAppPoolState" : "Get-WebsiteState") + $"-Name {name} " + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.;  $exitStatus = 20; } finally {if ($State - eq $null ) { $StateResult = \"null\";} else{ $StateResult = $State.Value} $result = $StateResult.ToString() + \":\" + $exitStatus; echo $result }");
            else
                return await ExecuteCommand(@"try {$Getname = hostname;  switch ($Getname) {  ""HF-ProgTest01""{ $name = ""HF-ProgTest02""; break; }""HF-ProgTest02""{ $name = ""HF-ProgTest01""; break;}}$TestConn = Test-Connection $name -count 2 -Quiet -ErrorAction Stop; } catch {echo ""An error occurred:""; echo $_;} finally { if ($TestConn) { echo $TestConn; } else{ echo $TestConn;}}");
        }

        

        #endregion
    }
}
