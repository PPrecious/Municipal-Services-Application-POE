# Municipal-Services-Application-POE

The Municipal Services Application is a Windows Forms desktop application developed in C#. It provides three main features:

1. **Report Issues** – Allows users to submit municipal service-related issues.
2. **Local Events and Announcements** – Allows users to view and interact with local events and announcements.
3. **Service Request Status** – Allows users to view service requests, search using a request ID, and monitor request progress.

---

## 2. System Requirements

Before compiling and running the application, ensure that the following software is available:

- Microsoft Windows operating system.
- Microsoft Visual Studio with Windows Forms development support.
- .NET Framework Developer Pack matching the target framework configured in the project.

---

## 3. Project Setup

Follow these steps to open the application in Visual Studio.

1. Locate the Municipal Services Application project folder.
2. Open Microsoft Visual Studio.
3. Select **Open a Project or Solution**.
4. Navigate to the project folder.
5. Select the solution file (`.sln`), if available.
6. Allow Visual Studio to load the project and its dependencies.

### 3.1 Check the project files

Ensure that the project contains the required application files and folders.

- `Program.cs`
- `MainMenuForm.cs`
- `ReportIssueForm.cs`
- `EventsForm.cs`
- `ServiceRequestStatusForm.cs`
- `ServiceRequest.cs`
- `ServiceRequestStructures.cs`
- `ServiceRequestRepository.cs`

Organised into folders such as:

- `Forms`
- `Models`
- `Data`

### 3.2 Check the startup form

Open `Program.cs` and ensure that the application starts by displaying `MainMenuForm`.

```csharp
using System;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.MainMenuForm());
        }
    }
}
```
---

## 4. Compiling the Application

Once the project has been opened and checked, follow these steps to compile it.

1. Open the project in Visual Studio.
2. Select **Build** from the top menu.
3. Select **Clean Solution** to remove previous build output.
4. Select **Build** again.
5. Click **Build Solution**.
6. Wait for Visual Studio to finish compiling.

### 4.1 Check the build results

Open the **Error List** window in Visual Studio.

- Check that there are no build errors.
- Review any warnings displayed.
- Ensure that all required forms and classes are recognised by the project.

If compilation is successful, Visual Studio will display a build succeeded message.

If errors occur, check:

- Missing source files.
- Incorrect namespaces.
- Missing project references.
- Incorrect class names.
- Missing Windows Forms references.

---

## 5. Running the Application

1. Locate the Visual Studio toolbar.
2. Select **Start** or press `F5` to run the application with debugging.
3. Wait for the application window to open.

The Municipal Services main menu should appear.

---

## 6. Using the Application

### 6.1 Report Issues

1. From the main menu, select **Report Issues**.
2. Wait for the Report Issues form to open.
3. Enter the required information about municipal issue.
4. Complete relevant fields.
5. Submit the issue.
6. Follow any confirmation message displayed.
7. Close the form or return to the main menu.

### 6.2 Local Events and Announcements

1. Select **Local Events and Announcements**.
2. Wait for events form to open.
3. Browse available local events and announcements.
4. Use available search or filtering functionality to find relevant information.
5. Select relevant event or announcement to view available details.
6. Close the form when finished.

### 6.3 Service Request Status

1. Select **Service Request Status**.
2. Wait for Service Request Status form to open.
3. View available service request records.
4. Locate request using its unique request ID.
5. Review request information.
6. Use the available status-management functionality to update a request where permitted.
7. Review the displayed request information after making update.
8. Close the form when finished.

---

## 7. Closing the Application

To close the application:
1. Click the window's close button (`X`).

---

## 8. Troubleshooting

### Application does not compile

- Check the Error List in Visual Studio.
- Ensure all required source files are included.
- Verify class names and namespaces.
- Check the configured .NET Framework version.
- Rebuild the solution after correcting errors.

### Main menu does not open

- Check that `Program.cs` starts `MainMenuForm`.
- Ensure the main menu class is included in the project.
- Check for errors in the startup configuration.

### A feature button does not open its form

- Verify the button's Click event is connected.
- Check the corresponding form class exists.
- Ensure form is included in the project.
- Verify the form's namespace is correct.

### Service Request Status does not display records

- Check the repository is initialised.
- Verify the sample service requests are loaded.
- Check the Service Request Status form retrieves records from the correct repository.

---

## Conclusion

The Municipal Services Application can be opened, compiled, and executed using Microsoft Visual Studio with the appropriate .NET Framework development tools.

Following the instructions in this README will help users set up the project, compile the source code, launch the application, and navigate its available functionality.
