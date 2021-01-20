# Retirado da Wiki | [Organização do Repositório](https://gitlab.estg.ipp.pt/lds-group-15/darkening-age/wikis/Configura%C3%A7%C3%A3o-do-Reposit%C3%B3rio)

- Visto no diagrama abaixo e tendo em conta que o projeto está desenvolvido num repositório único, existirá para cada componente do projeto um GIT [Orphan Branches](https://www.git-scm.com/docs/git-checkout#Documentation/git-checkout.txt---orphanltnewbranchgt). 
- Isto é, existirá um branch para desenvolvimento do serviço web, outro para o cliente web no browser, outro para o cliente desktop no Unity e outro para o servidor dedicado do jogo. Igualmente, para cada componente existe um branch de *deploy* e outro para *development*, para 
- Resumidamente, todo o código deverá ser construido nos últimos e depois incluido no branch de *development* através de *merge requests*. O mesmo deverá acontecer entre os branches de *deploy* e *development* para os momentos de delivery do projeto.

![branch_organization](/uploads/82d6711b37e6a3618a9755b86eca6522/branch_organization.jpg)

# Práticas para organização de *Commits*

Para cada commit, normalmente, a descrição deverá começar com pelo menos uma das *tags* seguintes:

- feature (código novo, nova funcionalidade);
- refactor (correções pontuais de código, entre commits);
- hotfix (correções de maior escala para problemas em específico, entre merge-requests);
- docs (atualização de documentação do código);

# CI & CD

Atualmente, o CI encontra-se com problemas de *back office*. No entanto, os ficheiros *.gitlab-ci.yml* já estão configurados e adicionados aos repositórios.