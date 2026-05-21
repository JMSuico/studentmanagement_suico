# System Documentation: Student Management System

## Phase 1: High-Level Architecture & Project Structure

### Overview
The Student Management System is a modern, responsive web application designed to manage student enrollments, academic progress, grading, and attendance. It serves multiple user roles including Administrators, Teachers (Instructors), and Students. 

### Core Architecture
The system is built on **.NET 9 Blazor Web App** using the **Interactive Server Render Mode**. This architecture provides a rich, Single Page Application (SPA) feel while running the application logic entirely on the server. The UI updates are communicated to the client browser via a persistent SignalR (WebSocket) connection.

**Key Technologies:**
- **Framework**: .NET 9 ASP.NET Core Blazor Web App (Server-side rendering and interactive server components).
- **UI Library**: MudBlazor (Material Design component library for Blazor).
- **ORM / Data Access**: Entity Framework Core 9 (EF Core) using Pomelo EntityFrameworkCore MySql provider.
- **Database**: MySQL Server 8.0+.
- **Authentication**: Cookie-based authentication integrated with Blazor's `AuthenticationStateProvider`.

### Project Structure
The project is encapsulated within the `StudentManagement` namespace and directory. Below is the explanation of the structural flow:

#### 1. Root Level configuration
- `Program.cs`: The entry point of the application. It configures the Dependency Injection (DI) container, sets up EF Core Context (`AppDbContext`), initializes authentication (Cookie), Data Protection keys, and maps endpoints.
- `StudentManagement.csproj`: Project configuration file containing NuGet package references and target frameworks.
- `appsettings.json`: Application configuration holding the `DefaultConnection` string for the MySQL database.

#### 2. Features Directory (`/Features`)
This directory acts as the core backend of the application, isolating business logic and data access from the UI components. It is divided into:
- **/Data**: Contains `AppDbContext` (EF Core database context), `Models` (Entities representing DB tables), and `Enums` (Enumerations for strong typing like `UserRole`, `AttendanceStatus`).
- **/Repositories**: Implements the Repository Pattern. 
  - `/Interfaces`: Contracts for data access.
  - `/Implementations`: Actual classes directly querying EF Core.
- **/Services**: Implements the Business Logic Layer.
  - `/Interfaces`: Contracts for services.
  - `/Implementations`: Classes that consume Repositories, apply business rules, and return data to the UI.
- **/Helpers**: Contains utility classes (`AttendanceHelper`, `GradeHelper`, etc.) that abstract away complex mathematical formulas or logic from the services.

#### 3. Components Directory (`/Components`)
Contains the Blazor UI code.
- `App.razor`: The root HTML template that loads CSS/JS assets, `HeadOutlet`, and `Routes`. It utilizes `prerender: true` to support persistent authentication state on refresh.
- `Routes.razor`: Sets up the router and `CascadingAuthenticationState`.
- **/Layout**: Contains `MainLayout.razor` (the primary application shell with the AppBar and Drawer) and `EmptyLayout.razor` (used for login/register pages).
- **/Pages**: Contains all the routable application views (e.g., `Dashboard`, `StudentsPage`, `GradeManagement`).
- **/Shared**: Contains reusable UI components and Dialogs (Modals) used across multiple pages (e.g., `StudentFormDialog`, `ConfirmDialog`).

### Architectural Flow Chain
1. **Client Browser** connects to the server.
2. **Program.cs** bootstraps the application and middleware.
3. The **Blazor Router** matches the URL to a Component in `/Components/Pages`.
4. The **Page Component** injects the required **Service** (from `/Features/Services`).
5. The **Service** applies business logic and calls the **Repository** (from `/Features/Repositories`).
6. The **Repository** queries or commands the **AppDbContext** (from `/Features/Data`).
7. **EF Core** translates the LINQ into SQL and hits the **MySQL Database**.
8. The result flows back up the chain to the UI, which updates the DOM over SignalR.
## Phase 2: Data Layer (Models & Database)

### AppDbContext
The `AppDbContext` (`/Features/Data/AppDbContext.cs`) is the Entity Framework Core context class. It bridges the C# domain models to the underlying MySQL database.
- **Connection**: It connects using the `Pomelo.EntityFrameworkCore.MySql` provider.
- **DbSets**: It exposes `DbSet<T>` for all entities (`Users`, `Students`, `Teachers`, `Subjects`, `ClassSections`, `Enrollments`, `Attendances`, `Grades`, `Tasks`, `AttendanceSessions`).
- **OnModelCreating**: The `OnModelCreating` method is heavily utilized to configure table relationships using the Fluent API. This ensures strict cascading deletion rules and maps foreign keys properly without relying solely on data annotations. For example, it defines that when a `ClassSection` is deleted, its related `Enrollments` and `AttendanceSessions` are properly cascaded or restricted based on business rules.

### Core Data Models (Entities)
The models located in `/Features/Data/Models` represent the database tables:

1. **User**: The base authentication entity. It stores `Username`, `Email`, `PasswordHash`, and `UserRole`. All other specific roles (Student, Teacher) link back to this via email or logic.
2. **Student**: Represents a student's profile containing `StudentNumber`, `FirstName`, `LastName`, `Course`, `YearLevel`, and `Section`. Holds a collection of `Enrollments`.
3. **Teacher**: Represents an instructor containing `EmployeeNumber`, `Specialization`, and `Department`.
4. **Subject**: The core curriculum definition. It holds `SubjectCode`, `SubjectName`, `Units`, `YearLevel`, and `Semester`.
5. **ClassSection**: Connects a `Subject` to a `Teacher` for a specific `SchoolYear` and `Semester`. It also defines the physical or virtual `Room` and `Schedule`.
6. **Enrollment**: The junction table (Many-to-Many resolution) between `Student` and `ClassSection`. It tracks the `EnrollmentDate` and `Status` (Active, Dropped, Completed).
7. **Task**: Represents an assignment, exam, or quiz created by a Teacher for a specific `ClassSection`. It has a `MaxScore` and `DueDate`.
8. **Grade**: Connects a `Student` to a `Subject` representing the final computed grade for a subject. It tracks the `OverallGrade` (numeric) and textual `Status` (Passed, Failed, Incomplete).
9. **AttendanceSession**: Defines a specific day/time a class took place.
10. **Attendance**: Tracks a specific `Student`'s presence during an `AttendanceSession` with statuses like Present, Absent, Late.

### Enums
Enums are used to enforce strongly-typed constraints on states within the system. Located in `/Features/Data/Enums`:
- `UserRole`: Admin, Teacher, Student, Staff.
- `EnrollmentStatus`: Pending, Active, Dropped, Completed.
- `AttendanceStatus`: Present, Absent, Late, Excused.
- `GradeStatus`: Passed, Failed, Incomplete, Dropped.
- `ClassStatus`: Active, Inactive, Completed.
- `TaskType`: Assignment, Quiz, Exam, Project, Activity.
## Phase 3: Repository & Service Layers (Business Logic)

The application employs a standard **N-Tier architectural pattern** within the Features folder, separating Data Access from Business Logic.

### Repository Layer
Located in `/Features/Repositories`, the repository pattern is used to decouple the business logic from EF Core entirely. 
- **Interfaces**: Contracts like `IStudentRepository` dictate what data operations are possible without exposing EF Core's `DbContext`.
- **Implementations**: Classes like `StudentRepository` implement these interfaces. They perform standard CRUD (Create, Read, Update, Delete) operations using LINQ. 
- **Eager Loading**: The repositories are responsible for `.Include()` and `.ThenInclude()` calls. For instance, when fetching an `Enrollment`, the repository ensures the related `Student` and `ClassSection` objects are attached so the UI does not face null references.
- **Null Safety Fix**: Eager loading was enhanced with null-forgiving operators (e.g., `.ThenInclude(c => c!.Subject)`) to prevent compiler warnings and guarantee runtime safety when traversing nullable relationships.

### Service Layer (Business Logic)
Located in `/Features/Services`, this layer holds the core business algorithms and data orchestration logic. Services act as the middleman between the Blazor UI components and the Repositories.
- **Interfaces**: Contracts like `IGradeService` provide abstractions for the UI components to inject via Dependency Injection.
- **Implementations**: Classes like `GradeService` execute the business rules. 
  - *Example Flow*: If the UI requests to drop a student, the UI calls `EnrollmentService.UpdateEnrollmentStatusAsync(id, Dropped)`. The service fetches the enrollment via the repository, applies any business logic (like automatically setting grades to Dropped or recalculating averages), and then saves the changes via the repository.
- **Dependency Injection**: All repositories and services are registered as `AddScoped` in `Program.cs`. This means they live for the duration of the user's SignalR connection (in Blazor Server), allowing them to maintain state securely per user session.

### Cross-Cutting Concerns
- **Error Handling**: Services catch low-level data exceptions and wrap them or return standard objects to the UI.
- **Asynchronous Flow**: Every call from UI to Service to Repository to Database is `async/await`. This prevents blocking the main thread, keeping the Blazor Server WebSocket highly responsive to thousands of concurrent users.
## Phase 4: Core Algorithms, Formulas & Helpers

The application abstracts complex calculations and repetitive logic into Services and Helpers. Below are the primary algorithms used in the system, complete with mathematical formulas.

### 1. Grading Algorithm
The system supports both simple legacy grading and advanced category-weighted grading based on class tasks.

#### Legacy Weighting (30/30/40)
When grades are submitted directly via `GradeService.CalculateOverallGrade`, a fixed 30/30/40 ratio is applied to Prelim, Midterm, and Final grades.
- **Formula**: `Overall Grade = (Prelim * 0.30) + (Midterm * 0.30) + (Final * 0.40)`

#### Dynamic Category-Weighted Algorithm
A more advanced system allows instructors to define weights per subject/semester via `SubjectGradeSetup`. Tasks (Assignments, Quizzes, Exams) fall under different categories.

**Algorithm Steps (`GradeService.RecalculateStudentGradeAsync`):**
1. Fetch the `SubjectGradeSetup` defining the percentage weights (e.g., Quizzes: 20%, Long Exams: 30%, Performance: 30%, Semester Exam: 20%).
2. Retrieve all `TaskResults` for the specific student and subject.
3. For each category $C$ with weight $W_C$:
   - Calculate total points earned in $C$: $E_C = \sum \text{PointsEarned}$
   - Calculate total max possible points in $C$: $M_C = \sum \text{MaxPoints}$
   - Calculate category percentage: $P_C = \left( \frac{E_C}{M_C} \right) \times 100$
   - Apply weight: $Score_C = P_C \times \left( \frac{W_C}{100} \right)$
4. **Final Grade Formula**: $Overall = \sum (Score_C)$

#### Grade Helpers (`GradeHelper.cs`)
- **GetGradeRemarks**: Determines standard remarks. If $\ge 75$, returns "Passed", else "Failed".
- **GetGradeEquivalent**: Converts the 100-point scale into a standard collegiate scale (e.g., $98\text{--}100 \rightarrow 1.00$, $95\text{--}97 \rightarrow 1.25$, down to $<75 \rightarrow 5.00$).

### 2. Academic Progress Algorithm
The `AcademicProgressService` generates a comprehensive overview of a student's standing.

**Steps:**
1. **Attendance Rate**: Calculates the percentage of sessions the student was Present, Late, or Excused vs Total Sessions.
   - **Formula**: $Rate = \left( \frac{Present + Late + Excused}{Total Sessions} \right) \times 100$
2. **Grade Average**: Averages the `OverallGrade` of all graded subjects.
3. **Academic Standing Logic**:
   - `Excellent`: Average $\ge 85$ AND Failed Subjects $= 0$
   - `Good`: Average $\ge 75$ AND Failed Subjects $= 0$
   - `Needs Improvement`: Failed Subjects $> 0$
4. **Remarks Generation**: Combines the standing with an attendance warning if the attendance rate drops below 80%.

### 3. Attendance Algorithms
The `AttendanceHelper.cs` aggregates raw attendance lists into meaningful metrics.
- **CalculateAttendancePercentage**: Only `Present` and `Late` are considered "in attendance". 
  - **Formula**: $Percentage = \left( \frac{Present + Late}{Total Records} \right) \times 100$
- **GetAttendanceSummary**: Generates a quick string representation, e.g., `P:10 L:2 A:1 E:0`.

### 4. Code / QR Generation Flow
The `QRHelper.cs` utilizes the ZXing.Net library to generate base64-encoded QR codes.
- **Purpose**: Can be used to encode student profiles or attendance session tokens.
- **Logic**: It creates a `BarcodeWriter`, sets format to `QR_CODE`, encodes a string value to a `Bitmap`, and converts the raw image bytes into a base64 string formatted as `data:image/png;base64,...` so it can be directly embedded in an `<img>` tag in Blazor.
## Phase 5: Presentation Layer & UI Components

The frontend of the system is built entirely using Blazor Web App (Interactive Server Mode) and styled using the **MudBlazor** Material Design component library. It sits inside the `/Components` folder.

### Core Layouts (`/Components/Layout`)
1. **MainLayout.razor**: The standard shell for authenticated users. It contains the `MudAppBar` (top navigation), `MudDrawer` (sidebar navigation menu), and the `MudMainContent` where pages are rendered. The layout dynamically pulls the user's name from the `AuthenticationStateProvider`.
2. **EmptyLayout.razor**: Used for pages that shouldn't show the navigation bar or drawer, such as the Login and Registration screens.

### Authentication Flow (Login & Register)
- **Login.razor**: Users enter their username and password. The component directly invokes `IUserService.AuthenticateAsync`. If valid, it triggers a hidden HTML form submission to `/api/auth/login`. This POST request is necessary because Blazor Server (over WebSockets) cannot easily issue an HTTP cookie. The API endpoint validates the user again, issues a persistent ASP.NET Core Authentication Cookie (`SignInAsync`), and redirects to `/dashboard`.
- **Register.razor**: Implements a highly interactive, 3-phase horizontal sliding form.
  1. *Role Selection*: Choose Student, Admin, or Instructor.
  2. *Profile*: Collect specific details dynamically (e.g., Year Level for Students, Specialization for Instructors).
  3. *Credentials*: Collect Username and Password.
  Once complete, it uses `IUserService` to hash the password and persist the user, automatically creating the linked `Student` or `Teacher` profile depending on the role.

### Dashboard & Role-Based Views
- **Dashboard.razor**: The homepage of the app post-login. It uses the `<AuthorizeView Roles="...">` component to conditionally render content.
  - *Students* see a limited view containing enrollment counts, recent grades, and a call-to-action for academic progress.
  - *Admins/Instructors* see an extensive statistical overview (Total Students, Total Classes, Global Attendance Statuses, Recent Enrollments).

### Data Management Pages (`/Components/Pages`)
Most pages follow a standard pattern: A data table (`MudDataGrid` or `MudTable`) displaying records, and Action buttons (Add, Edit, View, Delete) that trigger pop-up Modals (Dialogs).
- **StudentsPage.razor / TeacherManagement.razor**: Standard CRUD pages for user management.
- **ClassManagement.razor**: Manages `ClassSection` creation, linking Subjects to Instructors for specific schedules.
- **AttendancePage.razor**: Allows tracking attendance. It lets instructors select a class, create a session for the day, and mark the status of enrolled students.
- **GradeManagement.razor / TaskScoringPage.razor**: Enables grading. It supports defining tasks and scoring students, which ties back into the Dynamic Grading Algorithm.

### Shared Dialogs (`/Components/Shared`)
The application relies heavily on the `IDialogService` from MudBlazor to prevent disruptive page navigations. Forms are presented as modals.
- **ConfirmDialog.razor**: A generic Yes/No confirmation box for destructive actions (like Deleting a record).
- **StudentFormDialog.razor**: The modal used to create or edit a student record.
- **StudentEnrollmentDialog.razor**: Allows selecting available class sections based on the student's year level, substituting standard radio buttons with interactive `MudIcon` elements to provide a custom UI.
## Phase 6: End-to-End System Flow Example

To fully understand the architecture and logic flow, here is a step-by-step trace of a common scenario: **An Instructor scoring a student's Quiz.**

### Flow Chain
1. **User Interaction**: The Instructor navigates to `TaskScoringPage.razor`. The Blazor Router matches the URL.
2. **Component Initialization (`OnInitializedAsync`)**:
   - The UI injects `ITaskService` and `IClassSectionService`.
   - The UI fetches the available tasks by calling `_taskService.GetTasksBySubjectAsync(subjectId)`.
3. **Data Retrieval (Service -> Repo -> DB)**:
   - `TaskService` calls `_taskRepository.GetTasksBySubjectAsync`.
   - `TaskRepository` uses EF Core: `_context.Tasks.Where(t => t.SubjectId == subjectId).ToListAsync()`.
   - EF Core translates this to `SELECT * FROM Tasks WHERE SubjectId = @subjectId` against MySQL.
   - The returned entities travel back up to the UI.
4. **Scoring Input**: The instructor enters "18" out of 20 for a student's quiz. They click "Save".
5. **Data Submission**:
   - The UI calls `_taskService.SaveStudentResultAsync(result)`.
   - The Service validates the result and checks if it exists. It updates or inserts the `StudentTaskResult`.
   - The Service recalculates the overall grade by invoking `_gradeService.RecalculateStudentGradeAsync(studentId, subjectId, semester)`.
6. **Algorithmic Recalculation**:
   - The Grade Service fetches the `SubjectGradeSetup` (weights).
   - It fetches all tasks for the student in that subject.
   - It applies the dynamic weighted algorithm formula $Overall = \sum \left( \left( \frac{\sum Points}{\sum Max} \times 100 \right) \times \frac{Weight}{100} \right)$.
   - It updates the single row in the `Grades` table representing the student's final standing.
7. **UI Update**: The Service returns success. Blazor pushes the updated state to the client browser instantly via SignalR. The instructor sees the newly calculated grade on screen.

### Summary
The Student Management System represents a robust, highly structured application leveraging modern .NET capabilities. Its strict separation of concerns (UI -> Services -> Repositories -> EF Core Context) ensures testability, scalability, and maintainability. Its use of algorithms for grading and attendance ensures automated, mathematically sound evaluation of student progress in real-time.
---

# Deep Dive: Components Directory (`/Components`)

This section provides a deep, comprehensive read and analysis of the entire `Components` directory. It maps out exactly how the Blazor Interactive Server UI interacts with the underlying N-Tier architecture, exploring the code, connections, logic, algorithms, and point-to-point flow of the frontend application.

## Part 1: Root Architecture, Routing, and Layouts

### 1. Root Components
The starting point of the UI layer sits at the root of the `/Components` folder.

#### `App.razor`
- **Purpose**: This is the root HTML template for the Blazor Web App. It contains the `<html>`, `<head>`, and `<body>` tags. 
- **Connections**: It loads the MudBlazor CSS and JS assets. It renders the `<Routes>` component.
- **Logic & Flow**: Critical to the application's authentication persistence is the configuration `prerender: true` on both the `<HeadOutlet>` and `<Routes>` inside the `InteractiveServerRenderMode`. 
  - *Why this approach?* In Blazor Server, the connection is maintained via WebSockets (SignalR). WebSockets do not automatically transmit HTTP Cookies after the initial handshake. By setting `prerender: true`, the initial page load acts as a standard HTTP request, allowing the ASP.NET Core `ServerAuthenticationStateProvider` to read the encrypted Auth Cookie, decrypt it, and construct the `ClaimsPrincipal` before the SignalR circuit even starts.

#### `Routes.razor`
- **Purpose**: Defines the routing mechanism and authorization boundaries.
- **Connections**: Wraps the entire application in a `<CascadingAuthenticationState>`.
- **Logic & Flow**: 
  - It uses `<Router>` to scan the assembly for `@page` directives.
  - Inside `Found`, it uses `<AuthorizeRouteView>`. This explicitly forces all routes to require authentication by default unless they have `[AllowAnonymous]`.
  - *Unauthorized Flow*: If a user tries to access a protected page without an auth cookie, it triggers the `<NotAuthorized>` block, which renders `<RedirectToLogin>`.

#### `_Imports.razor`
- **Purpose**: A global using directive file for the UI.
- **Logic**: Any `using` statement placed here (e.g., `using MudBlazor`, `using StudentManagement.Features.Data.Models`) is automatically available to every `.razor` file in the project, eliminating redundant using declarations.

### 2. Layouts (`/Components/Layout`)
Layouts define the visual shell that wraps around the routable pages.

#### `MainLayout.razor`
- **Purpose**: The primary layout for logged-in users.
- **Connections**: Injects `AuthenticationStateProvider` to read the current user's name and role.
- **Logic & Flow**:
  - Contains a `MudThemeProvider`, `MudDialogProvider`, and `MudSnackbarProvider` which are required to initialize MudBlazor's global services (Themes, Modals, and Toasts).
  - Implements a top `MudAppBar` (Header) and a side `MudDrawer` (Sidebar).
  - Uses an `<AuthorizeView>` to dynamically extract the `@context.User.Identity?.Name` to greet the user in the App Bar.
  - The `<article>` tag renders the `@Body` (the specific routed page).

#### `NavMenu.razor`
- **Purpose**: The sidebar navigation links.
- **Connections**: Uses `<AuthorizeView>` to conditionally render links based on the `Roles` claim.
- **Logic & Flow**:
  - *Admin/Instructor View*: Renders links to `Students`, `Teachers`, `Subjects`, `Classes`, `Enrollments`, `Task Scoring`, etc.
  - *Student View*: Renders restricted links like `My Dashboard`, `My Profile`, and `Request Enrollment`.
  - *Why this approach?* Conditional rendering at the layout level prevents users from even seeing options they aren't authorized to use, reinforcing security (though actual security is enforced at the Service/API level).

#### `EmptyLayout.razor`
- **Purpose**: A blank canvas layout used strictly for the `Login` and `Register` pages.
- **Logic**: It bypasses the AppBar and Drawer, providing a clean screen to center the authentication forms.
## Part 2: Authentication UI Flow

The Authentication flow manages how users enter the system and establish identity. It bypasses standard Blazor Server state in favor of secure HTTP Cookies to ensure persistence.

### `Pages/Login.razor`
- **Purpose**: The entry point for unauthenticated users.
- **Connections**: 
  - Layout: `EmptyLayout`
  - Injects: `IUserService`, `NavigationManager`
- **Point-to-Point Logic & Flow**:
  1. **User Input**: User enters Username and Password into the `MudForm`.
  2. **Initial Validation**: User clicks "Login". The `Submit` method calls `_userService.AuthenticateAsync(username, password)`.
  3. **Service Execution**: The Service hashes the entered password, queries the `UserRepository` to find a match.
  4. **The Blazor Constraint Bypass**: If valid, the component *does not* set the auth state internally. Instead, it renders a hidden standard HTML `<form>` targeting `/api/auth/login` and utilizes a tiny block of JavaScript (`document.getElementById('loginForm').submit();`) to POST the credentials.
  5. **Why this approach?**: Blazor Server components run over WebSockets. WebSockets cannot manipulate HTTP Cookies on the browser. By forcing a standard HTTP POST, the ASP.NET Core `/api/auth/login` endpoint (defined in `Program.cs`) intercepts the request, calls `HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ...)` with `IsPersistent = true`, and issues a secure, encrypted HTTP Cookie to the browser.
  6. **Redirection**: The API endpoint then issues an HTTP 302 Redirect to `/dashboard`. When the browser hits `/dashboard`, the cookie is attached, the `App.razor` reads it during Prerendering, and identity is established.

### `Pages/Register.razor`
- **Purpose**: Handles new user onboarding.
- **Connections**: Injects `IUserService`, `NavigationManager`, `ISnackbar`.
- **Flow**: Uses a 3-step `MudCarousel` to break down complex data entry.
  - *Phase 1*: Role Selection (Student, Admin, Instructor).
  - *Phase 2*: Profile details. The UI dynamically changes inputs based on Phase 1 (e.g., showing 'Course' for students, 'Specialization' for teachers).
  - *Phase 3*: Credentials (Username, Password, Confirm Password).
- **Execution Logic**: 
  - Calls `_userService.RegisterAsync(user)`.
  - The Service hashes the password securely using `SHA256` (via `PasswordHasher.HashPassword`).
  - Depending on the `UserRole`, the Service transactionally creates the base `User` record, and then *immediately* creates the linked `Student` or `Teacher` profile in the database.
  - Upon success, redirects the user to `/login`.

### `Shared/LogoutDialog.razor`
- **Purpose**: Provides a confirmation prompt before destroying the session.
- **Flow**: Triggers a standard `<form action="/api/auth/logout" method="post">`. Similar to the Login bypass, this POST hits `Program.cs`, which calls `HttpContext.SignOutAsync()`, destroying the cookie, and redirecting the user back to the public root.

### `Shared/RedirectToLogin.razor`
- **Purpose**: A silent utility component.
- **Flow**: Used exclusively in `Routes.razor` inside the `<NotAuthorized>` tag. As soon as it renders, its `OnInitialized` method executes `NavigationManager.NavigateTo("/login", true)`, forcing an unauthorized user to log in.
## Part 3: Core Entity Management (CRUD UIs)

The management pages handle standard Create, Read, Update, and Delete operations. They share a similar structural pattern utilizing `MudDataGrid` for tabular data and `IDialogService` for forms.

### Standard Flow for Management Pages
*(Applies to `StudentsPage.razor`, `TeacherManagement.razor`, `SubjectManagement.razor`)*

1. **Initialization (`OnInitializedAsync`)**:
   - The UI component injects the relevant service (e.g., `IStudentService`).
   - It calls `await _studentService.GetAllStudentsAsync()` to populate an `IEnumerable<T>` list.
   - The `MudDataGrid` binds to this list, instantly rendering rows.

2. **Creating/Editing Data (The Dialog Pattern)**:
   - When the user clicks "Add" or "Edit", the page invokes `IDialogService.ShowAsync<DialogComponent>()`.
   - **Data Passing**: If Editing, it passes the selected entity's ID or object via `DialogParameters` to the Dialog component (e.g., `StudentFormDialog`).
   - **Dialog Execution**: The Dialog component binds a `MudForm` to a local model. When the user clicks "Save", the Dialog calls the Service (`CreateAsync` or `UpdateAsync`) directly.
   - **Dialog Resolution**: The Dialog calls `MudDialog.Close(DialogResult.Ok(true))`.
   - **Page Refresh**: The parent page awaits the dialog result. If `result.Canceled` is false, it means data changed. The parent page then re-fetches the list `await LoadData()` and calls `StateHasChanged()`, updating the grid without a page refresh.

3. **Deleting Data**:
   - Clicking "Delete" opens `Shared/ConfirmDialog.razor`.
   - If confirmed, the parent page directly calls the Delete service method (e.g., `_studentService.DeleteStudentAsync(id)`) and refreshes the grid.

### Specific Component Breakdowns

#### `Pages/StudentsPage.razor` & Dialogs
- **Connections**: Injects `IStudentService`.
- **`StudentFormDialog.razor`**: A modal with a `MudForm` binding fields like `FirstName`, `LastName`, `Course`, `YearLevel`, and `Section`.
- **`StudentViewDialog.razor`**: A read-only modal displaying student details, utilizing `MudAvatar` to display their profile picture, and `QRHelper.GenerateQRCodeBase64` to instantly render a secure QR Code tied to their `StudentNumber`.
- **`ProfileEditDialog.razor`**: specifically used by a logged-in Student to modify their own profile picture or details, executing an update specifically tailored to their own record via `_userService.UpdateProfileImageAsync`.

#### `Pages/TeacherManagement.razor` & Dialogs
- **Connections**: Injects `ITeacherService`.
- **`TeacherFormDialog.razor`**: Collects `FirstName`, `LastName`, `EmployeeNumber`, `Specialization`, and `Department`.

#### `Pages/SubjectManagement.razor` & Dialogs
- **Connections**: Injects `ISubjectService`.
- **`SubjectFormDialog.razor`**: Manages the curriculum catalog. Collects `SubjectCode`, `SubjectName`, `Units`, `YearLevel`, and `Semester`. This sets up the foundational data needed before classes can be scheduled.
## Part 4: Academic Operations (Classes & Enrollments)

This tier of components links the foundational entities (Students, Subjects, Teachers) into actionable academic structures.

### `Pages/ClassManagement.razor`
- **Purpose**: Allows Admins to schedule a specific `Subject` under a `Teacher` for a given term.
- **Connections**: Injects `IClassSectionService`.
- **Flow**: 
  - Displays a grid of `ClassSections`.
  - When creating via `Shared/ClassSectionFormDialog.razor`, the dialog pulls in Lists of `Subjects` and `Teachers` via their respective services to populate dropdowns (`MudSelect`).
  - The admin inputs the `SchoolYear`, `Semester`, `Room`, and `Schedule`.
  - This creates the unique `ClassSectionId` which acts as the cornerstone for enrollments, tasks, and attendance.

### `Pages/EnrollmentsPage.razor`
- **Purpose**: The central hub for connecting a Student to a ClassSection.
- **Connections**: Injects `IEnrollmentService`.
- **Admin/Teacher Workflow (`EnrollmentFormDialog.razor`)**: 
  - The Admin selects a Student and a ClassSection from dropdowns to forcefully enroll them.
- **Student Workflow (`StudentEnrollmentDialog.razor`)**:
  - A logged-in Student clicks "Request Enrollment".
  - **Dynamic Filtering Algorithm**: The Dialog fetches *all* `ClassSections`, but immediately applies LINQ filters based on the `selectedYearLevel` (defaulted to the student's current year), `selectedSemester`, and `selectedSchoolYear`.
  - Only `ClassSections` that have an active status AND match the student's curriculum year level are displayed.
  - The student selects a class by clicking a row in a custom `MudSimpleTable`. The row uses `MudIcon` bound to `@onclick` to simulate a radio button selection visually.
  - Upon submission, it creates a `Pending` Enrollment record.
- **Detail View (`StudentEnrollmentDetailDialog.razor`)**:
  - Displays the full history of a specific enrollment, including the Date Enrolled and its Status changes. Admins can update the status (e.g., Active -> Dropped) from here, which triggers the backend service to finalize or drop associated records.
## Part 5: Evaluation, Progress, and Dashboard Metrics

This tier handles the dynamic, real-time calculations required for grading, attendance tracking, and statistical overviews.

### `Pages/TaskScoringPage.razor`
- **Purpose**: Allows an instructor to create class assignments and score enrolled students.
- **Connections**: Injects `ITaskService`, `IEnrollmentService`.
- **Flow & Logic**:
  1. Instructor selects a `ClassSection`. The page fetches all `ClassTask` records (e.g., "Midterm Exam").
  2. Clicking "Score" opens a scoring panel. The page fetches all `Students` currently `Active` in that `ClassSection` via `_enrollmentService`.
  3. The instructor enters raw scores into text fields.
  4. **Math Application**: Upon entering a score, the UI bounds it visually. When saving, `_taskService.SaveStudentResultAsync` calculates the internal task percentage.
  5. **Chained Algorithmic Trigger**: Saving a task result explicitly triggers `GradeService.RecalculateStudentGradeAsync()`. This means the moment a Quiz score is saved, the system re-runs the weighted average formula (e.g., extracting Quiz Weight % from `SubjectGradeSetup`) and instantly updates the `OverallGrade` in the `Grades` table.

### `Pages/GradeManagement.razor`
- **Purpose**: Displays the aggregated grades for classes.
- **Connections**: Injects `IGradeService`.
- **Flow**:
  - Displays the final `OverallGrade`.
  - Uses `GradeHelper.GetGradeRemarks` to determine if the grade is "Passed" or "Failed" visually, coloring `MudChip` components (Green for passed, Red for failed).
  - Uses `Shared/GradeSetupDialog.razor` to allow instructors to define the mathematical weights (e.g., Quizzes: 20%, Exam: 40%). The sum must equal 100%. This setup drives the dynamic algorithmic trigger in `TaskScoringPage`.

### `Pages/AttendancePage.razor`
- **Purpose**: Tracks daily presence.
- **Connections**: Injects `IAttendanceService`, `IClassSectionService`.
- **Flow**:
  - The instructor creates an `AttendanceSession` (a specific Date).
  - A grid loads all enrolled students. The instructor uses a `MudSelect` to mark `Present`, `Absent`, `Late`, or `Excused`.
  - Saving the session posts the batch to the database. This data is later scraped by the `AcademicProgressService` to calculate the exact percentage.

### `Shared/AcademicProgressDialog.razor`
- **Purpose**: A comprehensive summary modal showing exactly how a student is doing.
- **Logic & Flow**:
  - Invokes `_academicProgressService.GetStudentProgressAsync(studentId)`.
  - **Math Visualized**: Displays the calculated `AttendanceRate` as a progress bar (`MudProgressLinear`). If the rate is $\ge 80\%$, it's green. Otherwise, it turns warning colors.
  - Displays the `GradeAverage` and the total `PassedSubjects` / `FailedSubjects`.
  - Shows the algorithmically generated `Remarks` (e.g., "Outstanding academic performance" or "Has 1 failed subject(s). Additional effort needed.").

### `Pages/Dashboard.razor`
- **Purpose**: The immediate landing page post-authentication.
- **Connections**: Injects `IDashboardService` and `AuthenticationStateProvider`.
- **Flow & Logic**:
  - Parses the user's role.
  - **Admin View**: Calls `GetAdminDashboardMetricsAsync()`. This queries the database to count `TotalStudents`, `TotalClasses`, and groups the global attendance statistics. It renders a summary card layout using `MudPaper` and `MudIcon`.
  - **Student View**: Calls `GetStudentDashboardMetricsAsync(studentId)`. It calculates how many classes they are enrolled in, and what their immediate recent grades look like.
  - **Real-time UX**: Because Blazor Server maintains a live WebSocket, as data updates in the DB, any page refresh or internal navigation to the dashboard instantly rerenders the UI with the latest computed mathematical statistics.
## Code-Level Deep Dive & Architectural Approaches

This section extracts critical blocks of code from the system to explain the underlying architectural approaches, line-by-line logic, and point-to-point data flow.

### Phase A: Data Binding & Form Submission Approaches

Blazor relies heavily on **Two-Way Data Binding** to keep the UI and the C# backend code in perfect sync without manual DOM manipulation.

#### 1. The MudForm Validation Approach
*File Reference: `Components/Shared/StudentFormDialog.razor`*

**Extracted Code Block:**
```html
<MudForm @ref="form" IsValid="@success">
    <MudTextField @bind-Value="student.FirstName" Label="First Name" Required="true" RequiredError="First name is required!" />
    <MudTextField @bind-Value="student.LastName" Label="Last Name" Required="true" RequiredError="Last name is required!" />
</MudForm>

<MudButton OnClick="Submit" Disabled="@(!success)">Save</MudButton>

@code {
    MudForm form = null!;
    bool success;
    private Student student = new Student();

    private async Task Submit()
    {
        await form.ValidateAsync();
        if (form.IsValid)
        {
            MudDialog.Close(DialogResult.Ok(student));
        }
    }
}
```

**Approach Name**: Component Reference Binding & Asynchronous Validation

**Line-by-Line Breakdown & Logic Flow:**
1. `<MudForm @ref="form" IsValid="@success">`: 
   - **`@ref="form"`**: This is Blazor's *Component Reference Approach*. It binds the HTML `<MudForm>` element directly to the C# variable `MudForm form = null!;` in the `@code` block. This allows the C# code to programmatically control the UI form.
   - **`IsValid="@success"`**: This is a *One-Way Binding*. The `MudForm` evaluates its internal state and constantly updates the boolean `success` variable.
2. `<MudTextField @bind-Value="student.FirstName" ... Required="true">`:
   - **`@bind-Value`**: This is the *Two-Way Data Binding Approach*. As the user types in the browser, the `student.FirstName` C# property updates instantly. If the C# code changes `student.FirstName`, the text box updates instantly.
   - **`Required="true"`**: MudBlazor's built-in rule engine hooks into this to prevent submission if empty.
3. `<MudButton ... Disabled="@(!success)">`: Visually disables the save button if the form logic dictates it is invalid.
4. `await form.ValidateAsync();`: 
   - **Point-to-Point Flow**: When the user clicks "Save", it triggers the `Submit` method. Before doing any database work, it calls the referenced `MudForm` to run all validation rules asynchronously. 
5. `MudDialog.Close(DialogResult.Ok(student));`:
   - **Dialog Callback Approach**: Instead of hitting the database directly, this component simply packages the validated `student` object and passes it back to whatever parent component opened the modal. This ensures the Dialog component remains decoupled from the Database context.
### Phase B: Dependency Injection & Component Lifecycle Approaches

In Blazor, pages do not fetch their own data via HTTP requests to themselves. Instead, they use **Dependency Injection (DI)** to request Data Services directly.

#### 1. The OnInitializedAsync Data Fetch Approach
*File Reference: `Components/Pages/Dashboard.razor`*

**Extracted Code Block:**
```csharp
@inject IDashboardService DashboardService
@inject AuthenticationStateProvider AuthStateProvider

@code {
    private AdminDashboardMetrics? adminMetrics;
    private StudentDashboardMetrics? studentMetrics;
    private bool isAdminOrTeacher;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.IsInRole(UserRole.Admin.ToString()) || user.IsInRole(UserRole.Teacher.ToString()))
        {
            isAdminOrTeacher = true;
            adminMetrics = await DashboardService.GetAdminDashboardMetricsAsync();
        }
        else if (user.IsInRole(UserRole.Student.ToString()))
        {
            isAdminOrTeacher = false;
            var studentIdClaim = user.FindFirst("StudentId")?.Value;
            if (int.TryParse(studentIdClaim, out int studentId))
            {
                studentMetrics = await DashboardService.GetStudentDashboardMetricsAsync(studentId);
            }
        }
    }
}
```

**Approach Name**: Scoped Dependency Injection & Lifecycle Hooking

**Line-by-Line Breakdown & Logic Flow:**
1. **`@inject IDashboardService DashboardService`**: 
   - **DI Approach**: Blazor asks the `IServiceProvider` (configured in `Program.cs` as `AddScoped`) to provide an instance of the `DashboardService`. Because it's scoped, this instance is unique to the user's current SignalR connection.
2. **`protected override async Task OnInitializedAsync()`**:
   - **Lifecycle Approach**: This is part of the Blazor Component Lifecycle. This method fires exactly once when the component is first created, *before* the HTML is rendered to the user. It is the designated place to fetch data asynchronously.
3. **`await AuthStateProvider.GetAuthenticationStateAsync();`**:
   - Fetches the decrypted Auth Cookie data that was parsed during the `prerender` phase. It allows the backend to know exactly *who* is looking at the dashboard.
4. **`if (user.IsInRole(...))`**:
   - **Role-Based Logic Flow**: The dashboard diverges its logic. If the user is an Admin, it calls `GetAdminDashboardMetricsAsync()`.
5. **`var studentIdClaim = user.FindFirst("StudentId")?.Value;`**:
   - **Claims Extraction**: Instead of doing a heavy database lookup to find the Student's ID based on their username, the architecture embeds the `StudentId` directly into the encrypted cookie as a Claim during Login. This extracts it instantly from memory, highly optimizing the flow.
6. **`studentMetrics = await DashboardService...`**:
   - Point-to-Point: UI calls Service $\rightarrow$ Service queries Repository $\rightarrow$ Repo queries Database. The result populates the `studentMetrics` variable. Once `OnInitializedAsync` completes, Blazor automatically calls `StateHasChanged()`, which renders the HTML using this new data.
### Phase C: Algorithmic & Mathematics Code Blocks

The backend contains heavy algorithmic lifting, translating visual mathematics into C# logic.

#### 1. The Dynamic Weighted Average Approach
*File Reference: `Features/Services/Implementations/GradeService.cs`*

**Extracted Code Block:**
```csharp
public async Task RecalculateStudentGradeAsync(int studentId, int subjectId, string semester)
{
    var setup = await GetGradeSetupAsync(subjectId, semester);
    if (setup == null) return;

    var taskResults = (await _taskRepository.GetResultsForStudentAsync(studentId, subjectId)).ToList();
    decimal finalGrade = 0;

    foreach (var category in Enum.GetValues<TaskCategory>())
    {
        var categoryResults = taskResults.Where(r => r.ClassTask!.Category == category && r.PointsEarned.HasValue).ToList();
        if (categoryResults.Any())
        {
            decimal totalEarned = categoryResults.Sum(r => r.PointsEarned!.Value);
            decimal totalMax = categoryResults.Sum(r => r.ClassTask!.MaxPoints);
            
            if (totalMax > 0)
            {
                decimal categoryScorePercentage = (totalEarned / totalMax) * 100m;
                
                decimal weight = category switch
                {
                    TaskCategory.Quiz => setup.QuizzesPercentage,
                    TaskCategory.LongExam => setup.LongExamPercentage,
                    TaskCategory.Performance => setup.PerformancePercentage,
                    TaskCategory.SemesterExam => setup.SemesterExamPercentage,
                    _ => 0
                };
                
                finalGrade += (categoryScorePercentage * (weight / 100m));
            }
        }
    }

    var gradeRecord = (await _gradeRepository.GetByStudentIdAsync(studentId)).FirstOrDefault(g => g.SubjectId == subjectId);
    if (gradeRecord != null)
    {
        gradeRecord.OverallGrade = finalGrade;
        await _gradeRepository.UpdateAsync(gradeRecord);
    }
}
```

**Approach Name**: Dynamic Weighted Aggregation & LINQ Filtering

**Line-by-Line Breakdown & Logic Flow:**
1. **`var setup = await GetGradeSetupAsync(...)`**: Fetches the instructor-defined percentage weights (e.g., Quizzes = 20%).
2. **`foreach (var category in Enum.GetValues<TaskCategory>())`**: Iterates through every possible task category (Quiz, Exam, Project).
3. **`var categoryResults = taskResults.Where(...)`**: 
   - **LINQ Approach**: Filters the massive list of tasks down to only tasks that match the current loop iteration (e.g., only Quizzes) AND where the student actually has a score (`PointsEarned.HasValue`).
4. **`decimal totalEarned = categoryResults.Sum(...)`**: 
   - Mathematical Translation: $\sum \text{PointsEarned}$. It sums all the scores the student got in this specific category.
5. **`decimal categoryScorePercentage = (totalEarned / totalMax) * 100m;`**: 
   - Mathematical Translation: $\left( \frac{E}{M} \right) \times 100$. This calculates the student's base percentage for this category before weight is applied.
6. **`decimal weight = category switch { ... }`**: 
   - **Pattern Matching Approach**: A modern C# `switch` expression maps the current enum category to the database-driven weight defined in `setup`.
7. **`finalGrade += (categoryScorePercentage * (weight / 100m));`**: 
   - Mathematical Translation: $Overall = \sum \left( Score \times \frac{W}{100} \right)$. It takes the raw percentage, reduces it by the weight (e.g., $90 \times 0.20 = 18$), and adds it to the accumulating `finalGrade`.
8. **`await _gradeRepository.UpdateAsync(gradeRecord);`**: Finally, it persists this newly calculated mathematical grade to the database.
### Phase D: The Authentication Bypass Approach (JS Interop)

Blazor Server creates a major architectural challenge for Authentication: You cannot issue an HTTP Cookie over a WebSocket connection.

#### 1. The Hidden Form POST Approach
*File Reference: `Components/Pages/Login.razor`*

**Extracted Code Block:**
```html
<form id="loginForm" action="/api/auth/login" method="post" style="display:none;">
    <input type="hidden" name="username" value="@username" />
    <input type="hidden" name="password" value="@password" />
</form>

@code {
    private async Task Submit()
    {
        var isValid = await _userService.AuthenticateAsync(username, password);
        
        if (isValid)
        {
            await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('loginForm').submit();");
        }
        else
        {
            errorMessage = "Invalid username or password.";
        }
    }
}
```

**Approach Name**: JavaScript Interoperability (JS Interop) & HTTP Form Hijacking

**Line-by-Line Breakdown & Logic Flow:**
1. **`<form id="loginForm" action="/api/auth/login" method="post" style="display:none;">`**:
   - **Architectural Approach**: A traditional HTML form is created but completely hidden from the user (`display:none`). It points to a traditional REST API endpoint (`/api/auth/login`) mapped in `Program.cs`.
2. **`<input type="hidden" name="username" value="@username" />`**:
   - The hidden inputs are bound to the C# properties `@username` and `@password`. As the user types in the visible `MudForm`, these hidden fields automatically update to match via Blazor's data binding.
3. **`var isValid = await _userService.AuthenticateAsync(username, password);`**:
   - The user clicks the Blazor "Login" button. The server-side code checks the database to verify the credentials *before* doing anything destructive to the UI state.
4. **`await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('loginForm').submit();");`**:
   - **JS Interop Approach**: If the credentials are valid, Blazor executes a tiny piece of JavaScript natively in the user's browser. 
   - This JavaScript forces the hidden HTML `<form>` to execute a standard HTTP POST.
   - **Point-to-Point Flow**:
     1. Browser executes JS: `submit()`.
     2. Browser sends a standard `POST` to `/api/auth/login`.
     3. The ASP.NET Core middleware (`Program.cs`) intercepts this.
     4. The endpoint calls `HttpContext.SignInAsync()`, which generates the encrypted Auth Cookie and sends it back to the browser in the HTTP Response Headers.
     5. The endpoint issues a 302 Redirect to `/dashboard`.
     6. The browser navigates to `/dashboard` with the new cookie attached, completing the secure authentication cycle.
