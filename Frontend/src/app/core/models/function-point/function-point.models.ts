import { ApiResponse } from '../cocomo2/cocomo.models';

/**
 * Function Point Estimation interface
 */
export interface FunctionPointEstimation {
  fpEstimationId: number;
  projectId: number;
  estimationName: string;
  externalInputs: number;
  externalOutputs: number;
  externalInquiries: number;
  internalLogicalFiles: number;
  externalInterfaceFiles: number;
  complexityLevel: string;
  unadjustedFp?: number;
  valueAdjustmentFactor?: number;
  adjustedFp?: number;
  estimatedEffort?: number;
  estimatedCost?: number;
  estimatedTime?: number;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  characteristics?: FunctionPointCharacteristic[];
}

/**
 * Function Point Characteristic interface
 */
export interface FunctionPointCharacteristic {
  fpCharId: number;
  fpEstimationId: number;
  characteristicName: string;
  influenceLevel: string;
  score: number;
  createdAt: string;
  updatedAt: string;
}

/**
 * Request to create a new Function Point estimation
 */
export interface CreateFunctionPointEstimationRequest {
  projectId: number;
  estimationName: string;
  externalInputs: number;
  externalOutputs: number;
  externalInquiries: number;
  internalLogicalFiles: number;
  externalInterfaceFiles: number;
  complexityLevel: string;
  notes?: string;
}

/**
 * Request to update an existing Function Point estimation
 */
export interface UpdateFunctionPointEstimationRequest {
  fpEstimationId: number;
  projectId: number;
  estimationName: string;
  externalInputs: number;
  externalOutputs: number;
  externalInquiries: number;
  internalLogicalFiles: number;
  externalInterfaceFiles: number;
  complexityLevel: string;
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

/**
 * Complexity levels for Function Point estimation
 */
export const ComplexityLevels = {
  LOW: 'LOW',
  AVERAGE: 'AVERAGE',
  HIGH: 'HIGH'
} as const;

export type ComplexityLevel = typeof ComplexityLevels[keyof typeof ComplexityLevels];
