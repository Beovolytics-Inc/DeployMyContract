using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeployMyContract.Core.Contracts;
using DeployMyContract.Core.Data;
using Newtonsoft.Json.Linq;

// ReSharper disable AccessToDisposedClosure

namespace DeployMyContract.Core.Logic
{
    public class ContractCompiler : IContractCompiler
    {
        //Nigtly build version string: 0.4.18-nightly.2017.10.9+commit.6f832cac
        //Release build version string: 0.4.18+commit.6f832cac
        private static readonly Regex M_VersionRegex = new Regex(
            @"^Version:\s*(?<version>(?<versionNumber>\d+\.\d+\.\d+)(-nightly\.\d+\.\d+\.\d+)?\+commit\.[0-9a-h]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly TimeSpan M_WaitCompilerProcessTimeout = TimeSpan.FromMinutes(1);

        private readonly FileInfo m_CompilerFile;

        public ContractCompiler(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            m_CompilerFile = new FileInfo(path);
        }

        public async Task<CompilerVersion> GetVersionAsync()
        {
            var versionString = await ExecuteProcessAsync("--version", null);
            var versionMatch = M_VersionRegex.Match(versionString.output);
            if (!versionMatch.Success)
                throw new ContractCompilationException("Couldn't determine the version of compiler. Is it really solc?");
            return new CompilerVersion
            {
                Version = Version.Parse(versionMatch.Groups["versionNumber"].Value),
                FullVersionString = versionMatch.Groups["version"].Value
            };
        }

        public async Task<CompilationResult> CompileAsync(string source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var compileResult = await ExecuteProcessAsync("--combined-json abi,bin --optimize", source);
            if (string.IsNullOrWhiteSpace(compileResult.output))
                throw new ContractCompilationException("Compilation error: " + compileResult.error);
            try
            {
                return new CompilationResult
                {
                    Warnings = compileResult.error,
                    Outputs = JObject.Parse(compileResult.output)["contracts"]
                        .Value<JObject>()
                        .Properties()
                        .Select(x => new CompilationOutput
                        {
                            ContractName = x.Name.Split(':').Last(),
                            ConstructorParameterTypes = ParseConstructorSignature(
                                JArray.Parse(x.Value["abi"].Value<string>())),
                            Bin = x.Value["bin"].Value<string>(),
                            Abi = x.Value["abi"].Value<string>()
                        })
                        .ToArray()
                };
            }
            catch (Exception ex)
            {
                throw new ContractCompilationException(ex.Message);
            }
        }

        private static IReadOnlyDictionary<string, string> ParseConstructorSignature(JArray abi)
        {
            var constructor = abi.FirstOrDefault(x => x["type"].Value<string>() == "constructor");
            if (constructor == null)
                return new Dictionary<string, string>();
            return constructor["inputs"]
                .Value<JArray>()
                .ToDictionary(x => x["name"].Value<string>(), x => x["type"].Value<string>());
        }

        private async Task<(string output, string error)> ExecuteProcessAsync(string arguments, string input)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = m_CompilerFile.FullName,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = input != null
                }
            })
            using (Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    x => process.OutputDataReceived += x,
                    x => process.OutputDataReceived -= x)
                .Select(x => x.EventArgs.Data)
                .Subscribe(x => outputBuilder.AppendLine(x)))
            using (Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    x => process.ErrorDataReceived += x,
                    x => process.ErrorDataReceived -= x)
                .Select(x => x.EventArgs.Data)
                .Subscribe(x => errorBuilder.AppendLine(x)))
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (input != null)
                    using (process.StandardInput)
                        await process.StandardInput.WriteLineAsync(input);

                var exitResult = await Task.Factory.StartNew(() =>
                    process.WaitForExit((int) M_WaitCompilerProcessTimeout.TotalMilliseconds));
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                process.CancelOutputRead();
                process.CancelErrorRead();
                if (exitResult)
                    return (outputBuilder.ToString(), errorBuilder.ToString());
                process.Kill();
                throw new ContractCompilationException("Compiler process didn't exit during specified timeout");
            }
        }
    }
}
