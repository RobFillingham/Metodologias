import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { StorageService } from './storage.service';
import { User } from '../models/user.model';

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
      this.storageService.getItem('currentUser')
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
    return !!this.currentUserValue;
  }

  /**
   * Login user
   */
  login(email: string, password: string): Observable<User> {
    return this.apiService.post<User>('auth/login', { email, password })
      .pipe(
        map(response => {
          if (response.success && response.data) {
            this.storageService.setItem('currentUser', response.data);
            this.currentUserSubject.next(response.data);
            return response.data;
          }
          throw new Error(response.message);
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
   * Register new user
   */
  register(email: string, password: string, name: string): Observable<User> {
    return this.apiService.post<User>('auth/register', { email, password, name })
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          }
          throw new Error(response.message);
        })
      );
  }
}
