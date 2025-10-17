# Angular Frontend Architecture Guide

## 📁 Complete Folder Structure

```
Frontend/src/
├── app/
│   ├── core/                          # ✅ CREATED - Core functionality
│   │   ├── guards/
│   │   │   └── auth.guard.ts         # Route protection
│   │   ├── interceptors/
│   │   │   ├── auth.interceptor.ts   # Add JWT to requests
│   │   │   └── error.interceptor.ts  # Global error handling
│   │   ├── services/
│   │   │   ├── api.service.ts        # Base HTTP service
│   │   │   ├── auth.service.ts       # Authentication logic
│   │   │   └── storage.service.ts    # LocalStorage wrapper
│   │   └── models/
│   │       ├── api-response.model.ts # Matches backend ApiResponse<T>
│   │       └── user.model.ts         # User model
│   │
│   ├── shared/                        # ✅ CREATED - Reusable components
│   │   ├── components/                # Add shared UI components here
│   │   ├── directives/                # Add custom directives here
│   │   ├── pipes/                     # Add custom pipes here
│   │   └── models/                    # Add shared models here
│   │
│   ├── features/                      # ✅ CREATED - Feature modules
│   │   ├── home/
│   │   │   ├── home.component.ts     # Home page component
│   │   │   └── home.routes.ts        # Home routes
│   │   └── auth/                      # Add auth feature here
│   │
│   ├── layout/                        # ✅ CREATED - Layout components
│   │   # Add layout components here
│   │
│   ├── app.component.ts               # ✅ UPDATED - Root component
│   ├── app.config.ts                  # ✅ UPDATED - HTTP & Interceptors
│   └── app.routes.ts                  # ✅ UPDATED - Main routing
│   
├── environments/                      # ✅ CREATED - Environment configs
│   ├── environment.ts                 # Development config
│   └── environment.prod.ts            # Production config
│
├── styles/                            # ✅ CREATED - Global styles
│   ├── _variables.scss                # CSS/SCSS variables
│   └── _global.scss                   # Global styles
│
└── styles.css                         # ✅ UPDATED - Main stylesheet
```

## 🎯 What's Been Set Up

### ✅ Core Services
- **ApiService**: Base service for all HTTP requests to backend
- **AuthService**: Handles login, logout, user session management
- **StorageService**: Manages localStorage/sessionStorage safely

### ✅ HTTP Interceptors
- **Auth Interceptor**: Automatically adds JWT token to all API requests
- **Error Interceptor**: Handles HTTP errors globally (401, 403, 404, 500)

### ✅ Route Guards
- **Auth Guard**: Protects routes that require authentication

### ✅ Models
- **ApiResponse<T>**: Matches your backend's ApiResponse structure
- **User**: Basic user model

### ✅ Environment Configuration
- Development environment points to `https://localhost:5001/api`
- Production environment ready to configure

### ✅ Routing
- Home page configured with lazy loading
- Wildcard route redirects to home
- Example of protected route (commented out)

### ✅ Global Styles
- CSS variables for theming
- Global utility classes
- SCSS support ready

## 🚀 Quick Start

### 1. Install Dependencies
```bash
cd Frontend
npm install
```

### 2. Start Development Server
```bash
npm start
```

Visit: `http://localhost:4200`

### 3. Verify Backend Connection
Make sure your .NET backend is running at `https://localhost:5001`

## 📝 How to Use

### Making API Calls

```typescript
import { Component, inject } from '@angular/core';
import { ApiService } from '../../core/services/api.service';

export class MyComponent {
  private apiService = inject(ApiService);

  loadData() {
    this.apiService.get<MyData>('sample').subscribe({
      next: (response) => {
        if (response.success) {
          console.log(response.data);
        }
      },
      error: (error) => {
        console.error('Error:', error);
      }
    });
  }
}
```

### Using Authentication

```typescript
import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

export class LoginComponent {
  private authService = inject(AuthService);

  login(email: string, password: string) {
    this.authService.login(email, password).subscribe({
      next: (user) => {
        console.log('Logged in:', user);
        // Navigate to dashboard
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }
}
```

### Protecting Routes

```typescript
// In app.routes.ts
{
  path: 'dashboard',
  canActivate: [authGuard],
  loadComponent: () => import('./features/dashboard/dashboard.component')
}
```

## 📦 Next Steps

### 1. Create Shared Components
```bash
cd src/app/shared/components
ng generate component navbar --standalone
ng generate component footer --standalone
ng generate component loading-spinner --standalone
```

### 2. Create Feature Modules
```bash
cd src/app/features
ng generate component auth/login --standalone
ng generate component auth/register --standalone
ng generate component dashboard --standalone
```

### 3. Create Layout Components
```bash
cd src/app/layout
ng generate component main-layout --standalone
ng generate component auth-layout --standalone
```

### 4. Add More Services
```bash
cd src/app/core/services
ng generate service user
ng generate service notification
```

## 🔧 Angular CLI Commands

```bash
# Generate component
ng generate component features/my-feature --standalone

# Generate service
ng generate service core/services/my-service

# Generate guard
ng generate guard core/guards/my-guard

# Generate pipe
ng generate pipe shared/pipes/my-pipe

# Generate directive
ng generate directive shared/directives/my-directive
```

## 🎨 Styling Guide

### Using CSS Variables
```css
.my-component {
  color: var(--primary-color);
  padding: var(--spacing-md);
  border-radius: var(--border-radius);
}
```

### Using Utility Classes
```html
<div class="container mt-2 p-2 text-center">
  <h1 class="mb-2">Title</h1>
  <p>Content</p>
</div>
```

## 🔗 Backend Integration

Your Angular app is configured to work with your .NET backend:

### API Response Format (Matches Backend)
```typescript
{
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
}
```

### Example API Endpoints
- GET `/api/sample` - Get greeting
- GET `/api/sample/{name}` - Get personalized greeting
- POST `/api/sample` - Create sample data

## ⚙️ Configuration

### Update API URL
Edit `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api', // Change this
};
```

### Add CORS in Backend
Make sure your backend allows requests from `http://localhost:4200`:
```csharp
// In Backend/Program.cs (already configured)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

## 📚 Architecture Principles

1. **Separation of Concerns**: Core, Shared, Features, Layout
2. **Lazy Loading**: Features load on demand
3. **Type Safety**: TypeScript interfaces for everything
4. **Reactive Programming**: Use RxJS observables
5. **Single Responsibility**: One component, one purpose
6. **DRY**: Don't Repeat Yourself - use shared components

## 🎯 Best Practices

✅ **DO**:
- Use standalone components (Angular 20+)
- Use signals for reactive state
- Use inject() function for dependency injection
- Handle errors in subscriptions
- Unsubscribe from observables (use async pipe when possible)
- Use lazy loading for features

❌ **DON'T**:
- Put business logic in components
- Make HTTP calls directly in components (use services)
- Forget to handle errors
- Leave console.log in production code
- Nest components too deeply

## 🔍 Troubleshooting

### Backend Connection Issues
1. Check backend is running: `https://localhost:5001`
2. Check CORS is enabled in backend
3. Check API URL in `environment.ts`
4. Open browser console for errors

### Build Errors
```bash
# Clear cache and rebuild
rm -rf node_modules
npm install
ng build
```

## 🎉 You're All Set!

Your Angular frontend is now fully structured and connected to your .NET backend. Start building your features! 🚀
