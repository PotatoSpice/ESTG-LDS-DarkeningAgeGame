# ESTG-LDS 2020/2021 | Browser Game Client

*Makeshift repository* para toda a implementação relativa ao componente *Game Client* do projeto, desenvolvido em Angular 11.

*O projeto foi desenvolvido pelo GitLab da instituição (ESTG), sendo agora trazido para este repositório GitHub.*

# Setup
Ambiente de desenvolvimento:

*após seguir os passos 1., 2. e 3. descritos pelo master README*

4. Instalar todas as dependências
    ```sh
    npm install
    ```
5. Atualizar o Host IP onde o *Game Server* e a *Game API* estão hospedados
    - './src/environments/environment.cs' e './src/proxy.config.js' para a API

6. Ligar o servidor de desenvolvimento

    Para execução num mesmo PC:
    ```sh
    npm start OR ng serve
    ```
    Para que o servidor fique visível na rede local:
    
    *ATENÇÃO: isto poderá tornar-se um risco de segurança se a rede a que o PC está ligado não fôr segura! Isto é só uma forma de tornar o multiplayer possível num ambiente de desenvolvimento.*
    ```sh
    ng serve --host 0.0.0.0
    ```

*This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 11.0.2.*