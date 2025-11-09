import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from '../api.service';
import { StorageService } from '../storage.service';
import { ApiResponse, Project, CreateProjectRequest, UpdateProjectRequest } from '../../models/cocomo-ii-stage3/cocomo.models';

/**
 * Service for project-related operations
 */
@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private currentProjectSubject: BehaviorSubject<Project | null>;
  public currentProject: Observable<Project | null>;

  constructor(
    private apiService: ApiService,
    private storageService: StorageService
  ) {
    this.currentProjectSubject = new BehaviorSubject<Project | null>(
      this.getProjectFromStorage()
    );
    this.currentProject = this.currentProjectSubject.asObservable();
  }

  /**
   * Get current project value
   */
  public get currentProjectValue(): Project | null {
    return this.currentProjectSubject.value;
  }

  /**
   * Set current project
   */
  setCurrentProject(project: Project | null): void {
    this.currentProjectSubject.next(project);
    if (project) {
      this.storageService.setItem('currentProject', project);
    } else {
      this.storageService.removeItem('currentProject');
    }
  }

  /**
   * Get project from storage
   */
  private getProjectFromStorage(): Project | null {
    return this.storageService.getItem<Project>('currentProject');
  }

  /**
   * Get all projects for the current user
   */
  getProjects(): Observable<ApiResponse<Project[]>> {
    return this.apiService.get<Project[]>('CocomoIIStage3/Projects');
  }

  /**
   * Get a specific project by ID
   */
  getProject(id: number): Observable<ApiResponse<Project>> {
    return this.apiService.get<Project>(`CocomoIIStage3/Projects/${id}`);
  }

  /**
   * Create a new project
   */
  createProject(request: CreateProjectRequest): Observable<ApiResponse<Project>> {
    return this.apiService.post<Project>('CocomoIIStage3/Projects', request);
  }

  /**
   * Update an existing project
   */
  updateProject(id: number, request: UpdateProjectRequest): Observable<ApiResponse<Project>> {
    return this.apiService.put<Project>(`CocomoIIStage3/Projects/${id}`, request);
  }

  /**
   * Delete a project
   */
  deleteProject(id: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`CocomoIIStage3/Projects/${id}`);
  }
}