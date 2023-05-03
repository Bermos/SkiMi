using UnityEngine;

public class DataDeleteConfirmation : MonoBehaviour
{

    public void Open(LoadoutState owner)
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        PlayerData.NewSave();
        Close();
    }

    public void Deny()
    {
        Close();
    }
}
