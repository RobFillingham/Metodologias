import { ApiResponse } from '../cocomo2/cocomo.models';

/**
 * KLOC Estimation interface
 */
export interface KlocEstimation {
  klocEstimationId: number;
  projectId: number;
  estimationName: string;
  linesOfCode: number;
  estimatedEffort?: number;
  estimatedCost?: number;
  estimatedTime?: number;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

/**
 * Request to create a new KLOC estimation
 */
export interface CreateKlocEstimationRequest {
  projectId: number;
  estimationName: string;
  linesOfCode: number;
  notes?: string;
}

/**
 * Request to update an existing KLOC estimation
 */
export interface UpdateKlocEstimationRequest {
  klocEstimationId: number;
  estimationName: string;
  linesOfCode: number;
  notes?: string;
}

/**
 * Project interface (shared)
 */
export interface Project {
  projectId: number;
  userId: number;
  projectName: string;
  description?: string;
  createdAt: string;
}
