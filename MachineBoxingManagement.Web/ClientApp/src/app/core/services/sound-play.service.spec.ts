import { TestBed } from '@angular/core/testing';

import { SoundPlayService } from './sound-play.service';

describe('SoundPlayService', () => {
  let service: SoundPlayService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SoundPlayService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
