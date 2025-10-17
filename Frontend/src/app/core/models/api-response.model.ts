/**
 * API Response model that matches the backend ApiResponse<T>
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
}
