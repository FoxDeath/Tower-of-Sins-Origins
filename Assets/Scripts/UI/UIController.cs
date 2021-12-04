using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Attributes
    private GameObject bossEnergyBar;

    private Image bossEnergyImage;
    private Image energyImage;
    private Image regenAmmountImage;
    #endregion
    
    #region MonoBehavior Methods
    private void Awake() 
    {
        Transform ui = GameObject.Find("UI").transform;

        energyImage = ui.Find("EnergyBar").Find("EnergyBarFillArea").Find("FillRight").GetComponent<Image>();

        regenAmmountImage = ui.Find("EnergyBar").Find("EnergyBarFillArea").Find("RegenAmmountFillRight").GetComponent<Image>();

        bossEnergyBar = ui.Find("BossEnergyBar").gameObject;

        bossEnergyImage = bossEnergyBar.transform.Find("Fill Area").GetComponentInChildren<Image>();
    }
    #endregion

    #region Normal Methods
    public void SetEnergy(float energyPoints)
    {
        energyImage.fillAmount = energyPoints;
    }

    public void SetRegenAmmount(float regenAmmount)
    {
        regenAmmountImage.fillAmount = regenAmmount;
    }

    public void SetBossEnergy(float energyPoints)
    {
        bossEnergyImage.fillAmount = energyPoints;
    }

    public void SetBossEnergyBarState(bool state)
    {
        bossEnergyBar.SetActive(state);
    }
    #endregion
}
