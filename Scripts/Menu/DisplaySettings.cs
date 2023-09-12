using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Serializable]
    private struct UniqueResolution
    {
        public int Width;
        public int Height;

        public UniqueResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    [SerializeField] private List<string> resolutionOptions;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button backButton;
    [SerializeField] private List<UniqueResolution> resolutions;
    [SerializeField] private int resolutionIndex = -1;

    public void Setup(UnityAction onBack)
    {
        backButton.onClick.AddListener(onBack);
    }
    
    private void Awake()
    {
        resolutionsDropdown.onValueChanged.AddListener(index => resolutionIndex = index);
        applyButton.onClick.AddListener(OnApply);
    }
    
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        Persistence.Load();
        RefreshResolutions();
    }

    private void OnDisable()
    {
        Persistence.Save();
    }

    private void OnApply()
    {
        Persistence.data.display.width = resolutions[resolutionIndex].Width;
        Persistence.data.display.height = resolutions[resolutionIndex].Height;
        Persistence.ApplyDisplay();
    }
    
    private void RefreshResolutions()
    {
        resolutions.Clear();
        resolutionOptions.Clear();

        Resolution[] rawResolutions = Screen.resolutions;

        for (int i = 0; i < rawResolutions.Length; i++)
        {
            UniqueResolution candidate = new UniqueResolution(rawResolutions[i].width, rawResolutions[i].height);
            if (resolutions.Contains(candidate)) continue;
            resolutions.Add(candidate);
            if (Persistence.data.display.width == candidate.Width &&
                Persistence.data.display.height == candidate.Height)
            {
                resolutionIndex = resolutions.Count - 1;
            }
            
            string text = string.Format("{0} x {1}", candidate.Width.ToString(), candidate.Height.ToString());
            resolutionOptions.Add(text);
        }

        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(resolutionOptions);
        resolutionsDropdown.value = resolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }
}