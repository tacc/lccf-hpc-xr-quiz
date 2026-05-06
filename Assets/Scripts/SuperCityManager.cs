using UnityEngine;

public class SuperCityManager : MonoBehaviour
{
    [Header("Game State")]
    public int currentPhase = 0; 

    [Header("City Layer (Analogies)")]
    public GameObject[] cityAnalogies; 

    [Header("Placement Layer (Models)")]
    public GameObject motherboardBase; 
    public GameObject[] hardwareModels; 

    void Start()
    {
        // Hide everything initially to reset the board
        HideAll();
        
        // Jump straight into the first puzzle
        StartAnalogyPhase(0);
    }

    private void HideAll()
    {
        if (motherboardBase != null) motherboardBase.SetActive(false);
        
        foreach (var analogy in cityAnalogies) 
        {
            if (analogy != null) analogy.SetActive(false);
        }
            
        foreach (var model in hardwareModels) 
        {
            if (model != null) model.SetActive(false);
        }
    }

    public void StartAnalogyPhase(int phaseIndex)
    {
        currentPhase = phaseIndex;
        if (cityAnalogies[currentPhase] != null)
        {
            cityAnalogies[currentPhase].SetActive(true);
            Debug.Log("Started Phase: " + currentPhase);
        }
    }

    public void OnAnalogySolved()
    {
        cityAnalogies[currentPhase].SetActive(false);
        
        // Show the motherboard since this is the first phase
        if (currentPhase == 0 && motherboardBase != null) 
        {
            motherboardBase.SetActive(true);
        }

        // Spawn the hardware block to pick up
        if (hardwareModels[currentPhase] != null)
        {
            hardwareModels[currentPhase].SetActive(true);
            Debug.Log("Analogy solved! Spawning hardware block.");
        }
    }

    public void OnHardwarePlaced()
    {
        Debug.Log("Hardware snapped in! Phase " + currentPhase + " complete.");
    }
}