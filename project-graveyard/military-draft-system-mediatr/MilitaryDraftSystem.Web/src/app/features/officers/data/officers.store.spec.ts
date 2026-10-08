import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { OfficersStore } from './officers.store';
import { environment } from '../../../../environments/environment';
import { RecruitmentOfficerSummaryDto } from '../../../core/api/models';

describe('OfficersStore', () => {
  let store: OfficersStore;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    store = TestBed.inject(OfficersStore);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loads officers and exposes the current player officer', () => {
    const officers: RecruitmentOfficerSummaryDto[] = [
      {
        id: 'o1',
        fullName: 'Jane Doe',
        department: 'Central',
        playerId: environment.defaultPlayerId,
        status: 'Active',
        isActive: true,
        hasEndedCareer: false
      },
      {
        id: 'o2',
        fullName: 'John Smith',
        department: 'North',
        playerId: 'someone-else',
        status: 'Retired',
        isActive: false,
        hasEndedCareer: true
      }
    ];

    store.reload();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/draft/officers`);
    req.flush(officers);

    expect(store.officers().length).toBe(2);
    expect(store.loaded()).toBe(true);
    expect(store.currentOfficer()?.id).toBe('o1');
    expect(store.hasActiveOfficer()).toBe(true);
    expect(store.myOfficers().length).toBe(1);
    expect(store.otherOfficers().length).toBe(1);
  });

  it('surfaces an ApiError on failure', () => {
    store.reload();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/draft/officers`);
    req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });

    expect(store.error()).not.toBeNull();
    expect(store.loading()).toBe(false);
  });
});
