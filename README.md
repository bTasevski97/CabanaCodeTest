# Resort Map — Developer Documentation

This document provides technical details, design rationale, and instructions for running and testing the Resort Map application.

## 🚀 Quick Start

The project includes enhanced cross-platform entry scripts that handle **validation**, **clean install**, **frontend build**, and **backend launch** in one step.

- **Windows:** `.\run.ps1`
- **Unix/Linux/macOS:** `./run.sh`

> [!NOTE]
> Please note that these scripts have **not** been tested on macOS.

### Smart CLI Features:
- **Fast Parameter Validation:** The scripts validate your arguments (filenames, formatting) *before* starting the build.
- **Interactive Map Selection:** If you run the script without a `--map` argument:
  - If multiple `.ascii` files are found in the root, it will prompt you with a numbered list to pick one. This makes testing different layouts comfortable.
  - If only one `.ascii` file exists, it will auto-select it and proceed.
- **Browser Auto-Open:** Once the backend is ready, the scripts automatically launch your default browser to the frontend URL.
- **Auto open app:** Uses the `wait-on` library to pulse the server and only open the browser once the application is genuinely ready to serve requests
- **Manual Overrides:** You can still pass standard arguments 
  `./run.sh --map map_large.ascii --bookings bookings.json`

---

## 🛠️ Technology Stack

| Layer | Choice | Rationale |
| :--- | :--- | :--- |
| **Backend** | .NET 10 (Minimal API) | High-performance |
| **Frontend** | React 19 + Vite + React Compiler | Most popular framework, with Vite which is currently the web standard. React Compiler is used to automatically optimize renders without manual memoization. |
| **Styling** | Tailwind CSS v4 | Fast, easy to manage zero runtime CSS. |
| **Testing** | xUnit & Vitest | Modern testing frameworks for both ecosystems that are fast and developer-friendly. |

---

## 👨‍💻 Development Mode

The project is configured with `Microsoft.AspNetCore.SpaProxy`, meaning you can develop both the backend and frontend simultaneously. The SPA Proxy integrates with Vite, providing **Hot Module Replacement (HMR)** for the frontend out of the box.

### Start Development Server
```bash
dotnet run --project src/ResortMap/ResortMap.csproj
```
*This will launch the backend and automatically start the Vite development server. Any changes to the React source files will automatically refresh in the browser.*
---

## 🧪 Running Tests

### Backend Tests
```bash
dotnet test tests/ResortMap.Tests/ResortMap.Tests.csproj
```
*Validated map parsing logic, path resolution algorithms, and guest validation logic.*

### Frontend Tests
```bash
cd src/ClientApp
npm run test
```
*Validated component rendering, accessibility compliance, and user interaction flows.*

---

## ✨ Design Rationale & Features

### 1. Simplicity & Minimal Libraries
The app focuses on using the built-in capabilities of the chosen frameworks. Instead of heavy state-management libraries or over-engineered abstractions, it relies on modern React patterns and .NET Minimal API's lean architecture. This keeps the bundle size small and the codebase easy to audit.

### 2. Accessibility (A11y) & Inclusivity
- **Keyboard Navigation:** Fully navigable via keyboard. Interactive elements like cabana tiles and modal dialogs are wired to capture focus seamlessly, ensuring power users and accessible devices can comfortably traverse the app without a mouse.
- **Screen Reader Friendliness:** Incorporates `aria-labels`, `aria-hidden`, `aria-live`, and accessibility attributes to map tiles where native elements fall short, helping assistive technologies provide proper context for the visual resort layout.
- **Semantic HTML Content:** Relies on structural HTML5 tags (`<main>`, `<nav>`, `<dialog>`, `<header>`) instead of nested `<div>` containers, building a logically parseable document tree right out of the box.
- **Visual Clarity:** Clear visual distinctions (legend, status-based colors) for available vs. booked cabanas.

### 3. Mobile First & Responsiveness
The resort map uses a **Dynamic CSS Grid** that adjusts to the map dimensions provided by the API while remaining responsive on smaller screens. 

---

## 🔮 Future "Real World" Improvements

If this were a production-scale application, I would implement the following:

### Frontend
- **Internationalization (i18n):** Implement support for multiple languages using **i18next** to ensure the application is accessible to global guests.
- **Optimistic Updates:** Implement optimistic UI updates so that when a user books a cabana, the map updates instantly before the server responds, improving perceived performance.
- **TanStack Query (React Query):** For more robust cache management, background synchronization, and standardized loading/error states.
- **Zod:** To implement schema-based validation for all forms and API responses, ensuring type safety at the boundary.
- **Component Library:** Transitioning to a library like **shadcn**

### Backend
- **Persistence:** Replace in-memory storage with a real database using **Entity Framework Core**.
- **Authentication/Authorization:** Implement proper JWT-based auth or integrate with an identity provider (for example: Auth0).
- **Observability:** Integrate structured logging and telemetry (OpenTelemetry/Application Insights).
- **API Documentation:** Enable Swagger for easier frontend integration and testing.

### Infrastructure & DevOps
- **Dockerization:** Containerize the application (frontend and backend) using Docker and to ensure consistent environments across development, testing, and production, and to simplify CI/CD deployments.
