using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio;
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
            var shell = await ServiceProvider.GetServiceAsync(typeof(SVsShell)) as IVsShell;
            var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;
            string folder = GetFolderPath(shell);
            OpenErrorFile(shell, dte);
        }

        private static void OpenErrorFile(IVsShell shell, DTE dte)
        {
            string folder = GetFolderPath(shell);
            if(folder == null)
            {
                return;
            }

            var file = Path.Combine(folder, "Microsoft.VisualStudio.Default.err");
            if (!File.Exists(file))
            {
                return;
            }

            dte.ItemOperations.OpenFile(file);
        }

        private static string GetFolderPath(IVsShell shell)
        {
            object root;

            // Gets the version number with the /rootsuffix. Example: "14.0Exp"
            if (shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out root) == VSConstants.S_OK)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string version = Path.GetFileName(root.ToString());

                return Path.Combine(appData, "Microsoft\\VisualStudio", version, "ComponentModelCache");
            }

            return null;
        }
    }
}
