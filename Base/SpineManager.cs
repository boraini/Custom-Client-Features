using System.Collections;
using System.Collections.Generic;
using System.IO;
using Spine;
using UnityEngine;

// Token: 0x02000768 RID: 1896
[AddComponentMenu("Bytebin/Graphics/SpineManager")]
public class SpineManager : Singleton<SpineManager>
{
    // Token: 0x06003A4A RID: 14922
    public SpineManager()
    {
    }

    // Token: 0x06003A4B RID: 14923
    private void Start()
    {
        base.StartCoroutine("LoadEverything");
    }

    // Token: 0x06003A4C RID: 14924
    public SkeletonDataAsset Skeleton(string name)
    {
        return this.skeletonDataAssets.Get(name);
    }
    
    private IEnumerator LoadEverything()
    {
        this.atlasAssets = new Dictionary<string, AtlasAsset>();
        this.skeletonDataAssets = new Dictionary<string, SkeletonDataAsset>();

        // Load bundled atlases
        string[] array = new string[] { "characters-animated", "entities-animated", "entities" };
        foreach (string atlasName in array)
        {
            string resourceLocation = "Atlases/" + Singleton<AtlasManager>.main.AtlasName(atlasName) + ".atlas";
            TextAsset textAsset = (TextAsset)Resources.Load(resourceLocation);
            LoadAtlasFromTextAsset(atlasName, resourceLocation, textAsset);
        }
        // Load external atlases
        foreach (ExternalAsset externalAsset in ExternalAssetManager.instance.loaded)
        {
            // "spine" is the directory name, and "atlas" is the file extension
            if (externalAsset.type != "spine" || !externalAsset.path.EndsWith(".atlas")) continue;
            WWW www = new WWW(externalAsset.path);
            yield return www;
            string atlasName = Path.GetFileNameWithoutExtension(externalAsset.name);
            TextAsset textAsset = new TextAsset(www.text);
            externalAsset.SetData(textAsset);
            LoadAtlasFromTextAsset(atlasName, externalAsset.path, textAsset);
        }

        /* ===== Spines need to be loaded after the atlases ===== */
        // Load bundled skeletons
        object[] array3 = Resources.LoadAll("Skeletons", typeof(TextAsset));
        foreach (TextAsset textAsset2 in array3)
        {
            LoadSkeletonFromTextAsset(textAsset2.name, textAsset2.name, textAsset2);
        }
        // Load external skeletons
        foreach (ExternalAsset externalAsset in ExternalAssetManager.instance.loaded)
        {
            // "spine" is the directory name, and "json" is the file extension
            if (externalAsset.type != "spine" || !externalAsset.path.EndsWith(".json")) continue;
            WWW www = new WWW(externalAsset.path);
            yield return www;
            TextAsset textAsset = new TextAsset(www.text);
            externalAsset.SetData(textAsset);
            string name = Path.GetFileNameWithoutExtension(externalAsset.name);
            LoadSkeletonFromTextAsset(name, externalAsset.path, textAsset);
            SkeletonDataAsset skeleton = this.skeletonDataAssets[name];
            if (skeleton == null)
            {
                Debug.Log("Skeleton " + name + " not found after import.");
                continue;
            }
            // Spine.SkeletonData skeletonData = skeleton.GetSkeletonData(false);
            // if (skeletonData == null)
            // {
            //     Debug.Log("Skeleton data not found for " + name);
            //     continue;
            // }
            // Debug.Log("Here are the animations for " + name + ":");
            // foreach (Spine.Animation anim in skeletonData.animations)
            // {
            //     Debug.Log("Spine Animation for " + name + ": " + anim.Name);
            // }
        }
    }

    private void LoadAtlasFromTextAsset(string name, string resourceLocation, TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.Log("[WARNING] Spine atlas is null: " + resourceLocation);
        }
        AtlasAsset atlasAsset = (AtlasAsset)ScriptableObject.CreateInstance(typeof(AtlasAsset));
        atlasAsset.atlasFile = textAsset;
        atlasAsset.materials = new Material[] { Singleton<AtlasManager>.main.Collection(name).materials[0] };

        this.atlasAssets[name] = atlasAsset;
    }

    private void LoadSkeletonFromTextAsset(string name, string resourceLocation, TextAsset textAsset2)
    {
        string text3 = name.Replace(".json", string.Empty);
        bool useDragon = text3 == "dragon-ess";
        bool useCharacter = text3 == "player" || text3 == "android";
        bool useStatic = text3 == "turret";
        SkeletonDataAsset skeletonDataAsset = (SkeletonDataAsset)ScriptableObject.CreateInstance(typeof(SkeletonDataAsset));
        skeletonDataAsset.skeletonJSON = textAsset2;
        string atlasName = useDragon ? "dragon" : (useCharacter ? "characters-animated" : (useStatic ? "entities" : "entities-animated"));
        skeletonDataAsset.atlasAsset = this.atlasAssets[atlasName];
        skeletonDataAsset.scale = Entity.SkeletonScale;
        skeletonDataAsset.fromAnimation = new string[0];
        skeletonDataAsset.toAnimation = new string[0];
        this.skeletonDataAssets[text3] = skeletonDataAsset;
        AnimationStateData animationStateData = skeletonDataAsset.GetAnimationStateData();
        if (animationStateData != null)
        {
            animationStateData.DefaultMix = 0.15f;
        }
    }

    // Token: 0x040020CD RID: 8397
    [SerializeField]
    private Dictionary<string, AtlasAsset> atlasAssets;

    // Token: 0x040020CE RID: 8398
    [SerializeField]
    private Dictionary<string, SkeletonDataAsset> skeletonDataAssets;
}
