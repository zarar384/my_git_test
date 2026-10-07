import { ChangeDetectionStrategy, Component, TemplateRef, contentChild, input } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';

export interface TableColumn<T> {
  key: string;
  header: string;
  /** Optional accessor used for the mobile card view's label/value pairing. */
  accessor?: (row: T) => string;
}

/**
 * Generic table shell: renders a real <table> on wide viewports and a
 * stacked card list on narrow ones, using the same row template for both via
 * content projection, so no data is duplicated or special-cased per screen.
 */
@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [NgTemplateOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="data-table">
      <table class="data-table__table">
        <thead>
          <tr>
            @for (column of columns(); track column.key) {
              <th scope="col">{{ column.header }}</th>
            }
          </tr>
        </thead>
        <tbody>
          @for (row of rows(); track trackBy()(row)) {
            <ng-container *ngTemplateOutlet="rowTemplate(); context: { $implicit: row }" />
          }
        </tbody>
      </table>

      <div class="data-table__cards">
        @for (row of rows(); track trackBy()(row)) {
          <ng-container *ngTemplateOutlet="cardTemplate() ?? rowTemplate(); context: { $implicit: row }" />
        }
      </div>
    </div>
  `,
  styleUrl: './data-table.component.scss'
})
export class DataTableComponent<T> {
  readonly columns = input.required<TableColumn<T>[]>();
  readonly rows = input.required<T[]>();
  readonly trackBy = input<(row: T) => unknown>((row: T) => row);

  readonly rowTemplate = contentChild.required<TemplateRef<{ $implicit: T }>>('row');
  readonly cardTemplate = contentChild<TemplateRef<{ $implicit: T }>>('card');
}
