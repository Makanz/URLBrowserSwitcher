# Browser Switcher

A simple app to automatically switch browsers for specific URLs based on a list of domains.

## Features
- Define a list of domains to open in specific browsers.
- Automatically redirects URLs to the browser of your choice.
- Simple configuration using a `.reg` file.

## Getting Started

### 1. Edit appsettings.json

```
{
  "defaultBrowser": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
  "alternativeBrowser": "C:\\Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe",
  "urls": [
    "https://www.youtube.com",
    "https://github.com"
  ]
}
```

### 2. Create a `.reg` File
Create a `.reg` file with the content below. Be sure to edit the paths to match the installation of the program.

```reg
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\URLBrowserSwitcher]
@="URL Browser Switcher"

[HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\URLBrowserSwitcher\Capabilities]
"ApplicationDescription"="Custom URL Handler"
"ApplicationName"="URL Browser Switcher"

[HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\URLBrowserSwitcher\Capabilities\Startmenu]
"StartMenuInternet"="URLBrowserSwitcher.exe"

[HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\URLBrowserSwitcher\Capabilities\URLAssociations]
"http"="URLBrowserSwitcherURL"
"https"="URLBrowserSwitcherURL"

[HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications]
"URL Browser Switcher"="Software\\Clients\\StartMenuInternet\\URLBrowserSwitcher\\Capabilities"

[HKEY_CLASSES_ROOT\URLBrowserSwitcherURL]
@="URL Browser Switcher URL Handler"
"URL Protocol"=""

[HKEY_CLASSES_ROOT\URLBrowserSwitcherURL\shell\open\command]
@="\"C:\\Program Files\\URLBrowserSwitcher\\URLBrowserSwitcher.exe\" \"%1\""

```

### 3. Set this program as default browser in windows

