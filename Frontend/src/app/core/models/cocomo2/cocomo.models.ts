import { ApiResponse } from '../api-response.model';

export type { ApiResponse };

/**
 * COCOMO II related models and interfaces
 */

// Project models
export interface Project {
  projectId: number;
  userId: number;
  projectName: string;
  description?: string;
  createdAt: string;
}

export interface CreateProjectRequest {
  projectName: string;
  description?: string;
}

export interface UpdateProjectRequest {
  projectId: number;
  projectName: string;
  description?: string;
}

// Language models
export interface Language {
  languageId: number;
  name: string;
  slocPerUfp: number;
}

// Parameter Set models
export interface ParameterSet {
  paramSetId: number;
  userId?: number;
  setName: string;
  isDefault: boolean;
  // Constants
  constA: number;
  constB: number;
  constC: number;
  constD: number;
  // Scale Factors (SF)
  sfPrecVlo: number; sfPrecLo: number; sfPrecNom: number; sfPrecHi: number; sfPrecVhi: number; sfPrecXhi: number;
  sfFlexVlo: number; sfFlexLo: number; sfFlexNom: number; sfFlexHi: number; sfFlexVhi: number; sfFlexXhi: number;
  sfReslVlo: number; sfReslLo: number; sfReslNom: number; sfReslHi: number; sfReslVhi: number; sfReslXhi: number;
  sfTeamVlo: number; sfTeamLo: number; sfTeamNom: number; sfTeamHi: number; sfTeamVhi: number; sfTeamXhi: number;
  sfPmatVlo: number; sfPmatLo: number; sfPmatNom: number; sfPmatHi: number; sfPmatVhi: number; sfPmatXhi: number;
  // Effort Multipliers (EM)
  emPersXlo: number; emPersVlo: number; emPersLo: number; emPersNom: number; emPersHi: number; emPersVhi: number; emPersXhi: number;
  emRcpxXlo: number; emRcpxVlo: number; emRcpxLo: number; emRcpxNom: number; emRcpxHi: number; emRcpxVhi: number; emRcpxXhi: number;
  emPdifXlo: number; emPdifVlo: number; emPdifLo: number; emPdifNom: number; emPdifHi: number; emPdifVhi: number; emPdifXhi: number;
  emPrexXlo: number; emPrexVlo: number; emPrexLo: number; emPrexNom: number; emPrexHi: number; emPrexVhi: number; emPrexXhi: number;
  emRuseXlo: number; emRuseVlo: number; emRuseLo: number; emRuseNom: number; emRuseHi: number; emRuseVhi: number; emRuseXhi: number;
  emFcilXlo: number; emFcilVlo: number; emFcilLo: number; emFcilNom: number; emFcilHi: number; emFcilVhi: number; emFcilXhi: number;
  emScedXlo: number; emScedVlo: number; emScedLo: number; emScedNom: number; emScedHi: number; emScedVhi: number; emScedXhi: number;
}

export interface CreateParameterSetRequest {
  setName: string;
  isDefault: boolean;
  // Include all the constants and ratings
  constA: number;
  constB: number;
  constC: number;
  constD: number;
  // SF and EM fields...
}

// Estimation models
export interface Estimation {
  estimationId: number;
  projectId: number;
  paramSetId: number;
  languageId: number;
  estimationName: string;
  createdAt: string;
  // Selected ratings
  selectedSfPrec: string; selectedSfFlex: string; selectedSfResl: string; selectedSfTeam: string; selectedSfPmat: string;
  selectedEmPers: string; selectedEmRcpx: string; selectedEmPdif: string; selectedEmPrex: string;
  selectedEmRuse: string; selectedEmFcil: string; selectedEmSced: string;
  // Calculated results
  totalUfp?: number; sloc?: number; ksloc?: number; sumSf?: number; exponentE?: number; eaf?: number;
  effortPm?: number; tdevMonths?: number; avgTeamSize?: number;
  // Actual results
  actualEffortPm?: number; actualTdevMonths?: number; actualSloc?: number;
  // Related data
  language?: Language;
  parameterSet?: ParameterSet;
  functions?: EstimationFunction[];
}

export interface CreateEstimationRequest {
  estimationName: string;
  paramSetId: number;
  languageId: number;
  selectedSfPrec: string; selectedSfFlex: string; selectedSfResl: string; selectedSfTeam: string; selectedSfPmat: string;
  selectedEmPers: string; selectedEmRcpx: string; selectedEmPdif: string; selectedEmPrex: string;
  selectedEmRuse: string; selectedEmFcil: string; selectedEmSced: string;
}

export interface UpdateEstimationRatingsRequest {
  selectedSfPrec?: string; selectedSfFlex?: string; selectedSfResl?: string; selectedSfTeam?: string; selectedSfPmat?: string;
  selectedEmPers?: string; selectedEmRcpx?: string; selectedEmPdif?: string; selectedEmPrex?: string;
  selectedEmRuse?: string; selectedEmFcil?: string; selectedEmSced?: string;
}

export interface UpdateEstimationActualResultsRequest {
  actualEffortPm?: number;
  actualTdevMonths?: number;
  actualSloc?: number;
}

// Estimation Function models
export interface EstimationFunction {
  functionId: number;
  estimationId: number;
  name: string;
  type: 'EI' | 'EO' | 'EQ' | 'ILF' | 'EIF';
  det: number;
  retFtr: number;
  complexity?: string;
  calculatedPoints?: number;
}

export interface CreateEstimationFunctionRequest {
  name: string;
  type: 'EI' | 'EO' | 'EQ' | 'ILF' | 'EIF';
  det: number;
  retFtr: number;
}

export interface UpdateEstimationFunctionRequest {
  functionId: number;
  name: string;
  type: 'EI' | 'EO' | 'EQ' | 'ILF' | 'EIF';
  det: number;
  retFtr: number;
}

export interface BatchCreateEstimationFunctionsRequest {
  functions: CreateEstimationFunctionRequest[];
}

// Rating options
export const RATING_OPTIONS = ['XLO', 'VLO', 'LO', 'NOM', 'HI', 'VHI', 'XHI'] as const;
export type RatingOption = typeof RATING_OPTIONS[number];

// Function types
export const FUNCTION_TYPES = ['EI', 'EO', 'EQ', 'ILF', 'EIF'] as const;
export type FunctionType = typeof FUNCTION_TYPES[number];