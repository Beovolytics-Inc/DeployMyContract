using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeployMyContract.Core.Contracts;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Core.Logic
{
    public class ContractSourceParser : IContractSourceParser
    {
        private static readonly Regex M_VersionPragmaRegex = new Regex(
            @"^\s*pragma solidity \^(?<version>\d+\.\d+\.\d+);",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex M_ImportDirectiveRegex = new Regex(
            @"^\s*import ['\""](?<path>.+?)['\""];",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        public ContractSource Parse(string source, DirectoryInfo currentDirectory)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (currentDirectory == null)
                throw new ArgumentNullException(nameof(currentDirectory));

            return ParseInternal(source, currentDirectory, new List<string>());
        }

        private ContractSource ParseInternal(string source, DirectoryInfo currentDirectory, List<string> importedFiles)
        {
            var versionPragma = M_VersionPragmaRegex.Match(source);
            if (!versionPragma.Success)
                throw new ContractParsingException("Version pragma directive not found");
            var importedSources = M_ImportDirectiveRegex.Matches(source)
                .Cast<Match>()
                .Select(x => x.Groups["path"].Value.TrimStart('/'))
                .Select(x =>
                    ParseFile(new FileInfo(Path.IsPathRooted(x) ? x : Path.Combine(currentDirectory.FullName, x)), importedFiles))
                .Where(x => x != null)
                .Select(x => x.SourceCode)
                .ToArray();
            return new ContractSource
            {
                PragmaVersion = Version.Parse(versionPragma.Groups["version"].Value),
                SourceCode = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    importedSources.Concat(new[]
                    {
                        M_ImportDirectiveRegex.Replace(source, string.Empty)
                    }))
            };
        }

        private ContractSource ParseFile(FileInfo file, List<string> importedFiles)
        {
            if (!file.Exists)
                throw new ContractParsingException($"File {file.FullName} doesn't exist");
            if (importedFiles.Contains(file.FullName, StringComparer.InvariantCultureIgnoreCase))
                return null;

            string source;
            try
            {
                using (var reader = file.OpenText())
                    source = reader.ReadToEnd();
                importedFiles.Add(file.FullName);
            }
            catch (Exception)
            {
                throw new ContractParsingException($"File {file.FullName}: I/O error");
            }
            return ParseInternal(source, file.Directory, importedFiles);
        }
    }
}
