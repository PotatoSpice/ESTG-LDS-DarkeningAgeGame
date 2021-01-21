<p align="center">
  <h1 align="center">ESTG-LDS 2020/2021 | Darkening Age</h1>
  
  <p align="center">
    Projeto desenvolvimento para a UC de Laboratório de Desenvolvimento de Software, ano letivo 2020/2021.
    <br/>
    Um simples *multiplayer Real Time Stategy game* onde quatro jogadores competem numa espécie de tabuleiro 2D, cada um rivalizando para estabelecer dominância sobre os outros.
  </p>
</p>

<p align="center">
  <a href="https://github.com/PotatoSpice/ESTG-LDS-DarkeningAgeGame">
    <img src="https://user-images.githubusercontent.com/44165718/105226974-9cbfd780-5b58-11eb-99dd-29e7ae8dc64b.png" alt="Logo" width="165" height="158">
  </a>
  
  <p align="center">
    <a href="https://github.com/PotatoSpice/ESTG-LDS-DarkeningAgeGame/wiki">Check Wiki</a>
    ·
    <a href="https://github.com/PotatoSpice/ESTG-LDS-DarkeningAgeGame/issues">Report Bug</a>
    ·
    <a href="https://github.com/PotatoSpice/ESTG-LDS-DarkeningAgeGame/issues">Request Feature</a>
  </p>
</p>

# Organização do Repositório

- Visto no diagrama abaixo e tendo em conta que as várias aplicações desenvolvidas estão num repositório único, existirá para cada componente do projeto um GIT [Orphan Branche](https://www.git-scm.com/docs/git-checkout#Documentation/git-checkout.txt---orphanltnewbranchgt). 

- Isto é, existe um branch para a API, para o *Game Client* no browser, para a GUI do jogo em Unity e outro para o servidor dedicado do jogo.

- Por sua vez, o *master branch* contém cada uma das aplicações, em princípio, com a versão mais atualizada sobre o *deployment*. Contudo, não chegou a realizar-se nenhum *deploy* das aplicações durante o projeto, ou seja, todas as aplicações devem ser executadas em ambiente de desenvolvimento.

![repo_organization](https://user-images.githubusercontent.com/44165718/105228731-d42f8380-5b5a-11eb-8c93-5df87addd30a.jpg)

# Frameworks e Tecnologia

### *Game Server*
Framework:
- [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-3.1?view=aspnetcore-3.1)

Principais bibliotecas:
- [Newtonsoft Json](https://www.newtonsoft.com/json) - mapeamento de ficheiros e objetos JSON
- [WebSockets](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-3.1) - comunicação em real-time

### *Game API*
Framework:
- [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-3.1?view=aspnetcore-3.1)

Principais bibliotecas:
- [Entity Framework](https://docs.microsoft.com/en-us/ef/) - armazenamento persistente
- [JWT Authentication](https://jwt.io/) - sessão e autenticação
- [AutoMapper](https://docs.automapper.org/en/stable/) - mapeamento de entidades
- [Swagger API docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-3.1) - documentação

### *Game Client*
Framework:
- [Angular v11](https://angular.io/)

Principais bibliotecas:
- [Bootstrap 4](https://getbootstrap.com/docs/4.6/getting-started/introduction/) - construção páginas web
- [RxJS](https://angular.io/guide/rx-library) - *event handling* e *reactive programming*, [docs](https://www.learnrxjs.io/)

### *Game GUI*
Framework:
- [Unity v2019.4.x](https://unity.com/solutions/game) | [download](https://unity3d.com/get-unity/download)

# Setup

Cada uma das aplicações requer um *setup* em específico, presente no próprio README de cada aplicação. No entanto, os comandos seguintes devem ser executados para qualquer um dos repositórios:

1. Clone do repositório:
```sh
git clone https://github.com/PotatoSpice/ESTG-LDS-DarkeningAgeGame.git
```
2. Checkout para o branch da aplicação, como exemplo:
```sh
git checkout -b gameServerDev origin/gameServerDev
```
3. Mudar para a pasta do projeto, como exemplo:
```sh
cd ./GameServer
```