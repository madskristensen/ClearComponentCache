namespace ClearComponentCache
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidClearCachePackageString = "22470c17-41ed-45b7-ad6f-59c94e7b62a7";
        public const string guidClearCachePackageCmdSetString = "a0c73f0b-ee00-4b48-be1d-99b6deb8652d";
        public static Guid guidClearCachePackage = new Guid(guidClearCachePackageString);
        public static Guid guidClearCachePackageCmdSet = new Guid(guidClearCachePackageCmdSetString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int ClearCacheId = 0x0064;
    }
}
