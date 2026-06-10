using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CopyMaterialsUsedByModels : EditorWindow
{
    private DefaultAsset folderToScan;
    private string backupFolderName = "USED_MODEL_MATERIALS_BACKUP";

    [MenuItem("Tools/Cleanup/Copy Materials Used By Models In Folder")]
    public static void ShowWindow()
    {
        GetWindow<CopyMaterialsUsedByModels>("Copy Model Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Materials Used By Models/Prefabs", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        folderToScan = (DefaultAsset)EditorGUILayout.ObjectField(
            "Folder To Scan",
            folderToScan,
            typeof(DefaultAsset),
            false
        );

        backupFolderName = EditorGUILayout.TextField("Backup Folder Name", backupFolderName);

        EditorGUILayout.HelpBox(
            "Drag your Assets/Models folder here. This scans model files and prefabs inside that folder, finds their material dependencies, and copies those .mat files into a backup folder.",
            MessageType.Info
        );

        if (GUILayout.Button("Copy Used Materials From Folder"))
        {
            CopyUsedMaterials();
        }
    }

    private void CopyUsedMaterials()
    {
        if (folderToScan == null)
        {
            EditorUtility.DisplayDialog(
                "No Folder Selected",
                "Drag your Models folder into Folder To Scan first.",
                "OK"
            );
            return;
        }

        string scanFolderPath = AssetDatabase.GetAssetPath(folderToScan);

        if (string.IsNullOrEmpty(scanFolderPath) || !AssetDatabase.IsValidFolder(scanFolderPath))
        {
            EditorUtility.DisplayDialog(
                "Invalid Folder",
                "Please select a real folder inside Assets.",
                "OK"
            );
            return;
        }

        string backupFolderPath = "Assets/" + backupFolderName;

        if (!AssetDatabase.IsValidFolder(backupFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", backupFolderName);
        }

        HashSet<string> usedMaterialPaths = new HashSet<string>();

        // Scan common asset types that may reference materials
        string[] assetGuids = AssetDatabase.FindAssets(
            "t:Prefab t:Model t:Material",
            new[] { scanFolderPath }
        );

        for (int i = 0; i < assetGuids.Length; i++)
        {
            bool cancel = EditorUtility.DisplayCancelableProgressBar(
                "Scanning Assets",
                "Scanning asset " + (i + 1) + " of " + assetGuids.Length,
                (float)i / assetGuids.Length
            );

            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Material scan canceled.");
                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);

            if (string.IsNullOrEmpty(assetPath))
            {
                continue;
            }

            // Get all dependencies for this model/prefab/material
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);

            foreach (string depPath in dependencies)
            {
                if (depPath.EndsWith(".mat"))
                {
                    usedMaterialPaths.Add(depPath);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        int copiedCount = 0;
        int skippedCount = 0;

        List<string> materialList = new List<string>(usedMaterialPaths);

        for (int i = 0; i < materialList.Count; i++)
        {
            bool cancel = EditorUtility.DisplayCancelableProgressBar(
                "Copying Materials",
                "Copying material " + (i + 1) + " of " + materialList.Count,
                (float)i / materialList.Count
            );

            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Material copy canceled.");
                return;
            }

            string originalPath = materialList[i];

            if (!File.Exists(originalPath))
            {
                skippedCount++;
                Debug.LogWarning("Skipped missing material file: " + originalPath);
                continue;
            }

            string fileName = Path.GetFileName(originalPath);
            string newPath = AssetDatabase.GenerateUniqueAssetPath(backupFolderPath + "/" + fileName);

            bool copied = AssetDatabase.CopyAsset(originalPath, newPath);

            if (copied)
            {
                copiedCount++;
            }
            else
            {
                skippedCount++;
                Debug.LogWarning("Could not copy material: " + originalPath);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        Debug.Log("Scan folder: " + scanFolderPath);
        Debug.Log("Found " + usedMaterialPaths.Count + " material(s) used by models/prefabs.");
        Debug.Log("Copied " + copiedCount + " material(s) to: " + backupFolderPath);
        Debug.Log("Skipped " + skippedCount + " material(s).");

        EditorUtility.DisplayDialog(
            "Done",
            "Copied " + copiedCount + " material(s) to " + backupFolderPath + ".\nSkipped " + skippedCount + ".",
            "OK"
        );
    }
}