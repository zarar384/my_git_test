import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CemeteryStore } from './cemetery.store';
import { environment } from '../../../../environments/environment';
import { CemeteryRecordDto } from '../../../core/api/models';

describe('CemeteryStore', () => {
  let store: CemeteryStore;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    store = TestBed.inject(CemeteryStore);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loads cemetery records', () => {
    const records: CemeteryRecordDto[] = [
      {
        id: 'r1',
        subjectType: 'Citizen',
        subjectId: 'c1',
        fullName: 'Jane Doe',
        reason: 'OldAge',
        diedAt: '2026-01-01T12:00:00+00:00',
        originalRecordDeleted: false
      }
    ];

    store.reload();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/population/cemetery`);
    req.flush(records);

    expect(store.records().length).toBe(1);
    expect(store.loaded()).toBe(true);
  });
});
