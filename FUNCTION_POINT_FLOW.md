# Function Point Entry - User Flow Diagram

## Navigation Flow

```
Dashboard
    ↓
Projects List
    ↓ (Click "View Estimations")
Estimations List (for selected project)
    ↓ (Click "View Details")
Estimation Detail + Function Point Entry
    ↓ (Add functions)
Results Auto-Update
```

## Component Hierarchy

```
┌─────────────────────────────────────────────────────────┐
│ Estimation Detail Component                             │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │ Estimation Header                              │    │
│  │ • Name: "Web App Estimation"                   │    │
│  │ • Created: January 15, 2024                    │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │ Results Summary (Calculated)                   │    │
│  │ ┌───────┐ ┌───────┐ ┌───────┐ ┌───────┐       │    │
│  │ │  UFP  │ │ SLOC  │ │Effort │ │Duration│       │    │
│  │ │  120  │ │ 12000 │ │ 45 PM │ │ 8.2 m │       │    │
│  │ └───────┘ └───────┘ └───────┘ └───────┘       │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │ Function Point Entry Component                 │    │
│  │                                                 │    │
│  │  Form:                                          │    │
│  │  ┌─────────────────────────────────────────┐   │    │
│  │  │ Name: [User Login Form            ]     │   │    │
│  │  │ Type: [EI ▼]  DET: [5]  RET: [2]       │   │    │
│  │  │              [Add Function]              │   │    │
│  │  └─────────────────────────────────────────┘   │    │
│  │                                                 │    │
│  │  Functions Table:                              │    │
│  │  ┌───────────────────────────────────────┐    │    │
│  │  │ Name          │Type│DET│RET│Comp │Pts│    │    │
│  │  ├───────────────────────────────────────┤    │    │
│  │  │ User Login    │ EI │ 5 │ 2 │Media│ 4 │🗑  │    │
│  │  │ Dashboard     │ EO │ 8 │ 3 │Alta │ 7 │🗑  │    │
│  │  │ User DB       │ILF │12 │ 4 │Alta │15 │🗑  │    │
│  │  └───────────────────────────────────────┘    │    │
│  │                                                 │    │
│  │  Total UFP: 26                                 │    │
│  │                                                 │    │
│  │  Info Panel:                                   │    │
│  │  • EI: External Input - data entry forms       │    │
│  │  • EO: External Output - reports, exports      │    │
│  │  • ILF: Internal Logical File - databases      │    │
│  └────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
```

## Data Flow

```
┌──────────────┐
│ User Actions │
└──────┬───────┘
       │
       ▼
┌─────────────────────────────────────────────┐
│ Frontend: Function Point Entry Component    │
│ • Validates input                           │
│ • Calls FunctionService.createFunction()    │
└──────┬──────────────────────────────────────┘
       │
       │ HTTP POST /api/Estimations/{projectId}/Estimations/{estimationId}/Functions
       │
       ▼
┌─────────────────────────────────────────────┐
│ Backend: FunctionsController                │
│ • Receives CreateFunctionDto                │
│ • Calls CocomoCalculationService            │
└──────┬──────────────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────────────┐
│ CocomoCalculationService                    │
│ • DetermineComplexity(type, det, retFtr)    │
│   - Uses COCOMO II tables                   │
│   - Returns: Baja/Media/Alta                │
│                                             │
│ • CalculateFunctionPoints(type, complexity) │
│   - Uses type-specific matrices             │
│   - Returns: point value                    │
│                                             │
│ • RecalculateEstimation()                   │
│   - Sums all function points → UFP          │
│   - UFP × Language Factor → SLOC            │
│   - Applies Scale Factors → Exponent E      │
│   - Applies Effort Multipliers → EAF        │
│   - SLOC × E × EAF → Effort (PM)            │
│   - Effort × Duration Formula → TDEV        │
│   - Effort / TDEV → Team Size               │
└──────┬──────────────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────────────┐
│ Database: EstimationFunctions + Estimations │
│ • Saves function with calculated values     │
│ • Updates estimation totals                 │
└──────┬──────────────────────────────────────┘
       │
       │ HTTP 201 Created (with calculated data)
       │
       ▼
┌─────────────────────────────────────────────┐
│ Frontend: Reload Estimation                 │
│ • Calls getEstimation() again               │
│ • Updates results summary                   │
│ • Shows new function in table               │
└─────────────────────────────────────────────┘
```

## Complexity Determination Example

### EI (External Input) Matrix:
```
        │ RET/FTR
DET     │  0-1    2     3+
────────┼────────────────────
 0-4    │ Baja  │ Baja │Media
 5-15   │ Baja  │Media │Alta
 16+    │Media  │Alta  │Alta
```

**Example**: 
- User Login Form
- Type: EI
- DET: 5 (username, password, remember_me, submit, error_message)
- RET: 2 (user table, session table)
- **Result**: Media complexity → 4 points

## Point Assignment Matrices

### EI (External Input):
- Baja: 3 points
- Media: 4 points  ← User Login Form gets this
- Alta: 6 points

### EO (External Output):
- Baja: 4 points
- Media: 5 points
- Alta: 7 points

### EQ (External Query):
- Baja: 3 points
- Media: 4 points
- Alta: 6 points

### ILF (Internal Logical File):
- Baja: 7 points
- Media: 10 points
- Alta: 15 points

### EIF (External Interface File):
- Baja: 5 points
- Media: 7 points
- Alta: 10 points

## Full COCOMO II Calculation Example

**Given:**
- Total UFP: 120
- Language: Java (factor = 53)
- Sum SF: 15.0
- EAF: 1.2

**Calculations:**
```
1. SLOC = UFP × Language Factor
   SLOC = 120 × 53 = 6,360 lines

2. KSLOC = SLOC / 1000
   KSLOC = 6.36

3. Exponent E = 0.91 + (0.01 × Sum SF)
   E = 0.91 + (0.01 × 15.0) = 1.06

4. Effort (PM) = 2.94 × EAF × (KSLOC^E)
   PM = 2.94 × 1.2 × (6.36^1.06)
   PM = 2.94 × 1.2 × 7.13
   PM = 25.15 person-months

5. Duration (TDEV) = 3.67 × (PM^0.28)
   TDEV = 3.67 × (25.15^0.28)
   TDEV = 3.67 × 2.15
   TDEV = 7.89 months

6. Team Size = PM / TDEV
   Team = 25.15 / 7.89
   Team = 3.19 people
```

---

This visualization shows the complete flow from user interaction through backend calculations to database storage and frontend updates!
