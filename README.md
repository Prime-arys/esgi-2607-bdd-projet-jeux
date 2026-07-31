# Projet Jeux — Librairie de gestion de parties (BDD)

Librairie .NET (10.0) (`JeuxLibrary`) qui encapsule la gestion d'une partie : joueurs,
tours, évolution des scores, conditions de fin. Aucun affichage : l'API est pilotée par
les scénarios Gherkin du projet `JeuxTests`.

```
JeuxLibrary/          Commun/ (Game, Manager, Player, CoupInvalideException)
                      TicTacToeGame/ · Darts/ · DefaultTestGame/ · Jeux.cs
JeuxTests/            Features/ (une par jeu) · StepDefinitions/ (communs + par jeu)
```

```bash
dotnet test JeuxBase.slnx
```


| Feature             | Scénarios | Couverture                                         |
| --------------------- | ------------ | ---------------------------------------------------- |
| `Jeux.feature`      | 4          | contrat commun : joueurs, tours, score, fin        |
| `TicTacToe.feature` | 15         | alignements, match nul, coups refusés             |
| `Darts.feature`     | 26         | décompte, volées, dépassement, sortie au double |

---

# a) Analyse et justification des scénarios

## a-1) Identification des cas de test

Même grille pour chaque jeu : **ce qui fait avancer la partie** (nominal), **ce qui
l'arrête** (limites), **ce que les règles refusent** (erreurs) — découpage rendu visible
dans les `.feature` par des bandeaux de commentaires.

**Fléchettes.** C'est là que porte l'effort, parce que c'est le jeu dont les règles sont
contre-intuitives : on y perd des points, et la partie peut se terminer par un
non-événement. En nominal, le décompte est vérifié zone par zone (simple, double, triple,
bull, bullseye, raté) — la table de multiplication du jeu — puis la volée de trois
fléchettes, le passage de main et la rotation à plus de deux joueurs. En limite, les deux
frontières du 501 : le *dépassement* sous ses trois formes (score négatif, ramené
exactement à 1, ou nul sans double), ce qu'il annule (**toute la volée**, pas seulement la
dernière fléchette — la règle la plus facile à implémenter à moitié), et la *sortie* sur
un double ou sur le bullseye. En erreur : zone qui n'existe pas (`S21`, `T25`, `X20`),
lancer après la victoire, format annoncé trop tard, manche sous le plus petit double,
joueur unique.

**TicTacToe.** Les scénarios d'origine décrivent le déroulement pas à pas, plateau dessiné
après chaque coup ; les huit alignements gagnants, eux, ne méritaient pas huit plateaux —
un `Scenario Outline` par famille de lignes suffit. Un scénario vérifie qu'on gagne aussi
bien avec le `O`, **par le joueur qui n'a pas engagé**. En limite, le match nul. En
erreur : case prise, coordonnée hors plateau, coup après victoire, symboles redistribués,
nombre de joueurs différent de deux.

> **Le cycle BDD a mordu ici.** Le scénario *« No move can be played once the game is
> won »* est resté rouge : `TicTacToe.Play` ne se gardait que du plateau plein
> (`_turnCount >= 9`) et de la case occupée, jamais d'une partie déjà gagnée. On pouvait
> poser un dixième symbole après la victoire et, la détection étant relancée derrière,
> faire changer le vainqueur. Le correctif remplace le compteur par une garde sur le
> statut — plateau plein et alignement gagnant passent tous deux par `Finished`.

Ne sont volontairement pas testés les `Equals`/`GetHashCode`/`ToString` de `Case` et
`Dart` : commodités de structure, pas règles du jeu.

## a-2) Priorisation des scénarios

**1. Le squelette d'abord (critique).** `Jeux.feature` valide le contrat commun sur
`DefaultTestGame`, un jeu témoin sans règles : créer, faire tourner, scorer, terminer.
Il ne sert qu'à isoler les défauts du socle de ceux des règles — quand un scénario de
fléchettes tombe, ces quatre-là disent de quel côté chercher.

**2. Les règles qui définissent le jeu (critiques).** Celles **sans lesquelles la partie
ne se termine jamais correctement** : l'alignement au morpion, le dépassement et la sortie
au double aux fléchettes. Écrites en premier, les plus densément couvertes — le
dépassement occupe quatre scénarios. Ce sont aussi les seules dont le défaut est
*silencieux* : une partie qui ne finit pas se voit, un vainqueur déclaré à tort non.

**3. Limites puis refus (secondaires).** Ils raffinent une règle déjà juste, ou décrivent
des situations qu'un appelant correct ne produit jamais ; leur valeur est de garantir un
refus propre plutôt qu'un comportement indéfini.

**Vérification.** Un test vert ne prouve rien s'il ne peut pas rougir : la règle du
dépassement a été mutée volontairement (`nouveauScore <= 1` → `< 0`), les scénarios du
« score exactement à 1 » et du « zéro sans double » sont tombés, la condition a été
rétablie.

---

# b) Architecture et représentation des données

## b-1) Lisibilité des données de test

**`Background`** ne contient que ce dont *tous* les scénarios de la feature ont besoin :
les joueurs, la création de la partie, les symboles au morpion. Le reste — dont le format
de la manche aux fléchettes — redescend dans les scénarios qui s'en servent.

**Tables**, deux usages seulement et jamais pour décorer : la liste des joueurs (une
colonne `Name`, qui dit qu'il peut y en avoir plus), et le plateau du morpion dessiné tel
qu'on le voit — seul format qui rende un match nul vérifiable d'un coup d'œil.

**`Scenario Outline`** quand la règle est une et les cas plusieurs, toujours avec une
colonne de justification en clair non consommée par le code :

```gherkin
| darts    | why the dart cannot be scored    |
| T20      | the score would go below zero    |
| T13      | the score would be exactly 1     |
| S20, S20 | zero is reached without a double |
```

Cette colonne est le cœur du choix : les trois lignes produisent le même effet observable,
sans elle un relecteur ne saurait pas *pourquoi* il en faut trois. À l'inverse, les
scénarios de déroulement restent déroulés pas à pas — leur intérêt est la séquence, qu'un
tableau aplatirait.

**Le format court comme donnée de test.** `Given each player starts at 40` est le choix le
plus rentable de la feature fléchettes : une manche de 501 demande huit volées avant
d'être à portée de sortie.  Et ce n'est pas une trappe de test — 301, 501, 701 sont les
formats réels du jeu, `ChoisirScoreDeDepart` est une méthode de production que refuse la
première fléchette lancée. De même, `throws T20, T20, D25` reprend la notation qu'un
joueur écrit au tableau.

## b-2) Extensibilité

**Ajouter un jeu** touche trois points : une valeur dans `GameType`, une classe héritant
de `Game` (`StartGame()` + `GetWinner()`), une ligne dans le `switch` de `Manager`. Le
reste est acquis — joueurs, tour courant, rotation, statut, fin forcée vivent dans
`Manager` et `Game`. Le jeu concret n'expose ses règles que sur son propre type
(`Play(Case)`, `Lancer(Dart)`), atteint par `Manager.GetGame<TicTacToe>()` : `Manager` ne
connaît aucune règle. Les fléchettes ont coûté une classe d'environ 130 lignes, socle
inchangé.

**Modifier une règle** reste local, chaque règle ayant un seul point d'ancrage : 501 → 301
via `ChoisirScoreDeDepart` ; sortie simple plutôt qu'au double via la condition
`EstUnDouble` de `Lancer`, un seul `if` ; morpion 4×4 via `Case.Taille`, seule `CheckWin`
demandant à être généralisée.

**Le point de tension assumé.** `Player.Score` porte, aux fléchettes, ce qu'il *reste* à
retrancher — exactement le tableau accroché à côté de la cible. Les steps de score communs
restent utilisables tels quels, mais `Game.AddScore()` (héritée, qui *ajoute*) est
inadaptée à ce jeu. Un jeu dont le score monte et descend obligerait à trancher :
`AddScore` virtuelle, ou score sorti de `Player`. Tant que deux jeux sur deux s'en
accommodent, la duplication coûterait plus que le compromis.

---

# c) Stratégie BDD et bonnes pratiques

## c-1) Langage ubiquitaire

**Chaque jeu parle la langue de ses joueurs.** Aux fléchettes : *oche*, *volley*, *bust*,
*checkout*, *leg*, *bed*, *bullseye* — vocabulaire réel du jeu, où « bust » n'a pas
d'équivalent français d'usage. Au morpion : *square*, *line*, *marker*, *draw*. Aucun
scénario n'emploie de terme technique : ni index, ni tableau, ni exception. Le refus
lui-même est formulé trois fois parce que ce n'est pas la même chose qui est refusée :
`the throw should be rejected`, `the move should be rejected`, `the last action should be rejected`.

**Jusque dans le code de production** : `Dart`, `Zone`, `Secteur`, `EstUnDouble`,
`ResultatLancer.Depassement`, `TerminerLaVolee`. Les types `Case` et `Dart` existent pour
ça — faire parler l'API la langue du joueur (`Analyser("T20")`) plutôt que celle du
tableau (`[1, 1]`), leur constructeur privé rendant impossible une zone qui n'existe pas.

## c-2)  Réutilisabilité — steps communs vs spécifiques

Règle du partage : **est commun ce qui relève du contrat `Game`/`Manager`, est spécifique
ce qui relève des règles d'un jeu.** 13 steps communs (`JeuxStepDefinitions`) ne
mentionnent aucun jeu et servent les trois features — c'est ce qui permet à la feature
fléchettes de réutiliser sans une ligne de code `the current player should be Bob` ou
`the winner should be Alice`. 6 steps morpion et 7 steps fléchettes couvrent ce qu'un jeu
est seul à savoir.

**Ce qui circule : `GameContext`.** Reqnroll instancie une classe de steps par scénario,
donc l'état qu'*une seule* classe manipule y reste (le dernier `ResultatLancer` est un
champ privé de `DartsStepDefinitions`). Ne remonte dans `GameContext` que ce que plusieurs
classes se passent : la partie, les joueurs, le refus des règles.

**Le refus est le cas intéressant.** Un scénario d'erreur a besoin que l'action aille au
bout du step pour être vérifiée par un `Then` — sinon l'exception casse le step au lieu de
décrire une règle. `GameContext.Execute()` met de côté les refus attendus
(`CoupInvalideException`, `ArgumentException`, `InvalidOperationException`) et laisse tout
le reste remonter : un `NullReferenceException` reste un défaut, pas un scénario. La
mécanique est commune, sa **formulation** propre à chaque jeu. C'est aussi pourquoi
`CoupInvalideException` a été introduite : les deux `Analyser` la documentaient déjà mais
levaient une `Exception` nue, que rien ne distinguait d'un vrai défaut.

## c-3) Maintenance

**Un test vert doit être un test qui a travaillé.** Risque du mécanisme ci-dessus :
l'erreur avalée, où tous les `Then` suivants passent sur un état vide. Le hook
`ErrorHooks` fait échouer tout scénario dont une action a été refusée sans qu'aucun step
ne le vérifie, en s'effaçant si le scénario a déjà son propre échec à raconter.

**Un fichier par sujet** — une feature, une classe de steps et un dossier de librairie par
jeu : ajouter un jeu, c'est trois fichiers neufs et une ligne modifiée.

**Les règles ne sont écrites qu'une fois** : le décompte vit dans `Dart.Points`, pas dans
`DartsGame` ; la validité d'une coordonnée dans `Case.Analyser`, pas dans les steps ; la
fin de volée dans `TerminerLaVolee`, appelée aux deux endroits qui la déclenchent.

**Le passage de main est une décision de chaque jeu.** Au morpion, `NextPlayer()` est
appelé de l'extérieur (`When the turn is ended`) : le joueur décide qu'il a fini. Aux
fléchettes, la fin de volée est une *règle* — trois fléchettes, ou un dépassement — donc
c'est `DartsGame` qui la déclenche, et aucun scénario n'a à l'écrire. Aligner les deux par
symétrie obligerait les scénarios à connaître une mécanique que les joueurs n'ont pas à
connaître.
