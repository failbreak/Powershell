using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClassLibrary1
{
    public class PowerShellRunner
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
        public static string CreateWebCommand(Website website)
            => @$"New-WebSite -Name {website.Name} -Port {website.Port} -IpAddress {website.Ipaddr} -PhysicalPath C:\inetpub\wwwroot\{website.Name} -ApplicationPool {website.Name}";
        public static string CreatePoolCommand(Website website)
            => @"$exitStatus = 0; try {New-WebAppPool -Name " + website.Name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus }";
        public static string StartWebCommand(Website website) 
            => @"$exitStatus = 0; try {Start-WebSite -Name " + website.Name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus }";
        public static string StartPoolCommand(Website website) 
            => @"exitStatus = 0; try {Start-WebAppPool -Name " + website.Name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string StopPoolCommand(Website website) 
            => @"exitStatus = 0; try {Stop-WebAppPool -Name " + website.Name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string StopWebCommand(Website website)
            => @"exitStatus = 0; try {Stop-Website -Name " + website.Name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string SetWebbindingCommand(Website website)
            => @"exitStatus = 0; try {Set-WebBinding -Name " + website.Name + " -Port " + website.Port + " -IpAddress " + website.Ipaddr + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string RenamefolderCommand(string name) 
            => @"exitStatus = 0; try {Rename-Item" + name + " -ErrorAction stop;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string UnzipCommand(string zipname) 
            => @"$exitStatus = 0; try { Expand-Archive -Path " + zipname + " -DestinationPath C:\\inetpub\\wwwroot\\ -ErrorAction stop; } catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; } ";
        public static string DeleteCommand(string pathname) 
            => @"exitStatus = 0; try {Remove-Item " + pathname + " -Recurse;} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string DeleteWebCommand(Website website) 
            => @"exitStatus = 0; try { Remove-WebAppPool -Name " + website.Name + "} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        public static string DeletePoolCommand(Website website) 
            => @"exitStatus = 0; try { Remove-WebApplication -Name " + website.Name + "} catch {echo \"An error occurred:\"; echo $_.ErrorDetals; $exitStatus = 20;} finally { echo $exitStatus; }";
        #endregion



        #region GetStates
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum State
        {
            Pool,
            Web,
            Server
        }
        public async Task<string> GetState(string name, State state)
        {
            if (state == State.Pool || state == State.Web)
                return await ExecuteCommand(@"$exitStatus = 0; try { $State = " + (state == State.Pool ? "Get-WebAppPoolState" : "Get-WebsiteState") + $" -Name {name} " + " -ErrorAction stop;} catch {echo "+@"""An error occurred:""" + "; echo $_;  $exitStatus = 20; } finally {if ($State -eq $null ) { $StateResult = " + @"""null""" + ";} else{ $StateResult = $State.Value} $result = $StateResult + " + @"':'" + " + $exitStatus; echo $result }");
            else
                return await ExecuteCommand(@"$TestConn = 1; try {$TestConn = Test-Connection """+ name + @""" -count 2 -Quiet -ErrorAction Stop; } catch {echo ""An error occurred:""; echo $_;} finally { echo $TestConn;}");
        }

        

        #endregion
    }
}
