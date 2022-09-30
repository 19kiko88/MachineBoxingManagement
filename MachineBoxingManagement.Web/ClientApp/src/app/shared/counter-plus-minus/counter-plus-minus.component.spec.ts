import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounterPlusMinusComponent } from './counter-plus-minus.component';

describe('CounterPlusMinusComponent', () => {
  let component: CounterPlusMinusComponent;
  let fixture: ComponentFixture<CounterPlusMinusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CounterPlusMinusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CounterPlusMinusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
