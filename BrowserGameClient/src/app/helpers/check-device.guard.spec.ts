import { TestBed } from '@angular/core/testing';

import { CheckDeviceGuard } from './check-device.guard';

describe('CheckDeviceGuard', () => {
  let guard: CheckDeviceGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(CheckDeviceGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
