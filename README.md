# Job Board Platform

A full-stack job board application built with Vue.js 3 + Vuetify frontend and .NET 8 Web API backend. The platform allows Admins to manage job postings and Candidates to browse and apply for jobs.

## Features

### Authentication
- JWT-based authentication
- Role-based access control (Admin/Candidate)
- User registration and login

### Admin Features
- Create, edit, and delete job postings
- View job details with application counts
- Review candidate applications for each job
- Protected admin dashboard

### Candidate Features
- Browse available job listings
- Apply to jobs with motivation messages
- One application per job per candidate
- Responsive job browsing interface

## Technology Stack

### Frontend
- **Vue.js 3** with Composition API
- **Vuetify 3** for UI components
- **Vue Router** for navigation
- **Pinia** for state management
- **TypeScript** for type safety
- **Vite** as build tool

### Backend
- **.NET 8** Web API
- **Entity Framework Core** with InMemory database
- **JWT Authentication** with role-based authorization
- **Clean Architecture** with Controllers, Services, and Data layers
- **Swagger/OpenAPI** for API documentation

## Project Structure

```
/
├── frontend/job-board-frontend/    # Vue.js frontend application
│   ├── src/
│   │   ├── components/            # Reusable Vue components
│   │   ├── views/                 # Page components
│   │   ├── stores/                # Pinia state management
│   │   ├── router/                # Vue Router configuration
│   │   └── main.ts                # Application entry point
│   └── package.json
└── backend/JobBoardApi/           # .NET Web API backend
    ├── Controllers/               # API controllers
    ├── Services/                  # Business logic services
    ├── Models/                    # Entity models
    ├── DTOs/                      # Data transfer objects
    ├── Data/                      # Entity Framework DbContext
    └── Program.cs                 # Application startup
```

## Setup Instructions

### Prerequisites
- **Node.js** (v18 or higher) and npm
- **.NET 8 SDK**
- **Git**

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/JobBoardApi
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Run the backend API:
   ```bash
   dotnet run
   ```

   The API will be available at `http://localhost:5000` (or `https://localhost:5001`)

4. API Documentation (Swagger) will be available at:
   ```
   http://localhost:5000/swagger
   ```

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend/job-board-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

   The frontend will be available at `http://localhost:5173`

### Default Admin Account

The application creates a default admin account on startup:
- **Email**: `admin@jobboard.com`
- **Password**: `admin123`

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Jobs (Protected)
- `GET /api/jobs` - Get all active jobs
- `GET /api/jobs/with-applications` - Get jobs with applications (Admin only)
- `GET /api/jobs/{id}` - Get job by ID
- `POST /api/jobs` - Create new job (Admin only)
- `PUT /api/jobs/{id}` - Update job (Admin only)
- `DELETE /api/jobs/{id}` - Delete job (Admin only)
- `POST /api/jobs/{id}/apply` - Apply to job (Candidate only)

## Development

### Frontend Development
- Run development server: `npm run dev`
- Build for production: `npm run build`
- Preview production build: `npm run preview`
- Type checking: `npm run type-check`
- Linting: `npm run lint`

### Backend Development
- Run in development mode: `dotnet run`
- Run with hot reload: `dotnet watch run`
- Build: `dotnet build`
- Run tests: `dotnet test` (when tests are added)

## Environment Variables

Create a `.env` file in the frontend directory for environment-specific configuration:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

## Database

The application uses Entity Framework Core with an in-memory database for development. For production, you would want to:

1. Update the connection string in `appsettings.json`
2. Change from `UseInMemoryDatabase` to `UseSqlServer` or your preferred database provider
3. Run migrations: `dotnet ef migrations add InitialCreate && dotnet ef database update`

## Security Considerations

- JWT tokens expire after 7 days
- Passwords are hashed using PBKDF2 with salt
- CORS is configured to allow frontend origin
- Role-based authorization protects admin endpoints
- Input validation on both frontend and backend

## Future Enhancements

- Add file upload for resumes
- Implement job categories and filtering
- Add email notifications
- Implement job search functionality
- Add pagination for large datasets
- Add unit and integration tests
- Docker containerization
- Production database configuration

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.