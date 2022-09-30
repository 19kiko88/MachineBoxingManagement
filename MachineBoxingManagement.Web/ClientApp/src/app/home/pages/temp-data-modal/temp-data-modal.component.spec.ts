import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TempDataModalComponent } from './temp-data-modal.component';

describe('TempDataModalComponent', () => {
  let component: TempDataModalComponent;
  let fixture: ComponentFixture<TempDataModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TempDataModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TempDataModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
