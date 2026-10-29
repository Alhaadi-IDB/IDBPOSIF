# IDBPOSInterface

IDBPOSInterface is a Windows VB.NET application that receives POS print requests and sends PDF receipts and order chits to a configured printer. It runs a local HTTP server and can optionally receive requests through a WebSocket connection.

## Requirements

- Windows with .NET Framework 4.8
- Visual Studio Community with **.NET desktop development** and the **.NET Framework 4.8 targeting pack** to build the project
- A printer configured in Windows
- Ghostscript when using the default Ghostscript print engine

## Build

1. Open `IDBPOSIf.vbproj` in Visual Studio Community.
2. Select the **Release** configuration and build the project.
3. Find the executable at `bin/Release/IDBPOSInterface.exe`.

Keep the executable, its `.exe.config` file, and required DLLs together when copying the build output to another Windows PC.

## Configure and run

Place a `config.ini` file beside `IDBPOSInterface.exe`. The main settings are:

| Setting | Purpose |
| --- | --- |
| `ServerUrl` | Server used to download receipt and order chit PDFs. |
| `IPAddress` and `Port` | Address and port for the local print-request server. |
| `cashierPrinter` | Windows printer name used for cashier receipts and the sample print. |
| `GhostscriptPath` | Path to the Ghostscript executable used for PDF printing. |
| `UseWebsocket` and `WebsocketAddress` | Optional WebSocket print-server connection. |

Example configuration:

```ini
[Interface Server]
ServerUrl=https://your-pos-server.example/pos/
GhostscriptPath=C:\path\to\gswin32.exe
cashierPrinter=Your Windows printer name
printerModel=Your Windows printer name
ServeAsPrintServer=False
IPAddress=127.0.0.1
Port=80

[PrintServer]
UseWebsocket=False
WebsocketAddress=wss://your-pos-server.example/ws/print
```

Update the example values for the target PC, then run `IDBPOSInterface.exe`. The application starts minimized in the Windows notification area. Click its icon to open the interface window.

## Print results and logs

The interface returns a result or error message to the requester after a print attempt. A successful result means the print command completed or was accepted by the print system; it does not confirm that paper physically came out of the printer.

Daily logs are written beside the executable in `log/log_YYYYMMDD.txt`. Check these logs when a request, PDF download, or print job fails.
