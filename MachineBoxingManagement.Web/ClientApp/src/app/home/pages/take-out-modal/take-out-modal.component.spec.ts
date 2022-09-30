import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TakeOutModalComponent } from './take-out-modal.component';

describe('TakeOutModalComponent', () => {
  let component: TakeOutModalComponent;
  let fixture: ComponentFixture<TakeOutModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TakeOutModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TakeOutModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
