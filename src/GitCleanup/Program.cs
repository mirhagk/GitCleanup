using Humanizer;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitCleanup
{
    class Program
    {
        class Args
        {
            //public enum ActionToTake
            //{
            //    Cleanup, WhatDo
            //}
            [PowerCommandParser.Position(0)]
            [PowerCommandParser.Required]
            //public ActionToTake Action { get; set; }
            public string Action { get; set; }
            [PowerCommandParser.Position(1)]
            public string Argument { get; set; }
            public string Repo { get; set; } = ".";
            public string Remote { get; set; } = "origin";
            public string Trunk { get; set; } = "master";
            public bool Cleanup { get; set; }
            public bool WhatDo { get; set; }
        }
        static void Cleanup(Repository repo, Args args)
        {
            RunCommand("git fetch -p", args.Repo);

            var trunk = repo.Branches.Single(b => b.FriendlyName == args.Trunk);
            var mergedBranches = new List<Branch>();
            foreach (var branch in repo.Branches)
            {
                if (branch == trunk)
                    continue;
                if (branch.IsRemote)
                    continue;
                if (trunk.Commits.Contains(branch.Tip))
                {
                    mergedBranches.Add(branch);
                    Console.WriteLine($"Removing {branch.FriendlyName} as it is merged into {trunk.FriendlyName}");
                    repo.Branches.Remove(branch);
                }
            }
            Console.WriteLine("Done cleaning up.");
        }
        static string RunCommand(string command, string workingDirectory)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo("cmd",$"/c {command}")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDirectory
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        static void GitFlowStartNewBranch(Repository repo, Args args)
        {
            var newBranchName = $"feature-{args.Argument.Dehumanize().Camelize()}";
            Console.WriteLine($"Creating new branch {newBranchName}");
        }
        static void DiscoverActionsToDo(Repository repo, Args args)
        {

        }
        static void Main(string[] stringArgs)
        {
            var args = PowerCommandParser.Parser.ParseArguments<Args>(stringArgs, true);
            if (args == null)
                return;
            args.Repo = System.IO.Path.GetFullPath(args.Repo);
            var repo = new Repository(Repository.Discover(args.Repo));

            switch (args.Action.ToLowerInvariant())
            {
                case "cleanup":
                    Cleanup(repo, args);
                    break;
                case "whatdo":
                    DiscoverActionsToDo(repo, args);
                    break;
                case "start":
                    GitFlowStartNewBranch(repo, args);
                    break;
            }
        }
    }
}
