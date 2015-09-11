## Clear Component Cache

A Visual Studio extension that clears the MEF (Managed Extensibility Framework) cache.

[![Build status](https://ci.appveyor.com/api/projects/status/96le2gaaxp6u82wh?svg=true)](https://ci.appveyor.com/project/madskristensen/clearcomponentcache)

Download the extension at the
[VS Gallery](https://visualstudiogallery.msdn.microsoft.com/3b329021-cd7a-4a01-86fc-714c2d05bb6c)
or get the
[nightly build](http://vsixgallery.com/extension/f5028141-9dd0-4ac4-ae6d-0481ae9a940d/)

### What does it do?

This extension clears the MEF cache on disk and lets Visual Studio rebuild it. The
reason for clearing the MEF cache is that it can become corrupted. This ususally
happens when installing or updating extensions or other Visual Studio components.

The Visual Studio component cache is located at
`%localappdata%\Microsoft\VisualStudio\14.0\ComponentModelCache`. This extension
makes it easy to delete that folder so you don't have to remember the location of
the cache directory.

### Using the extension

In Visual Studio's top menu under Tools, a new command is now visible:

![Menu Button](art/menu-button.png)

Clicking the **Clear MEF Component Cache** button will prompt you to confirm
and then restart Visual Studio.

![Popup dialog](art/prompt.png)

Restarting Visual Studio will automatically trigger a reconstruction of the
MEF cache. This is a safe operation that doesn't case any unwanted side effects.
