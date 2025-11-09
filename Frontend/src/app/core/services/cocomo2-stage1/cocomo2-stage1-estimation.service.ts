import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../models/api-response.model';
import {
  Cocomo2Stage1Estimation,
  CreateCocomo2Stage1EstimationRequest,
  UpdateRatingsCocomo2Stage1Request,
  UpdateActualResultsCocomo2Stage1Request,
  Language,
  ParameterSet
} from '../../models/cocomo2-stage1/cocomo2-stage1.models';

@Injectable({
  providedIn: 'root'
})
export class Cocomo2Stage1EstimationService {
  private apiUrl = `${environment.apiUrl}/cocomo2-stage1/estimations`;
  private paramSetsUrl = `${environment.apiUrl}/cocomo2-stage1/parameter-sets`;

  constructor(private http: HttpClient) { }

  /**
   * Get all estimations for a specific project
   */
  getEstimationsByProject(projectId: number): Observable<ApiResponse<Cocomo2Stage1Estimation[]>> {
    return this.http.get<ApiResponse<Cocomo2Stage1Estimation[]>>(`${this.apiUrl}/project/${projectId}`);
  }

  /**
   * Get a specific estimation by ID
   */
  getEstimationById(id: number): Observable<ApiResponse<Cocomo2Stage1Estimation>> {
    return this.http.get<ApiResponse<Cocomo2Stage1Estimation>>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new estimation
   */
  createEstimation(request: CreateCocomo2Stage1EstimationRequest): Observable<ApiResponse<Cocomo2Stage1Estimation>> {
    return this.http.post<ApiResponse<Cocomo2Stage1Estimation>>(this.apiUrl, request);
  }

  /**
   * Update estimation ratings
   */
  updateRatings(id: number, request: UpdateRatingsCocomo2Stage1Request): Observable<ApiResponse<Cocomo2Stage1Estimation>> {
    return this.http.put<ApiResponse<Cocomo2Stage1Estimation>>(`${this.apiUrl}/${id}/ratings`, request);
  }

  /**
   * Update actual results
   */
  updateActualResults(id: number, request: UpdateActualResultsCocomo2Stage1Request): Observable<ApiResponse<Cocomo2Stage1Estimation>> {
    return this.http.put<ApiResponse<Cocomo2Stage1Estimation>>(`${this.apiUrl}/${id}/actual-results`, request);
  }

  /**
   * Delete an estimation
   */
  deleteEstimation(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get default parameter sets
   */
  getDefaultParameterSets(): Observable<ApiResponse<ParameterSet[]>> {
    return this.http.get<ApiResponse<ParameterSet[]>>(`${this.paramSetsUrl}/default`);
  }

  /**
   * Get user's custom parameter sets
   */
  getUserParameterSets(): Observable<ApiResponse<ParameterSet[]>> {
    return this.http.get<ApiResponse<ParameterSet[]>>(`${this.paramSetsUrl}/my`);
  }
}
