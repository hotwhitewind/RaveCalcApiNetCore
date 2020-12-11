import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserdateComponent } from './userdate.component';

describe('UserdateComponent', () => {
  let component: UserdateComponent;
  let fixture: ComponentFixture<UserdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserdateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
