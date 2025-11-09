import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from '../api.service';
import { StorageService } from '../storage.service';
import { ApiResponse, Estimation, CreateEstimationRequest, UpdateEstimationRatingsRequest, UpdateEstimationActualResultsRequest } from '../../models/cocomo-ii-stage3/cocomo.models';

/**
 * Service for estimation-related operations
 */
@Injectable({
  providedIn: 'root'
})
export class EstimationService {
  private currentEstimationSubject: BehaviorSubject<Estimation | null>;
  public currentEstimation: Observable<Estimation | null>;

  constructor(
    private apiService: ApiService,
    private storageService: StorageService
  ) {
    this.currentEstimationSubject = new BehaviorSubject<Estimation | null>(
      this.getEstimationFromStorage()
    );
    this.currentEstimation = this.currentEstimationSubject.asObservable();
  }

  /**
   * Get current estimation value
   */
  public get currentEstimationValue(): Estimation | null {
    return this.currentEstimationSubject.value;
  }

  /**
   * Set current estimation
   */
  setCurrentEstimation(estimation: Estimation | null): void {
    this.currentEstimationSubject.next(estimation);
    if (estimation) {
      this.storageService.setItem('currentEstimation', estimation);
    } else {
      this.storageService.removeItem('currentEstimation');
    }
  }

  /**
   * Get estimation from storage
   */
  private getEstimationFromStorage(): Estimation | null {
    return this.storageService.getItem<Estimation>('currentEstimation');
  }

  /**
   * Get all estimations for a project
   */
  getEstimationsByProject(projectId: number): Observable<ApiResponse<Estimation[]>> {
    return this.apiService.get<Estimation[]>(`CocomoIIStage3/Projects/${projectId}/Estimations`);
  }

  /**
   * Get a specific estimation
   */
  getEstimation(projectId: number, estimationId: number): Observable<ApiResponse<Estimation>> {
    return this.apiService.get<Estimation>(`CocomoIIStage3/Projects/${projectId}/Estimations/${estimationId}`);
  }

  /**
   * Create a new estimation
   */
  createEstimation(projectId: number, request: CreateEstimationRequest): Observable<ApiResponse<Estimation>> {
    return this.apiService.post<Estimation>(`CocomoIIStage3/Projects/${projectId}/Estimations`, request);
  }

  /**
   * Update estimation ratings (triggers recalculation)
   */
  updateEstimationRatings(projectId: number, estimationId: number, request: UpdateEstimationRatingsRequest): Observable<ApiResponse<Estimation>> {
    return this.apiService.put<Estimation>(`CocomoIIStage3/Projects/${projectId}/Estimations/${estimationId}/Ratings`, request);
  }

  /**
   * Update actual results
   */
  updateEstimationActualResults(projectId: number, estimationId: number, request: UpdateEstimationActualResultsRequest): Observable<ApiResponse<Estimation>> {
    return this.apiService.put<Estimation>(`CocomoIIStage3/Projects/${projectId}/Estimations/${estimationId}/ActualResults`, request);
  }

  /**
   * Delete an estimation
   */
  deleteEstimation(projectId: number, estimationId: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`CocomoIIStage3/Projects/${projectId}/Estimations/${estimationId}`);
  }
}