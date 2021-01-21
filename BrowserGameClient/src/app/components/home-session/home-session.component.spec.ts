import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeSessionComponent } from './home-session.component';

describe('HomeSessionComponent', () => {
  let component: HomeSessionComponent;
  let fixture: ComponentFixture<HomeSessionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeSessionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeSessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
