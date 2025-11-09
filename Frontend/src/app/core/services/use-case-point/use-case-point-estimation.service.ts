import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { 
  UseCasePointEstimation, 
  CreateUseCasePointEstimationRequest, 
  UpdateUseCasePointEstimationRequest 
} from '../../models/use-case-point/use-case-point.models';
import { ApiResponse } from '../../models/cocomo2/cocomo.models';

/**
 * Service for Use Case Point estimation operations
 */
@Injectable({
  providedIn: 'root'
})
export class UseCasePointEstimationService {

  constructor(private apiService: ApiService) { }

  /**
   * Get all Use Case Point estimations for a specific project
   */
  getEstimationsByProject(projectId: number): Observable<ApiResponse<UseCasePointEstimation[]>> {
    return this.apiService.get<UseCasePointEstimation[]>(`UseCasePointEstimations/project/${projectId}`);
  }

  /**
   * Get a specific Use Case Point estimation by ID
   */
  getEstimationById(estimationId: number): Observable<ApiResponse<UseCasePointEstimation>> {
    return this.apiService.get<UseCasePointEstimation>(`UseCasePointEstimations/${estimationId}`);
  }

  /**
   * Create a new Use Case Point estimation
   */
  createEstimation(request: CreateUseCasePointEstimationRequest): Observable<ApiResponse<UseCasePointEstimation>> {
    return this.apiService.post<UseCasePointEstimation>('UseCasePointEstimations', request);
  }

  /**
   * Update an existing Use Case Point estimation
   */
  updateEstimation(estimationId: number, request: UpdateUseCasePointEstimationRequest): Observable<ApiResponse<UseCasePointEstimation>> {
    return this.apiService.put<UseCasePointEstimation>(`UseCasePointEstimations/${estimationId}`, request);
  }

  /**
   * Delete a Use Case Point estimation
   */
  deleteEstimation(estimationId: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`UseCasePointEstimations/${estimationId}`);
  }
}
