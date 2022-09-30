import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoxOutComponent } from './box-out.component';

describe('BoxOutComponent', () => {
  let component: BoxOutComponent;
  let fixture: ComponentFixture<BoxOutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BoxOutComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BoxOutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
