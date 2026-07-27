Feature: Jeux

Generic tests for the Jeux library

Scenario: Create a new game
    Given the following players:
        | Name  |
        | Alice |
        | Bob   |
    When I create a new game "DefaultTestGame"
    Then the game should be created with 2 players


Scenario: Test, turn order
    Given the following players:
        | Name  |
        | Alice |
        | Bob   |
    When I create a new game "DefaultTestGame"
    Then the current player should be Alice
    When the turn is ended
    Then the current player should be Bob
    When the turn is ended
    Then the current player should be Alice

Scenario: Test, score update
    Given the following players:
        | Name  |
        | Alice |
        | Bob   |
    When I create a new game "DefaultTestGame"
    Then the current player should be Alice
    When I update the score of the current player by 10
    Then the score of Alice should be 10
    When the turn is ended
    Then the current player should be Bob
    When I update the score of the current player by 5
    Then the score of Bob should be 5


Scenario: Test, end game
    Given the following players:
        | Name  |
        | Alice |
        | Bob   |
    When I create a new game "DefaultTestGame"
    Then the current player should be Alice
    When I update the score of the current player by 10
    Then the score of Alice should be 10
    When the turn is ended
    Then the current player should be Bob
    When I update the score of the current player by 5
    Then the score of Bob should be 5
    When the game is ended
    Then the game should be over
    And the winner should be Alice