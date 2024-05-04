using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PaniniPlugin.Panini;

public class InjectConfig : MonoBehaviour
{
    /*
     * Canvas
     * OptionsMenu 18
     * Video Options 4
     * ScrollRect 1
     * Contents 0
     */
    
    private static Transform GetChildByName(Transform tr, string childName)
    {
        var childNames = new List<string>();
        foreach (Transform child in tr)
        {
            if (child.name.Equals(childName)) return child;
            childNames.Add(childName);
        }
        
        Debug.LogError("Could not find child " + childName + ", saw " + string.Join(", ", childNames));
        return null;
    }
    
    private void Start()
    {
        var obj = gameObject.transform;
        
        obj = GetChildByName(obj, "OptionsMenu");
        obj = GetChildByName(obj, "Video Options");
        obj = obj.GetChild(1);
        
        var contents = GetChildByName(obj, "Contents");
        var fov = GetChildByName(contents, "FOV");
        var bsChance = GetChildByName(contents, "Bloodstain Chance");

        var warp = Instantiate(bsChance.gameObject, contents);
        warp.name = "Warp";

        var warpText = GetChildByName(warp.transform, "Text").gameObject;
        var tmp = warpText.GetComponent<TextMeshProUGUI>();
        tmp.text = "FOV WARPING";

        obj = GetChildByName(warp.transform, "Button");
        var warpSlider = obj.GetChild(0).gameObject.GetComponent<Slider>();
        
        var resetButton = GetChildByName(warp.transform, "Reset Button").gameObject;
        var srdb = resetButton.GetComponent<SettingsRestoreDefaultButton>();
        srdb.defaultFloat = 0.5f;
        srdb.settingKey = "None";
        
        warpSlider.onValueChanged.RemoveAllListeners();
        warpSlider.onValueChanged.AddListener(_ => srdb.UpdateSelf());
        warpSlider.onValueChanged.AddListener(f => Plugin.ConfigWarp.Value = f);

        int fovIndex = fov.GetSiblingIndex();
        warp.transform.SetSiblingIndex(fovIndex + 1);
    }
}