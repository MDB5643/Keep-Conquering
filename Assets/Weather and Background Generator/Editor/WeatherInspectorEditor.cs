using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WeatherController))]
public class WeatherInspectorEditor : Editor
{
    public int selected = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Spawn Weather Elements", EditorStyles.boldLabel);

        WeatherController myWeatherController = (WeatherController)target;

        string[] options = new string[myWeatherController.weatherParticleEffects.Length];

        for (int i = 0; i < myWeatherController.weatherParticleEffects.Length; i++)
        {
            options[i] = myWeatherController.weatherParticleEffects[i].effectName;
        }

        selected = EditorGUILayout.Popup("Weather Element to Spawn", selected, options);

        if (GUILayout.Button("Spawn Weather Element: '" + options[selected] + "'"))
        {
            myWeatherController.SpawnWeatherElement(myWeatherController.weatherParticleEffects[selected]);
        }

    }

}
