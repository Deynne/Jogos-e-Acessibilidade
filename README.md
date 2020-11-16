# Jogos-e-Acessibilidade
Um conjunto de jogos inclusivo com enfoque em usuários portadores de deficiência visual.


## Organização de arquivos do projeto
A organização de arquivos do projeto deve ser feita segundo o seguinte modelo
* Assets
  * Game
    * Scenes
    * Animations
    * Prefabs
    * \<etc\>
  * Scripts
  * UI
    * Atlas
    * Prefabs
    * \<etc\>
  * Externos
  
## Padrões de Scenes
As 'Scenes' também devem seguir um modelo de organização dos objetos, para melhor leitura. Cada 'Scene' deve seguir a seguinte organização
* Light 
* Camera
* World
  * \<Objetos que afetam o mundo como player ou estruturas\>
* UI
  * \<Objetos que afetam a UI como botões e Labels\>
* Handlers
* Dynamic
