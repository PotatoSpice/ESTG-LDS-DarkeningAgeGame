import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameReconnectComponent } from './game-reconnect.component';

describe('GameReconnectComponent', () => {
  let component: GameReconnectComponent;
  let fixture: ComponentFixture<GameReconnectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameReconnectComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GameReconnectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
