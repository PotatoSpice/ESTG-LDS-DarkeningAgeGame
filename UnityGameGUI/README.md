# ESTG-LDS 2020/2021 | Unity Game GUI

*Makeshift repository* para toda a implementação relativa ao componente GUI do jogo, desenvolvida através do *game engine* Unity.

*O projeto foi desenvolvido pelo GitLab da instituição (ESTG), sendo agora trazido para este repositório GitHub.*

# Setup
Ambiente de desenvolvimento:

*após seguir os passos 1., 2. e 3. descritos pelo master README*

4. Atualizar o Host IP onde o *Game Server* está hospedado
    - './Assets/Scripts/GameManager/GameHandler.cs', linha 268, trocar o parametro da instancia da classe ClientConnection para "ws://localhost:5000/ws-game/?room-id=" + roomID + "&player-auth=" + userToken

5. Fazer Build do projeto pelo IDE

6. Criar um instalador para o jogo, devido à necessidade de criação de um protocolo no registo do Windows
	
    - Utilizando o Inno para criar o instalador:
    - Adicionar a variavel que contem o nome do protocolo
        #define MyAppProtocolName "DarkeningAgeGame"
    - Adicionar o codigo que vai tratar de criar o registo
        [Registry]
	    Root: HKCR; Subkey: "{#MyAppProtocolName}"; ValueType: "string"; ValueData: "URL:Custom Protocol"; Flags: uninsdeletekey
	    Root: HKCR; Subkey: "{#MyAppProtocolName}"; ValueType: "string"; ValueName: "URL Protocol"; ValueData: ""
	    Root: HKCR; Subkey: "{#MyAppProtocolName}\DefaultIcon"; ValueType: "string"; ValueData: "{app}\{#MyAppExeName},0"
	    Root: HKCR; Subkey: "{#MyAppProtocolName}\shell\open\command"; ValueType: "string"; ValueData: """{app}\{#MyAppExeName}"" ""%1"""