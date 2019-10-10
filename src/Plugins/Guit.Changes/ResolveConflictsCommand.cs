﻿using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Merq;

namespace Guit.Plugin.Changes
{
    [Shared]
    [MenuCommand(CommandIds.ResolveConflicts, 'v', ContentViewIds.Changes, typeof(ResolveConflictsCommand))]
    public class ResolveConflictsCommand : IMenuCommand
    {
        readonly IEventStream eventStream;
        readonly MainThread mainThread;
        readonly IRepository repository;
        readonly ChangesView changes;

        [ImportingConstructor]
        public ResolveConflictsCommand(IEventStream eventStream, MainThread mainThread, IRepository repository, ChangesView changes)
        {
            this.eventStream = eventStream;
            this.mainThread = mainThread;
            this.repository = repository;
            this.changes = changes;
        }

        public Task ExecuteAsync(CancellationToken cancellation)
        {
            var dialog = new ResolveConflictsDialog(repository, repository.Index.Conflicts);
            if (mainThread.Invoke(() => dialog.ShowDialog()) == true)
            {
                foreach (var resolved in dialog.GetResolvedConflicts().ToArray())
                {
                    repository.Index.Add(resolved.Ours.Path);
                    repository.Index.Write();
                }
            }

            return Task.CompletedTask;
        }
   }
}