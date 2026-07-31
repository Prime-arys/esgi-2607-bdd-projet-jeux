Feature: Jeux Darts
Tests for a leg of 501, straight in and double out.

A leg opens with every player on 501. Players take the oche in turn and throw a
volley of three darts, each dart subtracting the points of the bed it lands in.
The leg is won by the first player to land exactly on zero with a double.

  Background:
    Given the following players:
      | Name  |
      | Alice |
      | Bob   |
    When I create a new game "Darts"

  # --- Opening the leg -------------------------------------------------------

  Scenario: A leg opens with both players on 501
    Then the game should be created with 2 players
    And the remaining score of Alice should be 501
    And the remaining score of Bob should be 501
    And the current player should be Alice
    And the current player should have 3 darts left in the volley

  # --- Scoring a dart --------------------------------------------------------

  # Un exemple par zone de la cible : c'est la table de multiplication du jeu,
  # et la seule chose que la notation S/D/T doive garantir.
  Scenario Outline: A dart subtracts the points of the bed it lands in
    When the current player throws <dart>
    Then the remaining score of Alice should be <remaining>
    And the current player should have 2 darts left in the volley

    Examples:
      | dart | remaining | bed                    |
      | S20  | 481       | single twenty          |
      | D20  | 461       | double twenty          |
      | T20  | 441       | treble twenty          |
      | S25  | 476       | outer bull             |
      | D25  | 451       | bullseye               |
      | Miss | 501       | dart lost off the wire |

  # --- Taking turns ----------------------------------------------------------

  Scenario: A volley of three darts hands the oche over to the opponent
    When the current player throws T20, T20, T20
    Then the remaining score of Alice should be 321
    And the current player should be Bob
    And the current player should have 3 darts left in the volley

  Scenario: Each player keeps their own remaining score
    When the current player throws T20, T20, T20
    And the current player throws S5, S1, Miss
    Then the remaining score of Alice should be 321
    And the remaining score of Bob should be 495
    And the current player should be Alice

  # Les fléchettes ne se jouent pas qu'à deux : la rotation doit suivre le nombre
  # de joueurs déclarés, d'où une partie montée à trois pour ce seul scénario.
  Scenario: More than two players take the oche in turn
    Given the following players:
      | Name    |
      | Alice   |
      | Bob     |
      | Charlie |
    When I create a new game "Darts"
    And the current player throws S1, S1, S1
    Then the current player should be Bob
    When the current player throws S1, S1, S1
    Then the current player should be Charlie
    When the current player throws S1, S1, S1
    Then the current player should be Alice

  # --- Busting ---------------------------------------------------------------

  # Ouvrir à 40 met la sortie à portée de la première volée : le scénario tient en
  # une ligne de lancers, sans les huit volées de remplissage qu'imposerait un 501.
  Scenario Outline: A dart that cannot be scored busts the volley
    Given each player starts at 40
    When the current player throws <darts>
    Then the volley should be a bust
    And the remaining score of Alice should be 40
    And the current player should be Bob

    Examples:
      | darts    | why the dart cannot be scored |
      | T20      | the score would go below zero |
      | T13      | the score would be exactly 1  |
      | S20, S20 | zero is reached without a double |

  Scenario: A bust gives back the whole volley, not just the last dart
    Given each player starts at 100
    When the current player throws T20, T20
    Then the volley should be a bust
    And the remaining score of Alice should be 100
    And the current player should be Bob

  # --- Checking out ----------------------------------------------------------

  Scenario: Landing on zero with a double wins the leg
    Given each player starts at 40
    When the current player throws D20
    Then the throw should win the leg
    And the remaining score of Alice should be 0
    And the game should be over
    And the winner should be Alice

  Scenario: The bullseye counts as a double and can check out
    Given each player starts at 50
    When the current player throws D25
    Then the throw should win the leg
    And the winner should be Alice

  Scenario: The leg ends on the checkout, whatever darts are left in hand
    Given each player starts at 60
    When the current player throws S20, D20
    Then the game should be over
    And the winner should be Alice
    And the current player should be Alice

  Scenario: The leg is still running as long as nobody has checked out
    Given each player starts at 60
    When the current player throws S20, S20
    Then the game should not be over
    And there should be no winner yet

  # --- Cases the rules refuse ------------------------------------------------

  Scenario Outline: A dart must land in a bed that exists on the board
    When the current player throws <notation>
    Then the throw should be rejected

    Examples:
      | notation | why it is not a bed             |
      | S21      | the board stops at 20           |
      | T25      | the bull has no treble ring     |
      | X20      | X is not a ring of the board    |
      | 20       | the ring is missing             |

  Scenario: No dart can be thrown once the leg has been won
    Given each player starts at 40
    When the current player throws D20
    And the current player throws S1
    Then the throw should be rejected
    And the winner should be Alice

  Scenario: The format is announced before the first dart, not after
    When the current player throws S20
    And each player starts at 301
    Then the last action should be rejected
    And the remaining score of Alice should be 481

  Scenario: A leg cannot open below the smallest double
    Given each player starts at 1
    Then the last action should be rejected

  Scenario: A leg needs at least two players
    Given the following players:
      | Name  |
      | Alice |
    When I create a new game "Darts"
    Then the last action should be rejected
