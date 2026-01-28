using UnityEngine;

public static class LevelProgression
{
    private const string KEY = "LevelUnlocked";

    public static int NiveauMaxDebloque
    {
        get => PlayerPrefs.GetInt(KEY, 1); // Niveau 1 débloqué par défaut
        set => PlayerPrefs.SetInt(KEY, value);
    }

    public static void DebloquerNiveau(int niveau)
    {
        if (niveau > NiveauMaxDebloque)
        {
            NiveauMaxDebloque = niveau;
            PlayerPrefs.Save();
        }
    }

    public static bool EstDebloque(int niveau)
    {
        return niveau <= NiveauMaxDebloque;
    }
}