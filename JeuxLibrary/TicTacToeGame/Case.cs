using System;
using System.Globalization;
using JeuxLibrary.Commun;

namespace JeuxLibrary.TicTacToeGame;

/// <summary>
/// Coordonnée d'une case du plateau, exprimée dans la notation des joueurs :
/// une colonne de A à C suivie d'une ligne de 1 à 3 (A1 en haut à gauche,
/// C3 en bas à droite).
/// </summary>
/// <remarks>
/// Ce type existe pour deux raisons : faire parler à l'API la langue du joueur
/// (« je joue en B2 ») plutôt que celle du tableau (<c>[1, 1]</c>), et rendre
/// impossible la construction d'une coordonnée hors plateau — le constructeur est
/// privé, la seule porte d'entrée valide bornes et notation.
/// </remarks>
public readonly struct Case : IEquatable<Case>
{
    // Nombre de lignes et de colonnes du plateau.
    public const int Taille = 3;

    private Case(char colonne, int ligne)
    {
        Colonne = colonne;
        Ligne = ligne;
    }

    public char Colonne { get; }
    public int Ligne { get; }

    internal int IndiceColonne => Colonne - 'A';
    internal int IndiceLigne => Ligne - 1;

    public static Case Analyser(string notation)
    {
        var texte = (notation ?? string.Empty).Trim().ToUpperInvariant();

        if (texte.Length == 2
            && texte[0] >= 'A' && texte[0] < 'A' + Taille
            && texte[1] >= '1' && texte[1] < '1' + Taille)
        {
            return new Case(texte[0], texte[1] - '0');
        }

        throw new CoupInvalideException(
            $"« {notation} » ne désigne pas une case du plateau : attendu une colonne de A à C suivie d'une ligne de 1 à 3.");
    }

    internal static Case DepuisIndices(int indiceLigne, int indiceColonne) =>
        new((char)('A' + indiceColonne), indiceLigne + 1);

    public bool Equals(Case autre) => Colonne == autre.Colonne && Ligne == autre.Ligne;

    public override bool Equals(object? obj) => obj is Case autre && Equals(autre);

    public override int GetHashCode() => (Colonne, Ligne).GetHashCode();

    public static bool operator ==(Case gauche, Case droite) => gauche.Equals(droite);

    public static bool operator !=(Case gauche, Case droite) => !gauche.Equals(droite);

    public override string ToString() =>
        string.Concat(Colonne, Ligne.ToString(CultureInfo.InvariantCulture));
}
