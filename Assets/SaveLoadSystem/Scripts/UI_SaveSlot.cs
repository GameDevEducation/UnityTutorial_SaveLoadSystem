using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_SaveSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI SlotName;
    [SerializeField] TextMeshProUGUI LastSavedTime_Manual;
    [SerializeField] TextMeshProUGUI LastSavedTime_Automatic;

    [SerializeField] Image ManualSaveBackground;
    [SerializeField] Image AutomaticSaveBackground;

    [SerializeField] Color DefaultColour = Color.black;
    [SerializeField] Color SelectedColour = Color.gray;

    public UnityEvent<ESaveSlot, ESaveType> OnSlotSelected = new UnityEvent<ESaveSlot, ESaveType>();

    UI_SaveLoadUI.EMode CurrentMode;
    ESaveSlot Slot;
    bool HasManualSave;
    bool HasAutomaticSave;

    public void PrepareForMode(UI_SaveLoadUI.EMode mode, ESaveSlot slot)
    {
        Slot = slot;
        CurrentMode = mode;

        HasManualSave = SaveLoadManager.Instance.DoesSaveExist(Slot, ESaveType.Manual);
        HasAutomaticSave = SaveLoadManager.Instance.DoesSaveExist(Slot, ESaveType.Automatic);

        SlotName.text = $"Slot {(int)Slot}";

        if (HasManualSave)
            LastSavedTime_Manual.text = SaveLoadManager.Instance.GetLastSavedTime(Slot, ESaveType.Manual);
        else
            LastSavedTime_Manual.text = CurrentMode == UI_SaveLoadUI.EMode.Save ? "Empty" : "None";

        if (HasAutomaticSave)
            LastSavedTime_Automatic.text = SaveLoadManager.Instance.GetLastSavedTime(Slot, ESaveType.Automatic);
        else
            LastSavedTime_Automatic.text = CurrentMode == UI_SaveLoadUI.EMode.Save ? "Empty" : "None";

        // in load mode - hide empty slots
        if (CurrentMode == UI_SaveLoadUI.EMode.Load && !HasManualSave && !HasAutomaticSave)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

        if (CurrentMode == UI_SaveLoadUI.EMode.Save)
            AutomaticSaveBackground.gameObject.SetActive(false);
        else
            AutomaticSaveBackground.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        AutomaticSaveBackground.color = DefaultColour;
        ManualSaveBackground.color = DefaultColour;
    }

    public void SetSelectedSlot(ESaveSlot slot)
    {
        if (slot != Slot)
        {
            AutomaticSaveBackground.color = DefaultColour;
            ManualSaveBackground.color = DefaultColour;
        }
    }

    public void OnSelectManualSave()
    {
        if (!HasManualSave && CurrentMode == UI_SaveLoadUI.EMode.Load)
            return;

        AutomaticSaveBackground.color = DefaultColour;
        ManualSaveBackground.color = SelectedColour;

        OnSlotSelected.Invoke(Slot, ESaveType.Manual);
    }

    public void OnSelectAutomaticSave()
    {
        if (!HasAutomaticSave && CurrentMode == UI_SaveLoadUI.EMode.Load)
            return;

        AutomaticSaveBackground.color = SelectedColour;
        ManualSaveBackground.color = DefaultColour;

        OnSlotSelected.Invoke(Slot, ESaveType.Automatic);
    }
}
