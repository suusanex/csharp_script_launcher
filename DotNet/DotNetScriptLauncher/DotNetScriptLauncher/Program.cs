using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

if (args.Length < 1)
{
    await Console.Error.WriteLineAsync("使用方法: DotNetScriptLauncher.exe <スクリプトファイル.csx> [引数1 引数2 ...]");
    return 1;
}

string scriptPath = args[0];
if (!File.Exists(scriptPath))
{
    await Console.Error.WriteLineAsync($"エラー: スクリプトファイルが見つかりません: {scriptPath}");
    return 1;
}

string scriptCode = await File.ReadAllTextAsync(scriptPath);
string[] scriptArgs = args.Length > 1 ? args[1..] : [];

var globals = new ScriptGlobals { Args = scriptArgs };

string[] importNameSpaces =
[
    "System",
    "System.IO",
    "System.Collections.Generic",
    "System.Console",
    "System.Diagnostics",
    "System.Dynamic",
    "System.Linq",
    "System.Linq.Expressions",
    "System.Text",
    "System.Threading",
    "System.Threading.Tasks"
];

string[] importAssemblies =
[
    "System",
    "System.Core",
    "System.Data",
    "System.Data.DataSetExtensions",
    "System.Runtime",
    "System.Xml",
    "System.Xml.Linq",
    "System.Net.Http",
    "Microsoft.CSharp",
];

var options = ScriptOptions.Default
    .AddReferences(importAssemblies)
    .AddImports(importNameSpaces);

try
{
    var script = CSharpScript.Create(scriptCode, options, globalsType: typeof(ScriptGlobals));
    var result = await script.RunAsync(globals);
    if (result.ReturnValue is int returnInt)
    {
        Console.WriteLine($"スクリプトの戻り値: {returnInt}");
        return returnInt;
    }

}
catch (CompilationErrorException ex)
{
    await Console.Error.WriteLineAsync("コンパイルエラー:");
    foreach (var diagnostic in ex.Diagnostics)
    {
        await Console.Error.WriteLineAsync(diagnostic.ToString());
    }
    return 1;
}
catch (Exception ex)
{
    await Console.Error.WriteLineAsync($"実行時エラー: {ex.Message}");
    return 1;
}

return 0;

public class ScriptGlobals
{
    public string[] Args { get; set; } = [];
}
