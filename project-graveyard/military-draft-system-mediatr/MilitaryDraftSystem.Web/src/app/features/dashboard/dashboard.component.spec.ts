import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { OfficersStore } from '../officers/data/officers.store';
import { signal } from '@angular/core';
import { ApiError } from '../../core/api/models';

describe('DashboardComponent', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let loadingSignal: ReturnType<typeof signal<boolean>>;
  let errorSignal: ReturnType<typeof signal<ApiError | null>>;
  let loadedSignal: ReturnType<typeof signal<boolean>>;
  let hasActiveOfficerSignal: ReturnType<typeof signal<boolean>>;

  function configure(): void {
    loadingSignal = signal(false);
    errorSignal = signal<ApiError | null>(null);
    loadedSignal = signal(false);
    hasActiveOfficerSignal = signal(false);

    const storeStub = {
      loading: loadingSignal,
      error: errorSignal,
      loaded: loadedSignal,
      hasActiveOfficer: hasActiveOfficerSignal,
      currentOfficer: signal(null),
      reload: () => {}
    };

    TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [provideRouter([]), { provide: OfficersStore, useValue: storeStub }]
    });

    fixture = TestBed.createComponent(DashboardComponent);
  }

  beforeEach(() => configure());

  it('shows a loading state while officers are being fetched, not "no active career"', () => {
    loadingSignal.set(true);
    loadedSignal.set(false);
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).not.toContain('You have no active career');
    expect(fixture.nativeElement.querySelector('app-loading-state')).not.toBeNull();
  });

  it('shows an error state with retry when the officers request fails, not "no active career"', () => {
    loadingSignal.set(false);
    loadedSignal.set(false);
    errorSignal.set({ status: 0, kind: 'network', message: 'Network error' });
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).not.toContain('You have no active career');
    expect(fixture.nativeElement.querySelector('app-error-state')).not.toBeNull();
  });

  it('shows "no active career" only after a successful response confirms there is none', () => {
    loadingSignal.set(false);
    loadedSignal.set(true);
    errorSignal.set(null);
    hasActiveOfficerSignal.set(false);
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('You have no active career');
  });
});
