import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomLobbyMatchmakingComponent } from './room-lobby-matchmaking.component';

describe('RoomLobbyMatchmakingComponent', () => {
  let component: RoomLobbyMatchmakingComponent;
  let fixture: ComponentFixture<RoomLobbyMatchmakingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomLobbyMatchmakingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomLobbyMatchmakingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
