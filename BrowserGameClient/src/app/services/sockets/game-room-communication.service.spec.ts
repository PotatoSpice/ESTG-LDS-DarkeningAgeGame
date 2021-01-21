import { TestBed } from '@angular/core/testing';

import { GameRoomCommunicationService } from './game-room-communication.service';

describe('GameRoomCommunicationService', () => {
  let service: GameRoomCommunicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GameRoomCommunicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
