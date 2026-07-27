Feature: Jeux TicTacToe
tests for TicTacToe game

  Background:
    Given the following players:
      | Name  |
      | Alice |
      | Bob   |
    When I create a new game "TicTacToe"
    And with Alice playing with marker X

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
    