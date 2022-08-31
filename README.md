<p align="center">
  <img src="https://www.rdr2mods.com/uploads/monthly_2022_08/Banner.png.1a0072b27e2627bd7ce22b3c134678ae.png">
</p>

# Getting Started
- Download the setup file and run it. 
- You can now access the app from Start Menu.
- Specify the folder where your local photos are stored (depends on the version: Steam, Launcher, ..)
- Click "Convert" and wait for a few seconds.

# Known Build Issues 
<p>
  <img width=500 height=210 src="https://user-images.githubusercontent.com/13592821/174165081-9c62d188-ecb6-4200-abd8-419afbaf32c2.png#gh-dark-mode-only">
</p>

This is a WPF .NET 6.0 application with a modern user interface library (WPFUI). There is a known issue where the project fails to build due to duplicate reference issue (https://github.com/dotnet/core/blob/main/release-notes/6.0/known-issues.md#issues-building-wpf-application-with-windows-desktop-607-and-608).

This problem has been permantely patched in the project files. However, you might experience invalid InteliSense issues. **This does not affect the build at all.**

