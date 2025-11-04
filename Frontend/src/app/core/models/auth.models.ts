/**
 * Login request DTO - matches backend LoginDto
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Register request DTO - matches backend RegisterDto
 */
export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

/**
 * Authentication response DTO - matches backend AuthResponseDto
 */
export interface AuthResponse {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  token: string;
  expiresAt: string; // ISO date string
}