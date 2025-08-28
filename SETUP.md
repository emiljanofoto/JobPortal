# Job Portal Setup Guide

## Quick Start

### Backend Setup (.NET API)
1. Navigate to backend directory:
   ```bash
   cd backend/JobBoardApi
   ```

2. Run the backend:
   ```bash
   dotnet run
   ```
   - Backend will start on: **http://localhost:5001**
   - Database will be automatically seeded with admin user

### Frontend Setup (Vue.js)
1. Navigate to frontend directory:
   ```bash
   cd frontend/job-board-frontend
   ```

2. Install dependencies (if not already done):
   ```bash
   npm install
   ```

3. Run the frontend:
   ```bash
   npm run dev
   ```
   - Frontend will be available on: **http://localhost:5174** (or similar port)

## Default Admin Credentials
- **Email**: admin@jobboard.com  
- **Password**: admin123

## Important Notes

### Database Seeding
- The application uses an **in-memory database** that resets on each restart
- Admin user is automatically created on startup with the credentials above
- The seeding logic ensures the admin user always exists with the correct password

### Port Configuration
- **Backend API**: http://localhost:5001
- **Frontend**: http://localhost:5174 (or as shown in terminal)
- CORS is configured to allow the frontend to communicate with the backend

### After Git Clone
1. No additional database setup required (in-memory database)
2. Admin user will be automatically created on first startup
3. Just run `dotnet run` in backend and `npm run dev` in frontend

## API Endpoints
- **Login**: POST /api/auth/login
- **Register**: POST /api/auth/register
- **Base URL**: http://localhost:5001

## Troubleshooting

### Port 5000 Conflicts
- macOS Control Center uses port 5000 for AirPlay
- This app is configured to use port 5001 to avoid conflicts

### Admin User Issues  
- Admin user is recreated automatically on each startup
- Password is always reset to "admin123" if it doesn't match
- Check console logs for seeding confirmation messages

### CORS Issues
- Frontend ports 5173, 5174, and 3000 are allowed
- If frontend runs on different port, update Program.cs CORS policy