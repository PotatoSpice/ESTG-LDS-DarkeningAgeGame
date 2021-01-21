# ESTG-LDS 2020/2021 | Game Web Server

*Makeshift repository* para toda a implementação relativa ao componente Servidor de Jogo, que trata de todo o funcionamento, lógica e dados de salas e jogos respetivos.

*O projeto foi desenvolvido pelo GitLab da instituição (ESTG), sendo agora trazido para este repositório GitHub.*

# Setup
Ambiente de desenvolvimento:

*após seguir os passos 1., 2. e 3. descritos pelo master README*

4. Verificar o IP de execução do servidor no ficheiro *./Properties/launchSettings.json*
  
  - Colocar o endereço local, *localhost* ou *127.0.0.1*, para a utilização das aplicações no mesmo PC
  
  - Para que o servidor fique visível em rede local ou [VPN (Hamachi)](https://vpn.net/), deverá ser inserido o wilcard *0.0.0.0* <br/>
    *ATENÇÃO: isto poderá tornar-se um risco de segurança se a rede a que o PC está ligado não fôr segura! Isto é só uma forma de tornar o multiplayer possível num ambiente de desenvolvimento.*

5. Atualizar as configurações no ficheiro '.appsettings' consoante necessário

6. Ligar o servidor de desenvolvimento
    ```sh
    dotnet run
    ```

# Notas Implementação

Por razões de segurança, os ficheiros de configuração *appsettings* com os dados de produção estão removidos do repositório através do *.gitignore*.

# Estrutura do repositório
Visto que o nosso *Game Server* faz uma mistura do padrão *MVC* com alguns padrões do .Net, como *Entity*, *Repository* e *Service*, temos no fundo 4 namespaces principais:
- Entities (GameWebServer.Entities)
  - Define modelos de dados, *entidades*, que descrevem os objetos guardados em memória, visto que o serviço não faz uso de armazenamento persistente, tal como métodos de controlo de acesso
- Models (GameWebServer.Models)
  - Define a estrutura das mensagens trocadas entre o cliente e o servidor pela conexão em *socket TCP*
- Repositories (GameWebServer.Repositories)
  - Métodos para acesso aos dados globais concorrentes da aplicação, guardados em memória
- Services (GameWebServer.Services)
  - Lógica e processamento de toda a informação

Da mesma forma, ferramentas e outras classes de utilidade temos:
- Exceptions (GameWebServer.Exceptions)
- Middlewares (GameWebServer.Middlewares)
  - Definem toda a lógica relativamente às conexões WebSocket
- Utils (GameWebServer.Utils)

### Models
- Requests (GameWebServer.Models.Requests)
  - Descrevem a estrutura das mensagens recebidas do cliente (socket)
- Responses (GameWebServer.Models.Responses)
  - Descrevem a estrutura das mensagens de retorno para o cliente (socket)

### Entities
- Game (GameWebServer.Entities.Game)
- Player (GameWebServer.Entities.Player)
- Room (GameWebServer.Entities.Room)
- DataManagers (GameWebServer.Entities)

# Implementação de novas funcionalidades
De forma a eficazmente implementar uma nova funcionalidade do jogo, é necessário seguir um conjunto determinado de passos que servirão para melhor organizar e optimizar a forma como o código é desenvolvimento.

O primeiro passo é efetivamente identificar qual a principal entidade sobre qual a informação da nova funcionalidade é referente. Por exemplo, algo referente uma entidade Fação ou as suas entidades relacionadas deverá ficar no seu “Manager” corresponde-te. Normalmente, deverá ser divida em métodos que sejam o mais atómicos e reutilizáveis possível, ou seja, um conjunto de métodos private que deverão ser chamados dentro de um método public que represente a totalidade da operação, contendo um conjunto de verificações que sejam dependentes da instância que está a guardar.
 
Depois da implementação desse método, este é incorporado na componente geral dentro da GameInstance. Após a implementação do método geral, este método deverá ser incorporado ao Protocolo, onde irá ser chamado pelo Evento recebido e recebendo a informação necessária para processar. 