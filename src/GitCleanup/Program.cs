using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitCleanup
{
    class Program
    {
        class Args
        {
            public string Repo { get; set; } = ".";
            public string Remote { get; set; } = "origin";
            public string Trunk { get; set; } = "master";
        }
        static void Main(string[] stringArgs)
        {
            var args = PowerCommandParser.Parser.ParseArguments<Args>(stringArgs, true);
            if (args == null)
                return;
            args.Repo = System.IO.Path.GetFullPath(args.Repo);
            var repo = new Repository(Repository.Discover(args.Repo));
            repo.Fetch("origin", new FetchOptions() { Prune = true });
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
    }
}
