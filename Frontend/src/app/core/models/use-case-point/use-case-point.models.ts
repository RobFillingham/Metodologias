import { ApiResponse } from '../cocomo2/cocomo.models';

/**
 * Use Case Point Estimation interface
 */
export interface UseCasePointEstimation {
  ucpEstimationId: number;
  projectId: number;
  estimationName: string;
  simpleUccCount: number;
  averageUccCount: number;
  complexUccCount: number;
  simpleActorCount: number;
  averageActorCount: number;
  complexActorCount: number;
  unadjustedUcp?: number;
  technicalComplexityFactor?: number;
  environmentFactor?: number;
  adjustedUcp?: number;
  estimatedEffort?: number;
  estimatedEffortPm?: number;
  estimatedCost?: number;
  estimatedTime?: number;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  technicalFactors?: UseCaseTechnicalFactor[];
  environmentFactors?: UseCaseEnvironmentFactor[];
}

/**
 * Use Case Technical Factor interface
 */
export interface UseCaseTechnicalFactor {
  ucpTechFactorId: number;
  ucpEstimationId: number;
  factorName: string;
  factorWeight: number;
  createdAt: string;
  updatedAt: string;
}

/**
 * Use Case Environment Factor interface
 */
export interface UseCaseEnvironmentFactor {
  ucpEnvFactorId: number;
  ucpEstimationId: number;
  factorName: string;
  factorWeight: number;
  createdAt: string;
  updatedAt: string;
}

/**
 * Request to create a new Use Case Point estimation
 */
export interface CreateUseCasePointEstimationRequest {
  projectId: number;
  estimationName: string;
  simpleUccCount: number;
  averageUccCount: number;
  complexUccCount: number;
  simpleActorCount: number;
  averageActorCount: number;
  complexActorCount: number;
  notes?: string;
}

/**
 * Request to update an existing Use Case Point estimation
 */
export interface UpdateUseCasePointEstimationRequest {
  ucpEstimationId: number;
  projectId: number;
  estimationName: string;
  simpleUccCount: number;
  averageUccCount: number;
  complexUccCount: number;
  simpleActorCount: number;
  averageActorCount: number;
  complexActorCount: number;
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
