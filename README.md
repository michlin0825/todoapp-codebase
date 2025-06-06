# Todo List Application

A simple ASP.NET Core MVC Todo List application that can be deployed to AWS Elastic Beanstalk.

## Features

- Create, read, update, and delete todo items
- Mark todo items as complete or incomplete
- Persistent storage using SQLite database
- Responsive design with Bootstrap
- AWS CloudWatch logging integration
- Elastic Beanstalk deployment support

## Project Structure

- **Controllers/**
  - `HomeController.cs` - Handles basic navigation
  - `TodosController.cs` - Handles CRUD operations for todo items
  
- **Models/**
  - `Todo.cs` - Represents a todo item
  - `ErrorViewModel.cs` - Used for error handling
  
- **Data/**
  - `TodoContext.cs` - Entity Framework Core database context
  - `SeedData.cs` - Initial data seeding
  
- **Views/**
  - Todo item views for listing, creating, editing, etc.
  
- **.ebextensions/**
  - `01_setup.config` - Elastic Beanstalk configuration for deployment

## Prerequisites

- .NET 8.0 SDK
- AWS CLI (for deployment)
- AWS account with appropriate permissions

## Local Development

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Database

The application uses SQLite for data storage:

- Development: `TodoList.db` in the project root
- Production: `/var/app/db/todoapp.db` on the Elastic Beanstalk instance

## Deployment

See the CloudFormation deployment instructions in the `TodoApp_Final_CloudFormation` directory for detailed deployment steps.

## Configuration

- `appsettings.json` - Default application settings
- `appsettings.Development.json` - Development-specific settings
- `appsettings.Production.json` - Production-specific settings

## Security Best Practices

1. **Never commit sensitive information to the repository:**
   - Database credentials
   - API keys or tokens
   - AWS access keys or secret keys
   - Connection strings with embedded credentials

2. **Use environment-specific configuration:**
   - Keep sensitive configuration in environment variables or AWS Parameter Store
   - Use IAM roles for AWS service access instead of hardcoded credentials

3. **Before committing code:**
   - Check for any hardcoded credentials or connection strings
   - Ensure the `.gitignore` file is properly configured
   - Verify that no sensitive local configuration files are included

## License

MIT
