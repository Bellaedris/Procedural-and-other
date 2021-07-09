using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        PlanetGenerator generator = (PlanetGenerator) target;
        
        if (DrawDefaultInspector()) {
            if (generator.autoUpdate) 
                generator.GenerateSphere();
        }

        if (GUILayout.Button("Generate")) {
            generator.GenerateSphere();
        }

    }
}
