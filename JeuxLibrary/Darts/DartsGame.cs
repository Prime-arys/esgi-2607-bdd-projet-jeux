using System;
using System.Collections.Generic;
using System.Linq;
using JeuxLibrary.Commun;

namespace JeuxLibrary.Darts;

public sealed class DartsGame : Game
{
    public const int ScoreDeDepartParDefaut = 501;

    public const int FlechettesParVolee = 3;
    public const int ScoreDeDepartMinimum = 2;

    private int _scoreDeDepart = ScoreDeDepartParDefaut;
    private int _scoreAvantVolee = ScoreDeDepartParDefaut;
    private int _flechettesRestantes = FlechettesParVolee;
    private int _flechettesLancees;
    private Player? _vainqueur;

    public DartsGame(Manager manager)
        : base(manager)
    {
        if (manager.players.Count < 2)
        {
            throw new ArgumentException("Une partie de fléchettes oppose au moins deux joueurs.");
        }
    }

    public int ScoreDeDepart => _scoreDeDepart;

    public int FlechettesRestantesDansLaVolee => _flechettesRestantes;

    public int ScoreRestant(Player joueur) => joueur.Score;

    public void ChoisirScoreDeDepart(int scoreDeDepart)
    {
        if (_flechettesLancees > 0)
        {
            throw new InvalidOperationException(
                "Le format de la manche ne se change plus une fois la première fléchette lancée."
            );
        }

        if (scoreDeDepart < ScoreDeDepartMinimum)
        {
            throw new ArgumentException(
                $"Une manche s'ouvre à {ScoreDeDepartMinimum} points au moins : en dessous, aucun double ne permet de sortir."
            );
        }

        _scoreDeDepart = scoreDeDepart;
        InitialiserLaManche();
    }

    public override void StartGame()
    {
        InitialiserLaManche();
        status = GameStatus.InProgress;
    }

    public ResultatLancer Lancer(Dart flechette)
    {
        if (status != GameStatus.InProgress)
        {
            throw new InvalidOperationException(
                "La partie n'est pas en cours : aucune fléchette ne peut être lancée."
            );
        }

        var joueur = manager.CurrentPlayer;
        _flechettesLancees++;
        _flechettesRestantes--;

        var nouveauScore = joueur.Score - flechette.Points;

        // Sortie au double : seul un double posé pile sur zéro clôt la manche.
        if (nouveauScore == 0 && flechette.EstUnDouble)
        {
            joueur.Score = 0;
            _vainqueur = joueur;
            status = GameStatus.Finished;
            return ResultatLancer.Victoire;
        }

        // Dépassement : score négatif, égal à 1 , ou nul sans double. Toute la volée est annulée.
        if (nouveauScore <= 1)
        {
            joueur.Score = _scoreAvantVolee;
            TerminerLaVolee();
            return ResultatLancer.Depassement;
        }

        joueur.Score = nouveauScore;

        if (_flechettesRestantes == 0)
        {
            TerminerLaVolee();
        }

        return ResultatLancer.Decompte;
    }

    public override Player? GetWinner()
    {
        return _vainqueur;
    }

    private void InitialiserLaManche()
    {
        foreach (var joueur in manager.players)
        {
            joueur.Score = _scoreDeDepart;
        }

        _scoreAvantVolee = _scoreDeDepart;
        _flechettesRestantes = FlechettesParVolee;
        _vainqueur = null;
    }

    private void TerminerLaVolee()
    {
        manager.NextPlayer();
        _flechettesRestantes = FlechettesParVolee;
        _scoreAvantVolee = manager.CurrentPlayer.Score;
    }
}
