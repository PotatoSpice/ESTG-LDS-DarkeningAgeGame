import { TestBed } from '@angular/core/testing';

import { SessionEndInterceptor } from './session-end.interceptor';

describe('SessionEndInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      SessionEndInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: SessionEndInterceptor = TestBed.inject(SessionEndInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
