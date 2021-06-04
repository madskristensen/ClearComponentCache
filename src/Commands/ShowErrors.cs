using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.ComponentModel.Design;
using System.IO;

namespace ClearComponentCache
{
    internal sealed class ShowErrors
    {
        private ShowErrors(OleMenuCommandService commandService, AsyncPackage package)
        {
            ServiceProvider = package;

            var commandID = new CommandID(PackageGuids.guidClearCachePackageCmdSet, PackageIds.ShowErrorsId);
            var button = new MenuCommand(OpenErrorFile, commandID);
            commandService.AddCommand(button);
        }

        public static ShowErrors Instance { get; private set; }

        private AsyncPackage ServiceProvider { get; }

        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ShowErrors(commandService, package);
        }

        private void OpenErrorFile(object sender, EventArgs e)
        {
            var folder = ServiceProvider.UserLocalDataPath;
            var file = Path.Combine(folder, "ComponentModelCache", "Microsoft.VisualStudio.Default.err");

            if (!File.Exists(file))
            {
                ServiceProvider.JoinableTaskFactory.RunAsync(async () =>
                {
                    var statusBar = await ServiceProvider.GetServiceAsync(typeof(SVsStatusbar)) as IVsStatusbar;
                    Assumes.Present(statusBar);

                    statusBar.IsFrozen(out int isFrozen);

                    if (isFrozen == 1)
                    {
                        statusBar.FreezeOutput(0);
                    }

                    statusBar.SetText($"Couldn't find file at '{file}'.");
                });
            }
            else
            {
                VsShellUtilities.OpenDocument(ServiceProvider, file, Guid.Empty, out _, out _, out _);
            }
        }
    }
}
