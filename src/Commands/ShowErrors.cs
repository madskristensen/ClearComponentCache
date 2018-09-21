using System;
using System.ComponentModel.Design;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

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

        private async void OpenErrorFile(object sender, EventArgs e)
        {
            var shell = await ServiceProvider.GetServiceAsync(typeof(SVsComponentModelHost)) as IVsComponentModelHost;
            var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;
            OpenErrorFile(shell, dte);
        }

        private static void OpenErrorFile(IVsComponentModelHost componentModelHost, DTE dte)
        {
            var file = componentModelHost.GetDefaultErrorFile();
            if (!File.Exists(file))
            {
                dte.StatusBar.Text = $"Couldn't find file at '{file}'.";
                return;
            }

            dte.ItemOperations.OpenFile(file);
        }
    }
}
