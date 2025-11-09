import { ApiResponse } from '../api-response.model';

export type { ApiResponse };

/**
 * COCOMO II Stage 3 related models and interfaces
 */

// Project models (reuses same Project table)
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
  // Scale Factors (SF) - 5 factors, 7 ratings each
  sfPrecXlo: number; sfPrecVlo: number; sfPrecLo: number; sfPrecNom: number; sfPrecHi: number; sfPrecVhi: number; sfPrecXhi: number;
  sfFlexXlo: number; sfFlexVlo: number; sfFlexLo: number; sfFlexNom: number; sfFlexHi: number; sfFlexVhi: number; sfFlexXhi: number;
  sfReslXlo: number; sfReslVlo: number; sfReslLo: number; sfReslNom: number; sfReslHi: number; sfReslVhi: number; sfReslXhi: number;
  sfTeamXlo: number; sfTeamVlo: number; sfTeamLo: number; sfTeamNom: number; sfTeamHi: number; sfTeamVhi: number; sfTeamXhi: number;
  sfPmatXlo: number; sfPmatVlo: number; sfPmatLo: number; sfPmatNom: number; sfPmatHi: number; sfPmatVhi: number; sfPmatXhi: number;
  // Effort Multipliers (EM) - 17 factors, 7 ratings each
  emRelyXlo: number; emRelyVlo: number; emRelyLo: number; emRelyNom: number; emRelyHi: number; emRelyVhi: number; emRelyXhi: number;
  emDataXlo: number; emDataVlo: number; emDataLo: number; emDataNom: number; emDataHi: number; emDataVhi: number; emDataXhi: number;
  emCplxXlo: number; emCplxVlo: number; emCplxLo: number; emCplxNom: number; emCplxHi: number; emCplxVhi: number; emCplxXhi: number;
  emRuseXlo: number; emRuseVlo: number; emRuseLo: number; emRuseNom: number; emRuseHi: number; emRuseVhi: number; emRuseXhi: number;
  emDocuXlo: number; emDocuVlo: number; emDocuLo: number; emDocuNom: number; emDocuHi: number; emDocuVhi: number; emDocuXhi: number;
  emTimeXlo: number; emTimeVlo: number; emTimeLo: number; emTimeNom: number; emTimeHi: number; emTimeVhi: number; emTimeXhi: number;
  emStorXlo: number; emStorVlo: number; emStorLo: number; emStorNom: number; emStorHi: number; emStorVhi: number; emStorXhi: number;
  emPvolXlo: number; emPvolVlo: number; emPvolLo: number; emPvolNom: number; emPvolHi: number; emPvolVhi: number; emPvolXhi: number;
  emAcapXlo: number; emAcapVlo: number; emAcapLo: number; emAcapNom: number; emAcapHi: number; emAcapVhi: number; emAcapXhi: number;
  emPcapXlo: number; emPcapVlo: number; emPcapLo: number; emPcapNom: number; emPcapHi: number; emPcapVhi: number; emPcapXhi: number;
  emPconXlo: number; emPconVlo: number; emPconLo: number; emPconNom: number; emPconHi: number; emPconVhi: number; emPconXhi: number;
  emApexXlo: number; emApexVlo: number; emApexLo: number; emApexNom: number; emApexHi: number; emApexVhi: number; emApexXhi: number;
  emPlexXlo: number; emPlexVlo: number; emPlexLo: number; emPlexNom: number; emPlexHi: number; emPlexVhi: number; emPlexXhi: number;
  emLtexXlo: number; emLtexVlo: number; emLtexLo: number; emLtexNom: number; emLtexHi: number; emLtexVhi: number; emLtexXhi: number;
  emToolXlo: number; emToolVlo: number; emToolLo: number; emToolNom: number; emToolHi: number; emToolVhi: number; emToolXhi: number;
  emSiteXlo: number; emSiteVlo: number; emSiteLo: number; emSiteNom: number; emSiteHi: number; emSiteVhi: number; emSiteXhi: number;
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
  // Selected ratings - SF
  selectedSfPrec: string; selectedSfFlex: string; selectedSfResl: string; selectedSfTeam: string; selectedSfPmat: string;
  // Selected ratings - EM (17 factors)
  selectedEmRely: string; selectedEmData: string; selectedEmCplx: string; selectedEmRuse: string;
  selectedEmDocu: string; selectedEmTime: string; selectedEmStor: string; selectedEmPvol: string;
  selectedEmAcap: string; selectedEmPcap: string; selectedEmPcon: string; selectedEmApex: string;
  selectedEmPlex: string; selectedEmLtex: string; selectedEmTool: string; selectedEmSite: string; selectedEmSced: string;
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
  // SF ratings
  selectedSfPrec: string; selectedSfFlex: string; selectedSfResl: string; selectedSfTeam: string; selectedSfPmat: string;
  // EM ratings (17 factors)
  selectedEmRely: string; selectedEmData: string; selectedEmCplx: string; selectedEmRuse: string;
  selectedEmDocu: string; selectedEmTime: string; selectedEmStor: string; selectedEmPvol: string;
  selectedEmAcap: string; selectedEmPcap: string; selectedEmPcon: string; selectedEmApex: string;
  selectedEmPlex: string; selectedEmLtex: string; selectedEmTool: string; selectedEmSite: string; selectedEmSced: string;
}

export interface UpdateEstimationRatingsRequest {
  // SF ratings (optional)
  selectedSfPrec?: string; selectedSfFlex?: string; selectedSfResl?: string; selectedSfTeam?: string; selectedSfPmat?: string;
  // EM ratings (17 factors, optional)
  selectedEmRely?: string; selectedEmData?: string; selectedEmCplx?: string; selectedEmRuse?: string;
  selectedEmDocu?: string; selectedEmTime?: string; selectedEmStor?: string; selectedEmPvol?: string;
  selectedEmAcap?: string; selectedEmPcap?: string; selectedEmPcon?: string; selectedEmApex?: string;
  selectedEmPlex?: string; selectedEmLtex?: string; selectedEmTool?: string; selectedEmSite?: string; selectedEmSced?: string;
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
