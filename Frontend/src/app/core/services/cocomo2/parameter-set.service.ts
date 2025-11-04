import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { ApiResponse, ParameterSet, CreateParameterSetRequest } from '../../models/cocomo2/cocomo.models';

/**
 * Service for parameter set-related operations
 */
@Injectable({
  providedIn: 'root'
})
export class ParameterSetService {
  constructor(private apiService: ApiService) {}

  /**
   * Get user's custom parameter sets
   */
  getMyParameterSets(): Observable<ApiResponse<ParameterSet[]>> {
    return this.apiService.get<ParameterSet[]>('ParameterSets/my');
  }

  /**
   * Get default system parameter sets
   */
  getDefaultParameterSets(): Observable<ApiResponse<ParameterSet[]>> {
    return this.apiService.get<ParameterSet[]>('ParameterSets/default');
  }

  /**
   * Get a specific parameter set by ID
   */
  getParameterSet(id: number): Observable<ApiResponse<ParameterSet>> {
    return this.apiService.get<ParameterSet>(`ParameterSets/${id}`);
  }

  /**
   * Get all available parameter sets (both default and custom)
   */
  getAllParameterSets(): Observable<ApiResponse<ParameterSet[]>> {
    return new Observable(observer => {
      const myParams: ParameterSet[] = [];
      const defaultParams: ParameterSet[] = [];
      let completed = 0;

      const checkComplete = () => {
        completed++;
        if (completed === 2) {
          // Combine and send response
          const allParams = [...defaultParams, ...myParams];
          observer.next({
            success: true,
            message: 'Parameter sets retrieved successfully',
            data: allParams,
            errors: []
          });
          observer.complete();
        }
      };

      // Load default parameter sets
      this.getDefaultParameterSets().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            defaultParams.push(...response.data);
          }
          checkComplete();
        },
        error: () => {
          checkComplete(); // Continue even if one fails
        }
      });

      // Load user's custom parameter sets
      this.getMyParameterSets().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            myParams.push(...response.data);
          }
          checkComplete();
        },
        error: () => {
          checkComplete(); // Continue even if one fails
        }
      });
    });
  }

  /**
   * Create a new parameter set
   */
  createParameterSet(request: CreateParameterSetRequest): Observable<ApiResponse<ParameterSet>> {
    return this.apiService.post<ParameterSet>('ParameterSets', request);
  }

  /**
   * Update an existing parameter set
   */
  updateParameterSet(id: number, request: Partial<CreateParameterSetRequest>): Observable<ApiResponse<ParameterSet>> {
    return this.apiService.put<ParameterSet>(`ParameterSets/${id}`, request);
  }

  /**
   * Delete a parameter set
   */
  deleteParameterSet(id: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`ParameterSets/${id}`);
  }
}