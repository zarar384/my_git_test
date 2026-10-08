import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { CemeteryStore } from './data/cemetery.store';
import {
  BadgeComponent,
  CardComponent,
  DataTableComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
  TableColumn
} from '../../shared/components';
import { CemeteryRecordDto } from '../../core/api/models';
import { deathReasonTone, humanizeEnumValue } from '../../shared/utils/enum-display';

@Component({
  selector: 'app-cemetery',
  standalone: true,
  imports: [
    DatePipe,
    BadgeComponent,
    CardComponent,
    DataTableComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './cemetery.component.html',
  styleUrl: './cemetery.component.scss'
})
export class CemeteryComponent implements OnInit {
  protected readonly store = inject(CemeteryStore);
  protected readonly tone = deathReasonTone;
  protected readonly humanize = humanizeEnumValue;

  protected readonly columns: TableColumn<CemeteryRecordDto>[] = [
    { key: 'fullName', header: 'Name' },
    { key: 'subjectType', header: 'Type' },
    { key: 'reason', header: 'Reason' },
    { key: 'diedAt', header: 'Died at' }
  ];

  ngOnInit(): void {
    if (!this.store.loaded()) {
      this.store.reload();
    }
  }

  protected trackById = (record: CemeteryRecordDto) => record.id;
}
