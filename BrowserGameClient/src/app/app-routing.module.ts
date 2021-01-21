import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SignInComponent } from './components/sign-in/sign-in.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

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
import { RoomLobbyComponent } from './components/lobby-rooms/room-lobby/room-lobby.component';

import { CheckDeviceGuard } from './helpers/check-device.guard';


const routes: Routes = [
  {
    path: 'sign-in',
    component: SignInComponent,
  },
  {
    path: 'sign-up',
    component: SignUpComponent,
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
  },
  {
    path: '',
    component: HomePageComponent // página para users sem sessão iniciada
  },
  {
    path: 'session',
    component: HomeSessionComponent, // página para users com sessão iniciada, verifica e guarda a sessão
    children: [
      // ####
      // página principal e dados sobre utilizador
      {
        path: 'hub',
        component: HubComponent, // contém o conteúdo do mockup Hub
        children: [
          {
            path: 'friend-list/hub',
            component: FriendListComponent // componente reutilizável da lista de amigos
          },
          {
            path: 'match-history/:userId',
            component: MatchHistoryComponent // componente reutilizável da lista de partidas
          }
        ]
      },
      { path: ':userId/profile', component: ProfileComponent }, // mostrar, editar ou apagar detalhes de perfil do utilizador
      // ####
      // páginas de salas e abertura do jogo
      {
        path: 'lobby',
        component: RoomLobbyComponent,
        canActivate: [CheckDeviceGuard],
        children: [
          {
            path: 'custom',
            component: RoomLobbyCustomComponent
          },
          {
            path: 'find-match',
            component: RoomLobbyMatchmakingComponent
          },
          {
            path: 'chat',
            component: ChatWindowComponent
          },
          {
            path: 'friend-list/lobby',
            component: FriendListComponent
          }
        ]
      },
      {
        path: 'game-starting',
        component: RoomGameStartingComponent,
        canActivate: [CheckDeviceGuard],
        children: [
          {
            path: 'chat',
            component: ChatWindowComponent
          }
        ]
      },
      { path: 'reconnect', component: GameReconnectComponent, canActivate: [CheckDeviceGuard] }
    ]
  },
  { path: "**", redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
