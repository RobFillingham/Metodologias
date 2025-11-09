import { Component, Input, Output, EventEmitter, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Estimation, UpdateEstimationRatingsRequest } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';
import { EstimationService } from '../../../../core/services/cocomo-ii-stage3/estimation.service';

interface RatingDefinition {
  key: string;
  label: string;
  field: keyof UpdateEstimationRatingsRequest;
  options: { value: string; label: string }[];
}

@Component({
  selector: 'app-parameter-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './parameter-editor.component.html',
  styleUrls: ['./parameter-editor.component.css']
})
export class ParameterEditorComponent implements OnInit {
  @Input() estimation!: Estimation;
  @Output() onSaved = new EventEmitter<void>();

  isEditing = signal(false);
  isSaving = signal(false);
  errorMessage = signal<string | null>(null);

  // Working copy of ratings
  ratings: UpdateEstimationRatingsRequest = {};

  // Scale Factor definitions
  sfRatings: RatingDefinition[] = [
    {
      key: 'PREC',
      label: 'Precedentedness (PREC)',
      field: 'selectedSfPrec',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'FLEX',
      label: 'Development Flexibility (FLEX)',
      field: 'selectedSfFlex',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'RESL',
      label: 'Architecture/Risk Resolution (RESL)',
      field: 'selectedSfResl',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'TEAM',
      label: 'Team Cohesion (TEAM)',
      field: 'selectedSfTeam',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'PMAT',
      label: 'Process Maturity (PMAT)',
      field: 'selectedSfPmat',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    }
  ];

  // Effort Multiplier definitions
  emRatings: RatingDefinition[] = [
    {
      key: 'RELY',
      label: 'Required Software Reliability (RELY)',
      field: 'selectedEmRely',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'DATA',
      label: 'Data Base Size (DATA)',
      field: 'selectedEmData',
      options: [
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'CPLX',
      label: 'Product Complexity (CPLX)',
      field: 'selectedEmCplx',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'RUSE',
      label: 'Required Reusability (RUSE)',
      field: 'selectedEmRuse',
      options: [
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'DOCU',
      label: 'Documentation Match to Life-Cycle Needs (DOCU)',
      field: 'selectedEmDocu',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'TIME',
      label: 'Execution Time Constraint (TIME)',
      field: 'selectedEmTime',
      options: [
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'STOR',
      label: 'Main Storage Constraint (STOR)',
      field: 'selectedEmStor',
      options: [
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'PVOL',
      label: 'Platform Volatility (PVOL)',
      field: 'selectedEmPvol',
      options: [
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' }
      ]
    },
    {
      key: 'ACAP',
      label: 'Analyst Capability (ACAP)',
      field: 'selectedEmAcap',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'PCAP',
      label: 'Programmer Capability (PCAP)',
      field: 'selectedEmPcap',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'PCON',
      label: 'Personnel Continuity (PCON)',
      field: 'selectedEmPcon',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'APEX',
      label: 'Applications Experience (APEX)',
      field: 'selectedEmApex',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'PLEX',
      label: 'Platform Experience (PLEX)',
      field: 'selectedEmPlex',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'LTEX',
      label: 'Language and Tool Experience (LTEX)',
      field: 'selectedEmLtex',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'TOOL',
      label: 'Use of Software Tools (TOOL)',
      field: 'selectedEmTool',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    },
    {
      key: 'SITE',
      label: 'Multisite Development (SITE)',
      field: 'selectedEmSite',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'SCED',
      label: 'Required Development Schedule (SCED)',
      field: 'selectedEmSced',
      options: [
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' }
      ]
    }
  ];

  constructor(private estimationService: EstimationService) {}

  ngOnInit(): void {
    this.loadCurrentRatings();
  }

  /**
   * Load current ratings from estimation
   */
  loadCurrentRatings(): void {
    this.ratings = {
      selectedSfPrec: this.estimation.selectedSfPrec,
      selectedSfFlex: this.estimation.selectedSfFlex,
      selectedSfResl: this.estimation.selectedSfResl,
      selectedSfTeam: this.estimation.selectedSfTeam,
      selectedSfPmat: this.estimation.selectedSfPmat,
      selectedEmRely: this.estimation.selectedEmRely,
      selectedEmData: this.estimation.selectedEmData,
      selectedEmCplx: this.estimation.selectedEmCplx,
      selectedEmRuse: this.estimation.selectedEmRuse,
      selectedEmDocu: this.estimation.selectedEmDocu,
      selectedEmTime: this.estimation.selectedEmTime,
      selectedEmStor: this.estimation.selectedEmStor,
      selectedEmPvol: this.estimation.selectedEmPvol,
      selectedEmAcap: this.estimation.selectedEmAcap,
      selectedEmPcap: this.estimation.selectedEmPcap,
      selectedEmPcon: this.estimation.selectedEmPcon,
      selectedEmApex: this.estimation.selectedEmApex,
      selectedEmPlex: this.estimation.selectedEmPlex,
      selectedEmLtex: this.estimation.selectedEmLtex,
      selectedEmTool: this.estimation.selectedEmTool,
      selectedEmSite: this.estimation.selectedEmSite,
      selectedEmSced: this.estimation.selectedEmSced
    };
  }

  /**
   * Toggle edit mode
   */
  toggleEdit(): void {
    if (this.isEditing()) {
      this.loadCurrentRatings(); // Reset changes
    }
    this.isEditing.set(!this.isEditing());
    this.errorMessage.set(null);
  }

  /**
   * Save updated ratings
   */
  saveRatings(): void {
    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.estimationService
      .updateEstimationRatings(this.estimation.projectId, this.estimation.estimationId, this.ratings)
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.isSaving.set(false);
            this.isEditing.set(false);
            this.onSaved.emit();
          } else {
            this.isSaving.set(false);
            this.errorMessage.set(response.errors?.[0] || 'Error al guardar las calificaciones');
          }
        },
        error: (err) => {
          this.isSaving.set(false);
          this.errorMessage.set('Error al guardar las calificaciones. Por favor, intenta de nuevo.');
          console.error('Error saving ratings:', err);
        }
      });
  }
}
