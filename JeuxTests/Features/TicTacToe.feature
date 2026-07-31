Feature: Jeux TicTacToe
tests for TicTacToe game

  Background:
    Given the following players:
      | Name  |
      | Alice |
      | Bob   |
    When I create a new game "TicTacToe"
    And with Alice playing with marker X

  # --- Ouverture de la partie ------------------------------------------------

  Scenario: Create a new game
    Then the game should be created with 2 players
    And the game board should look like:
      | row | A | B | C |
      |   1 | . | . | . |
      |   2 | . | . | . |
      |   3 | . | . | . |

  Scenario: Test, action and turn order
    Then the current player should be Alice
    When the current player plays at position (1, A)
    Then the game board should look like:
      | row | A | B | C |
      |   1 | X | . | . |
      |   2 | . | . | . |
      |   3 | . | . | . |
    When the turn is ended
    Then the current player should be Bob
    When the current player plays at position (2, B)
    Then the game board should look like:
      | row | A | B | C |
      |   1 | X | . | . |
      |   2 | . | O | . |
      |   3 | . | . | . |

  # --- Fin de partie ---------------------------------------------------------

  Scenario: Test, win condition
    Then the current player should be Alice
    When the current player plays at position (1, B)
    Then the game board should look like:
      | row | A | B | C |
      |   1 | . | X | . |
      |   2 | . | . | . |
      |   3 | . | . | . |
    When the turn is ended
    Then the current player should be Bob
    When the current player plays at position (2, A)
    Then the game board should look like:
      | row | A | B | C |
      |   1 | . | X | . |
      |   2 | O | . | . |
      |   3 | . | . | . |
    When the turn is ended
    When the current player plays at position (1, C)
    When the turn is ended
    When the current player plays at position (2, C)
    When the turn is ended
    When the current player plays at position (1, A)
    Then the game board should look like:
      | row | A | B | C |
      |   1 | X | X | X |
      |   2 | O | . | O |
      |   3 | . | . | . |
    Then the game should be over
    And the winner should be Alice

  # Les huit alignements se valent : plutôt que huit scénarios peints case par case,
  # une suite de coups par famille de lignes suffit à couvrir la règle.
  Scenario Outline: Three symbols in a line win the game, whatever the line
    When the players take turns playing <moves>
    Then the game should be over
    And the winner should be Alice

    Examples:
      | line          | moves                  |
      | top row       | A1, A2, B1, B2, C1     |
      | left column   | A1, B1, A2, B2, A3     |
      | main diagonal | A1, B1, B2, C1, C3     |
      | anti diagonal | C1, A1, B2, A2, A3     |

  # L'attribution des symboles ne doit pas privilégier celui qui engage : la partie
  # se gagne aussi bien avec le O, et par le joueur qui n'a pas ouvert.
  Scenario: The player holding O wins just the same
    When the players take turns playing A1, B1, A2, B2, C3, B3
    Then the game board should look like:
      | row | A | B | C |
      |   1 | X | O | . |
      |   2 | X | O | . |
      |   3 | . | O | X |
    And the game should be over
    And the winner should be Bob

  Scenario: A full board with no alignment is a draw
    When the players take turns playing A1, B1, C1, B2, A2, C2, B3, A3, C3
    Then the game board should look like:
      | row | A | B | C |
      |   1 | X | O | X |
      |   2 | X | O | O |
      |   3 | O | X | X |
    And the game should be a draw

  # --- Coups que les règles refusent -----------------------------------------

  Scenario: A square can only be taken once
    When the current player plays at position (1, A)
    And the turn is ended
    And the current player plays at position (1, A)
    Then the move should be rejected
    And the game board should look like:
      | row | A | B | C |
      |   1 | X | . | . |
      |   2 | . | . | . |
      |   3 | . | . | . |

  Scenario Outline: A move must designate a square of the board
    When the current player plays at position (<row>, <column>)
    Then the move should be rejected

    Examples:
      | row | column | why it is off the board |
      | 4   | A      | the board has 3 rows    |
      | 1   | D      | the board has 3 columns |

  Scenario: No move can be played once the game is won
    When the players take turns playing A1, A2, B1, B2, C1
    Then the winner should be Alice
    When the turn is ended
    And the current player plays at position (3, A)
    Then the move should be rejected
    And the winner should be Alice

  Scenario: The markers are handed out once and for all
    When with Bob playing with marker X
    Then the last action should be rejected

  Scenario: TicTacToe is played by exactly two players
    Given the following players:
      | Name    |
      | Alice   |
      | Bob     |
      | Charlie |
    When I create a new game "TicTacToe"
    Then the last action should be rejected
