import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, EstimationFunction, CreateEstimationFunctionRequest, BatchCreateEstimationFunctionsRequest } from '../models/cocomo.models';

/**
 * Service for estimation function-related operations
 */
@Injectable({
  providedIn: 'root'
})
export class EstimationFunctionService {
  constructor(private apiService: ApiService) {}

  /**
   * Get all functions for an estimation
   */
  getFunctions(estimationId: number): Observable<ApiResponse<EstimationFunction[]>> {
    return this.apiService.get<EstimationFunction[]>(`Estimations/${estimationId}/Functions`);
  }

  /**
   * Get a specific function
   */
  getFunction(estimationId: number, functionId: number): Observable<ApiResponse<EstimationFunction>> {
    return this.apiService.get<EstimationFunction>(`Estimations/${estimationId}/Functions/${functionId}`);
  }

  /**
   * Add a new function (triggers recalculation)
   */
  addFunction(estimationId: number, request: CreateEstimationFunctionRequest): Observable<ApiResponse<EstimationFunction>> {
    return this.apiService.post<EstimationFunction>(`Estimations/${estimationId}/Functions`, request);
  }

  /**
   * Add multiple functions in batch (triggers single recalculation)
   */
  addFunctionsBatch(estimationId: number, request: BatchCreateEstimationFunctionsRequest): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`Estimations/${estimationId}/Functions/Batch`, request);
  }

  /**
   * Update an existing function (triggers recalculation)
   */
  updateFunction(estimationId: number, functionId: number, request: Partial<CreateEstimationFunctionRequest>): Observable<ApiResponse<EstimationFunction>> {
    return this.apiService.put<EstimationFunction>(`Estimations/${estimationId}/Functions/${functionId}`, request);
  }

  /**
   * Delete a function (triggers recalculation)
   */
  deleteFunction(estimationId: number, functionId: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`Estimations/${estimationId}/Functions/${functionId}`);
  }
}