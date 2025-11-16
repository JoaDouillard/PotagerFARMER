using UnityEngine;

public enum TypeMenu
{
    Aucun,
    Pause,
    Shop,
    Inventaire
}

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public bool unMenuEstOuvert = false;
    public TypeMenu menuActuel = TypeMenu.Aucun;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool PeutOuvrirMenu(TypeMenu type)
    {
        if (!unMenuEstOuvert)
        {
            return true;
        }

        return menuActuel == type;
    }

    public void OuvrirMenu(TypeMenu type)
    {
        unMenuEstOuvert = true;
        menuActuel = type;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FermerMenu()
    {
        unMenuEstOuvert = false;
        menuActuel = TypeMenu.Aucun;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FermerMenuSpecifique(TypeMenu type)
    {
        if (menuActuel == type)
        {
            FermerMenu();
        }
    }
}
