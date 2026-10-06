import { ChangeDetectionStrategy, Component, ElementRef, input, output, viewChild, effect } from '@angular/core';
import { ButtonComponent } from '../button/button.component';

/**
 * Accessible confirmation dialog, used only for consequential / irreversible
 * actions (career-ending officer actions). Uses the native <dialog> element
 * for built-in focus trapping and Escape-to-close behavior.
 */
@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <dialog #dialogEl class="confirm-dialog" (close)="cancelled.emit()">
      <h3 class="confirm-dialog__title">{{ title() }}</h3>
      <p class="confirm-dialog__message">{{ message() }}</p>
      <div class="confirm-dialog__actions">
        <app-button variant="secondary" (pressed)="close(false)">{{ cancelLabel() }}</app-button>
        <app-button variant="danger" [busy]="busy()" (pressed)="close(true)">{{ confirmLabel() }}</app-button>
      </div>
    </dialog>
  `,
  styleUrl: './confirm-dialog.component.scss'
})
export class ConfirmDialogComponent {
  readonly title = input.required<string>();
  readonly message = input.required<string>();
  readonly confirmLabel = input('Confirm');
  readonly cancelLabel = input('Cancel');
  readonly open = input(false);
  readonly busy = input(false);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  private readonly dialogRef = viewChild.required<ElementRef<HTMLDialogElement>>('dialogEl');

  constructor() {
    effect(() => {
      const dialog = this.dialogRef().nativeElement;
      if (this.open() && !dialog.open) {
        dialog.showModal();
      } else if (!this.open() && dialog.open) {
        dialog.close();
      }
    });
  }

  close(confirm: boolean): void {
    if (confirm) {
      this.confirmed.emit();
    } else {
      this.dialogRef().nativeElement.close();
      this.cancelled.emit();
    }
  }
}
