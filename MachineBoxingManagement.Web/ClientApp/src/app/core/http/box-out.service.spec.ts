import { TestBed } from '@angular/core/testing';

import { BoxOutService } from './box-out.service';

describe('BoxOutService', () => {
  let service: BoxOutService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoxOutService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
