import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomLobbyCustomComponent } from './room-lobby-custom.component';

describe('RoomLobbyCustomComponent', () => {
  let component: RoomLobbyCustomComponent;
  let fixture: ComponentFixture<RoomLobbyCustomComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomLobbyCustomComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomLobbyCustomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
