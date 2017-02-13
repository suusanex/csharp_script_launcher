using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSScriptLauncher
{
    class Program
    {

        static void Trace(TraceEventType type, string msg)
        {
            switch (type)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    Console.Error.WriteLine(msg);
                    break;
                case TraceEventType.Information:
                case TraceEventType.Verbose:
                default:
                    Console.Out.WriteLine(msg);
                    break;
            }
        }

        static async Task<int> RunScript(string ScriptPath)
        {
            //#loadの参照先をカレントフォルダ基準にする
            //#rの参照先をカレントフォルダ基準にする
            //（スクリプト単独のフォルダで完結させたいため）
            var ssr = ScriptSourceResolver.Default
                .WithBaseDirectory(Environment.CurrentDirectory);
            var smr = ScriptMetadataResolver.Default
                .WithBaseDirectory(Environment.CurrentDirectory);
            var so = ScriptOptions.Default
                .WithSourceResolver(ssr)
                .WithMetadataResolver(smr);

            var scriptText = System.IO.File.ReadAllText(ScriptPath);
            var script = CSharpScript.Create(scriptText, so);
            var ret = await script.RunAsync();

            var retval = (int)ret.ReturnValue;
            return retval;

        }

        static int Main(string[] args)
        {
            try
            {
                if (!args.Any())
                {
                    Trace(TraceEventType.Error, "argument[0]:Script File Full Path");
                    return 1;
                }
                
                var task = RunScript(args[0]);
                task.Wait();
                
                if(task.Result != 0)
                {
                    Trace(TraceEventType.Error, $"Script Fail, ReturnCode={task.Result:d}");
                    return 3;
                }

            }
            catch (Exception ex)
            {
                Trace(TraceEventType.Error, "Script Run Fail, " + ex.ToString());
                return 2;
            }

            return 0;
        }
    }
}
