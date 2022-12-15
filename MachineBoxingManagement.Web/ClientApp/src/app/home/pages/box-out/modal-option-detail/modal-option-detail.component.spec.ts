import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalOptionDetailComponent } from './modal-option-detail.component';

describe('ModalOptionDetailComponent', () => {
  let component: ModalOptionDetailComponent;
  let fixture: ComponentFixture<ModalOptionDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalOptionDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalOptionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
