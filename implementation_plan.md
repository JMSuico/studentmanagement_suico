# Implementation Plan - Student Management System Updates

This plan details the updates to be made to the Student Management System, including UI adjustments, functionality enhancements, and bug fixes.

## User Review Required

> [!NOTE]
> Please review the proposed changes and verify the behavior of the enrollment subjects list sorting and the database logic.

## Proposed Changes

### 1. Enrollment Request Dialog & Row Click Fix
We will address the checklist removal, detail modal contents, and the "cant clickable" row click issue.

#### [MODIFY] [StudentEnrollmentDetailDialog.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Shared/StudentEnrollmentDetailDialog.razor)
- Remove the `<!-- Static Checklist of Requirements -->` card block entirely.
- Ensure all required student info, subject details, year level, semester, school year, and status are clearly displayed.
- Keep the Close, Submit request, and Cancel request actions aligned with roles.

#### [MODIFY] [EnrollmentsPage.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Pages/EnrollmentsPage.razor)
- Change the `MudTable` binding from `OnRowClick="@(async (args) => await OnRowClicked(args))"` to `OnRowClick="OnRowClicked"` (method group binding) to prevent delegation overhead and ensure click events propagate correctly.
- Set `RowStyle="cursor: pointer;"` unconditionally for the table to ensure clickability is visually clear.
- Verify `OnRowClicked` calls `OpenStudentDetailDialog(args.Item)` to open the floating modal card for students when any row is clicked.

---

### 2. Enrollable Subjects Sorting and Dynamic School Year
Ensure students see the correct subjects for enrollment based on Year Level and Semester, sorted properly.

#### [MODIFY] [StudentEnrollmentDialog.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Shared/StudentEnrollmentDialog.razor)
- Compute `schoolYearBase` dynamically for the *selected semester* so that 1st Semester and 2nd Semester follow their corresponding school year bases (by querying the latest school year associated with that semester).
- Replace the simple class selection dropdown with a sorted list/table inside the dialog displaying the enrollable subjects/classes, sorted by Year Level and Semester.
- Allow the student to select a class/subject directly from this list/table before submitting.

---

### 3. Subjects, Students, and Classes Verification
We will double check and ensure the requirements for Year Level dropdowns, Student filters, and read-only Classes Module access are fully compliant.

#### [MODIFY] [SubjectManagement.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Pages/SubjectManagement.razor)
- Verify the Year Level dropdown (1st Year, 2nd Year, 3rd Year, 4th Year) works correctly as a filter for all users.

#### [MODIFY] [StudentsPage.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Pages/StudentsPage.razor)
- Verify the Year Level and Section dropdown filters are fully functional.

#### [MODIFY] [ClassManagement.razor](file:///C:/Users/provu/Desktop/StudentManagement%20system/StudentManagement/Components/Pages/ClassManagement.razor)
- Ensure the student role has read-only access: add/edit/delete buttons and schedule modification actions are hidden.
- Ensure the schedule details table shows Date (`MM/dd/yyyy`), Day, Start Time, and End Time when a row is clicked and expanded.

## Verification Plan

### Automated/Interactive Verification
- Rebuild the solution using `dotnet build`.
- Use the browser agent to log in as a student, navigate to Enrollments, test row clicks, request enrollment with the new sorted list/table, and verify the checklist is removed.
- Log in as admin, verify the dropdown filters on Students and Subjects pages work, and verify that Admin/Instructor approval actions remain functional.
