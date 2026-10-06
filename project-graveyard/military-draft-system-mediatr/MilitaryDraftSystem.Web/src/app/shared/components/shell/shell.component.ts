import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss'
})
export class ShellComponent {
  protected readonly navLinks = [
    { path: '/dashboard', label: 'Dashboard' },
    { path: '/officers', label: 'Recruitment Officers' },
    { path: '/population', label: 'Population' },
    { path: '/cemetery', label: 'Cemetery' },
    { path: '/statistics', label: 'Statistics' }
  ];
}
