import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../models/api-response.model';
import {
  Component,
  CreateComponentRequest,
  UpdateComponentRequest,
  CreateBatchComponentsRequest
} from '../../models/cocomo2-stage1/cocomo2-stage1.models';

@Injectable({
  providedIn: 'root'
})
export class Cocomo2Stage1ComponentService {
  private apiUrl = `${environment.apiUrl}/cocomo2-stage1/estimations`;

  constructor(private http: HttpClient) { }

  /**
   * Get all components for an estimation
   */
  getEstimationComponents(estimationId: number): Observable<ApiResponse<Component[]>> {
    return this.http.get<ApiResponse<Component[]>>(`${this.apiUrl}/${estimationId}/components`);
  }

  /**
   * Get a specific component by ID
   */
  getComponentById(estimationId: number, componentId: number): Observable<ApiResponse<Component>> {
    return this.http.get<ApiResponse<Component>>(`${this.apiUrl}/${estimationId}/components/${componentId}`);
  }

  /**
   * Create a new component
   */
  createComponent(estimationId: number, request: CreateComponentRequest): Observable<ApiResponse<Component>> {
    return this.http.post<ApiResponse<Component>>(`${this.apiUrl}/${estimationId}/components`, request);
  }

  /**
   * Create multiple components in batch
   */
  createBatchComponents(estimationId: number, request: CreateBatchComponentsRequest): Observable<ApiResponse<Component[]>> {
    return this.http.post<ApiResponse<Component[]>>(`${this.apiUrl}/${estimationId}/components/batch`, request);
  }

  /**
   * Update a component
   */
  updateComponent(estimationId: number, componentId: number, request: UpdateComponentRequest): Observable<ApiResponse<Component>> {
    return this.http.put<ApiResponse<Component>>(`${this.apiUrl}/${estimationId}/components/${componentId}`, request);
  }

  /**
   * Delete a component
   */
  deleteComponent(estimationId: number, componentId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${estimationId}/components/${componentId}`);
  }
}
