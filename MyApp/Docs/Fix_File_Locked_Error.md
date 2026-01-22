# Fix "File is Locked" Build Error

## ?? Error Message
```
Could not copy "apphost.exe" to "MyApp.exe". 
The file is locked by: "MyApp (2688)"
```

## ? Quick Solutions

### **Solution 1: Stop in Visual Studio (Fastest)**
1. Press **`Shift + F5`** (Stop Debugging)
2. Or click the **?? Stop** button in toolbar
3. Press **`F5`** to run again

### **Solution 2: Use Included Scripts**

**Option A - PowerShell Script:**
1. Right-click `MyApp\Stop-MyApp.ps1`
2. Select **"Run with PowerShell"**
3. Build again

**Option B - Batch Script:**
1. Double-click `MyApp\Stop-MyApp.bat`
2. Build again

**Option C - From Visual Studio Terminal:**
```powershell
cd MyApp
.\Stop-MyApp.ps1
```

### **Solution 3: Task Manager**
1. Press **`Ctrl + Shift + Esc`**
2. Find **MyApp.exe** (PID 2688)
3. Right-click ? **End Task**
4. Build again

### **Solution 4: Command Line**
```cmd
taskkill /F /IM MyApp.exe
```

---

## ?? Prevent Future Occurrences

### **Option 1: Add Pre-Build Event in Visual Studio**
1. Right-click **MyApp project** ? **Properties**
2. Go to **Build Events** ? **Pre-build event**
3. Add this command:
```cmd
taskkill /F /IM MyApp.exe /FI "STATUS eq RUNNING" 2>nul || exit /b 0
```
4. Click **Save**

### **Option 2: Edit .csproj File Manually**
1. Right-click **MyApp.csproj** ? **Edit Project File**
2. Add this before the closing `</Project>` tag:

```xml
<!-- Auto-kill running process before build -->
<Target Name="KillRunningProcess" BeforeTargets="PreBuildEvent">
  <Exec Command="taskkill /F /IM MyApp.exe /FI &quot;STATUS eq RUNNING&quot; 2&gt;nul || exit /b 0" 
        IgnoreExitCode="true" 
        ContinueOnError="true" />
</Target>
```

3. Save and reload project

### **Option 3: Enable Hot Reload (Recommended)**
1. Go to **Tools** ? **Options**
2. Navigate to **Debugging** ? **Hot Reload**
3. Enable:
   - ? **"Enable Hot Reload when debugging"**
   - ? **"Apply Hot Reload changes on file save"**
4. Click **OK**

Now you can make code changes without restarting the app!

---

## ?? Best Practice Workflow

### **Development Workflow:**
1. **First Run:** Press **`F5`** to start debugging
2. **Make Changes:** Edit your code
3. **Hot Reload:** Changes apply automatically (no restart needed)
4. **If Hot Reload fails:**
   - Press **`Shift + F5`** to stop
   - Press **`F5`** to restart
5. **If still locked:** Run `Stop-MyApp.bat` or `Stop-MyApp.ps1`

### **Quick Keyboard Shortcuts:**
| Action | Shortcut |
|--------|----------|
| Start Debugging | `F5` |
| Stop Debugging | `Shift + F5` |
| Restart | `Ctrl + Shift + F5` |
| Build | `Ctrl + Shift + B` |

---

## ?? Common Causes

1. **Previous debug session didn't close properly**
2. **Exception occurred and app didn't terminate**
3. **Background service still running** (like `StatisticsCacheService`)
4. **Multiple Visual Studio instances** running the same project
5. **Antivirus** holding file handles

---

## ?? Verify Process is Stopped

**PowerShell:**
```powershell
Get-Process -Name "MyApp" -ErrorAction SilentlyContinue
```
If nothing shows, it's stopped ?

**Command Prompt:**
```cmd
tasklist | findstr MyApp.exe
```
If nothing shows, it's stopped ?

---

## ?? Pro Tips

1. **Always use `Shift + F5`** before rebuilding
2. **Enable Hot Reload** to avoid restarts
3. **Don't close console window** where the app is running (close from VS instead)
4. **Use the included scripts** for quick cleanup
5. **Check Task Manager** if issues persist

---

## ?? For Background Services

If you have background services like `StatisticsCacheService`, they might keep the process alive. To ensure clean shutdown:

**Update `Program.cs`:**
```csharp
var app = builder.Build();

// ... middleware configuration ...

// Graceful shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application is stopping...");
});

app.Run();
```

This ensures all services stop cleanly when you press `Shift + F5`.

---

## ?? Still Having Issues?

1. **Restart Visual Studio**
2. **Restart your computer** (releases all file handles)
3. **Check for zombie processes:**
   ```powershell
   Get-Process | Where-Object {$_.ProcessName -like "*MyApp*"}
   ```
4. **Disable antivirus temporarily** (during development)
5. **Run Visual Studio as Administrator**

---

## ? Summary

**Immediate Fix:**
```cmd
taskkill /F /IM MyApp.exe
```

**Long-term Solution:**
1. Add pre-build event to `.csproj`
2. Enable Hot Reload in Visual Studio
3. Always use `Shift + F5` to stop before rebuilding

**Quick Scripts:**
- `Stop-MyApp.ps1` (PowerShell)
- `Stop-MyApp.bat` (Batch)

Both scripts are now in your `MyApp` folder! ??
