import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { 
  FunctionPointEstimation, 
  CreateFunctionPointEstimationRequest, 
  UpdateFunctionPointEstimationRequest 
} from '../../models/function-point/function-point.models';
import { ApiResponse } from '../../models/cocomo2/cocomo.models';

/**
 * Service for Function Point estimation operations
 */
@Injectable({
  providedIn: 'root'
})
export class FunctionPointEstimationService {

  constructor(private apiService: ApiService) { }

  /**
   * Get all Function Point estimations for a specific project
   */
  getEstimationsByProject(projectId: number): Observable<ApiResponse<FunctionPointEstimation[]>> {
    return this.apiService.get<FunctionPointEstimation[]>(`FunctionPointEstimations/project/${projectId}`);
  }

  /**
   * Get a specific Function Point estimation by ID
   */
  getEstimationById(estimationId: number): Observable<ApiResponse<FunctionPointEstimation>> {
    return this.apiService.get<FunctionPointEstimation>(`FunctionPointEstimations/${estimationId}`);
  }

  /**
   * Create a new Function Point estimation
   */
  createEstimation(request: CreateFunctionPointEstimationRequest): Observable<ApiResponse<FunctionPointEstimation>> {
    return this.apiService.post<FunctionPointEstimation>('FunctionPointEstimations', request);
  }

  /**
   * Update an existing Function Point estimation
   */
  updateEstimation(estimationId: number, request: UpdateFunctionPointEstimationRequest): Observable<ApiResponse<FunctionPointEstimation>> {
    return this.apiService.put<FunctionPointEstimation>(`FunctionPointEstimations/${estimationId}`, request);
  }

  /**
   * Delete a Function Point estimation
   */
  deleteEstimation(estimationId: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`FunctionPointEstimations/${estimationId}`);
  }
}
