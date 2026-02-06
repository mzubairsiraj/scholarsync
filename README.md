# ScholarSync - College Management System

<div align="center">

![ScholarSync Logo](ScholarSync/Resourses/Logo.png)

**A comprehensive college management system for Government College of Management Sciences, Dera Ismail Khan, Pakistan**

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-336791.svg)](https://www.postgresql.org/)
[![C#](https://img.shields.io/badge/C%23-7.3-239120.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

[Features](#features) • [Installation](#installation-guide) • [Database Setup](#database-setup) • [User Guide](#user-guide) • [Documentation](#documentation)

</div>

---

## 📋 Table of Contents

- [Overview](#overview)
- [Latest Updates](#latest-updates)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [System Requirements](#system-requirements)
- [Installation Guide](#installation-guide)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [User Guide](#user-guide)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## 📖 Overview

**ScholarSync** is a modern, full-featured Windows desktop application designed for **Government College of Management Sciences (GCMS), Dera Ismail Khan**. Built with .NET Framework 4.8.1 and PostgreSQL, it streamlines academic operations including student enrollment, teacher management, department administration, result processing, and transcript generation.

### 🎯 Key Highlights

- 🏫 **Complete Academic Management**: Departments, Programs, Subjects, and Semesters
- 👨‍🎓 **Student Lifecycle Management**: Registration, Enrollment, Records, Academic History
- 👨‍🏫 **Teacher Administration**: Profile Management, Department Assignment
- 📊 **Result Processing**: Grade Entry, Real-time GPA Calculation, Transcript Generation
- 🔐 **Role-Based Access Control**: Admin and Teacher roles with specific permissions
- 📈 **Real-Time Analytics**: Instant GPA computation using Gomal University scale
- 📄 **Professional Transcripts**: PDF generation with university branding
- 🎨 **Modern UI**: Full-width responsive layouts, loading indicators, intuitive navigation

---



## 🚀 Features

### 🏫 Academic Management

#### Department Management
- Create, update, and manage academic departments
- Assign Head of Department (HOD)
- Department code and name management
- Soft delete with status tracking (`is_active` flag)
- Full CRUD operations with validation

#### Program Management
- Create academic programs linked to departments
- Configure program duration (1-6 years)
- Program name management (unique constraint)
- Soft delete functionality
- Full-width responsive UI
- Real-time validation and error handling

#### Subject Management
- Subject creation with department mapping
- Credit hours configuration (1-6 credits)
- Subject code and name management
- Delete and update capabilities
- Department-based filtering

#### Semester Management
- Semester creation with date ranges
- Start and end date validation
- Current semester designation
- Academic calendar management
- Update and delete functionality

### 👨‍🎓 Student Management

#### Student Registration
- Comprehensive personal information capture:
  - CNIC, Roll Number, Full Name
  - Date of Birth, Gender, Blood Group
  - Religion, Nationality, Domicile
  - Contact details (Mobile, WhatsApp)
- Guardian information (Father/Mother/Guardian)
- Academic history tracking
- Program enrollment
- Admission tracking with batch and session

#### Student Records
- Comprehensive student listing with DataGridView
- Advanced search and filter capabilities
- Update student information
- View detailed student profiles
- Soft delete with data preservation
- Guardian and admission information display

### 👨‍🏫 Teacher Management

#### Teacher Profiles
- Personal and professional information
- Department assignment
- Designation tracking (Lecturer, Professor, etc.)
- Specialization field
- Contact information management

#### Teacher Operations
- Create new teacher profiles
- Update existing information
- View all teachers with pagination
- Delete teacher records
- Department-based filtering

### 📊 Result Management

#### Result Entry
- Student-wise result entry with search
- Semester-based organization
- Multiple exam types:
  - Mid Term Examination
  - Final Term Examination
  - Sessional Marks
  - Quiz Marks
  - Assignment Marks
- Subject-wise mark entry
- Real-time GPA calculation
- Automatic percentage, grade, and status display
- One result per student per subject per semester

#### Result Viewing
- Comprehensive result listings
- Filter by semester and student
- View detailed subject results
- Export capabilities
- Grade and GPA display

#### Result Updates
- Modify existing results
- Automatic GPA recalculation
- Update or replace functionality
- Validation and error handling

### 📄 Transcript Generation

#### Full Transcript
- All semesters included
- Overall CGPA calculation
- Total credit hours computation
- Subject-wise breakdown
- Professional PDF format
- University branding

#### Semester Transcript
- Single semester focus
- Semester GPA
- Subject-wise results
- Credit hours per subject
- Official university template

### 🔐 Security & Access Control

#### User Authentication
- CNIC-based login (13 digits)
- Secure password authentication
- Role-based authorization
- Session management

#### Role Management
- **Admin**: Full system access
  - All CRUD operations
  - User management
  - System configuration
- **Teacher**: Limited access
  - Result entry and viewing
  - Student information viewing (read-only)
  - Transcript generation
- Feature-level access control

---

## ⚙️ Technology Stack

### Frontend
- **Language**: C# 7.3
- **Framework**: Windows Forms (.NET Framework 4.8.1)
- **UI Components**: 
  - Custom UserControls
  - DataGridView with custom styling
  - Loading indicators with animation
  - Form validation
  - Dynamic layouts

### Backend
- **Database**: PostgreSQL 13+
- **ORM**: ADO.NET with Npgsql 4.1.14
- **Connection**: Neon PostgreSQL (Cloud/Local)
- **Data Access**: Repository Pattern

### Libraries & Dependencies
```xml
<packages>
  <package id="Npgsql" version="4.1.14" targetFramework="net481" />
  <package id="Microsoft.Bcl.AsyncInterfaces" version="1.1.0" targetFramework="net481" />
  <package id="System.Buffers" version="4.5.0" targetFramework="net481" />
  <package id="System.Memory" version="4.5.3" targetFramework="net481" />
  <package id="System.Numerics.Vectors" version="4.5.0" targetFramework="net481" />
  <package id="System.Runtime.CompilerServices.Unsafe" version="4.6.0" targetFramework="net481" />
  <package id="System.Text.Encodings.Web" version="4.6.0" targetFramework="net481" />
  <package id="System.Text.Json" version="4.6.0" targetFramework="net481" />
  <package id="System.Threading.Tasks.Extensions" version="4.5.3" targetFramework="net481" />
  <package id="System.ValueTuple" version="4.5.0" targetFramework="net481" />
</packages>
```

### Development Tools
- **IDE**: Visual Studio 2019/2022
- **Version Control**: Git & GitHub
- **Database Tools**: pgAdmin 4, DBeaver
- **Package Manager**: NuGet
- **Build System**: MSBuild

---

## 💻 System Requirements

### Minimum Requirements

| Component | Requirement |
|-----------|-------------|
| **Operating System** | Windows 7 SP1 or later (64-bit) |
| **Processor** | Intel Core i3 or equivalent |
| **RAM** | 4 GB |
| **Storage** | 500 MB free space |
| **Display** | 1366 x 768 resolution |
| **.NET Framework** | 4.8.1 |
| **Internet** | Required for cloud database |

### Recommended Requirements

| Component | Requirement |
|-----------|-------------|
| **Operating System** | Windows 10/11 (64-bit) |
| **Processor** | Intel Core i5 or better |
| **RAM** | 8 GB or more |
| **Storage** | 1 GB free space |
| **Display** | 1920 x 1080 resolution or higher |
| **.NET Framework** | 4.8.1 |
| **Internet** | Broadband connection |

---

## 📦 Installation Guide

### Prerequisites

#### 1. Install .NET Framework 4.8.1

**Check if already installed:**
```powershell
# Open PowerShell and run:
Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\' | Get-ItemPropertyValue -Name Release | ForEach-Object { $_ -ge 528040 }
```

If returns `False` or command fails:

1. Download .NET Framework 4.8.1 from Microsoft:
   - [.NET Framework 4.8.1 Offline Installer](https://dotnet.microsoft.com/download/dotnet-framework/net481)
   
2. Run the installer
3. Restart your computer if prompted

#### 2. Install Visual Studio (For Development)

**Option 1: Visual Studio Community (Free)**

1. Download from [Visual Studio Downloads](https://visualstudio.microsoft.com/downloads/)
2. During installation, select:
   - ? **.NET desktop development** workload
   - ? **Data storage and processing** (optional, for database tools)

**Option 2: Visual Studio Code (Lightweight)**

1. Download from [VS Code](https://code.visualstudio.com/)
2. Install C# extension
3. Install .NET Framework tools

#### 3. Install Git (For Version Control)

1. Download from [git-scm.com](https://git-scm.com/download/win)
2. Run installer with default settings
3. Verify installation:
   ```bash
   git --version
   ```

#### 4. Install PostgreSQL Client (Optional)

**Option 1: pgAdmin 4** (Recommended)
- Download from [pgAdmin Downloads](https://www.pgadmin.org/download/pgadmin-4-windows/)
- Install with default settings
- Useful for database management and SQL queries

**Option 2: DBeaver Community**
- Download from [DBeaver Downloads](https://dbeaver.io/download/)
- Free, multi-database support
- Lightweight alternative

---

### 🗄️ Database Setup

ScholarSync supports both **cloud** and **local** PostgreSQL databases.

#### Option A: Use Cloud Database (Recommended for Quick Start)

The application can connect to a cloud-hosted PostgreSQL database (Neon).

**No local installation required!**

#### Option B: Set Up Local PostgreSQL Database

##### Step 1: Install PostgreSQL

1. **Download PostgreSQL**
   - Visit [PostgreSQL Downloads](https://www.postgresql.org/download/windows/)
   - Download version 13 or higher

2. **Install PostgreSQL**
   - Run the installer
   - Set a master password (remember this!)
   - Default port: **5432**
   - Default username: **postgres**

3. **Verify Installation**
   ```bash
   psql --version
   # Should show: psql (PostgreSQL) 13.x or higher
   ```

##### Step 2: Create Database

1. **Open pgAdmin 4** or **psql** command line

2. **Create Database**
   ```sql
   CREATE DATABASE scholarsync;
   ```

3. **Connect to Database**
   ```sql
   \c scholarsync
   ```

##### Step 3: Run Database Schema

Execute the following SQL script to create all tables:

```sql
-- 1. Extensions and Enums
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE user_role AS ENUM ('Admin', 'Teacher', 'Student');
CREATE TYPE gender_type AS ENUM ('Male', 'Female', 'Other');
CREATE TYPE guardian_relation AS ENUM ('Father', 'Mother', 'Guardian');
CREATE TYPE admission_status AS ENUM ('Pending', 'Approved', 'Rejected');

-- 2. Core User Management
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cnic TEXT UNIQUE NOT NULL,
    name TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,
    role user_role NOT NULL DEFAULT 'Admin',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 3. Academic Structure
CREATE TABLE departments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT UNIQUE NOT NULL,
    code TEXT UNIQUE NOT NULL,
    head_of_department_id UUID,
    is_active BOOLEAN DEFAULT true
);

CREATE TABLE programs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dept_id UUID REFERENCES departments(id) ON DELETE CASCADE,
    name TEXT UNIQUE NOT NULL,
    duration_years INT NOT NULL,
    is_active BOOLEAN DEFAULT true
);

CREATE TABLE semesters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    is_current BOOLEAN DEFAULT false,
    start_date DATE,
    end_date DATE,
    CONSTRAINT check_dates CHECK (end_date > start_date)
);

CREATE TABLE subjects (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dept_id UUID REFERENCES departments(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    code TEXT NOT NULL
);

-- 4. Profiles
CREATE TABLE teachers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    department_id UUID REFERENCES departments(id),
    designation TEXT,
    specialization TEXT
);

CREATE TABLE students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(id),
    cnic TEXT UNIQUE NOT NULL,
    roll_number TEXT UNIQUE NOT NULL,
    full_name TEXT NOT NULL,
    date_of_birth DATE NOT NULL,
    gender gender_type NOT NULL,
    blood_group TEXT,
    religion TEXT,
    nationality TEXT,
    domicile_district TEXT,
    mobile_no TEXT NOT NULL,
    whatsapp_no TEXT
);

-- 5. Student Support Data
CREATE TABLE guardians (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    relation guardian_relation NOT NULL,
    full_name TEXT NOT NULL,
    cnic TEXT,
    mobile_no TEXT NOT NULL,
    address TEXT,
    is_deceased BOOLEAN DEFAULT false
);

CREATE TABLE academic_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    degree_title TEXT NOT NULL,
    board_university TEXT,
    passing_year INT,
    roll_number TEXT,
    total_marks INT,
    obtained_marks INT,
    grade_division TEXT
);

-- 6. Admissions and Result Management
CREATE TABLE admissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(id),
    session_year INT,
    application_date DATE DEFAULT CURRENT_DATE,
    status admission_status DEFAULT 'Pending',
    form_number TEXT UNIQUE
);

CREATE TABLE enrollments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    subject_id UUID REFERENCES subjects(id) ON DELETE CASCADE,
    semester_id UUID REFERENCES semesters(id) ON DELETE SET NULL,
    UNIQUE(student_id, subject_id, semester_id) 
);

CREATE TABLE results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    enrollment_id UUID UNIQUE REFERENCES enrollments(id) ON DELETE CASCADE,
    total_marks INT DEFAULT 100,
    obtained_marks DECIMAL(5,2),
    gpa DECIMAL(3,2) CHECK (gpa >= 0 AND gpa <= 4.0),
    grade_letter VARCHAR(5),
    remarks TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
```

##### Step 4: Create Default Admin User

```sql
-- Create default admin user
INSERT INTO users (cnic, name, email, password, role)
VALUES (
    '1234567890123',
    'System Administrator',
    'admin@gcms.edu.pk',
    'admin123',  -- ⚠️ Change this password immediately!
    'Admin'
);
```

⚠️ **Security Warning**: Change the default password after first login!

##### Step 5: Update Connection String

Open `ScholarSync\Commons\Constants.cs` and update:

```csharp
public static string ConnectionString { get; } = 
    "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=scholarsync";
```

Replace `YOUR_PASSWORD` with your PostgreSQL password.

---

### 📥 Application Setup

#### Method 1: Clone from GitHub (Recommended)

```bash
# 1. Open Command Prompt or PowerShell
cd C:\Projects  # or your preferred location

# 2. Clone the repository
git clone https://github.com/mzubairsiraj/scholarsync.git

# 3. Navigate to project directory
cd scholarsync

# 4. Open solution in Visual Studio
start ScholarSync.sln
```

#### Method 2: Download ZIP

1. Visit [GitHub Repository](https://github.com/mzubairsiraj/scholarsync)
2. Click **Code** ? **Download ZIP**
3. Extract to desired location (e.g., `C:\Projects\ScholarSync`)
4. Open `ScholarSync.sln` in Visual Studio

#### Building the Application

##### In Visual Studio:

1. **Restore NuGet Packages**
   - Right-click solution in Solution Explorer
   - Select **Restore NuGet Packages**
   
   Or via Package Manager Console:
   ```powershell
   Update-Package -reinstall
   ```

2. **Build Solution**
   - Press `Ctrl + Shift + B` or
   - Menu: `Build ? Build Solution`
   - Wait for "Build succeeded" message

3. **Verify Build**
   - Check Output window (View ? Output)
   - Should show: "========== Build: 1 succeeded, 0 failed =========="
   - Ensure 0 errors, 0 warnings

##### Via Command Line (Advanced):

```bash
# Navigate to solution directory
cd "C:\Projects\ScholarSync"

# Restore NuGet packages
nuget restore ScholarSync.sln

# Build using MSBuild
msbuild ScholarSync.sln /p:Configuration=Release /p:Platform="Any CPU"
```

#### Running the Application

##### From Visual Studio:

1. Press `F5` (Debug mode) or `Ctrl + F5` (Release mode)
2. Or click **Start** button (green arrow) in toolbar
3. Application window should launch

##### From Executable:

Navigate to:
```
ScholarSync\ScholarSync\bin\Debug\ScholarSync.exe
```
or
```
ScholarSync\ScholarSync\bin\Release\ScholarSync.exe
```

Double-click `ScholarSync.exe` to run.

#### Creating Desktop Shortcut

```bash
# Right-click ScholarSync.exe
# Select "Send to" ? "Desktop (create shortcut)"
# Or drag and drop while holding Alt key
```

---

## 🎯 Configuration

### Database Configuration

**File**: `ScholarSync\Commons\Constants.cs`

```csharp
public static string ConnectionString { get; } = 
    "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=scholarsync";
```

**Connection String Parameters:**

| Parameter | Description | Example |
|-----------|-------------|---------|
| `Host` | Database server address | `localhost` or `your-server.com` |
| `Port` | PostgreSQL port | `5432` (default) |
| `Username` | Database username | `postgres` |
| `Password` | Database password | `your_password` |
| `Database` | Database name | `scholarsync` |
| `SSL Mode` | SSL connection (cloud only) | `Require` |

**Cloud Database Example** (Neon):
```csharp
public static string ConnectionString { get; } = 
    "Host=ep-super-tree-a8g0f75w-pooler.eastus2.azure.neon.tech;" +
    "Port=5432;" +
    "Username=neondb_owner;" +
    "Password=npg_peLsj5fhm7aq;" +
    "Database=neondb;" +
    "SSL Mode=Require;";
```

**Local Database Example**:
```csharp
public static string ConnectionString { get; } = 
    "Host=localhost;" +
    "Port=5432;" +
    "Username=postgres;" +
    "Password=zubair@1326;" +
    "Database=scholarsync";
```

### UI Configuration

**File**: `ScholarSync\Commons\Constants.cs`

```csharp
// Theme Colors
public static Color SSWhiteColor { get; } = ColorTranslator.FromHtml("#E7F0FA");
public static Color SSLightNavyColor { get; } = ColorTranslator.FromHtml("#7BA4D0");
public static Color SSDarkBlueColor { get; } = ColorTranslator.FromHtml("#2E5E99");
public static Color SSDarkNavyColor { get; } = ColorTranslator.FromHtml("#0D2440");

// UI Colors
public static Color SSLightGrayBackground { get; } = Color.FromArgb(245, 245, 245);
public static Color SSBorderGray { get; } = Color.FromArgb(224, 224, 224);

// Status Colors
public static Color SSSuccessGreen { get; } = Color.Green;
public static Color SSErrorRed { get; } = Color.OrangeRed;
public static Color SSWarningOrange { get; } = Color.Orange;

// Screen Dimensions (Auto-detected)
public static int ScreenWidth { get; } = Screen.PrimaryScreen.Bounds.Width;
public static int ScreenHeight { get; } = Screen.PrimaryScreen.Bounds.Height;
```

### College Information

**File**: `ScholarSync\Forms\WelcomeForm.cs`

```csharp
String collegeName = "Government College Of Management Sciences \n No-1 D-I-Khan";
```

**To customize:**
1. Open `WelcomeForm.cs`
2. Find the `collegeName` variable
3. Change to your institution's name
4. Rebuild application

### Logo and Branding

**Location**: `ScholarSync\Resourses\`

**Files to replace:**
- `Logo.png` - Main application logo (150x150px recommended)
- `ScholarSync.ico` - Application icon (256x256px recommended)

**Steps:**
1. Replace image files in `Resourses` folder
2. Keep same filenames
3. Rebuild application
4. Logo appears on Welcome screen and Dashboard

---

## 📖 User Guide

### First-Time Setup

#### Default Admin Login

**After database setup, use these credentials:**

- **User Type**: Admin
- **CNIC**: `1234567890123`
- **Password**: `admin123`

🔐 **IMPORTANT**: Change the default password immediately!

#### Creating Additional Users

Currently, users must be created directly in the database:

**Create Admin User:**
```sql
INSERT INTO users (cnic, name, email, password, role)
VALUES (
    '9876543210987',      -- 13-digit CNIC
    'John Doe',           -- Full name
    'john@gcms.edu.pk',   -- Email
    'secure_password',    -- Password (plain text - consider hashing)
    'Admin'               -- Role: Admin or Teacher
);
```

**Create Teacher User:**
```sql
-- Step 1: Create user account
INSERT INTO users (cnic, name, email, password, role)
VALUES (
    '1112223334445',
    'Jane Smith',
    'jane@gcms.edu.pk',
    'teacher123',
    'Teacher'
)
RETURNING id;  -- Note this ID

-- Step 2: Create teacher profile
INSERT INTO teachers (user_id, department_id, designation, specialization)
VALUES (
    'USER_ID_FROM_STEP_1',  -- UUID from previous query
    'DEPARTMENT_UUID',       -- Get from departments table
    'Assistant Professor',
    'Computer Science'
);
```

---

### 👨‍💼 Admin Workflow

#### Initial Academic Setup

**Recommended Order:**

1. **Create Departments** ? 2. **Create Programs** ? 3. **Create Subjects** ? 4. **Create Semesters**

---

#### 1️⃣ Department Management

**Navigate**: Dashboard ? Academic ? Manage Departments

##### Create Department
```
1. Click "Manage Departments"
2. Fill in:
   - Department Name (e.g., "Computer Science")
   - Department Code (e.g., "CS")
3. Click "Add Department"
4. Success message appears
5. Department added to grid
```

##### Update Department
```
1. Click on department row in grid
2. Form populates with data
3. Modify name/code
4. Click "Update Department"
5. Changes saved
```

##### Delete Department
```
1. Click on department row
2. Click "Delete Department"
3. Confirm deletion
4. Department status set to inactive
```

---

#### 2️⃣ Program Management

**Navigate**: Dashboard ? Academic ? Manage Programs

##### Create Program
```
1. Click "Manage Programs"
2. Full-width form appears at top
3. Fill in:
   - Program Name (e.g., "BS Computer Science")
   - Select Department
   - Duration (1-6 years, default: 4)
4. Click "Add Program"
5. DataGrid below shows all programs
```

**UI Layout:**
```
???????????????????????????????????????????????????????
? Program Name: [___________________________]         ?
? Department: [CS ?]  Duration: [4 years ?]          ?
? [Add] [Update] [Delete] [Clear]                     ?
???????????????????????????????????????????????????????
???????????????????????????????????????????????????????
? Program Name      ? Department   ? Duration         ?
? BS Computer Sci   ? CS           ? 4 years          ?
? BS Software Eng   ? CS           ? 4 years          ?
???????????????????????????????????????????????????????
```

##### Update Program
```
1. Click on program row in DataGrid
2. Form populates with data
3. Modify fields
4. Click "Update Program"
5. Changes reflected in grid
```

##### Delete Program (Soft Delete)
```
1. Click on program row
2. Click "Delete Program"
3. Confirm action
4. Program marked inactive (is_active = false)
5. No longer appears in listings
```

---

#### 3️⃣ Subject Management

**Navigate**: Dashboard ? Academic ? Manage Subjects

##### Create Subject
```
1. Fill in:
   - Subject Name (e.g., "Data Structures")
   - Subject Code (e.g., "CS-201")
   - Select Department
2. Click "Add Subject"
```

##### Update/Delete Subject
```
Similar to departments
```

---

#### 4️⃣ Semester Management

**Navigate**: Dashboard ? Academic ? Manage Semesters

##### Create Semester
```
1. Fill in:
   - Semester Name (e.g., "Spring 2024", "1st Semester")
   - Start Date (calendar picker)
   - End Date (calendar picker)
   - Set as Current (checkbox)
2. Click "Add Semester"
3. Validation: End date must be after start date
```

**Note**: Only one semester can be marked as "current" at a time.

---

#### 5️⃣ Teacher Management

##### Create Teacher

**Navigate**: Dashboard ? Teacher ? Create Teacher

```
1. Fill in personal information:
   - CNIC (13 digits, no dashes)
   - Full Name
   - Email
   - Password
2. Fill in professional information:
   - Select Department
   - Designation
   - Specialization
3. Click "Save Teacher"
4. User account created automatically
5. Teacher profile linked to user
```

##### View/Update Teachers

**Navigate**: Dashboard ? Teacher ? List Teachers

```
- View all teachers in DataGridView
- Search by name, CNIC, department
- Click row to view details
- Update information as needed
```

##### Delete Teacher

```
1. Select teacher from list
2. Click "Delete Teacher"
3. Confirm deletion
4. Teacher and associated user account removed
```

---

#### 6️⃣ Student Management

##### Register Student

**Navigate**: Dashboard ? Student ? Register Student

```
**Personal Information:**
1. CNIC (13 digits)
2. Roll Number (unique)
3. Full Name
4. Date of Birth
5. Gender (dropdown)
6. Blood Group
7. Religion, Nationality
8. Domicile District
9. Mobile Number
10. WhatsApp Number

**Guardian Information:**
11. Relation (Father/Mother/Guardian)
12. Guardian Name
13. Guardian CNIC
14. Guardian Mobile
15. Guardian Address
16. Is Deceased (checkbox)

**Academic Information:**
17. Select Program
18. Admission Session/Batch

**Submit:**
19. Click "Register Student"
20. User account created
21. Student profile created
22. Guardian record created
```

##### Update Student

**Navigate**: Dashboard ? Student ? Update Student

```
1. Search student by:
   - CNIC
   - Roll Number
   - Name
2. Select student
3. Modify information
4. Click "Update Student"
5. Changes saved
```

##### View Students

**Navigate**: Dashboard ? Student ? List Students

```
- Comprehensive list in DataGridView
- Columns: Roll No, Name, CNIC, Program, Department
- Search and filter
- Click for details
```

##### Delete Student

**Navigate**: Dashboard ? Student ? Delete Student

```
1. Search and select student
2. Click "Delete Student"
3. Confirm deletion
4. Student record removed (cascade: guardian, admissions, enrollments)
```

---

### 👨‍🏫 Teacher Workflow

Teachers have limited access compared to Admin.

#### 1. View Students (Read-Only)

**Navigate**: Dashboard ? Student ? List Students

```
- View student information
- Cannot modify or delete
- Search and filter available
```

#### 2. Enter Results

**Navigate**: Dashboard ? Result ? Add Result

```
**Step 1: Select Student**
1. Search by:
   - Roll Number (recommended)
   - CNIC
   - Name
2. Select student from dropdown

**Step 2: Select Parameters**
3. Select Semester (current semester default)
4. Select Exam Type:
   - Mid Term
   - Final Term
   - Sessional
   - Quiz
   - Assignment
5. Select Subject

**Step 3: Enter Marks**
6. Total Marks: 100 (default, can change)
7. Obtained Marks: [Enter value]

**Step 4: View Auto-Calculations**
8. Percentage: Calculated automatically
9. GPA: Calculated using Gomal University scale
10. Grade: Letter grade (A+, A, B+, etc.)
11. Status: Pass/Fail

**Step 5: Save**
12. Click "Save Result"
13. Result stored in database
14. Success message displayed
```

**Real-Time Calculation Example:**
```
Total Marks: 100
Obtained Marks: 76

Automatically shows:
? Percentage: 76.00%
? GPA: 3.60
? Grade: A
? Status: Pass
```

**GPA Calculation (Gomal University Scale):**

| Marks % | Grade | GPA Range | Example (76%) |
|---------|-------|-----------|---------------|
| 80-100  | A+    | 4.00      | -             |
| 75-79   | A     | 3.50-3.99 | **3.60** ?    |
| 70-74   | B+    | 3.00-3.49 | -             |
| 65-69   | B     | 2.50-2.99 | -             |
| 60-64   | C     | 2.00-2.49 | -             |
| 55-59   | D+    | 1.50-1.99 | -             |
| 50-54   | D     | 1.00-1.49 | -             |
| 0-49    | F     | 0.00      | -             |

**Formula**: GPA is calculated proportionally within each range.

For 76%: `GPA = 3.50 + ((76 - 75) / (79 - 75)) * (3.99 - 3.50) = 3.6025 ? 3.60`

#### 3. View Results

**Navigate**: Dashboard ? Result ? View Results

```
1. Filter by semester
2. Filter by student
3. View subject-wise results
4. See GPA, grade, percentage
5. Export if needed
```

#### 4. Generate Transcripts

**Navigate**: Dashboard ? Result ? Generate Transcript

```
**Step 1: Select Student**
1. Search by roll number, CNIC, or name
2. Select student

**Step 2: Choose Transcript Type**

**Option A: Full Transcript**
- Includes all semesters
- Overall CGPA
- Total credit hours
- Complete academic record

**Option B: Semester Transcript**
- Single semester
- Semester GPA
- Subject-wise breakdown

**Step 3: Generate**
3. Click "Generate Transcript"
4. PDF created with:
   - University header
   - Student details
   - Semester-wise results
   - Subject marks, GPA, grades
   - CGPA calculation
   - Grade legend
5. Save PDF to desired location
```

**Transcript Includes:**
- Student Information (Name, Roll No, Program)
- Semester Details
- Subject Code, Name, Credit Hours
- Obtained Marks, Total Marks
- GPA per subject
- Semester GPA / CGPA
- Grade Letter
- University seal/branding

---

## 📁 Project Structure

```
ScholarSync/
?
??? ScholarSync/                          # Main project directory
?   ?
?   ??? Commons/                          # Shared utilities and constants
?   ?   ??? Constants.cs                 # Configuration (DB, UI colors, screen size)
?   ?   ??? SessionManager.cs            # User session management
?   ?   ??? UIHelper.cs                  # UI component factory methods
?   ?   ??? GPACalculator.cs             # Gomal University GPA calculation engine
?   ?
?   ??? Controls/                         # Reusable UserControls (UI components)
?   ?   ?
?   ?   ??? Academic/                     # Academic management screens
?   ?   ?   ??? ManageDepartmentsControl.cs
?   ?   ?   ??? ManageProgramsControl.cs  # ? Recently updated (full-width UI)
?   ?   ?   ??? ManageSubjectsControl.cs
?   ?   ?   ??? ManageSemestersControl.cs
?   ?   ?
?   ?   ??? Student/                      # Student management screens
?   ?   ?   ??? RegisterStudentControl.cs
?   ?   ?   ??? ListStudentsControl.cs
?   ?   ?   ??? UpdateStudentControl.cs
?   ?   ?   ??? DeleteStudentControl.cs
?   ?   ?
?   ?   ??? Teacher/                      # Teacher management screens
?   ?   ?   ??? CreateTeacherControl.cs
?   ?   ?   ??? ListTeachersControl.cs
?   ?   ?   ??? UpdateTeacherControl.cs
?   ?   ?   ??? DeleteTeacherControl.cs
?   ?   ?
?   ?   ??? Result/                       # Result management screens
?   ?   ?   ??? AddResultControl.cs
?   ?   ?   ??? ListResultsControl.cs
?   ?   ?   ??? UpdateResultControl.cs
?   ?   ?   ??? GenerateTranscriptControl.cs
?   ?   ?
?   ?   ??? Home/                         # Dashboard
?   ?   ?   ??? HomeDashboardControl.cs
?   ?   ?
?   ?   ??? LoadingIndicator.cs           # ? Recently updated (fixed visibility)
?   ?
?   ??? Db Connection/                    # Database layer
?   ?   ??? DbConnector.cs                # PostgreSQL connection manager
?   ?
?   ??? Forms/                            # Windows Forms
?   ?   ??? WelcomeForm.cs                # ? Recently updated (no forgot password)
?   ?   ??? DashBoardForm.cs              # Main dashboard with sidebar navigation
?   ?   ??? ResultSelectionForm.cs        # Dialog for result selection
?   ?   ??? ResultDetailsForm.cs          # Result details viewer
?   ?   ??? ViewSubjectResultsForm.cs     # Subject results viewer
?   ?
?   ??? Models/                           # Data models (POCOs)
?   ?   ??? UserModel.cs
?   ?   ??? StudentModel.cs
?   ?   ??? StudentViewModel.cs
?   ?   ??? TeacherModel.cs
?   ?   ??? TeacherViewModel.cs
?   ?   ??? DepartmentModel.cs
?   ?   ??? ProgramModel.cs               # ? Recently updated (no Code field)
?   ?   ??? SubjectModel.cs
?   ?   ??? SemesterModel.cs
?   ?   ??? EnrollmentModel.cs
?   ?   ??? EnrollmentViewModel.cs
?   ?   ??? ResultModel.cs
?   ?   ??? TranscriptModel.cs
?   ?   ??? GuardianModel.cs
?   ?   ??? AdmissionModel.cs
?   ?   ??? AcademicHistoryModel.cs
?   ?
?   ??? Repositories/                     # Data access layer (Repository Pattern)
?   ?   ??? StudentRepository.cs
?   ?   ??? TeacherRepository.cs
?   ?   ??? DepartmentRepository.cs
?   ?   ??? ProgramRepository.cs          # ? Recently updated (aligned with DB schema)
?   ?   ??? SubjectRepository.cs
?   ?   ??? SemesterRepository.cs
?   ?   ??? EnrollmentRepository.cs
?   ?   ??? TranscriptRepository.cs
?   ?
?   ??? Services/                         # Business logic services
?   ?   ??? TranscriptPdfService.cs       # PDF generation for transcripts
?   ?
?   ??? Resourses/                        # Application resources
?   ?   ??? Logo.png                      # Application logo (150x150)
?   ?   ??? [Other assets]
?   ?
?   ??? Properties/                       # Project metadata
?   ?   ??? AssemblyInfo.cs
?   ?   ??? Resources.Designer.cs
?   ?   ??? Settings.Designer.cs
?   ?
?   ??? App.config                        # Application configuration
?   ??? packages.config                   # NuGet package dependencies
?   ??? Program.cs                        # Application entry point (Main method)
?   ??? ScholarSync.csproj                # Visual Studio project file
?
??? Documentation/                         # Project documentation (Markdown files)
?   ??? GOMAL_UNIVERSITY_GPA_IMPLEMENTATION.md
?   ??? GPA_UPDATE_SUMMARY.md
?   ??? GPA_QUICK_REFERENCE.md
?   ??? FIX_DUPLICATE_ENROLLMENT_ERROR.md
?   ??? FIX_DUPLICATE_RESULT_ERROR.md
?   ??? MANAGE_SEMESTERS_UPDATE_GUIDE.md
?   ??? MANAGE_PROGRAMS_UI_UPDATES.md      # ? New
?   ??? LOADING_INDICATOR_FIX.md           # ? New
?   ??? WELCOME_FORM_UPDATES.md            # ? New
?
??? .git/                                  # Git version control
??? .gitignore                             # Git ignore rules
??? README.md                              # This file
??? LICENSE                                # MIT License
```

### Key Components Explained

#### Commons/
**Shared utilities used across the application**
- `Constants.cs`: Database connection string, UI colors, screen dimensions
- `SessionManager.cs`: Manages logged-in user session
- `UIHelper.cs`: Factory methods for creating styled buttons
- `GPACalculator.cs`: Gomal University GPA calculation logic

#### Controls/
**Reusable UserControls for different features**
- Organized by module (Academic, Student, Teacher, Result)
- Each control is a self-contained UI component
- Can be loaded into main content panel

#### Db Connection/
**Database connectivity layer**
- `DbConnector.cs`: Provides PostgreSQL connection instances
- Uses Npgsql library
- Connection pooling enabled

#### Forms/
**Top-level application windows**
- `WelcomeForm.cs`: Login screen
- `DashBoardForm.cs`: Main application window with sidebar

#### Models/
**Data transfer objects (DTOs)**
- Plain C# classes (POCOs)
- Represent database entities
- ViewModels for complex queries with joins

#### Repositories/
**Data access layer (DAL)**
- Each repository handles one entity
- CRUD operations
- Uses ADO.NET with Npgsql
- SQL queries with parameterized inputs

#### Services/
**Business logic services**
- `TranscriptPdfService.cs`: Generates PDF transcripts
- Can be extended for other services

---

## 🗃️ Database Schema

### Complete SQL Schema

```sql
-- ================================================
-- ScholarSync Database Schema
-- PostgreSQL 13+
-- ================================================

-- 1. Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 2. Custom Types (Enums)
CREATE TYPE user_role AS ENUM ('Admin', 'Teacher', 'Student');
CREATE TYPE gender_type AS ENUM ('Male', 'Female', 'Other');
CREATE TYPE guardian_relation AS ENUM ('Father', 'Mother', 'Guardian');
CREATE TYPE admission_status AS ENUM ('Pending', 'Approved', 'Rejected');

-- 3. Core Tables

-- Users Table (Authentication & Authorization)
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cnic TEXT UNIQUE NOT NULL,                      -- 13-digit CNIC
    name TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,                          -- ⚠️ Plain text (consider hashing)
    role user_role NOT NULL DEFAULT 'Admin',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Departments Table
CREATE TABLE departments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT UNIQUE NOT NULL,
    code TEXT UNIQUE NOT NULL,
    head_of_department_id UUID,                      -- References teacher id
    is_active BOOLEAN DEFAULT true                   -- Soft delete flag
);

-- Programs Table (Degree Programs)
CREATE TABLE programs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dept_id UUID REFERENCES departments(id) ON DELETE CASCADE,
    name TEXT UNIQUE NOT NULL,
    duration_years INT NOT NULL,                     -- 1-6 years
    is_active BOOLEAN DEFAULT true                   -- Soft delete flag
);

-- Semesters Table
CREATE TABLE semesters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,                              -- e.g., '1st Semester', 'Spring 2024'
    is_current BOOLEAN DEFAULT false,                -- Only one can be current
    start_date DATE,
    end_date DATE,
    CONSTRAINT check_dates CHECK (end_date > start_date)
);

-- Subjects Table (Courses)
CREATE TABLE subjects (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dept_id UUID REFERENCES departments(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    code TEXT NOT NULL
);

-- Teachers Table (Faculty Profiles)
CREATE TABLE teachers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    department_id UUID REFERENCES departments(id),
    designation TEXT,                                -- e.g., Lecturer, Professor
    specialization TEXT
);

-- Students Table (Student Profiles)
CREATE TABLE students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(id),
    cnic TEXT UNIQUE NOT NULL,
    roll_number TEXT UNIQUE NOT NULL,
    full_name TEXT NOT NULL,
    date_of_birth DATE NOT NULL,
    gender gender_type NOT NULL,
    blood_group TEXT,
    religion TEXT,
    nationality TEXT,
    domicile_district TEXT,
    mobile_no TEXT NOT NULL,
    whatsapp_no TEXT
);

-- Guardians Table (Parent/Guardian Information)
CREATE TABLE guardians (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    relation guardian_relation NOT NULL,
    full_name TEXT NOT NULL,
    cnic TEXT,
    mobile_no TEXT NOT NULL,
    address TEXT,
    is_deceased BOOLEAN DEFAULT false
);

-- Academic History Table (Previous Education)
CREATE TABLE academic_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    degree_title TEXT NOT NULL,
    board_university TEXT,
    passing_year INT,
    roll_number TEXT,
    total_marks INT,
    obtained_marks INT,
    grade_division TEXT
);

-- Admissions Table (Student Admissions)
CREATE TABLE admissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(id),
    session_year INT,
    application_date DATE DEFAULT CURRENT_DATE,
    status admission_status DEFAULT 'Pending',
    form_number TEXT UNIQUE
);

-- Enrollments Table (Student-Subject Registration)
CREATE TABLE enrollments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id) ON DELETE CASCADE,
    subject_id UUID REFERENCES subjects(id) ON DELETE CASCADE,
    semester_id UUID REFERENCES semesters(id) ON DELETE SET NULL,
    UNIQUE(student_id, subject_id, semester_id)      -- One enrollment per student-subject-semester
);

-- Results Table (Exam Results & Grades)
CREATE TABLE results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    enrollment_id UUID UNIQUE REFERENCES enrollments(id) ON DELETE CASCADE,
    total_marks INT DEFAULT 100,
    obtained_marks DECIMAL(5,2),
    gpa DECIMAL(3,2) CHECK (gpa >= 0 AND gpa <= 4.0),
    grade_letter VARCHAR(5),                          -- A+, A, B+, etc.
    remarks TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 4. Indexes (for performance)
CREATE INDEX idx_users_cnic ON users(cnic);
CREATE INDEX idx_students_roll_number ON students(roll_number);
CREATE INDEX idx_enrollments_student ON enrollments(student_id);
CREATE INDEX idx_results_enrollment ON results(enrollment_id);

-- 5. Sample Data (for testing)

-- Insert default admin user
INSERT INTO users (cnic, name, email, password, role)
VALUES (
    '1234567890123',
    'System Administrator',
    'admin@gcms.edu.pk',
    'admin123',  -- ⚠️ Change this!
    'Admin'
);

-- Insert sample department
INSERT INTO departments (name, code, is_active)
VALUES ('Computer Science', 'CS', true);
```

### Entity Relationships

```
users (1) ??? (1) teachers
users (1) ??? (1) students

departments (1) ??? (*) programs
departments (1) ??? (*) subjects
departments (1) ??? (*) teachers

programs (1) ??? (*) students
programs (1) ??? (*) admissions

students (1) ??? (*) guardians
students (1) ??? (*) academic_history
students (1) ??? (*) admissions
students (1) ??? (*) enrollments

subjects (1) ??? (*) enrollments

semesters (1) ??? (*) enrollments

enrollments (1) ??? (1) results
```

### Key Constraints

1. **Unique Constraints:**
   - `users.cnic`, `users.email`
   - `departments.name`, `departments.code`
   - `programs.name`
   - `students.cnic`, `students.roll_number`
   - `enrollments (student_id, subject_id, semester_id)` composite unique

2. **Check Constraints:**
   - `semesters.end_date > start_date`
   - `results.gpa` between 0 and 4.0

3. **Cascade Deletes:**
   - Deleting user ? deletes teacher/student
   - Deleting department ? deletes programs, subjects
   - Deleting student ? deletes guardians, admissions, enrollments
   - Deleting enrollment ? deletes result

---

## 🔧 Troubleshooting

### Common Issues & Solutions

#### 1. Database Connection Errors

**Error**: `Could not connect to database`

**Solutions:**
```
✅ Check internet connection (if using cloud database)
✅ Verify connection string in Constants.cs
✅ Ensure PostgreSQL service is running (local database)
✅ Check firewall settings
✅ Verify database exists: SELECT datname FROM pg_database;
✅ Test connection using pgAdmin or DBeaver
```

**Check Connection String:**
```csharp
// Open Constants.cs
// Verify format:
"Host=YOUR_HOST;Port=5432;Username=YOUR_USER;Password=YOUR_PASS;Database=scholarsync"
```

---

#### 2. NuGet Package Errors

**Error**: `Could not load file or assembly 'Npgsql'`

**Solution:**
```powershell
# In Visual Studio Package Manager Console:
Update-Package Npgsql -reinstall

# Or for all packages:
Update-Package -reinstall
```

**Manual Fix:**
```
1. Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution
2. Find Npgsql
3. Uninstall
4. Install version 4.1.14
5. Rebuild solution
```

---

#### 3. .NET Framework Not Found

**Error**: `.NET Framework 4.8.1 not found`

**Solution:**
```
1. Download from: https://dotnet.microsoft.com/download/dotnet-framework/net481
2. Install .NET Framework 4.8.1
3. Restart computer
4. Rebuild application
```

---

#### 4. Login Issues

**Error**: `Invalid CNIC or Password`

**Checks:**
```sql
-- Verify user exists in database
SELECT * FROM users WHERE cnic = '1234567890123';

-- Check password (plain text, case-sensitive)
-- CNIC should be exactly 13 digits with no dashes
```

**Common Mistakes:**
```
❌ CNIC with dashes: 12345-6789012-3
✅ CNIC without dashes: 1234567890123

❌ Wrong role selected (Admin vs Teacher)
✅ Match role in database

❌ Leading/trailing spaces in password
✅ Exact password match
```

---

#### 5. Loading Indicator Not Showing

**If loader disappears immediately:**

**Check:**
```csharp
// LoadingIndicator.cs should have:
public void Show(string message = "Loading")
{
    // ... existing code ...
    this.Refresh();          // Must be here!
    Application.DoEvents();
}

// WelcomeForm.cs AuthenticateUser should have:
loadingIndicator.Show("Authenticating");
System.Threading.Thread.Sleep(100);  // Brief delay
```

**Solution:**
```
If missing, add:
1. Refresh() call in LoadingIndicator.Show()
2. 100ms delay after Show() in fast operations
```

---

#### 6. Build Errors

**Error**: `Build failed with multiple errors`

**Solutions:**
```
1. Clean Solution:
   Build ? Clean Solution

2. Rebuild Solution:
   Build ? Rebuild Solution

3. Restore NuGet Packages:
   Tools ? NuGet ? Restore Packages

4. Check Output window for specific errors

5. Common fixes:
   - Delete bin/ and obj/ folders
   - Close and reopen Visual Studio
   - Update NuGet packages
```

---

#### 7. PDF Generation Errors

**Error**: `Could not generate transcript PDF`

**Checks:**
```
✅ System.Drawing library available
✅ Write permissions on save location
✅ Sufficient disk space
✅ Student has results to display
```

---

#### 8. DataGrid Not Populating

**Error**: `Grid shows no data`

**Solutions:**
```sql
-- Check if data exists in database
SELECT * FROM programs WHERE is_active = true;
SELECT * FROM students;

-- Verify repository queries return data
-- Check for null results
```

**In Application:**
```
1. Check loading indicator hides after loading
2. Verify no exceptions in database queries
3. Ensure DataSource is set: dgv.DataSource = list;
4. Check AutoGenerateColumns = false and columns defined
```

---

#### 9. Form Layout Issues

**Issue**: Controls overlapping or cut off

**Solutions:**
```
1. Check screen resolution matches ConfigurationConstants
2. Verify ScreenWidth and ScreenHeight are correct
3. Form should maximize: this.WindowState = FormWindowState.Maximized
4. Controls should use responsive sizing:
   - controlWidth = formPanel.Width - xControlPos - 40;
```

---

#### 10. Duplicate Entry Errors

**Error**: `Duplicate key value violates unique constraint`

**Common Scenarios:**
```
❌ Same CNIC used for multiple users
❌ Same roll number for students
❌ Department/program name already exists
❌ Enrollment already exists for student-subject-semester

✅ Solutions:
- Check for existing record before insert
- Use UPDATE instead of INSERT if record exists
- Implement better validation
```

---

### Getting Help

If issues persist:

1. **Check Documentation**
   - Review relevant .md files in Documentation/ folder
   - Check this README thoroughly

2. **Database Logs**
   ```sql
   -- Check PostgreSQL logs
   SELECT * FROM pg_stat_activity;
   ```

3. **Application Logs**
   - Check Output window in Visual Studio (View ? Output)
   - Look for exception stack traces

4. **Contact Developer**
   - Email: mzubairsiraj@gmail.com
   - GitHub Issues: https://github.com/mzubairsiraj/scholarsync/issues

5. **Provide Information**
   ```
   - Error message (full text)
   - Stack trace (if available)
   - Steps to reproduce
   - Screenshots
   - Database connection type (local/cloud)
   - Windows version
   - Visual Studio version
   ```

---

## 🤝 Contributing

We welcome contributions to ScholarSync!

### How to Contribute

1. **Fork the Repository**
   ```bash
   # On GitHub, click "Fork" button
   git clone https://github.com/YOUR_USERNAME/scholarsync.git
   ```

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b bugfix/issue-description
   ```

3. **Make Changes**
   - Follow existing code style
   - Add comments for complex logic
   - Test thoroughly

4. **Commit Changes**
   ```bash
   git add .
   git commit -m "Add feature: your feature description"
   ```

5. **Push to Your Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request**
   - Go to original repository on GitHub
   - Click "New Pull Request"
   - Select your branch
   - Describe changes clearly

### Contribution Guidelines

#### Code Style
```csharp
// Use meaningful names
private void LoadStudents() { }  // ✅ Good
private void LS() { }            // ❌ Bad

// Add XML comments for public methods
/// <summary>
/// Loads all active students from database
/// </summary>
public List<StudentModel> GetStudents() { }

// Use try-catch for database operations
try
{
    // Database code
}
catch (Exception ex)
{
    MessageBox.Show($"Error: {ex.Message}");
}
```

#### Commit Messages
```
✅ Good:
- "Add program management full-width UI"
- "Fix loading indicator visibility issue"
- "Update database schema documentation"

❌ Bad:
- "Update"
- "Fix stuff"
- "Changes"
```

#### Testing
```
Before submitting PR:
✅ Build succeeds with no errors
✅ Test on clean database
✅ Test all CRUD operations
✅ Verify no regressions
✅ Update documentation if needed
```

### Areas for Contribution

**High Priority:**
- Password hashing/encryption
- User management UI (create/update/delete users)
- Advanced search and filtering
- Report generation (attendance, performance)
- Bulk import/export features

**Medium Priority:**
- Email notifications
- SMS integration
- Dashboard analytics/charts
- Backup and restore features
- User activity logging

**Low Priority:**
- Dark mode theme
- Multi-language support
- Mobile app companion
- API for integrations

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2024 Muhammad Zubair Siraj

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 📧 Contact

### Lead Developer

**Muhammad Zubair Siraj**

- ?? **Email**: mzubairsiraj@gmail.com
- ?? **GitHub**: [@mzubairsiraj](https://github.com/mzubairsiraj)
- ?? **LinkedIn**: [Muhammad Zubair Siraj](https://linkedin.com/in/mzubairsiraj)
- ?? **Portfolio**: [mzubairsiraj.com](https://mzubairsiraj.com) _(if available)_

### Institution

**Government College of Management Sciences**  
Dera Ismail Khan, Khyber Pakhtunkhwa, Pakistan

### Project Links

- ?? **Repository**: [https://github.com/mzubairsiraj/scholarsync](https://github.com/mzubairsiraj/scholarsync)
- ?? **Documentation**: [GitHub Wiki](https://github.com/mzubairsiraj/scholarsync/wiki) _(if available)_
- ?? **Issues**: [GitHub Issues](https://github.com/mzubairsiraj/scholarsync/issues)
- ?? **Project Board**: [GitHub Projects](https://github.com/mzubairsiraj/scholarsync/projects) _(if available)_

---

<div align="center">

**Made with ?? for Government College of Management Sciences**

**Empowering Education. Simplifying Management.**

---

### Quick Links

[⬆ Back to Top](#scholarsync---college-management-system) | [Features](#features) | [Installation](#installation-guide) | [User Guide](#user-guide) | [Contributing](#contributing)

---

**Last Updated**: December 2024 | **Version**: 1.5.0

</div>
