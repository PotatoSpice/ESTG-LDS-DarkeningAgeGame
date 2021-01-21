import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomGameStartingComponent } from './room-game-starting.component';

describe('RoomGameStartingComponent', () => {
  let component: RoomGameStartingComponent;
  let fixture: ComponentFixture<RoomGameStartingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomGameStartingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomGameStartingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
