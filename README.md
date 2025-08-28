# Job Board App

Simple job board built with Vue.js frontend and .NET backend. Admins can post jobs, candidates can apply.

## Quick Start

### Requirements
- Node.js 18+
- .NET 9 SDK

### Running the App

1. **Start Backend**
```bash
cd backend/JobBoardApi
dotnet run
```
Backend runs on http://localhost:5000

2. **Start Frontend** 
```bash
cd frontend/job-board-frontend
npm install
npm run dev
```
Frontend runs on http://localhost:5173

### Login

**Admin Account:**
- Email: admin@jobboard.com
- Password: admin123

**New Candidate:** Register through the app

## What it does

**Admins can:**
- Create/edit/delete job posts
- View candidate applications
- See job statistics

**Candidates can:**  
- Browse available jobs
- Apply with a message
- One application per job

## Tech Stack
- Frontend: Vue 3, Vuetify, TypeScript
- Backend: .NET 9, Entity Framework (in-memory DB)
- Auth: JWT tokens

## Development

Frontend commands:
```bash
npm run dev    # development server
npm run build  # production build
```

Backend commands:
```bash
dotnet run           # start API
dotnet watch run     # with hot reload
```

The app uses in-memory database so no additional setup needed.
