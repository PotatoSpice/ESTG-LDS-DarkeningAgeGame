import { TestBed } from '@angular/core/testing';

import { SocketConnectionService } from './socket-connection.service';

describe('SocketConnectionService', () => {
  let service: SocketConnectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SocketConnectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
