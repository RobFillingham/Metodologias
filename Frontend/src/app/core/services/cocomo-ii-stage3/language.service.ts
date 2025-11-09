import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { ApiResponse, Language } from '../../models/cocomo-ii-stage3/cocomo.models';

export interface CreateLanguageDto {
  name: string;
  slocPerUfp: number;
}

export interface UpdateLanguageDto {
  name: string;
  slocPerUfp: number;
}

/**
 * Service for language-related operations
 */
@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  constructor(private apiService: ApiService) {}

  /**
   * Get all available languages
   */
  getLanguages(): Observable<ApiResponse<Language[]>> {
    return this.apiService.get<Language[]>('CocomoIIStage3/Languages');
  }

  /**
   * Get a specific language by ID
   */
  getLanguage(id: number): Observable<ApiResponse<Language>> {
    return this.apiService.get<Language>(`CocomoIIStage3/Languages/${id}`);
  }

  /**
   * Create a new language
   */
  createLanguage(dto: CreateLanguageDto): Observable<ApiResponse<Language>> {
    return this.apiService.post<Language>('CocomoIIStage3/Languages', dto);
  }

  /**
   * Update an existing language
   */
  updateLanguage(id: number, dto: UpdateLanguageDto): Observable<ApiResponse<Language>> {
    return this.apiService.put<Language>(`CocomoIIStage3/Languages/${id}`, dto);
  }

  /**
   * Delete a language
   */
  deleteLanguage(id: number): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`CocomoIIStage3/Languages/${id}`);
  }
}