
# 概要

CSScriptLauncher.exe：C#スクリプトを実行する自作exe。csi.exeの簡易版に過ぎないが、csi.exeの再頒布方法がわからなかったので作成した。

使用方法としては、次のコマンドを実行する。＜＞は可変値。
CSScriptLauncher.exe ＜.csxのフルパス＞ ＜コマンドラインオプション＞

例："E:\tool\CSScriptLauncher.exe" "E:\tool\tool.csx"


# .csxファイルTIPS

## コマンドライン引数

.csxファイルに次のように記載することでコマンドライン引数を取得可能。

    string[] CommandLineOptions = Environment.GetCommandLineArgs().Skip(2).ToArray();

CSScriptLauncher.exeは第1引数にスクリプトのパスを受け取る。そのため、第2引数以降が本スクリプト向けのオプションなので、2つSkipするとコマンドラインの配列になる。

## アプリケーション終了コード

.csxファイルでreturn 0;のように数値を返すと、それがアプリケーション終了コードになる。
