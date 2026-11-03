using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// One-time project upgrade for Unity 6:
//  - Built-in render pipeline + Post-Processing v2  ->  URP with the 2D Renderer + URP Volumes
//  - Sprite materials -> URP Sprite-Unlit-Default (same unlit look as before)
//  - Message log legacy UI Text -> TextMeshPro
//  - Active Input Handling -> Both (old Input Manager + new Input System)
// Safe to run more than once; each step skips work that's already done.
// Delete this file once the upgrade is done and committed.
public static class ThicketUpgrade
{
    const string SettingsDir = "Assets/Settings";
    const string URPPackage = "Packages/com.unity.render-pipelines.universal";
    const string MessageFontPath = "Assets/Fonts/Courier Prime Code.ttf";
    const string MessageFontAssetPath = "Assets/Fonts/Courier Prime Code SDF.asset";
    static readonly string[] ScenePaths = { "Assets/Scenes/Play.unity", "Assets/Scenes/Attract.unity" };

    [MenuItem("Thicket/Upgrade Project (URP 2D, TextMeshPro, Input)")]
    static void Run()
    {
        if (!EditorUtility.DisplayDialog("Thicket Upgrade",
            "This will switch the project to URP (2D Renderer), convert post-processing to URP Volumes, " +
            "convert sprite materials, convert the message log to TextMeshPro, and set Input Handling to Both.\n\n" +
            "Play.unity and Attract.unity will be opened, modified and saved. Make sure your work is committed first.",
            "Upgrade", "Cancel"))
            return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();
        var log = new StringBuilder("Thicket upgrade:\n");
        bool restartNeeded = false;

        try
        {
            if (!AssetDatabase.IsValidFolder(SettingsDir))
                AssetDatabase.CreateFolder("Assets", "Settings");

            SetupPipeline(log);

            // values carried over from the old Post-Processing v2 profiles in Assets/Scenes/Play_Profiles
            VolumeProfile day = GetOrCreateProfile("PostFX-Day", log,
                temperature: 17, tint: 12, saturation: 58, brightness: 18,
                bloomIntensity: 0.4f, bloomThreshold: 0.5f, bloomTint: new Color(0.5415428f, 0.9339623f, 0.4537647f),
                vignetteIntensity: 0.32f);
            VolumeProfile night = GetOrCreateProfile("PostFX-Night", log,
                temperature: -36, tint: 19, saturation: -36, brightness: -17,
                bloomIntensity: 0f, bloomThreshold: 0f, bloomTint: Color.white,
                vignetteIntensity: 0.48f);

            Material legacySprite = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            Material urpSprite = AssetDatabase.LoadAssetAtPath<Material>(URPPackage + "/Runtime/Materials/Sprite-Unlit-Default.mat");

            ConvertMaterials(log);
            ConvertPrefabs(legacySprite, urpSprite, log);

            GetOrCreateFontAsset(log);

            // Opening a scene unloads assets, so write everything to disk now and
            // reload by path inside ConvertScene instead of holding object references.
            AssetDatabase.SaveAssets();
            var volumes = new Dictionary<string, (string profilePath, float weight)>
            {
                { "Post Effects - Day", (AssetDatabase.GetAssetPath(day), 0f) },
                { "Post Effects - Night", (AssetDatabase.GetAssetPath(night), 1f) },
                { "Post Effects", (AssetDatabase.GetAssetPath(night), 0.662f) }, // Attract scene
            };
            foreach (string scenePath in ScenePaths)
                ConvertScene(scenePath, volumes, log);

            restartNeeded = SetInputHandlingBoth(log);
            AssetDatabase.SaveAssets();
        }
        finally
        {
            if (previousSetup.Length > 0)
                EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
        }

        Debug.Log(log.ToString());

        if (restartNeeded && EditorUtility.DisplayDialog("Thicket Upgrade",
            "Upgrade finished (details in the Console).\n\nInput Handling was changed to Both, which needs an editor restart.",
            "Restart now", "Later"))
        {
            EditorApplication.OpenProject(Directory.GetCurrentDirectory());
        }
        else if (!restartNeeded)
        {
            EditorUtility.DisplayDialog("Thicket Upgrade", "Upgrade finished. Details are in the Console.", "OK");
        }
    }

    // --- Render pipeline ---

    static void SetupPipeline(StringBuilder log)
    {
        if (GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset)
        {
            log.AppendLine("- URP already active");
            return;
        }

        string rendererPath = SettingsDir + "/Renderer2D.asset";
        var rendererData = AssetDatabase.LoadAssetAtPath<Renderer2DData>(rendererPath);
        if (rendererData == null)
        {
            // mirrors URP's own "Create > Rendering > URP 2D Renderer" menu
            rendererData = ScriptableObject.CreateInstance<Renderer2DData>();
            AssetDatabase.CreateAsset(rendererData, rendererPath);
            ResourceReloader.ReloadAllNullIn(rendererData, URPPackage);
            var so = new SerializedObject(rendererData);
            SerializedProperty postProcessData = so.FindProperty("m_PostProcessData");
            if (postProcessData != null && postProcessData.objectReferenceValue == null)
            {
                postProcessData.objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<PostProcessData>(URPPackage + "/Runtime/Data/PostProcessData.asset");
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(rendererData);
        }

        string pipelinePath = SettingsDir + "/URP-2D.asset";
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
        if (pipeline == null)
        {
            pipeline = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(pipeline, pipelinePath);
        }

        GraphicsSettings.defaultRenderPipeline = pipeline;
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            RenderPipelineAsset qualityAsset = QualitySettings.GetRenderPipelineAssetAt(i);
            if (qualityAsset != null && qualityAsset != pipeline)
                log.AppendLine("- WARNING: quality level '" + QualitySettings.names[i] + "' overrides the render pipeline; clear it in Project Settings > Quality");
        }
        log.AppendLine("- Created " + pipelinePath + " + " + rendererPath + " and set URP as the render pipeline");
    }

    // --- Post-processing profiles ---

    static VolumeProfile GetOrCreateProfile(string name, StringBuilder log,
        float temperature, float tint, float saturation, float brightness,
        float bloomIntensity, float bloomThreshold, Color bloomTint, float vignetteIntensity)
    {
        string path = SettingsDir + "/" + name + ".asset";
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
        if (profile != null) return profile;

        profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, path);

        var whiteBalance = AddOverride<WhiteBalance>(profile);
        whiteBalance.temperature.Override(temperature);
        whiteBalance.tint.Override(tint);

        var colorAdjustments = AddOverride<ColorAdjustments>(profile);
        colorAdjustments.saturation.Override(saturation);
        // PPv2's LDR "brightness" has no URP equivalent; approximate it as an exposure change
        colorAdjustments.postExposure.Override(Mathf.Log(1f + brightness / 100f, 2f));

        if (bloomIntensity > 0f)
        {
            var bloom = AddOverride<Bloom>(profile);
            bloom.intensity.Override(bloomIntensity);
            bloom.threshold.Override(bloomThreshold);
            bloom.tint.Override(bloomTint);
        }

        var vignette = AddOverride<Vignette>(profile);
        vignette.color.Override(new Color(0.09054972f, 0.06581523f, 0.2735849f));
        vignette.center.Override(new Vector2(0.5f, 0.5f));
        vignette.intensity.Override(vignetteIntensity);
        vignette.smoothness.Override(0.2f);
        vignette.rounded.Override(false);

        var grain = AddOverride<FilmGrain>(profile);
        grain.intensity.Override(0.09f);
        grain.response.Override(0.738f);

        // Motion blur (camera-motion only in URP) and the disabled chromatic aberration are not carried over.
        EditorUtility.SetDirty(profile);
        log.AppendLine("- Created volume profile " + path);
        return profile;
    }

    static T AddOverride<T>(VolumeProfile profile) where T : VolumeComponent
    {
        T component = profile.Add<T>();
        component.name = typeof(T).Name;
        component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(component, profile);
        return component;
    }

    // --- Materials ---

    static void ConvertMaterials(StringBuilder log)
    {
        Shader urpSpriteShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (urpSpriteShader == null)
        {
            log.AppendLine("- WARNING: URP sprite shader not found; materials not converted");
            return;
        }
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null || material.shader == urpSpriteShader || material.shader.name != "Sprites/Default")
                continue;
            Color tint = material.HasProperty("_Color") ? material.color : Color.white;
            material.shader = urpSpriteShader;
            material.shaderKeywords = new string[0];
            material.color = tint; // URP's sprite shader also multiplies by _Color, so the fog colour is unchanged
            EditorUtility.SetDirty(material);
            log.AppendLine("- Converted " + path + " to URP Sprite-Unlit-Default");
        }
    }

    static bool SwapSpriteMaterials(GameObject root, Material legacy, Material urp)
    {
        if (legacy == null || urp == null) return false;
        bool changed = false;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (!(renderer is SpriteRenderer) && !(renderer is TilemapRenderer)) continue;
            Material[] materials = renderer.sharedMaterials;
            bool rendererChanged = false;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == legacy)
                {
                    materials[i] = urp;
                    rendererChanged = true;
                }
            }
            if (rendererChanged)
            {
                renderer.sharedMaterials = materials;
                changed = true;
            }
        }
        return changed;
    }

    static void ConvertPrefabs(Material legacy, Material urp, StringBuilder log)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (SwapSpriteMaterials(contents, legacy, urp))
                {
                    PrefabUtility.SaveAsPrefabAsset(contents, path);
                    log.AppendLine("- " + path + ": sprite material -> URP");
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }
    }

    // --- TextMeshPro ---

    static TMP_FontAsset GetOrCreateFontAsset(StringBuilder log)
    {
        var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MessageFontAssetPath);
        if (fontAsset != null) return fontAsset;

        var font = AssetDatabase.LoadAssetAtPath<Font>(MessageFontPath);
        if (font == null)
        {
            log.AppendLine("- WARNING: " + MessageFontPath + " not found; message log will use the default TMP font");
            return null;
        }

        fontAsset = TMP_FontAsset.CreateFontAsset(font); // dynamic: glyphs are added as they're used
        fontAsset.name = Path.GetFileNameWithoutExtension(MessageFontAssetPath);
        AssetDatabase.CreateAsset(fontAsset, MessageFontAssetPath);
        fontAsset.atlasTextures[0].name = fontAsset.name + " Atlas";
        AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
        fontAsset.material.name = fontAsset.name + " Material";
        AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        EditorUtility.SetDirty(fontAsset);
        log.AppendLine("- Created TMP font asset " + MessageFontAssetPath);
        return fontAsset;
    }

    static void ConvertMessageText(GameObject go, TMP_FontAsset font, StringBuilder log)
    {
        var legacy = go.GetComponent<UnityEngine.UI.Text>();
        if (legacy == null) return;

        string text = legacy.text;
        int fontSize = legacy.fontSize;
        Color color = legacy.color;
        TextAnchor anchor = legacy.alignment;
        bool richText = legacy.supportRichText;
        bool raycastTarget = legacy.raycastTarget;
        bool wrap = legacy.horizontalOverflow == HorizontalWrapMode.Wrap;

        Object.DestroyImmediate(legacy);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = ToTMPAlignment(anchor);
        tmp.richText = richText;
        tmp.raycastTarget = raycastTarget;
        tmp.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
        tmp.overflowMode = TextOverflowModes.Overflow;
        log.AppendLine("  - '" + go.name + "': UI Text -> TextMeshPro");
    }

    static TextAlignmentOptions ToTMPAlignment(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            default: return TextAlignmentOptions.BottomRight;
        }
    }

    // --- Scenes ---

    static void ConvertScene(string scenePath, Dictionary<string, (string profilePath, float weight)> volumes, StringBuilder log)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Material legacySprite = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        Material urpSprite = AssetDatabase.LoadAssetAtPath<Material>(URPPackage + "/Runtime/Materials/Sprite-Unlit-Default.mat");
        TMP_FontAsset messageFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MessageFontAssetPath);
        log.AppendLine("- " + scenePath + ":");
        bool isPlayScene = scenePath.EndsWith("Play.unity");

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                GameObject go = t.gameObject;
                bool isPostEffects = volumes.ContainsKey(go.name);
                Camera camera = go.GetComponent<Camera>();

                // Old Post-Processing v2 components: real ones if the package is still installed,
                // otherwise they show up as missing scripts on the camera / volume objects.
                foreach (Component c in go.GetComponents<Component>())
                {
                    if (c != null && c.GetType().Namespace == "UnityEngine.Rendering.PostProcessing")
                    {
                        Object.DestroyImmediate(c);
                        log.AppendLine("  - '" + go.name + "': removed Post-Processing v2 component");
                    }
                }
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missing > 0)
                {
                    if ((isPostEffects || camera != null) && !PrefabUtility.IsPartOfPrefabInstance(go))
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        log.AppendLine("  - '" + go.name + "': removed " + missing + " missing (Post-Processing v2) script(s)");
                    }
                    else
                    {
                        log.AppendLine("  - WARNING: '" + go.name + "' has " + missing + " missing script(s); left alone");
                    }
                }

                if (isPostEffects)
                {
                    var settings = volumes[go.name];
                    var volume = go.GetComponent<Volume>();
                    if (volume == null)
                    {
                        volume = go.AddComponent<Volume>();
                        volume.isGlobal = true;
                        volume.weight = settings.weight;
                        log.AppendLine("  - '" + go.name + "': added URP Volume");
                    }
                    if (volume.sharedProfile == null) // also repairs volumes left empty by an earlier failed run
                    {
                        volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(settings.profilePath);
                        EditorUtility.SetDirty(volume);
                        log.AppendLine("  - '" + go.name + "': volume profile = " + settings.profilePath);
                    }
                }

                if (camera != null)
                {
                    UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
                    cameraData.renderPostProcessing = true;
                    cameraData.volumeLayerMask = ~0; // the global volumes live on the "Post" layer
                    EditorUtility.SetDirty(cameraData);
                    log.AppendLine("  - '" + go.name + "': camera post-processing enabled");
                }

                if (isPlayScene && go.name == "Text" && go.GetComponentInParent<Canvas>(true) != null)
                    ConvertMessageText(go, messageFont, log);
            }

            if (SwapSpriteMaterials(root, legacySprite, urpSprite))
                log.AppendLine("  - '" + root.name + "': sprite materials -> URP");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    // --- Input ---

    static bool SetInputHandlingBoth(StringBuilder log)
    {
        const int Both = 2; // 0 = Input Manager (old), 1 = Input System (new), 2 = Both
        Object playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
        var so = new SerializedObject(playerSettings);
        SerializedProperty handler = so.FindProperty("activeInputHandler");
        if (handler == null || handler.intValue == Both)
            return false;
        handler.intValue = Both;
        so.ApplyModifiedPropertiesWithoutUndo();
        log.AppendLine("- Active Input Handling set to Both (restart required)");
        return true;
    }
}
