import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { JwtTokenInterceptor } from './helpers/http/jwt-token.interceptor';
import { SessionEndInterceptor } from './helpers/http/session-end.interceptor';

import { SignInComponent } from './components/sign-in/sign-in.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';

import { MatchHistoryComponent } from './components/reusable-components/match-history/match-history.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { HomeSessionComponent } from './components/home-session/home-session.component';

import { HubComponent } from './components/home-session/hub/hub.component';
import { GameReconnectComponent } from './components/lobby-rooms/game-reconnect/game-reconnect.component';
import { ProfileComponent } from './components/home-session/profile/profile.component';
import { FriendListComponent } from './components/reusable-components/friend-list/friend-list.component';

import { ChatWindowComponent } from './components/lobby-rooms/chat-window/chat-window.component';
import { RoomLobbyCustomComponent } from './components/lobby-rooms/room-lobby-custom/room-lobby-custom.component';
import { RoomLobbyMatchmakingComponent } from './components/lobby-rooms/room-lobby-matchmaking/room-lobby-matchmaking.component';
import { RoomGameStartingComponent } from './components/lobby-rooms/room-game-starting/room-game-starting.component';
import { ToastComponent } from './components/reusable-components/toast/toast.component';

import { ConfirmPasswordDirective } from './helpers/confirm-password.directive';
import { RoomLobbyComponent } from './components/lobby-rooms/room-lobby/room-lobby.component';
import { NotificationsComponent } from './components/reusable-components/notifications/notifications.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    SignUpComponent,
    MatchHistoryComponent,
    HomePageComponent,
    HomeSessionComponent,
    HubComponent,
    ProfileComponent,
    FriendListComponent,
    ChatWindowComponent,
    GameReconnectComponent,
    RoomLobbyCustomComponent,
    RoomLobbyMatchmakingComponent,
    RoomGameStartingComponent,
    ToastComponent,
    ConfirmPasswordDirective,
    RoomLobbyComponent,
    NotificationsComponent,
    ResetPasswordComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgbModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtTokenInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SessionEndInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }