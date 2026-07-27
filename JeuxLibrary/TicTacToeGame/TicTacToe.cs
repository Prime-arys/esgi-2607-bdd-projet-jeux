using System.Collections.Generic;
using JeuxLibrary.Commun;

namespace JeuxLibrary.TicTacToeGame;

/// <summary>
/// Partie de morpion (TicTacToe) sur un plateau 3x3.
/// X engage, puis les joueurs alternent. Le premier à aligner trois symboles
/// (ligne, colonne ou diagonale) gagne ; un plateau rempli sans alignement est nul.
/// </summary>
public sealed class TicTacToe : Game
{
    private readonly Symbole[,] _plateau = new Symbole[Case.Taille, Case.Taille];
    private Dictionary<Player, Symbole> _playerSymbols = new Dictionary<Player, Symbole>();
    private bool _symbolsAssigned = false;
    private int _turnCount = 0;
    private Player? _winner = null;

    public TicTacToe(Manager manager)
        : base(manager)
    {
        if (manager.players.Count != 2)
        {
            throw new ArgumentException("Le morpion se joue à deux joueurs.");
        }
    }

    public void ChoseSymbole(Player player, Symbole symbole)
    {
        if (_symbolsAssigned)
        {
            throw new InvalidOperationException("Les symboles ont déjà été attribués aux joueurs.");
        }
        _playerSymbols[player] = symbole;
        Player otherPlayer = manager.players.First(p => p != player);
        _playerSymbols[otherPlayer] = symbole == Symbole.X ? Symbole.O : Symbole.X;
        _symbolsAssigned = true;
    }

    public override void StartGame()
    {
        // Initialisation du plateau
        for (int i = 0; i < Case.Taille; i++)
        {
            for (int j = 0; j < Case.Taille; j++)
            {
                _plateau[i, j] = Symbole.Vide;
            }
        }

        // Attribution des symboles aux joueurs
        if (!_symbolsAssigned)
        {
            _playerSymbols[manager.players[0]] = Symbole.X;
            _playerSymbols[manager.players[1]] = Symbole.O;
        }

        status = GameStatus.InProgress;
    }

    /// Pose le symbole du joueur courant sur la case indiquée.
    /// La main ne change pas ici : c'est Manager.NextPlayer() qui termine le tour.
    public void Play(Case position)
    {
        if (_turnCount >= Case.Taille * Case.Taille)
        {
            throw new InvalidOperationException("Le plateau est plein. La partie est terminée.");
        }

        if (_plateau[position.IndiceLigne, position.IndiceColonne] != Symbole.Vide)
        {
            throw new ArgumentException($"La case {position} est déjà occupée.");
        }

        _plateau[position.IndiceLigne, position.IndiceColonne] = _playerSymbols[
            manager.CurrentPlayer
        ];
        _turnCount++;

        CheckFinish();
    }

    private void CheckFinish()
    {
        if (_turnCount < 5)
        {
            return; // Impossible de gagner avant le 5ème coup
        }

        foreach (var player in manager.players)
        {
            if (CheckWin(_playerSymbols[player]))
            {
                _winner = player;
                status = GameStatus.Finished;
                return;
            }
        }

        if (_turnCount == Case.Taille * Case.Taille)
        {
            status = GameStatus.Finished; // Match nul possible
        }
        
    }

    private bool CheckWin(Symbole symbole)
    {
        // Vérification des lignes
        for (int i = 0; i < Case.Taille; i++)
        {
            if (_plateau[i, 0] == symbole && _plateau[i, 1] == symbole && _plateau[i, 2] == symbole)
            {
                return true;
            }
        }

        // Vérification des colonnes
        for (int j = 0; j < Case.Taille; j++)
        {
            if (_plateau[0, j] == symbole && _plateau[1, j] == symbole && _plateau[2, j] == symbole)
            {
                return true;
            }
        }

        // Vérification des diagonales
        if (_plateau[0, 0] == symbole && _plateau[1, 1] == symbole && _plateau[2, 2] == symbole)
        {
            return true;
        }
        if (_plateau[0, 2] == symbole && _plateau[1, 1] == symbole && _plateau[2, 0] == symbole)
        {
            return true;
        }

        return false;
    }



    public override Player? GetWinner()
    {
        return _winner;
    }

    public Symbole[,] GetGameBoard()
    {
        return _plateau;
    }
}
