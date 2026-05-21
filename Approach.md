# Grading Approach — Term-Period Weighted System

## Overview

This document describes the **Term-Period Weighted Grading Approach** used in the Student Management System's **Academics → Grades → Setup Grading** module.

## Formula

The **Overall Grade** for each subject is computed as:

```
Overall Grade = (Prelim Grade × 30%) + (Midterm Grade × 30%) + (Final Term Grade × 40%)
```

| Term Period      | Weight toward Final Grade |
|------------------|--------------------------|
| Prelim Grade     | **30%**                  |
| Midterm Grade    | **30%**                  |
| Final Term Grade | **40%**                  |
| **Total**        | **100%**                 |

## Detailed Explanation

### 1. Prelim Grade (30%)
- Covers the first third of the semester
- Includes quizzes, assignments, hands-on activities, and a preliminary exam
- The Prelim Grade is computed internally from **task scores** using the **Setup Grading** category weights (Quizzes, Long Exam, Performance, Semester Exam)

### 2. Midterm Grade (30%)
- Covers the middle portion of the semester
- Same internal structure: quizzes, long exams, performance tasks, and a midterm exam
- Computed from task scores using the same Setup Grading weights

### 3. Final Term Grade (40%)
- Covers the last portion of the semester and carries the **highest weight** because it reflects cumulative understanding
- Includes all category types plus a comprehensive final exam
- Computed identically from task scores using Setup Grading weights

## Calculation Example

Suppose a student has the following term grades for a subject:

| Term     | Grade |
|----------|-------|
| Prelim   | 85    |
| Midterm  | 90    |
| Finals   | 88    |

**Overall Grade** = (85 × 0.30) + (90 × 0.30) + (88 × 0.40)
                  = 25.5 + 27.0 + 35.2
                  = **87.7**

## Academic Standing Rules

| Overall Grade | Standing           |
|---------------|--------------------|
| ≥ 85          | Excellent          |
| 75 – 84.99    | Good               |
| < 75          | Needs Improvement  |

A student **passes** a subject if their Overall Grade ≥ 75.

## How It Connects to Setup Grading

The **Setup Grading** module defines **within-term** category weights:

| Category       | Default Weight |
|----------------|----------------|
| Quizzes        | 10%            |
| Long Exam      | 15%            |
| Performance    | 60%            |
| Semester Exam  | 15%            |
| **Total**      | **100%**       |

Each term (Prelim, Midterm, Finals) uses these category weights to compute its term grade from individual task scores. Then the three term grades are combined using the **30-30-40** formula above.

### Two-Layer Computation

```
Layer 1 (per term):
  Term Grade = Σ (category_score × category_weight)
  where category_score = total_earned / total_max × 100

Layer 2 (overall):
  Overall = (Prelim × 0.30) + (Midterm × 0.30) + (Finals × 0.40)
```

## Implementation Status ✅

The `GradeService.CalculateOverallGrade` method now uses the **30-30-40 weighted formula**:

```csharp
// Implemented in GradeService.cs — CalculateOverallGrade()
return (prelim * 0.30m) + (midterm * 0.30m) + (final * 0.40m);
```

This formula is applied in:
- `SubmitGradeAsync()` — when grades are first submitted
- `UpdateGradeAsync()` — when grades are edited
- `RecalculateStudentGradeAsync()` — when auto-computing from task scores

> [!IMPORTANT]
> Both semesters (1st and 2nd) use the same 30-30-40 term weighting structure. Each semester independently has its own Prelim, Midterm, and Finals periods.

## Summary

- **Prelim = 30%**, **Midterm = 30%**, **Finals = 40%** — standard Philippine higher education weighting
- Finals carries more weight because it is cumulative and comprehensive
- Within each term, task categories (Quiz, Long Exam, Performance, Semester Exam) are weighted per the Setup Grading configuration

