import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { 
  KlocEstimation, 
  CreateKlocEstimationRequest, 
  UpdateKlocEstimationRequest 
} from '../../models/kloc/kloc.models';
import { ApiResponse } from '../../models/cocomo2/cocomo.models';

/**
 * Service for KLOC estimation operations
 */
@Injectable({
  providedIn: 'root'
})
export class KlocEstimationService {

  constructor(private apiService: ApiService) { }

  /**
   * Get all KLOC estimations for a specific project
   */
  getEstimationsByProject(projectId: number): Observable<ApiResponse<KlocEstimation[]>> {
    return this.apiService.get<KlocEstimation[]>(`KlocEstimations/project/${projectId}`);
  }

  /**
   * Get a specific KLOC estimation by ID
   */
  getEstimationById(estimationId: number): Observable<ApiResponse<KlocEstimation>> {
    return this.apiService.get<KlocEstimation>(`KlocEstimations/${estimationId}`);
  }

  /**
   * Create a new KLOC estimation
   */
  createEstimation(request: CreateKlocEstimationRequest): Observable<ApiResponse<KlocEstimation>> {
    return this.apiService.post<KlocEstimation>('KlocEstimations', request);
  }

  /**
   * Update an existing KLOC estimation
   */
  updateEstimation(estimationId: number, request: UpdateKlocEstimationRequest): Observable<ApiResponse<KlocEstimation>> {
    return this.apiService.put<KlocEstimation>(`KlocEstimations/${estimationId}`, request);
  }

  /**
   * Delete a KLOC estimation
   */
  deleteEstimation(estimationId: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`KlocEstimations/${estimationId}`);
  }
}
