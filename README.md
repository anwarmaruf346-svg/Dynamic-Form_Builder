# Dynamic Form Builder - ASP.NET Core MVC

## Project Overview

A complete Dynamic Form Builder application built with ASP.NET Core MVC, featuring:

- **Frontend**: HTML, CSS, JavaScript, jQuery, jQuery Validation, DataTables
- **Backend**: ASP.NET Core MVC, ADO.NET with Stored Procedures
- **Database**: MS SQL Server
- **UI Framework**: Bootstrap 5

##  Project Features

### 1. **Form Creation**
- Dynamic form builder interface
- Add/remove dropdown fields dynamically
- Set field labels, required status, and default values
- Client-side validation using jQuery Validation plugin
- Server-side validation

### 2. **Form Management**
- View all created forms in a DataTable with server-side pagination
- Search and filter forms by title
- Sort by date and other columns
- Export data (CSV, Excel, Print)

### 3. **Form Preview**
- Preview forms in readonly mode
- Display required field indicators (red asterisk *)
- Show dropdown options with selected values
- ReadOnly fields styling

### 4. **API Integration**
- RESTful API endpoints for form operations
- AJAX-based form submission and data retrieval
- JSON response format

### 5. **Database**
- Proper database normalization with primary and foreign keys
- Stored procedures for all database operations
- Transaction handling and error management
- Indexed columns for performance

##  Project Structure

```
DynamicFormBuilder/
 Controllers/
 HomeController.cs
 FormController.cs              # MVC Controller for Form operations
 FormsApiController.cs          # API Controller
 Data/
 IFormRepository.cs             # Repository Interface
 FormRepository.cs              # ADO.NET Implementation
 Models/
 ErrorViewModel.cs
 FormModels.cs                  # All DTOs and ViewModels
 Views/
 Home/
 Form/
 Create.cshtml              # Form builder UI
 Grid.cshtml                # Forms list with DataTable
 Preview.cshtml             # Form preview (readonly)
 Shared/
 _Layout.cshtml
 wwwroot/
 css/
 site.css                   # Custom styling
 js/
 Database/
 DynamicFormBuilderDB.sql       # Database schema and SPs
 Program.cs
 appsettings.json
 DynamicFormBuilder.csproj
```

## ?? Setup Instructions

### Prerequisites
- .NET 8 SDK
- MS SQL Server (2019 or later)
- Visual Studio 2022 or Visual Studio Code

### Step 1: Clone/Download Project
```bash
git clone <repository-url>
cd DynamicFormBuilder
```

### Step 2: Update Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DynamicFormBuilderDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  }
}
```

### Step 3: Create Database
1. Open SQL Server Management Studio (SSMS)
2. Open the file: `DynamicFormBuilder/Database/DynamicFormBuilderDB.sql`
3. Execute the script to create database, tables, and stored procedures

### Step 4: Run the Application
```bash
dotnet restore
dotnet build
dotnet run
```

Application will start at: `https://localhost:5001` or `http://localhost:5000`

##  Database Schema

### Tables

#### Forms Table
```sql
CREATE TABLE Forms
(
    FormId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME NULL
);
```

#### DropdownFields Table
```sql
CREATE TABLE DropdownFields
(
    FieldId INT PRIMARY KEY IDENTITY(1,1),
    FormId INT NOT NULL,
    Label NVARCHAR(255) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    SelectedValue NVARCHAR(MAX),
    OptionList NVARCHAR(MAX),
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_DropdownFields_Forms FOREIGN KEY (FormId) 
        REFERENCES Forms(FormId) ON DELETE CASCADE
);
```

### Stored Procedures

1. **sp_CreateForm** - Create new form
2. **sp_CreateDropdownField** - Add dropdown field to form
3. **sp_GetAllForms** - Retrieve all forms
4. **sp_GetFormById** - Get form details
5. **sp_GetFormFields** - Get all fields for a form
6. **sp_GetFormsCount** - Get total forms count (with search)
7. **sp_GetFormsPaged** - Get paginated forms (for DataTable)

## ?? API Endpoints

### Forms API

#### 1. Create Form
```
POST /api/formsapi/create
Content-Type: application/json

{
  "title": "Customer Survey",
  "dropdownFields": [
    {
      "label": "Country",
      "isRequired": true,
      "selectedValue": "Option 1",
      "availableOptions": ["Option 1", "Option 2", "Option 3"]
    }
  ]
}

Response: { success: true, formId: 1, message: "Form created successfully" }
```

#### 2. Get All Forms
```
GET /api/formsapi/list

Response: 
[
  {
    "formId": 1,
    "title": "Customer Survey",
    "createdDate": "2025-01-15T10:30:00"
  }
]
```

#### 3. Get Form Details
```
GET /api/formsapi/{id}

Response:
{
  "formId": 1,
  "title": "Customer Survey",
  "createdDate": "2025-01-15T10:30:00",
  "dropdownFields": [...]
}
```

#### 4. Get Paged Forms (DataTable)
```
POST /api/formsapi/paged
Content-Type: application/json

{
  "draw": 1,
  "start": 0,
  "length": 10,
  "search": { "value": "" }
}

Response:
{
  "draw": 1,
  "recordsTotal": 50,
  "recordsFiltered": 50,
  "data": [...]
}
```

##  Frontend Components

### 1. Form Builder Page (`/Form/Create`)
- **Title Input**: Text input for form name
- **Dynamic Field Container**: Adds/removes dropdown fields
- **Field Configuration**:
  - Label: Text input
  - Required: Checkbox
  - Default Value: Dropdown selector
- **Validation**: jQuery Validation plugin
- **Submit**: AJAX-based submission

### 2. Forms Grid (`/Form/Grid`)
- **Server-Side DataTable**: 
  - Pagination: 5, 10, 25, 50 rows per page
  - Search functionality
  - Sortable columns
  - Export buttons (CSV, Excel, Print)
- **Columns**:
  - Form ID
  - Title
  - Created Date
  - Actions (Preview button)

### 3. Form Preview (`/Form/Preview/{id}`)
- **ReadOnly Display**: Dropdown fields are disabled
- **Required Field Indicators**: Red asterisk (*)
- **Field Status Badges**: Required/Optional badges
- **Back Navigation**: Link to forms grid

##  Technologies Used

### Backend
- **Framework**: ASP.NET Core 8
- **Database**: MS SQL Server
- **ORM**: ADO.NET with Stored Procedures (No Entity Framework)
- **API**: RESTful API

### Frontend
- **HTML5**: Semantic markup
- **CSS3**: Bootstrap 5
- **JavaScript**: Vanilla JS + jQuery
- **Libraries**:
  - jQuery v3.x
  - jQuery Validation v1.19.5
  - DataTables v1.11.5
  - SweetAlert2 v11
  - Bootstrap 5

### Development
- **.NET 8**
- **C# 12**
- **Visual Studio 2022**

##  Code Examples

### Creating a Form (JavaScript)
```javascript
$.ajax({
    url: '/Form/Create',
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({
        title: "My Form",
        dropdownFields: [
            {
                label: "Country",
                isRequired: true,
                selectedValue: "Option 1",
                availableOptions: ["Option 1", "Option 2", "Option 3"]
            }
        ]
    }),
    success: function(response) {
        if (response.success) {
            alert("Form created successfully!");
        }
    }
});
```

### DataTable Server-Side Processing (JavaScript)
```javascript
$('#formsTable').DataTable({
    processing: true,
    serverSide: true,
    ajax: {
        url: '/Form/GetFormsData',
        type: 'POST',
        contentType: 'application/json',
        data: function (d) {
            return JSON.stringify(d);
        }
    },
    columns: [
        { data: 'formId' },
        { data: 'title' },
        { data: 'createdDateFormatted' },
        {
            data: 'formId',
            render: function (data) {
                return '<a href="/Form/Preview/' + data + '">Preview</a>';
            }
        }
    ]
});
```

### ADO.NET Repository Pattern (C#)
```csharp
public async Task<int> CreateFormAsync(CreateFormViewModel model)
{
    int formId = 0;
    
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        
        using (SqlCommand command = new SqlCommand("sp_CreateForm", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Title", model.Title);
            
            SqlParameter formIdParam = new SqlParameter("@FormId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(formIdParam);
            
            await command.ExecuteNonQueryAsync();
            formId = (int)formIdParam.Value;
        }
    }
    
    return formId;
}
```

##  Validation Rules

### Client-Side (jQuery Validation)
- Form Title: Required, min 3 chars, max 100 chars
- Field Labels: Required, non-empty
- Required Fields: Dropdown selection required when marked

### Server-Side (C#)
- Title validation before database insert
- Field label validation
- Foreign key constraint enforcement
- Transaction handling for data consistency

##  Security Features

- HTTPS enforcement
- Input validation (client & server)
- SQL Injection prevention (parameterized queries)
- XSS protection (ASP.NET Core built-in)
- CSRF tokens (if needed)

##  Performance Considerations

- **Database Indexing**: FormId indexed on DropdownFields
- **Server-Side Pagination**: DataTable pagination with LIMIT/OFFSET
- **Async/Await**: All database calls are asynchronous
- **Query Optimization**: Stored procedures for complex queries
- **Caching**: Can be added for frequently accessed data

##  Testing

### Manual Testing Checklist
- [ ] Create form with 1 dropdown field
- [ ] Create form with multiple dropdown fields
- [ ] Add/remove dropdown fields dynamically
- [ ] Validate required field indicators
- [ ] Test form submission
- [ ] View forms grid with pagination
- [ ] Search forms by title
- [ ] Preview form in readonly mode
- [ ] Test API endpoints with Postman

### Sample Test Data SQL
```sql
INSERT INTO Forms (Title, CreatedDate) 
VALUES ('Customer Feedback Form', GETUTCDATE());

INSERT INTO DropdownFields (FormId, Label, IsRequired, SelectedValue, OptionList)
VALUES (1, 'Country', 1, 'Option 1', 'Option 1,Option 2,Option 3');
```

##  Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [jQuery Validation Documentation](https://jqueryvalidation.org/)
- [DataTables Documentation](https://datatables.net/)
- [SQL Server Stored Procedures](https://docs.microsoft.com/sql/relational-databases/stored-procedures/stored-procedures-database-engine)

##  Contributing

1. Create a feature branch
2. Make your changes
3. Commit with descriptive messages
4. Push to branch
5. Create Pull Request

##  License

This project is open source and available under the MIT License.

##  Author

**Dynamic Form Builder Team**
- Created: 2025
- Last Updated: 2025

##  Troubleshooting

### Connection String Issues
- Verify SQL Server is running
- Check server name format (use `(localdb)\mssqllocaldb` for LocalDB)
- Ensure database user has appropriate permissions

### Migration Issues
- Run SQL script manually in SSMS
- Check if stored procedures already exist
- Verify database name matches connection string

### Page Not Found (404)
- Verify controller and action names are correct
- Check routing in Program.cs
- Ensure views are in correct folders

### JavaScript Errors
- Check browser console (F12)
- Verify jQuery is loaded before custom scripts
- Check AJAX endpoints are correct

##  Support

For issues or questions:
1. Check troubleshooting section
2. Review code comments
3. Check database stored procedures
4. Verify connection string

---


