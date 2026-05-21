````md
# Prompt: C# MudBlazor Architecture, Rules, Chain Flow, and Structure

You are a **Senior Full-Stack Software Architect** and **Project Structure Designer** for a **C# MudBlazor** application.

Design a **production-ready, scalable, modular, and maintainable** system for a **Student Management System** using:

- **Frontend/UI:** Blazor + MudBlazor
- **Language:** C#
- **Data Layer:** Entity Framework Core
- **Architecture:** Vertical slice / feature-based architecture
- **Styling/UI system:** MudBlazor components and layout system
- **Database:** SQL Server, MySQL, or PostgreSQL only
- **Strict rule:** **No SQLite**

The system must follow the **existing project structure and chain flow exactly**.

Do **not** change the architecture flow.  
Do **not** introduce alternate flow paths.  
Do **not** create spaghetti code.  

---

## Main rule

Everything must follow the correct responsibility chain.

Always know:

- where to put code
- what each folder is for
- what each file does
- when code should be added
- how the flow must be followed

No random file placement.  
No mixed responsibilities.  
No shortcuts.  
No bypassing layers.  

---

# Project structure rule

Use the existing structure as the reference and preserve it.

## Frontend / UI structure

```txt
Components/                          # UI layer for MudBlazor pages and shared components
├── Layout/                          # Application shell and navigation layout
│   ├── EmptyLayout.razor            # Layout for minimal pages like login/register
│   ├── MainLayout.razor             # Main app layout wrapper
│   ├── MainLayout.razor.css         # Styling for the main layout
│   ├── NavMenu.razor                # Sidebar or navigation menu
│   └── NavMenu.razor.css            # Styling for the navigation menu
│
├── Pages/                           # Route-level pages, keeps page composition only
│   ├── AttendancePage.razor        # Attendance management screen
│   ├── ClassManagement.razor       # Class and section management screen
│   ├── Dashboard.razor             # Main dashboard page
│   ├── EnrollmentsPage.razor       # Enrollment workflow page
│   ├── GradeManagement.razor       # Grades and assessment page
│   ├── Index.razor                  # Default landing page
│   ├── Login.razor                 # Login page
│   ├── Logout.razor                # Logout handler page
│   ├── Profile.razor               # Profile page
│   ├── Register.razor              # Registration page
│   ├── StudentsPage.razor          # Student management page
│   ├── SubjectManagement.razor     # Subject management page
│   ├── TaskManagement.razor        # Task management page
│   ├── TaskScoringPage.razor       # Task scoring page
│   ├── TeacherManagement.razor     # Teacher management page
│   └── AcademicProgress.razor      # Academic progress tracking page
│
├── Shared/                          # Reusable UI components and dialogs
│   ├── AttendanceDialog.razor      # Dialog for attendance actions
│   ├── ConfirmDialog.razor         # Generic confirmation dialog
│   ├── GradeFormDialog.razor       # Dialog for creating/editing grade data
│   ├── GradeViewDialog.razor       # Dialog for viewing grade details
│   ├── StudentFormDialog.razor     # Dialog for creating/editing student data
│   ├── StudentViewDialog.razor     # Dialog for viewing student details
│   ├── SubjectFormDialog.razor     # Dialog for creating/editing subject data
│   ├── TeacherFormDialog.razor     # Dialog for creating/editing teacher data
│   └── UserFormDialog.razor        # Dialog for creating/editing users
│
├── App.razor                        # Root app component
├── Routes.razor                     # Routing definitions
└── _Imports.razor                   # Shared using directives
````

## Feature structure

```txt
Features/                                      # Core business architecture and shared technical layers
├── Data/                                      # Foundation layer: entities, enums, DbContext
│   ├── AppDbContext.cs                        # Entity Framework Core database context
│   │   -> Handles table mapping and DbSet definitions
│   │   -> No business logic allowed here
│   │
│   ├── Enums/                                 # Fixed choices and system states
│   │   ├── UserRole.cs                        # User roles for authentication and authorization
│   │   ├── StudentStatus.cs                   # Student lifecycle states
│   │   ├── EnrollmentStatus.cs                # Enrollment lifecycle states
│   │   ├── AttendanceStatus.cs                # Attendance states
│   │   ├── GradeStatus.cs                     # Grade record states
│   │   └── ClassStatus.cs                     # Class/section lifecycle states
│   │
│   └── Models/                                # Database entities and core data objects
│       ├── User.cs                            # User entity table
│       ├── Student.cs                         # Student entity table
│       ├── Teacher.cs                         # Teacher entity table
│       ├── Subject.cs                         # Subject entity table
│       ├── ClassSection.cs                    # Class/section entity table
│       ├── Enrollment.cs                      # Enrollment entity
│       ├── Attendance.cs                      # Attendance entity
│       ├── Grade.cs                           # Grade entity
│       └── Guardian.cs                        # Guardian/contact entity
│
├── Helpers/                                   # Shared reusable logic, not business workflows
│   ├── AuthenticationStateProvider.cs         # Handles login state and auth persistence
│   ├── AttendanceHelper.cs                    # Attendance calculations and status utilities
│   ├── DateHelper.cs                          # Date formatting and date utility functions
│   ├── GradeHelper.cs                         # Grade calculation helper logic
│   └── QRHelper.cs                            # QR encoding/decoding support helpers
│
├── Repositories/                              # Data access layer only
│   ├── Interfaces/                            # Repository contracts
│   │   ├── IUserRepository.cs                 # User data access contract
│   │   ├── IStudentRepository.cs              # Student data access contract
│   │   ├── ITeacherRepository.cs              # Teacher data access contract
│   │   ├── ISubjectRepository.cs              # Subject data access contract
│   │   ├── IClassSectionRepository.cs         # Class data access contract
│   │   ├── IEnrollmentRepository.cs           # Enrollment data access contract
│   │   ├── IAttendanceRepository.cs           # Attendance data access contract
│   │   ├── IAttendanceSessionRepository.cs    # Attendance session data access contract
│   │   ├── IGradeRepository.cs                # Grade data access contract
│   │   └── ITaskRepository.cs                 # Task data access contract
│   │
│   └── Implementations/                       # EF Core repository implementations
│       ├── UserRepository.cs                  # User database operations
│       ├── StudentRepository.cs               # Student database operations
│       ├── TeacherRepository.cs               # Teacher database operations
│       ├── SubjectRepository.cs               # Subject database operations
│       ├── ClassSectionRepository.cs          # Class database operations
│       ├── EnrollmentRepository.cs            # Enrollment database operations
│       ├── AttendanceRepository.cs            # Attendance database operations
│       ├── AttendanceSessionRepository.cs     # Attendance session database operations
│       ├── GradeRepository.cs                 # Grade database operations
│       └── TaskRepository.cs                  # Task database operations
│
├── Services/                                  # Business logic layer only
│   ├── Interfaces/                            # Service contracts
│   │   ├── IUserService.cs                    # User workflow contract
│   │   ├── IStudentService.cs                 # Student workflow contract
│   │   ├── ITeacherService.cs                 # Teacher workflow contract
│   │   ├── ISubjectService.cs                 # Subject workflow contract
│   │   ├── IClassSectionService.cs            # Class workflow contract
│   │   ├── IEnrollmentService.cs              # Enrollment workflow contract
│   │   ├── IAttendanceService.cs              # Attendance workflow contract
│   │   ├── IAttendanceSessionService.cs       # Attendance session workflow contract
│   │   ├── IGradeService.cs                   # Grade workflow contract
│   │   ├── ITaskService.cs                    # Task workflow contract
│   │   └── IAcademicProgressService.cs        # Academic progress workflow contract
│   │
│   └── Implementations/                       # Business rules and workflow orchestration
│       ├── UserService.cs                     # Registration, login, role rules
│       ├── StudentService.cs                  # Student management rules
│       ├── TeacherService.cs                  # Teacher management rules
│       ├── SubjectService.cs                  # Subject management rules
│       ├── ClassSectionService.cs             # Class management rules
│       ├── EnrollmentService.cs               # Enrollment workflow logic
│       ├── AttendanceService.cs               # Attendance workflow logic
│       ├── AttendanceSessionService.cs        # Attendance session workflow logic
│       ├── GradeService.cs                    # Grade workflow logic
│       ├── TaskService.cs                     # Task workflow logic
│       └── AcademicProgressService.cs         # Academic progress workflow logic
```

## Supporting project files

```txt
Migrations/                            # Entity Framework Core migration files
Program.cs                             # Application startup, DI, and service registration
appsettings.json                       # Main application configuration
appsettings.Development.json           # Development environment configuration
wwwroot/                               # Static assets
Logs/                                  # Implementation logs and error logs
EnrollmentNotes.md                     # Enrollment related documentation or notes
ErrorLogs.md                          # Error log tracking file
```

---

# Frontend and UI rules

## Pages

Pages are for:

* route-level composition
* layout assembly
* connecting feature logic to UI
* showing data from services
* handling navigation flow

Pages must **not** contain:

* heavy business logic
* repository logic
* database logic
* duplicated helper logic
* random UI workflows

Pages should stay thin and clean.

---

## Shared components

Shared components are for:

* dialogs
* reusable MudBlazor UI
* tables
* forms
* confirmation windows
* scanner UI
* view dialogs
* reusable page parts

Shared components must remain **presentation-focused**.

Do not place:

* business rules
* database calls
* service logic
* page workflows

inside shared components.

---

## Layout structure

Use `Components/Layout/` for:

* main application layout
* empty layout
* navigation menu
* routing shell
* page framing

Layout files control structure only.

---

# Feature layer rules

## Data layer

`Features/Data/` is the foundation.

### `Models/`

Use models for:

* users
* students
* teachers
* subjects
* class sections
* enrollments
* attendance records
* grades
* guardian/contact records
* other core data objects

Models represent database tables and core entities.

### `Enums/`

Use enums for:

* role types
* student statuses
* enrollment statuses
* attendance statuses
* grade statuses
* class statuses
* any fixed system choice values

### `AppDbContext.cs`

`AppDbContext` is the ORM boundary.

It must:

* map entities
* define DbSet collections
* configure relationships
* keep ORM configuration clean

It must **not** contain business rules.

---

## Repositories

Repositories are for **data access only**.

### Interfaces

Repository interfaces define contracts such as:

* get all
* get by id
* create
* update
* delete
* search
* filter
* enrollment lookup
* attendance lookup
* grade lookup

### Implementations

Repository implementations handle:

* EF Core queries
* inserts
* updates
* deletes
* includes
* tracking rules
* query optimization

Repositories must **not** contain business logic.

---

## Services

Services are for **business logic only**.

Use services to handle:

* login validation
* registration logic
* student management rules
* teacher management rules
* subject management rules
* enrollment rules
* attendance rules
* grade rules
* role-based access logic
* workflow orchestration

Services must call repositories.

Services must **not** directly access UI or layout files.

Services must **not** be replaced by page logic.

---

## Helpers

Helpers are for shared logic that is not a repository and not a service.

Use helpers for:

* authentication state helpers
* date formatting
* grade calculation support
* attendance helper logic
* QR helper logic
* reusable utility functions

Helpers must stay reusable and focused.

---

# Chain flow rule

The chain flow must always stay in the same order.

## Backend / feature flow

```txt
Models
↓
Enums
↓
AppDbContext
↓
Repository Interface
↓
Repository Implementation
↓
Service Interface
↓
Service Implementation
↓
Helpers
↓
Shared Components / Pages
↓
Routing
↓
Program.cs
↓
appsettings.json
↓
Migrations
```

## Request flow

```txt
UI Request
↓
MudBlazor Page or Component
↓
Service
↓
Repository
↓
DbContext
↓
Database
```

No skipping layers.
No reversing order.
No alternate chain flow.

---

# Placement rules

Always follow this placement logic:

## Put this in Data when:

* it is a model
* it is an enum
* it maps database structure
* it represents a table or entity

## Put this in Repositories when:

* it reads from the database
* it writes to the database
* it queries data
* it handles persistence logic

## Put this in Services when:

* it is business logic
* it validates workflows
* it coordinates multiple repositories
* it applies rules before saving or returning data

## Put this in Helpers when:

* it is reusable utility logic
* it is not tied to a single workflow
* it supports services or UI
* it formats or transforms data

## Put this in Pages when:

* it is a route-level screen
* it composes UI
* it connects dialogs, tables, and actions
* it is a page entry point

## Put this in Shared when:

* it is reusable MudBlazor UI
* it is a dialog
* it is a form
* it is a view component
* it is a scanner component
* it is a confirmation component

## Put this in Layout when:

* it controls app shell
* it controls navigation
* it controls page framing
* it controls layout switching

## Put this in Migrations when:

* it is generated by EF Core
* it is a database schema update
* it is a model change migration

---

# MudBlazor rules

Use MudBlazor properly and consistently.

* keep UI clean
* keep UI reusable
* keep dialogs in shared components
* keep layouts centralized
* keep forms modular
* keep navigation in layout components
* keep styling consistent with MudBlazor theme rules

Do not invent a separate UI system that conflicts with MudBlazor.

---

# Feature rules

The system must support features such as:

* login
* register
* dashboard
* student management
* class and section management
* subject management
* teacher management
* enrollment
* attendance tracking
* grades and assessments
* logout
* redirect handling
* role handling
* academic progress tracking

Each feature must keep its own logic cleanly separated.

---

# QR scanner and QR helper rules

QR logic must be isolated.

* QR UI belongs in `Shared/QRScanner.razor`
* QR helper logic belongs in `Features/Helpers/QRHelper.cs`
* QR workflow logic belongs in Services
* QR data access belongs in Repositories
* QR entities belong in Models

Do not place QR workflow directly inside pages.

Do not mix QR logic with layout logic.

---

# Authentication rules

Authentication must be handled using:

* `AuthenticationStateProvider`
* services
* helpers
* route protection

Authentication rules:

* login logic must be centralized
* register logic must be centralized
* role checks must be enforced
* redirect logic must be clean
* unauthorized users must be protected from restricted pages

Do not scatter auth logic across pages and dialogs.

---

# Logging rules

Keep logs separate and structured.

## Implementation logs

Use implementation logs to record:

* feature changes
* architecture updates
* code placement decisions
* workflow updates
* model changes
* service changes
* repository changes
* UI updates

## Error logs

Use error logs to record:

* runtime errors
* build issues
* UI issues
* database issues
* service failures
* repository failures
* attendance failures
* authentication failures

Logging must not be mixed into business logic files.

---

# Database rules

## Allowed databases only

Use only:

* SQL Server
* MySQL
* PostgreSQL

## Strict prohibited rule

* **Do not use SQLite**

## Database placement

Database configuration belongs in:

* `Program.cs`
* `appsettings.json`
* `appsettings.Development.json`
* `Features/Data/AppDbContext.cs`

The database selection and connection setup must be clean and centralized.

---

# Program and configuration rules

## `Program.cs`

Use `Program.cs` for:

* dependency injection
* MudBlazor setup
* DbContext registration
* service registration
* repository registration
* authentication registration
* route and app bootstrapping

Do not overload `Program.cs` with business logic.

## `appsettings.json`

Use configuration files for:

* connection strings
* logging settings
* app options
* environment settings

Do not hardcode configuration values in random files.

---

# Migrations rules

Migrations must stay aligned with model changes.

* add model first
* update DbContext if needed
* create migration
* update database
* keep migration history clean
* never manually break the chain

Migrations are a result of model changes, not a replacement for architecture.

---

# Where to put what

## Student-related logic

* models → `Features/Data/Models/`
* status enum → `Features/Data/Enums/`
* query logic → `Repositories`
* business logic → `Services`
* helper functions → `Helpers`
* pages → `Components/Pages/StudentManagement.razor`
* dialogs → `Components/Shared/StudentFormDialog.razor`, `StudentViewDialog.razor`

## Attendance-related logic

* models → `Features/Data/Models/`
* status enum → `Features/Data/Enums/`
* query logic → `Repositories`
* business logic → `Services`
* helper functions → `Helpers`
* page → `Components/Pages/Attendance.razor`
* dialog → `Components/Shared/AttendanceDialog.razor`

## Grade-related logic

* models → `Features/Data/Models/`
* status enum → `Features/Data/Enums/`
* repository → `Repositories`
* service → `Services`
* UI page → `Components/Pages/GradeManagement.razor`
* dialog → `Components/Shared/GradeFormDialog.razor`, `GradeViewDialog.razor`

## Enrollment logic

* models → `Features/Data/Models/`
* repository → `Repositories`
* service → `Services`
* UI page → `Components/Pages/Enrollment.razor`

## Login and register logic

* page → `Components/Pages/Login.razor`, `Register.razor`
* service → `Services`
* auth helper → `Helpers`
* auth provider → `AuthenticationStateProvider.cs`

## Dashboard and navigation

* dashboard page → `Components/Pages/Dashboard.razor`
* main layout → `Components/Layout/MainLayout.razor`
* nav menu → `Components/Layout/NavMenu.razor`

---

# Strict development rules

* No spaghetti code.
* No alternate chain flow.
* No changing the architecture order.
* No bypassing repositories.
* No putting business logic in pages.
* No putting database logic in services.
* No putting UI logic in helpers.
* No random file placement.
* No SQLite.
* No mixing page logic with data access.
* No mixing layout code with workflow code.
* No hidden dependencies.

---

# What the system must achieve

The final structure must be:

* scalable
* modular
* maintainable
* reusable
* easy to extend
* easy to debug
* easy to refactor
* future-ready
* clean for team development

It must support a clear separation between:

* UI
* layout
* shared components
* pages
* helpers
* services
* repositories
* data models
* enums
* DbContext
* migrations
* configuration
* logs

---

# Final output requirement

When generating code or structure:

* keep the exact chain flow
* keep the project structure consistent
* keep MudBlazor usage clean
* keep C# files in the correct layer
* keep UI and business logic separated
* keep database access inside repositories
* keep services as the business layer
* keep helpers reusable
* keep pages thin
* keep shared dialogs reusable
* keep logs separate
* keep everything free from spaghetti code

```
```
Task

Here is the **remaining modules** written in the same detailed style as your **Register, Login, and Student Management** requirements.

---

# Modules Pages

## 1) Dashboard

Add a **Dashboard page** that will serve as the main overview of the whole system.

### Dashboard should display:

* Total number of students
* Total number of instructors
* Total number of admins
* Total number of classes and sections
* Total number of subjects
* Total number of enrolled students
* Total number of present, absent, and late students
* Total number of grades recorded
* Total academic progress status
* Recent activities or recent logs

### Dashboard functions:

* Show summary cards
* Show chart or graph for student data
* Show recent enrolled students
* Show recent attendance records
* Show recent grade updates
* Show quick action buttons like:

  * Add Student
  * Add Subject
  * Add Teacher
  * Add Enrollment
  * View Attendance
  * View Grades

### Dashboard access:

* **Instructor** can view dashboard and important records
* **Admin** can view dashboard and manage all records
* **Student** can view their own academic summary only

---

## 2) Class and Section Management

Add a **Class and Section Management** module that will handle all classes, year levels, and sections in the system.

### Class and Section fields:

* Class ID
* Class Name
* Department
* Course
* Year Level
* Section/Block
* Adviser/Intructor Name
* Academic Year
* Semester
* Status

### Class and Section functions:

* Add class
* View class
* Edit class
* Delete class
* Assign students to section
* Assign instructor to class
* Filter by department
* Filter by course
* Filter by year level
* Filter by section/block

### Class and Section display:

* Grid view
* Table view
* Card view

### Class and Section modal:

When clicking **Add Class** or **Edit Class**, display a floating modal card with the following values:

* Class Name
* Department
* Course
* Year Level
* Section/Block
* Adviser/Instructor
* Academic Year
* Semester
* Status

Buttons:

* Cancel
* Save
* Update

---

## 3) Subject Management

Add a **Subject Management** module that will handle all subjects included in the system.

### Subject fields:

* Subject ID
* Subject Code
* Subject Name
* Subject Description
* Units
* Department
* Course
* Year Level
* Semester
* Assigned Instructor
* Status

### Subject functions:

* Add subject
* View subject
* Edit subject
* Delete subject
* Assign subject to instructor
* Assign subject to class/section
* Search subject
* Filter subject by department, course, or year level

### Subject display:

* Grid view
* Table view
* Card view

### Subject modal:

When clicking **Add Subject** or **Edit Subject**, show a floating modal card with the following values:

* Subject Code
* Subject Name
* Subject Description
* Units
* Department
* Course
* Year Level
* Semester
* Assigned Instructor
* Status

Buttons:

* Cancel
* Save
* Update

---

## 4) Teacher Management

Add a **Teacher Management** module that will manage instructor records in the system.

### Teacher fields:

* Teacher ID
* Upload Image
* First Name
* Middle Name
* Last Name
* Email
* Username
* Password
* Location
* Phone Number
* Department
* Major Subject Handling
* Status

### Teacher functions:

* Add teacher
* View teacher
* Edit teacher
* Delete teacher
* Assign subjects
* Assign class/section
* Search teacher
* Filter by department
* Filter by status

### Teacher display:

* Grid view
* Table view
* Card view

### Teacher modal:

When clicking **Add Teacher** or **Edit Teacher**, show a floating modal card with the following values:

* Upload Image
* First Name
* Middle Name
* Last Name
* Email
* Username
* Location
* Phone Number
* Department
* Major Subject Handling
* Password
* Confirm Password
* Status

Buttons:

* Cancel
* Save
* Update

---

## 5) Enrollment

Add an **Enrollment** module that will handle the enrollment of students into classes and subjects.

### Enrollment fields:

* Enrollment ID
* Student ID
* Student Name
* Class/Section
* Course
* Year Level
* Subject
* Semester
* Academic Year
* Enrollment Date
* Status

### Enrollment functions:

* Add enrollment
* View enrollment
* Edit enrollment
* Delete enrollment
* Search enrollment
* Filter by class, section, subject, or student name
* Approve enrollment
* Cancel enrollment
* Print enrollment record

### Enrollment display:

* Grid view
* Table view
* Card view

### Enrollment modal:

When clicking **Add Enrollment** or **Edit Enrollment**, show a floating modal card with the following values:

* Student Name
* Student ID
* Class/Section
* Course
* Year Level
* Subject
* Semester
* Academic Year
* Enrollment Date
* Status

Buttons:

* Cancel
* Save
* Update

---

## 6) Attendance Tracking

Add an **Attendance Tracking** module that will record and monitor student attendance.

### Attendance fields:

* Attendance ID
* Student ID
* Student Name
* Class/Section
* Subject
* Date
* Time In
* Time Out
* Attendance Status
* Remarks

### Attendance status values:

* Present
* Absent
* Late
* Excused

### Attendance functions:

* Mark attendance
* View attendance
* Edit attendance
* Delete attendance
* Search attendance
* Filter by date
* Filter by class/section
* Filter by subject
* Filter by student
* Generate attendance report

### Attendance display:

* Grid view
* Table view
* Card view

### Attendance modal:

When clicking **Mark Attendance** or **Edit Attendance**, show a floating modal card with the following values:

* Student Name
* Student ID
* Class/Section
* Subject
* Date
* Time In
* Time Out
* Attendance Status
* Remarks

Buttons:

* Cancel
* Save
* Update

---

## 7) Grades and Assessments

Add a **Grades and Assessments** module that will handle quizzes, seatworks, activities, exams, and final grades.

### Grades and Assessments fields:

* Grade ID
* Student ID
* Student Name
* Subject
* Class/Section
* Assessment Type
* Score
* Total Items
* Percentage
* Remarks
* Academic Term

### Assessment type values:

* Quiz
* Activity
* Assignment
* Seatwork
* Project
* Midterm Exam
* Final Exam
* Final Grade

### Grades and Assessments functions:

* Add grade
* View grade
* Edit grade
* Delete grade
* Search grade
* Filter by subject
* Filter by section
* Filter by student
* Automatically calculate total grade
* Automatically calculate remarks

### Grades and Assessments display:

* Grid view
* Table view
* Card view

### Grades and Assessments modal:

When clicking **Add Grade** or **Edit Grade**, show a floating modal card with the following values:

* Student Name
* Student ID
* Subject
* Class/Section
* Assessment Type
* Score
* Total Items
* Percentage
* Remarks
* Academic Term

Buttons:

* Cancel
* Save
* Update

---

## 8) Academic Progress Tracking

Add an **Academic Progress Tracking** module that will show the academic performance of each student.

### Academic Progress fields:

* Progress ID
* Student ID
* Student Name
* Course
* Year Level
* Section/Block
* Subject
* Attendance Rate
* Grade Average
* Passed Subjects
* Failed Subjects
* Academic Standing
* Remarks

### Academic Progress functions:

* View student progress
* Track attendance performance
* Track grade performance
* Show semester performance
* Show yearly performance
* Generate progress report
* Compare current and previous performance
* Print academic summary

### Academic Progress display:

* Grid view
* Table view
* Card view
* Chart or graph view

### Academic Progress modal:

When clicking **View Progress**, show a floating modal card with the following values:

* Student Name
* Student ID
* Course
* Year Level
* Section/Block
* Subject
* Attendance Rate
* Grade Average
* Passed Subjects
* Failed Subjects
* Academic Standing
* Remarks

Buttons:

* Close
* Print
* Export

---

## 9) Profile

Add a **Profile** module that will allow the logged in user to view and edit their personal account information.

### Profile fields:

* Profile Image
* Full Name
* Username
* Email
* Role
* Location
* Phone Number
* Password
* Confirm Password

### Profile functions:

* View profile
* Edit profile
* Change password
* Update profile image
* Update contact information

### Profile display:

* Profile card
* Editable form
* Account details section

### Profile modal:

When clicking **Edit Profile**, show a floating modal card with the following values:

* Upload Image
* Full Name
* Username
* Email
* Location
* Phone Number
* Password
* Confirm Password

Buttons:

* Cancel
* Save
* Update

---

## 10) Logout

Add a **Logout** module that will safely log out the user from the system.

### Logout functions:

* Confirm logout
* Clear session
* Clear user token
* Redirect to login page

### Logout confirmation modal:

When clicking logout, show a floating modal card that says:

**“Do you want to logout?”**

Buttons:

* Cancel
* Logout

### Logout behavior:

* If user confirms, redirect to login page
* If user cancels, stay on current page

---

# Final Flow of the System

1. User registers using the multi-sliding modal.
2. User logs in.
3. System detects if the account is **Instructor**, **Admin**, or **Student**.
4. User is redirected to the correct dashboard or page.
5. Instructor and Admin can manage academic records and student records.
6. Student can only view personal profile and academic progress.
7. Logout safely closes the session.

---

If you want, I can turn this next into a **clean AI prompt format** that you can directly paste into your code generator or developer tool.


------

Function

Below is your **updated detailed imaginative output** with **comments on what each module/function does**. I kept the same style of your request, but made it cleaner and easier to follow.

---

# Student Management System Modules with Comments

## 1) Register Page

**Comment:** This page is for creating a new account. It uses a **multi-sliding modal** so the form is easier to finish step by step.

### In the Register add:

### **First Phase**

**Account Type**

* Instructor
* Admin
* Student

**Comment:** This first step lets the system know what kind of user is registering. The fields that appear next will depend on the selected account type.

---

### **If User selects Instructor**

#### **Second Phase**

* First Name
* Last Name
* Email
* Location
* Major Subject Handling

**Comment:** This part is for instructor personal information and the subject they will handle in the system.

#### **Third Phase**

* Username
* Password `[Eye Toggle]`
* Confirm Password `[Eye Toggle]`

**Comment:** This is the account security part. The eye toggle is added so the user can show or hide the password while typing.

---

### **If User selects Admin**

#### **Second Phase**

* First Name
* Last Name
* Email
* Location

**Comment:** This part stores the basic personal details of the admin account.

#### **Third Phase**

* Username
* Password `[Eye Toggle]`
* Confirm Password `[Eye Toggle]`

**Comment:** This part creates the login credentials for the admin.

---

### **If User selects Student**

#### **Second Phase**

* First Name
* Middle Name
* Last Name
* Email
* Location
* Phone Number
* College Department
* Course
* Year Level (1st, 2nd, 3rd, 4th)
* Section/Block (A, B, C, D, E, F)

**Comment:** This part stores the student profile information, class placement, and contact details.

#### **Third Phase**

* Username
* Password `[Eye Toggle]`
* Confirm Password `[Eye Toggle]`

**Comment:** This part creates the student account login details.

---

## 2) Login Page

**Comment:** This page is for signing in to the system. The system must automatically detect what type of account is logging in.

### Login Functions:

* Detect if the user is **Instructor**, **Admin**, or **Student**
* Redirect the user to the correct page after login
* Validate username and password
* Show error if credentials are wrong
* Allow password eye toggle if needed

**Comment:** This page controls access to the system and ensures that each role goes to the correct dashboard or module page.

---

## 3) User Role Enumeration

**Comment:** This defines the allowed account roles in the system.

### Add User Role in the Enums:

* Instructor
* Admin
* Student

**Comment:** These roles are used for login access, page permissions, and module control.

---

# Module Pages

## 4) Dashboard

**Comment:** This is the main summary page of the whole system. It shows an overview of all important records.

### Dashboard Functions:

* Show total number of students
* Show total number of instructors
* Show total number of admins
* Show total number of classes and sections
* Show total number of subjects
* Show total number of enrolled students
* Show total attendance status
* Show total grade records
* Show academic progress summary
* Show recent activities or logs

**Comment:** The dashboard helps the user quickly see what is happening in the system without opening each module one by one.

### Dashboard Features:

* Summary cards
* Graphs or charts
* Recent records
* Quick action buttons

**Comment:** These features make the dashboard easier to use and more informative.

---

## 5) Class and Section Management

**Comment:** This module manages the grouping of students into classes, sections, and year levels.

### Class and Section Fields:

* Class ID
* Class Name
* Department
* Course
* Year Level
* Section/Block
* Adviser/Instructor
* Academic Year
* Semester
* Status

### Class and Section Functions:

* Add class
* View class
* Edit class
* Delete class
* Assign students to section
* Assign instructor to class
* Filter by course, year level, or section

**Comment:** This module organizes students into proper academic groups so the school records stay structured.

### Display Options:

* Grid view
* Table view
* Card view

**Comment:** These display types make the records easier to browse and manage.

---

## 6) Student Management

**Comment:** This module handles all student records and is controlled by **Instructor** and **Admin** only.

### Student Management Access:

* Instructor -> allowed
* Admin -> allowed
* Student -> limited access only

**Comment:** Only authorized roles can create, edit, or delete student records.

### Add Student Button

When clicked, it should show a floating modal card with:

* Upload Image
* First Name
* Middle Name
* Last Name
* Email
* Username
* Location
* Phone Number
* College Department
* Course
* Year Level (1st, 2nd, 3rd, 4th)
* Section/Block (A, B, C, D, E, F)
* Password
* Confirm Password

Buttons:

* Cancel
* Save

**Comment:** This modal is used for creating a new student record.

### Display Function:

* Grid to Table
* Table to Card
* Card display

**Comment:** This gives flexibility in how the student list is viewed.

### Action Buttons:

* View icon
* Edit icon
* Delete icon

**Comment:** These actions allow the user to check, update, or remove student information.

### View Action

Show a floating modal card with all student details.

**Comment:** This is used for reading the full student record without editing.

### Edit Action

Show a floating modal card with the same fields as Add Student.

**Comment:** This is used for updating student information.

### Delete Action

Show a floating modal card saying:
**“Do you want to Delete?”**
Buttons:

* Cancel
* Delete

**Comment:** This gives a confirmation step before removing the record.

---

## 7) Subject Management

**Comment:** This module is for managing all subjects offered in the system.

### Subject Fields:

* Subject ID
* Subject Code
* Subject Name
* Description
* Units
* Department
* Course
* Year Level
* Semester
* Assigned Instructor
* Status

### Subject Functions:

* Add subject
* View subject
* Edit subject
* Delete subject
* Assign subject to instructor
* Assign subject to class/section

**Comment:** This module helps organize school subjects and connect them to the correct instructor and class.

---

## 8) Teacher Management

**Comment:** This module manages instructor or teacher records.

### Teacher Fields:

* Teacher ID
* Upload Image
* First Name
* Middle Name
* Last Name
* Email
* Username
* Password
* Location
* Phone Number
* Department
* Major Subject Handling
* Status

### Teacher Functions:

* Add teacher
* View teacher
* Edit teacher
* Delete teacher
* Assign subject
* Assign class section

**Comment:** This module stores the teacher profile and links them to the classes and subjects they handle.

---

## 9) Enrollment

**Comment:** This module is for enrolling students into classes and subjects.

### Enrollment Fields:

* Enrollment ID
* Student ID
* Student Name
* Class/Section
* Course
* Year Level
* Subject
* Semester
* Academic Year
* Enrollment Date
* Status

### Enrollment Functions:

* Add enrollment
* View enrollment
* Edit enrollment
* Delete enrollment
* Approve enrollment
* Cancel enrollment
* Search enrollment records

**Comment:** This module is used to officially place a student into the system’s academic records.

---

## 10) Attendance Tracking

**Comment:** This module records whether a student is present, absent, or late.

### Attendance Fields:

* Attendance ID
* Student ID
* Student Name
* Class/Section
* Subject
* Date
* Time In
* Time Out
* Attendance Status
* Remarks

### Attendance Status:

* Present
* Absent
* Late
* Excused

### Attendance Functions:

* Mark attendance
* View attendance
* Edit attendance
* Delete attendance
* Filter by date, subject, class, or student

**Comment:** This module helps teachers and admins monitor student attendance behavior and records.

---

## 11) Grades and Assessments

**Comment:** This module is for recording student scores and assessments.

### Grades and Assessments Fields:

* Grade ID
* Student ID
* Student Name
* Subject
* Class/Section
* Assessment Type
* Score
* Total Items
* Percentage
* Remarks
* Academic Term

### Assessment Types:

* Quiz
* Activity
* Assignment
* Seatwork
* Project
* Midterm Exam
* Final Exam
* Final Grade

### Grades and Assessments Functions:

* Add grade
* View grade
* Edit grade
* Delete grade
* Automatically compute grade
* Automatically compute remarks

**Comment:** This module stores all performance results and can be used to calculate final standing.

---

## 12) Academic Progress Tracking

**Comment:** This module shows how the student is performing over time.

### Academic Progress Fields:

* Progress ID
* Student ID
* Student Name
* Course
* Year Level
* Section/Block
* Subject
* Attendance Rate
* Grade Average
* Passed Subjects
* Failed Subjects
* Academic Standing
* Remarks

### Academic Progress Functions:

* View student progress
* Track attendance performance
* Track grade performance
* Compare semester results
* Generate progress report

**Comment:** This module gives a clear picture of the student’s overall performance in school.

---

## 13) Profile

**Comment:** This module allows the logged-in user to manage their own personal account information.

### Profile Fields:

* Profile Image
* Full Name
* Username
* Email
* Role
* Location
* Phone Number
* Password
* Confirm Password

### Profile Functions:

* View profile
* Edit profile
* Change password
* Update image
* Update contact details

**Comment:** This page is for account maintenance and personal information updates.

---

## 14) Logout

**Comment:** This module safely ends the user session and returns the user to the login page.

### Logout Functions:

* Confirm logout
* Clear session
* Clear tokens
* Redirect to login page

### Logout Confirmation Modal:

Message:
**“Do you want to logout?”**

Buttons:

* Cancel
* Logout

**Comment:** This prevents accidental logout and makes the system more secure.

---

# Final System Flow Comment

1. User registers using the multi-step sliding modal.
2. User logs in.
3. System detects the role: Instructor, Admin, or Student.
4. User is redirected to the proper page.
5. Instructor and Admin can manage records.
6. Student can only view limited personal information.
7. Logout safely ends the session.

**Comment:** This flow makes the system organized, role-based, and easier to manage.

---

If you want, I can turn this into a **more polished prompt format for AI coding** so you can paste it directly into your code generator.

---


-------------------------------------


# Additional Module Functions and Improved System Flow

## 1) Student Enrollment Module with Limited Access

### Add function in the Student user:

The **Student** account will have a limited-access **Enrollment Module**.

### Student enrollment flow:

* The student will log in to their account.
* After login, the system will automatically navigate them to the **Enrollment Module**.
* The student will not manually select their name because the system will automatically provide the logged-in student’s name.
* The student will only fill in the allowed enrollment request information.
* After entering the required data, the student will click **Submit**.

### Enrollment request behavior:

* The submitted request will appear in the **Admin** and **Instructor** accounts as **Request Pending**.
* The **Admin** and **Instructor** must review the request.
* They will then approve or reject the request by changing the status to:

  * **Enrolled**
  * **Dropped**
  * **Completed**
  * **Cancelled**

### Student access level:

* The student can only create an enrollment request.
* The student cannot directly approve or modify the final enrollment status.
* The student can only view the result of the enrollment request after approval or rejection.

**Comment:** This makes the enrollment process controlled and secure because only Admin and Instructor can approve enrollment requests.

---

## 2) Student Subject Module with Semester-Based Viewing

### Add function in the Student user:

In the **Management -> Subject Module**, the student will only see the list of subjects assigned for the current semester.

### Subject list display:

The table will show the following columns:

* **Code**
* **Name**
* **Units**
* **Status**
* **Actions**

### Semester selection:

* Add a dropdown for semester and school year selection.
* The dropdown must show the **latest semester first**, then the older semesters below it.
* Example order:

  * **1st Sem School Year 2025–2026**
  * **2nd Sem School Year 2024–2025**
  * **1st Sem School Year 2024–2025**

### Default display:

* The system must always show the **latest semester and school year** by default.
* When the semester is changed, the subject list must also update automatically.

### Student action buttons:

In the **Student User**, the only available action buttons should be:

* **Enroll**
* **Cancel**

**Comment:** This keeps the student subject module simple and limited to viewing and requesting enrollment only.

---

## 3) Attendance Module with Schedule-Based Code System

### Admin and Instructor Attendance Module:

In **Academic -> Attendance Module** for **Admin** and **Instructor**, connect the attendance system to the **Management -> Classes Module**.

### Required function:

* Display upcoming classes based on the scheduled time.
* Each upcoming class should generate a **6-digit random code**.
* This code will be used by the student to mark attendance.

### Code sending behavior:

* The system must show the exact date and time when the code was generated and sent.
* The Admin and Instructor must be able to view the attendance code history.

### Student Attendance Module:

In **Academic -> Attendance Module** for **Student**, add a function where the student must:

* Enter the 6-digit attendance code
* Submit the code to mark themselves as present

### Attendance time rules:

The system must automatically determine the student’s attendance status based on the time they enter the code:

* **Present** – if the code is entered within the scheduled time
* **Late** – if the code is entered from the schedule time up to 15 minutes after
* **Absent** – if the code is entered 15 minutes before the schedule time or after the allowed window

### Attendance logic:

* The code must be valid only for the correct class and schedule.
* The student must enter the code before the allowed time ends.
* The Admin and Instructor must see the exact attendance record, including:

  * Student name
  * Code used
  * Date
  * Time sent
  * Time submitted
  * Final status

**Comment:** This creates a smart attendance system that is connected to class schedules and automatically evaluates attendance status.

---

## 4) Grades Module for Admin and Instructor

### Admin and Instructor Grades Module:

In **Academic -> Grades Module**, the Admin and Instructor will see the list of subjects in a table.

### Table display:

The table should show:

* Subject
* Section
* Semester
* Grade setup
* Status
* Actions

### Add function:

Each subject can be edited through a floating modal card where the grading percentage can be configured.

### Grading percentage setup:

The system must allow the Admin and Instructor to set the grading components such as:

* **Quizzes – 10%**
* **Long Exam – 15%**
* **Performance – 60%**
* **Semester Exam – 15%**

### Performance breakdown:

The **Performance (60%)** can be divided into:

* Hands-on – 20%
* Practical Exam – 20%
* Oral / Assignment – 20%

### Flexible grade structure:

The system must allow adding more input fields if needed, and the grade computation must automatically adjust based on the set percentage.

### Semester-based grading:

The system must support semester-based grading structure such as:

* **Midterm 1st Sem**
* **Final Term 1st Sem**
* **Midterm 2nd Sem**
* **Final Term 2nd Sem**

### Grade computation behavior:

* The system must automatically compute the final grades based on the subject grading configuration.
* The system must rearrange the computation if percentages are changed.
* The system must ensure the total percentage always matches the final grading setup.

### Student Grades Module:

In **Academic -> Grades Module** for **Student**, the student will only be able to:

* View their own grades
* See the breakdown of their performance
* View the final computed result

**Comment:** This makes the grading system flexible for instructors while keeping student access read-only.

---

## 5) Assign Task Module for Admin and Instructor

### Add module:

In **Academic -> Assign Task Module**, the Admin and Instructor can manage class tasks and assessment activities.

### Task structure:

The module should be organized by subject, then by grading category:

* **Quizzes**
* **Long Exam**
* **Performance**
* **Semester Exam**

### Tab-based layout:

Add tabs so that when a tab is clicked, the Admin and Instructor can manage the tasks under that category.

Example tabs:

* Quizzes
* Long Exam
* Performance
* Semester Exam

### Add Task function:

When clicking **Add Task**, a floating modal card should appear with the following fields:

* Task Title
* Task Description
* Task Points
* Deadline Duration

Buttons:

* Cancel
* Add Task

### View Task function:

When clicking **View**, show a floating modal card with:

* Task Title
* Task Description
* Task Points
* Deadline Duration

Buttons:

* Back

### Edit Task function:

When clicking **Edit**, show a floating modal card with the same fields:

* Task Title
* Task Description
* Task Points
* Deadline Duration

Buttons:

* Cancel
* Update

### Delete Task function:

When clicking **Delete**, show a confirmation modal saying:
**“Do you want to delete this task?”**

Buttons:

* Cancel
* Delete

**Comment:** This module helps Admin and Instructor organize assessments under the proper grading category.

---

## 6) Student Points Submission and Task Result Tracking

### Add function in the student list action:

In the task module, the **View** action should open a modal floating card showing the list of students.

### Student task scoring:

Inside the student list modal, Admin and Instructor can:

* Enter the **points** of each student
* The system will automatically calculate the score based on the task percentage
* The calculation must depend on:

  * The selected tab
  * The selected task
  * The percentage assigned to that assessment category

### Student task status:

Each student task record must include a status:

* **Done**
* **Late**
* **No Submission**

### Percentage behavior:

* The percentage must depend on the selected task category.
* The system must automatically adjust the score calculation according to the grading setup.
* The points entered for each student must match the correct category weighting.

### Student task view:

The modal should show:

* Student list
* Student name
* Points
* Computed score
* Status
* Remarks if needed

**Comment:** This makes task evaluation automatic and consistent with the grading structure of the subject.

---

# Corrected Overall Flow

1. The **Student** logs in and goes to the **Enrollment Module**.
2. The student submits an enrollment request.
3. The **Admin** and **Instructor** review it and change the status to Enrolled, Dropped, Completed, or Cancelled.
4. The **Student** views only the assigned subjects for the selected semester.
5. The **Admin** and **Instructor** generate attendance codes from the class schedule.
6. The **Student** enters the 6-digit code to mark attendance.
7. The system determines whether the student is Present, Late, or Absent based on time.
8. The **Admin** and **Instructor** manage subject grading setup.
9. The **Assign Task Module** is used to create and manage tasks under each grading category.
10. The system automatically computes grades based on the percentage setup.
11. The **Student** can only view their own grades and task results.

---

If you want, I can turn this next into a **single polished prompt format** so you can paste it directly into your AI coding tool.
