using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace ClearComponentCache
{
    public static class ComponentModelExtensions
    {
        public static string GetDefaultErrorFile(this IVsComponentModelHost componentModelHost)
        {
            var folder = GetFolderPath(componentModelHost);
            return Path.Combine(folder, "Microsoft.VisualStudio.Default.err");
        }

        public static string GetFolderPath(this IVsComponentModelHost componentModelHost)
        {
            string folderPath;
            componentModelHost.GetCatalogCacheFolder(out folderPath);
            return folderPath;
        }
    }
}
