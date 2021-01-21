# ESTG-LDS 2020/2021 | RESTful API

*Makeshift repository* para toda a implementação relativa ao componente RESTful API do projeto.

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

Contudo, estão por sua vez disponíveis as configurações utilizadas em ambiente de desenvolvimento, descrito numa tabela abaixo.

## Servidor SMTP para envio de emails para reset de password, entre outros
Para *development stage*, é utilizado o website https://ethereal.email/ para configuração do Host e Utilizador SMTP para a API.

> Ethereal is a fake SMTP service, mostly aimed at Nodemailer users (but not limited to). It's a completely free anti-transactional email service where messages never get delivered.

Para aceder a todos os emails enviados pela conta configurada:
- começa-se por entrar no [website](https://ethereal.email/)
- na barra superior à direita clicar em *Login*
- utilizar as credenciais presentes na tabela abaixo
- novamente na barra superior, selecionar *Messages*

## Base de Dados

É de notar a utilização da biblioteca *Entity Framework* para armazenamento e acesso aos dados da API. Adicionalmente, temos em uso uma base de dados SQLite em ambiente de desenvolvimento, que deverá ser atualizada para SqlServer em momentos de *deploy*.

Para manutenção da base de dados em SQLite em ambiente de desenvolvimento, devem ser utilizados os seguintes comandos:

- `dotnet ef migrations add <descriptive_name>` para configurar as novas entities, tabelas e contexto.

- `dotnet ef database update` para atualizar a base de dados com a nova configuração.

Para o *Package Manager* do Visual Studio:

- `PM> Add-Migration <descriptive_name>`

- `PM> Update-Database`

----

## Tabela de Configurações em *development* para a API

Descrição | Propriedade em *AppSettings* | Config 
------ | ------ | ------
Base de Dados Sqlite | "SqliteConnection" em "ConnectionStrings" | Data Source=DarkeningAgeDb;
Configuração JWT Secret | "Secret" em "JwtSettings" | sQ+jW5@iH5!}eUfiQxTC/LbFR*!!?l&i5Hj?TVjc7rIxrw'@[_*qZo12Fj@6%a
Configuração do servidor de Emails SMTP | "EmailSettings" | EmailFrom: ines.feil@ethereal.email <br> SmtpHost: smtp.ethereal.email, <br> SmtpPort: 587, <br> SmtpUser: ines.feil@ethereal.email, <br> SmtpPwd: tnrdgsX9TwyWaDHmGq