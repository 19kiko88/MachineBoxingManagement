import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizeCheckComponent } from './authorize-check.component';

describe('AuthorizeCheckComponent', () => {
  let component: AuthorizeCheckComponent;
  let fixture: ComponentFixture<AuthorizeCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizeCheckComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthorizeCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
