// COCOMO II Stage 1 Models

export interface Project {
  projectId: number;
  projectName: string;
  description?: string;
  createdAt: string;
}

export interface Language {
  languageId: number;
  name: string;
  slocPerUfp: number;
}

export interface ParameterSet {
  paramSetId: number;
  setName: string;
  isDefault: boolean;
  constA: number;
  constB: number;
  createdAt?: string;
  userId?: number;
}

export interface Component {
  componentId: number;
  estimationId: number;
  componentName: string;
  description?: string;
  componentType: string; // 'new', 'reused', 'modified'
  sizeFp: number;
  reusePercent?: number;
  changePercent?: number;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Cocomo2Stage1Estimation {
  estimationId: number;
  projectId: number;
  estimationName: string;
  createdAt: string;
  updatedAt: string;

  // Selected ratings
  selectedAexp: string;
  selectedPexp: string;
  selectedPrec: string;
  selectedRely: string;
  selectedTmsp: string;

  // Calculated results
  totalFp?: number;
  sloc?: number;
  ksloc?: number;
  aexpMultiplier?: number;
  pexpMultiplier?: number;
  precMultiplier?: number;
  relyMultiplier?: number;
  tmspMultiplier?: number;
  eaf?: number;
  effortPm?: number;
  effortHours?: number;

  // Actual results
  actualEffortPm?: number;
  actualEffortHours?: number;
  actualSloc?: number;

  // Related entities
  language?: Language;
  parameterSet?: ParameterSet;
  components?: Component[];
}

export interface CreateCocomo2Stage1EstimationRequest {
  projectId: number;
  estimationName: string;
  paramSetId: number;
  languageId: number;
  selectedAexp: string;
  selectedPexp: string;
  selectedPrec: string;
  selectedRely: string;
  selectedTmsp: string;
}

export interface UpdateRatingsCocomo2Stage1Request {
  selectedAexp?: string;
  selectedPexp?: string;
  selectedPrec?: string;
  selectedRely?: string;
  selectedTmsp?: string;
}

export interface UpdateActualResultsCocomo2Stage1Request {
  actualEffortPm?: number;
  actualEffortHours?: number;
  actualSloc?: number;
}

export interface CreateComponentRequest {
  componentName: string;
  description?: string;
  componentType: string;
  sizeFp: number;
  reusePercent?: number;
  changePercent?: number;
  notes?: string;
}

export interface UpdateComponentRequest {
  componentId: number;
  componentName: string;
  description?: string;
  componentType: string;
  sizeFp: number;
  reusePercent?: number;
  changePercent?: number;
  notes?: string;
}

export interface CreateBatchComponentsRequest {
  components: CreateComponentRequest[];
}
