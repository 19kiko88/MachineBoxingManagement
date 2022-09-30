import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TempListModalComponent } from './take-in-modal.component';

describe('TempListModalComponent', () => {
  let component: TempListModalComponent;
  let fixture: ComponentFixture<TempListModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TempListModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TempListModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
