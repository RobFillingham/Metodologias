import { Component, Input, Output, EventEmitter, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Estimation, UpdateEstimationRatingsRequest } from '../../../../core/models/cocomo2/cocomo.models';
import { EstimationService } from '../../../../core/services/cocomo2/estimation.service';

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
      key: 'PERS',
      label: 'Personnel Capability (PERS)',
      field: 'selectedEmPers',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'RCPX',
      label: 'Product Reliability and Complexity (RCPX)',
      field: 'selectedEmRcpx',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'PDIF',
      label: 'Platform Difficulty (PDIF)',
      field: 'selectedEmPdif',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'PREX',
      label: 'Personnel Experience (PREX)',
      field: 'selectedEmPrex',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
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
      label: 'Reusability (RUSE)',
      field: 'selectedEmRuse',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
      ]
    },
    {
      key: 'FCIL',
      label: 'Facilities (FCIL)',
      field: 'selectedEmFcil',
      options: [
        { value: 'XLO', label: 'Extra Bajo' },
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
        { value: 'XLO', label: 'Extra Bajo' },
        { value: 'VLO', label: 'Muy Bajo' },
        { value: 'LO', label: 'Bajo' },
        { value: 'NOM', label: 'Nominal' },
        { value: 'HI', label: 'Alto' },
        { value: 'VHI', label: 'Muy Alto' },
        { value: 'XHI', label: 'Extra Alto' }
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
      selectedEmPers: this.estimation.selectedEmPers,
      selectedEmRcpx: this.estimation.selectedEmRcpx,
      selectedEmPdif: this.estimation.selectedEmPdif,
      selectedEmPrex: this.estimation.selectedEmPrex,
      selectedEmRuse: this.estimation.selectedEmRuse,
      selectedEmFcil: this.estimation.selectedEmFcil,
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
