using System;
using System.Globalization;
using JeuxLibrary.Commun;

namespace JeuxLibrary.Darts;

public enum Zone
{
    HorsCible,

    Simple,

    Double,

    Triple,
}

public readonly struct Dart : IEquatable<Dart>
{
    public const int SecteurBull = 25;

    public const int SecteurMaximum = 20;

    private Dart(Zone zone, int secteur)
    {
        Zone = zone;
        Secteur = secteur;
    }

    public Zone Zone { get; }

    public int Secteur { get; }

    public static Dart Rate => new(Zone.HorsCible, 0);

    public int Points =>
        Zone switch
        {
            Zone.Simple => Secteur,
            Zone.Double => Secteur * 2,
            Zone.Triple => Secteur * 3,
            _ => 0,
        };

    // Indique si la fléchette est un double. Seul un double — bullseye compris —
    // permet de clôturer une partie de 501.
    public bool EstUnDouble => Zone == Zone.Double;

    public static Dart Analyser(string notation)
    {
        var texte = (notation ?? string.Empty).Trim().ToUpperInvariant();

        if (texte is "RATE" or "RATÉ" or "MISS" or "0")
        {
            return Rate;
        }

        if (
            texte.Length >= 2
            && TryLireZone(texte[0], out var zone)
            && int.TryParse(
                texte.Substring(1),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var secteur
            )
        )
        {
            // Le bull n'a pas d'anneau triple : seuls S25 (25 points) et D25 (50 points) existent.
            if (secteur == SecteurBull && zone != Zone.Triple)
            {
                return new Dart(zone, SecteurBull);
            }

            if (secteur >= 1 && secteur <= SecteurMaximum)
            {
                return new Dart(zone, secteur);
            }
        }

        throw new CoupInvalideException(
            $"« {notation} » n'est pas une zone de la cible : attendu S, D ou T suivi d'un secteur de 1 à 20, S25, D25 ou Raté."
        );
    }

    private static bool TryLireZone(char lettre, out Zone zone)
    {
        switch (lettre)
        {
            case 'S':
                zone = Zone.Simple;
                return true;
            case 'D':
                zone = Zone.Double;
                return true;
            case 'T':
                zone = Zone.Triple;
                return true;
            default:
                zone = Zone.HorsCible;
                return false;
        }
    }

    public bool Equals(Dart autre) => Zone == autre.Zone && Secteur == autre.Secteur;

    public override bool Equals(object? obj) => obj is Dart autre && Equals(autre);

    public override int GetHashCode() => ((int)Zone, Secteur).GetHashCode();

    public static bool operator ==(Dart gauche, Dart droite) => gauche.Equals(droite);

    public static bool operator !=(Dart gauche, Dart droite) => !gauche.Equals(droite);

    public override string ToString() =>
        Zone switch
        {
            Zone.Simple => $"S{Secteur}",
            Zone.Double => $"D{Secteur}",
            Zone.Triple => $"T{Secteur}",
            _ => "Raté",
        };
}
