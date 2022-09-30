import { TestBed } from '@angular/core/testing';

import { BoxInService } from './box-in.service';

describe('BoxInService', () => {
  let service: BoxInService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoxInService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
