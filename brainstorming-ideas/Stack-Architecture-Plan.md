┌─────────────────────────────────────────────────────┐
│            SHARED REACT FRONTEND                     │
│   (Write ONCE, use with ALL 3 backends)             │
│   - React + TypeScript + Vite                       │
│   - Tailwind CSS                                    │
│   - Jest + React Testing Library                   │
│   - Playwright (E2E tests)                          │
└─────────────────────────────────────────────────────┘
                        │
                        │ HTTP/REST API calls
                        │
        ┌───────────────┼───────────────┐
        │               │               │
┌───────▼──────┐ ┌─────▼──────┐ ┌─────▼──────┐
│   Stack 1    │ │  Stack 2   │ │  Stack 3   │
│   .NET 8     │ │   MERN     │ │  Laravel   │
│ + PostgreSQL │ │+ MongoDB   │ │+ PostgreSQL│
│ + Redis      │ │+ Redis     │ │+ Redis     │
└──────────────┘ └────────────┘ └────────────┘


Stack 1: .NET

Frontend:  React + TypeScript + Vite + Tailwind (shadcn/ui)
    React Context API for State management
    TanStack Query (React Query): For API data caching (HIGHLY RECOMMENDED) or Zustand
Backend:   ASP.NET Core 8 Web API
Database:  PostgreSQL
Cache:     Redis
ORM:       Entity Framework Core
Auth:      JWT (built-in)
Testing:   xUnit + Moq + WebApplicationFactory
Hosting:   Railway (backend) + Vercel (frontend)

Stack 2: JS

Frontend:  SHARED React (same as Stack 1)
Backend:   Node.js + NestJS (instead of Express)
Database:  MongoDB (document-based, different from PostgreSQL) - schema flexibility
Cache:     Redis
ORM:       Mongoose (for MongoDB) OR Prisma (if using PostgreSQL)
Auth:      JWT + Passport.js
Testing:   Vitest + Playwright for E2E
Hosting:   Railway (backend) + Vercel (frontend - same deployment!)

Stack 3: PHP

Frontend:  SHARED React (same as Stacks 1 & 2)
Backend:   Laravel 11
Database:  PostgreSQL (reuse .NET schema knowledge!)
Cache:     Redis
ORM:       Eloquent (Laravel's built-in ORM)
Auth:      Laravel Sanctum (JWT-like tokens)
Testing:   Pest (modern PHPUnit alternative)
Hosting:   Railway (backend) + Vercel (frontend - same!)

Looking at something like this:

// Package.json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.20.0",      // Routing
    "axios": "^1.6.0",                   // HTTP requests
    "@tanstack/react-query": "^5.0.0",  // Data fetching
    "react-hook-form": "^7.48.0",       // Forms
    "zod": "^3.22.0",                    // Validation
    "zustand": "^4.4.0"                  // State (if needed)
  },
  "devDependencies": {
    "typescript": "^5.2.0",
    "vite": "^5.0.0",
    "tailwindcss": "^3.3.0",
    "@vitejs/plugin-react": "^4.2.0",
    "vitest": "^1.0.0",                  // Testing
    "@testing-library/react": "^14.0.0",
    "@playwright/test": "^1.40.0"       // E2E tests
  }
}

### **All 3 Stacks:**
```
┌─────────────────┐
│     Vercel      │  ← ONE React frontend (serves all 3 backends)
└────────┬────────┘
         │
    ┌────┴────┬────────┬────────┐
    │         │        │        │
┌───▼───┐ ┌──▼──┐ ┌───▼───┐ ┌──▼──┐
│ .NET  │ │NestJS│ │Laravel│ │ DB  │
│Railway│ │Railway│ │Railway│ │ DB  │
└───────┘ └──────┘ └───────┘ └─────┘
```

**Vercel (Frontend):**
- ONE deployment
- Environment variables for backend URLs:
```
  VITE_API_URL_DOTNET=https://dotnet-api.railway.app
  VITE_API_URL_NESTJS=https://nestjs-api.railway.app
  VITE_API_URL_LARAVEL=https://laravel-api.railway.app

Switch backends with a dropdown in UI - possibilty / how?