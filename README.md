# Candidate Test Platform

## Architecture

- Backend: .NET 8 Web API
- Frontend: React + TypeScript (Vite)
- Database: SQL Server
- Real-time: SignalR
- UI: TailwindCSS + Framer Motion + Monaco Editor

## Backend startup

1. Go to `/backend`
2. `dotnet restore`
3. `dotnet ef database update` (requires EF tools)
4. `dotnet run`

## Frontend startup

1. Go to `/frontend`
2. `npm install`
3. `npm run dev`

## Notes

- Add strong secret in `appsettings.json` for JWT
- Adjust SQL Server connection string as needed
