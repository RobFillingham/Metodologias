import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, Language } from '../models/cocomo.models';

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
    return this.apiService.get<Language[]>('Languages');
  }

  /**
   * Get a specific language by ID
   */
  getLanguage(id: number): Observable<ApiResponse<Language>> {
    return this.apiService.get<Language>(`Languages/${id}`);
  }
}