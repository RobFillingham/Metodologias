import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { StorageService } from './storage.service';
import { User } from '../models/user.model';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';

/**
 * Authentication service
 * Handles login, logout, and user session
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(
    private apiService: ApiService,
    private storageService: StorageService
  ) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      this.getUserFromStorage()
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  /**
   * Get current user value
   */
  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  /**
   * Check if user is authenticated
   */
  public get isAuthenticated(): boolean {
    return !!this.currentUserValue && !!this.getToken();
  }

  /**
   * Get JWT token
   */
  public getToken(): string | null {
    return this.storageService.getItem('token');
  }

  /**
   * Login user
   */
  login(loginRequest: LoginRequest): Observable<User> {
    return this.apiService.post<AuthResponse>('auth/login', loginRequest)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            const authData = response.data;
            const user: User = {
              id: authData.userId,
              email: authData.email,
              firstName: authData.firstName,
              lastName: authData.lastName,
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
              isActive: true
            };

            // Store user and token
            this.storageService.setItem('currentUser', user);
            this.storageService.setItem('token', authData.token);
            this.currentUserSubject.next(user);
            return user;
          }
          throw new Error(response.message || 'Login failed');
        })
      );
  }

  /**
   * Register new user
   */
  register(registerRequest: RegisterRequest): Observable<User> {
    return this.apiService.post<AuthResponse>('auth/register', registerRequest)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            const authData = response.data;
            const user: User = {
              id: authData.userId,
              email: authData.email,
              firstName: authData.firstName,
              lastName: authData.lastName,
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
              isActive: true
            };

            // Store user and token
            this.storageService.setItem('currentUser', user);
            this.storageService.setItem('token', authData.token);
            this.currentUserSubject.next(user);
            return user;
          }
          throw new Error(response.message || 'Registration failed');
        })
      );
  }

  /**
   * Logout user
   */
  logout(): void {
    this.storageService.removeItem('currentUser');
    this.storageService.removeItem('token');
    this.currentUserSubject.next(null);
  }

  /**
   * Get user from storage and validate token
   */
  private getUserFromStorage(): User | null {
    const user = this.storageService.getItem<User>('currentUser');
    const token = this.storageService.getItem('token');

    if (user && token) {
      // TODO: Add token expiration check
      return user;
    }

    return null;
  }
}
