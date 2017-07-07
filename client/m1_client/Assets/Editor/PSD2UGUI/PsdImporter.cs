namespace PsdLayoutTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using PhotoshopFile;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using TMPro;


    public static class PsdImporter
    {
        // 注，以下是系统绝对路径
        public static readonly string texturePath = ResDefine.GetResPath(EResType.eResPicture);  //"Texture/AutoGenerate/";
        public static readonly string prefabPath = ResDefine.GetResPath(EResType.eResUI);        //"Resources/UI/AutoGenerate/";

        // 需要查重的目录
        public static string[] checkDir = {
            texturePath,
        };

        //private static string currentPath;

        private static GameObject rootPsdGameObject;
        
        private static GameObject currentGroupGameObject;
        
        private static float currentDepth;
        
        private static float depthStep;
        
        static PsdImporter()
        {
            MaximumDepth = 0.001f;
            PixelsToUnits = 1;
            CreateCanvas = false;
        }
        
        public static float MaximumDepth { get; set; }
        
        public static float PixelsToUnits { get; set; }
        
        public static bool UseMd5 { get; set; }

        public static bool CreateCanvas { get; set; }
        
        private static bool LayoutInScene { get; set; }
        
        private static bool CreatePrefab { get; set; }
        
        private static Vector2 CanvasSize { get; set; }
        
        private static string PsdName { get; set; }
        
        private static GameObject Canvas { get; set; }
                
        public static void ExportLayersAsTextures(string assetPath)
        {
            LayoutInScene = false;
            CreatePrefab = false;
            Import(assetPath);
        }
        
        public static void LayoutInCurrentScene(string assetPath)
        {
            LayoutInScene = true;
            CreatePrefab = false;
            Import(assetPath);
        }
        
        public static void GeneratePrefab(string assetPath)
        {
            LayoutInScene = false;
            CreatePrefab = true;
            Import(assetPath);
        }

        static string ImportTexturePath;
        private static void Import(string asset)
        {
            int lastSlash = asset.LastIndexOf('/');
            string assetPathWithoutFilename = asset.Remove(lastSlash + 1, asset.Length - (lastSlash + 1));
            PsdName = asset.Replace(assetPathWithoutFilename, string.Empty).Replace(".psd", string.Empty).Trim();

            ImportTexturePath = Path.Combine(texturePath + "/AutoGenerate/", PsdName);
            if (!Directory.Exists(prefabPath))
                Directory.CreateDirectory(prefabPath);
            if (!Directory.Exists(ImportTexturePath))
                Directory.CreateDirectory(ImportTexturePath);
            
            currentDepth = MaximumDepth;
            string fullPath = Path.Combine(GetFullProjectPath(), asset.Replace('\\', '/'));

            PsdFile psd = new PsdFile(fullPath);
            CanvasSize = new Vector2(psd.Width, psd.Height);

            // Set the depth step based on the layer count.  If there are no layers, default to 0.1f.
            depthStep = psd.Layers.Count != 0 ? MaximumDepth / psd.Layers.Count : 0.1f;


            //currentPath = GetFullProjectPath() + "Assets";
            //currentPath = Path.Combine(currentPath, ImportTexturePath);
            //Directory.CreateDirectory(currentPath);

            if (LayoutInScene || CreatePrefab)
            {
                if (CreateCanvas)
                {
                    CreateUIEventSystem();
                    CreateUICanvas();
                    rootPsdGameObject = Canvas;
                }
                else
                {
                    rootPsdGameObject = new GameObject(PsdName);
                    RectTransform rect = rootPsdGameObject.AddComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.sizeDelta = new Vector2(0, 0);
                }

                currentGroupGameObject = rootPsdGameObject;
            }

            List<Layer> tree = BuildLayerTree(psd.Layers);
            ExportTree(tree);

            if (CreatePrefab)
            {
                string assetPrefabPath = GetRelativePath(prefabPath + "/AutoGenerate/");
                UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(assetPrefabPath + PsdName + ".prefab");
                PrefabUtility.ReplacePrefab(rootPsdGameObject, prefab);
                
                if (!LayoutInScene)
                {
                    // if we are not flagged to layout in the scene, delete the GameObject used to generate the prefab
                    UnityEngine.Object.DestroyImmediate(rootPsdGameObject);
                }                
            }
            
            AssetDatabase.Refresh();
        }

        private static List<Layer> BuildLayerTree(List<Layer> flatLayers)
        {
            // There is no tree to create if there are no layers
            if (flatLayers == null)
            {
                return null;
            }

            // PSD layers are stored backwards (with End Groups before Start Groups), so we must reverse them
            flatLayers.Reverse();

            List<Layer> tree = new List<Layer>();
            Layer currentGroupLayer = null;
            Stack<Layer> previousLayers = new Stack<Layer>();

            foreach (Layer layer in flatLayers)
            {
                if (IsEndGroup(layer))
                {
                    if (previousLayers.Count > 0)
                    {
                        Layer previousLayer = previousLayers.Pop();
                        previousLayer.Children.Add(currentGroupLayer);
                        currentGroupLayer = previousLayer;
                    }
                    else if (currentGroupLayer != null)
                    {
                        tree.Add(currentGroupLayer);
                        currentGroupLayer = null;
                    }
                }
                else if (IsStartGroup(layer))
                {
                    // push the current layer
                    if (currentGroupLayer != null)
                    {
                        previousLayers.Push(currentGroupLayer);
                    }

                    currentGroupLayer = layer;
                }
                else if (layer.Rect.width != 0 && layer.Rect.height != 0)
                {
                    // It must be a text layer or image layer
                    if (currentGroupLayer != null)
                    {
                        currentGroupLayer.Children.Add(layer);
                    }
                    else
                    {
                        tree.Add(layer);
                    }
                }
            }

            // if there are any dangling layers, add them to the tree
            if (tree.Count == 0 && currentGroupLayer != null && currentGroupLayer.Children.Count > 0)
            {
                tree.Add(currentGroupLayer);
            }

            return tree;
        }
        
        private static string MakeNameSafe(string name)
        {
            // replace all special characters with an underscore
            Regex pattern = new Regex("[/:&.<>,$¢;+]");
            string newName = pattern.Replace(name, "_");

            if (name != newName)
            {
                Debug.Log(string.Format("Layer name \"{0}\" was changed to \"{1}\"", name, newName));
            }

            return newName.Trim();
        }
        
        private static bool IsStartGroup(Layer layer)
        {
            return layer.IsPixelDataIrrelevant;
        }
        
        private static bool IsEndGroup(Layer layer)
        {
            return layer.Name.Contains("</Layer set>") ||
                layer.Name.Contains("</Layer group>") ||
                (layer.Name == " copy" && layer.Rect.height == 0);
        }
        
        public static string GetFullProjectPath()
        {
            string projectDirectory = Application.dataPath;

            // remove the Assets folder from the end since each imported asset has it already in its local path
            if (projectDirectory.EndsWith("Assets"))
            {
                projectDirectory = projectDirectory.Remove(projectDirectory.Length - "Assets".Length);
            }

            return projectDirectory;
        }
        
        public static string GetRelativePath(string fullPath)
        {
            return fullPath.Replace(GetFullProjectPath(), string.Empty);
        }

        #region Layer Exporting Methods
        
        private static void ExportTree(List<Layer> tree)
        {
            // we must go through the tree in reverse order since Unity draws from back to front, but PSDs are stored front to back
            for (int i = tree.Count - 1; i >= 0; i--)
            {
                ExportLayer(tree[i]);
            }
        }
        
        private static void ExportLayer(Layer layer)
        {
            layer.Name = MakeNameSafe(layer.Name);
            if (string.IsNullOrEmpty(layer.Name))
            {
                return;
            }
            if (layer.IsFolderLayer)
            {
                ExportFolderLayer(layer);
            }
            else //textlayer or imagelayer
            {
                ExportArtLayer(layer);
            }
        }

        private static void ExportFolderLayer(Layer layer)
        {
            if (layer.Name.ContainsIgnoreCase("|Button"))
            {
                layer.Name = layer.Name.ReplaceIgnoreCase("|Button", string.Empty);
                CreateUIButton(layer);
            }
            else if (layer.Name.ContainsIgnoreCase("|InputField"))
            {
                layer.Name = layer.Name.ReplaceIgnoreCase("|InputField", string.Empty);
                CreateUIInputField(layer);
            }
            else if (layer.Name.ContainsIgnoreCase("|Toggle"))
            {
                layer.Name = layer.Name.ReplaceIgnoreCase("|Toggle", string.Empty);
                CreateUIToggle(layer);
            }
            else if (layer.Name.ContainsIgnoreCase("|ScrollView"))
            {
                layer.Name = layer.Name.Replace("|ScrollView", string.Empty);
                CreateUIScrollview(layer);
            }
            else if (layer.Name.ContainsIgnoreCase("|ProgressBar"))
            {
                layer.Name = layer.Name.Replace("|ProgressBar", string.Empty);
                CreateUIProgressBar(layer);
            }

            else
            {
                // it is a "normal" folder layer that contains children layers
                //if (layer.Name.Contains("|"))
                //{
                //    Debug.LogError(string.Format("图层名中含有未知的标签！Name = {0}", layer.Name));
                //    return;
                //}

                if (layer.Name.ContainsIgnoreCase("|"))
                {
                    layer.Name = layer.Name.ReplaceIgnoreCase("|", "_");
                }

                //string oldPath = currentPath;
                GameObject oldGroupObject = currentGroupGameObject;
                //currentPath = Path.Combine(currentPath, layer.Name);
                //Directory.CreateDirectory(currentPath);

                if (LayoutInScene || CreatePrefab)
                {
                    currentGroupGameObject = new GameObject(layer.Name);

                    RectTransform rect = currentGroupGameObject.AddComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.sizeDelta = new Vector2(0, 0);

                    currentGroupGameObject.transform.parent = oldGroupObject.transform;
                }

                ExportTree(layer.Children);

                //currentPath = oldPath;
                currentGroupGameObject = oldGroupObject;
            }
        }
        
        private static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            string lowSource = source.ToLower();
            string lowCheck = toCheck.ToLower();
            return lowSource.IndexOf(lowCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        
        private static string ReplaceIgnoreCase(this string str, string oldValue, string newValue)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
            }

            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
        
        private static void ExportArtLayer(Layer layer)
        {
            if (layer.IsImageLayer)
            {
                if (LayoutInScene || CreatePrefab)
                {
                    // create a sprite from the layer to lay it out in the scene
                    CreateUIImage(layer);
                }
                else
                {
                    // it is not being laid out in the scene, so simply save out the .png file
                    CreatePNG(layer);
                }
            }
            else
            {
                // it is a text layer
                if (LayoutInScene || CreatePrefab)
                {
                    CreateUIText(layer);
                }
            }
        }

        private static string CreatePNG(Layer layer)
        {
            string file = string.Empty;

            if (layer.Children.Count == 0 && layer.Rect.width > 0)
            {
                //decode the layer into a texture
                Texture2D texture = ImageDecoder.DecodeImage(layer);
                
                string pngPath = ImportTexturePath;
                file = Path.Combine(pngPath, layer.Name + ".png");

                //int i = 0;
                //while (File.Exists(file))
                //{
                //    i++;
                //    file = Path.Combine(pngPath, layer.Name + i.ToString() + ".png");
                //}
                file = file.Replace("@", string.Empty);
                file = file.Replace("\\", "/");
                if (File.Exists(file))
                {
                    return file;
                }
                File.WriteAllBytes(file, texture.EncodeToPNG());                
            }

            return file;
        }


        #region delete repeat sprite
        // 是否是同一份sprite文件，注此方法要求texture2d代码可读！
        private static bool IsSameSpriteFile(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                return false;
            }
            string relName1 = GetRelativePath(file1);
            Texture2D tex1 = (Texture2D)AssetDatabase.LoadAssetAtPath(relName1, typeof(Texture2D));

            string relName2 = GetRelativePath(file2);
            Texture2D tex2 = (Texture2D)AssetDatabase.LoadAssetAtPath(relName2, typeof(Texture2D));

            return IsSameSprite(tex1, tex2);
        }

        // 判定是否是同一张图片
        private static bool IsSameSprite(Texture2D tex1, Texture2D tex2)
        {
            if (tex1.width != tex2.width || tex1.height != tex2.height)
            {
                return false;
            }
            byte[] bytes = tex1.EncodeToPNG();

            //int dif = 0;
            for (int w = 0; w < tex1.width; w += 5)
            {
                for (int h = 0; h < tex1.height; h += 5)
                {
                    Color c1 = tex1.GetPixel(w, h);
                    Color c2 = tex2.GetPixel(w, h);
                    if (!c1.Equals(c2))
                    {
                        return false;
                        //++dif;
                        //Debug.Log(string.Format("???? dif: w={0} h={1} c1=({2},{3},{4}) c2=({5},{6},{7})", w, h, c1.r, c1.g, c1.b, c2.r, c2.g, c2.b));
                        //return false;
                    }
                }
            }
            //if (dif > 1)
            //{
            //    return false;
            //}
            return true;
        }

        private static bool IsSameSpriteMd5File(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                return false;
            }
            string m1 = Util.md5file(file1);
            string m2 = Util.md5file(file2);
            return m1.Equals(m2);
        }
        
        // 替换prefab中重复的资源
        private static void resetPrefabSpite(string spriteFile, string spriteToDel)
        {
            string assetPrefabPath = GetRelativePath(prefabPath);
            string findPath = Path.GetDirectoryName(assetPrefabPath);
            string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { findPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                List<Image> imgs = GetObjectDependencies<Image>(go);
                for (int j = 0; j < imgs.Count; j++)
                {
                    if ((imgs[j].sprite.ToString() != "null"))
                    {
                        string spritePathDel = GetRelativePath(spriteToDel);
                        string spritePath = AssetDatabase.GetAssetPath(imgs[j].sprite);
                        if (spritePath == spritePathDel)
                        {
                            imgs[j].sprite = (Sprite)AssetDatabase.LoadAssetAtPath(GetRelativePath(spriteFile), typeof(Sprite));
                            EditorUtility.SetDirty(go);
                        }                        
                    }
                }
            }
        }
        
        // 获取gameobject中T类型的组件
        private static List<T> GetObjectDependencies<T>(GameObject go)
        {
            List<T> results = new List<T>();
            UnityEngine.Object[] roots = new UnityEngine.Object[] { go };
            UnityEngine.Object[] dependObjs = EditorUtility.CollectDependencies(roots);
            foreach (UnityEngine.Object dependObj in dependObjs)
            {
                if (dependObj != null && dependObj.GetType() == typeof(T))
                {
                    results.Add((T)System.Convert.ChangeType(dependObj, typeof(T)));
                }
            }

            return results;
        }

        static List<string> allPngFiles = new List<string>();
        static void collectAllPngFiles()
        {
            allPngFiles.Clear();

            for (int i = 0; i < checkDir.Length; i++)
            {
                string checkPath = checkDir[i];
                getAllFiles(checkPath);
            }
        }
        static void getAllFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.png");
                for (int j = 0; j < files.Length; j++)
                {
                    string fullName = files[j].Replace("\\", "/");
                    allPngFiles.Add(fullName);
                }

                string[] directories = Directory.GetDirectories(path);
                for (int i = 0; i < directories.Length; i++)
                {
                    getAllFiles(directories[i]);
                }
            }
        }
        
        // 删除重复的图片，并将重新修改prefab中的引用
        public static void DeleteRepeatSprites(bool useMd5 = false)
        {
            collectAllPngFiles();

            for (int i = 0; i < allPngFiles.Count; i++)
            {
                for (int j = i + 1; j < allPngFiles.Count; j++)
                {
                    string file = allPngFiles[i];
                    string fileToBeDelete = allPngFiles[j];
                    if (file != fileToBeDelete)
                    {
                        bool same = false;
                        if (useMd5)
                        {
                            same = IsSameSpriteMd5File(file, fileToBeDelete);
                        }
                        else
                        {
                            //same = IsSameSpriteFile(file, fileToBeDelete);
                            same = PngUtil.IsSamePng(file, fileToBeDelete);
                        } 
                        if (same) {
                            resetPrefabSpite(file, fileToBeDelete);
                            File.Delete(fileToBeDelete);
                        }                            
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DeleteCurrentPsdRepeatSprites(string assetPath, bool useMd5 = false)
        {
            string currentPath = Path.Combine(texturePath, assetPath);
            if (!Directory.Exists(currentPath))
            {
                return;
            }
            string[] files = Directory.GetFiles(currentPath, "*.png");
            for (int j = 0; j < files.Length; j++)
            {
                files[j] = files[j].Replace("\\", "/");
            }
            for (int i = 0; i < files.Length; i++)
            {
                for (int j = i + 1; j < files.Length; j++)
                {
                    string file = files[i];
                    string fileToBeDelete = files[j];
                    if (file != fileToBeDelete)
                    {
                        bool same = false;
                        if (useMd5)
                        {
                            same = IsSameSpriteMd5File(file, fileToBeDelete);
                        }
                        else
                        {
                            //same = IsSameSpriteFile(file, fileToBeDelete);
                            same = PngUtil.IsSamePng(file, fileToBeDelete);
                        }
                        if (same)
                        {
                            resetPrefabSpite(file, fileToBeDelete);
                            File.Delete(fileToBeDelete);
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static void PrintAllReference(string assetPath)
        {
            string assetPrefabPath = GetRelativePath(prefabPath);
            string findPath = Path.GetDirectoryName(assetPrefabPath);
            Sprite sp = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { findPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                List<Image> imgs = GetObjectDependencies<Image>(go);
                
                for (int j = 0; j < imgs.Count; j++)
                {
                    if (imgs[j].sprite == sp)
                    {
                        Debug.Log(path,go);
                    }
                }
            }
        }
        public static void DeleteUnusedSprites()
        {
            collectAllPngFiles();
            List<string> allUsedSprites = new List<string>();

            string assetPrefabPath = GetRelativePath(prefabPath);
            string findPath = Path.GetDirectoryName(assetPrefabPath);
            string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { findPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                List<Image> imgs = GetObjectDependencies<Image>(go);
                for (int j = 0; j < imgs.Count; j++)
                {
                    if ((imgs[j].sprite.ToString() != "null"))
                    {
                        string spritePath = AssetDatabase.GetAssetPath(imgs[j].sprite);
                        if (!allUsedSprites.Contains(spritePath))
                        {
                            allUsedSprites.Add(spritePath);
                        }                        
                    }
                }
            }

            for (int i = 0; i < allPngFiles.Count; i++)
            {
                string relPath = GetRelativePath(allPngFiles[i]);
                if (!allUsedSprites.Contains(relPath))
                {
                    File.Delete(allPngFiles[i]);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DeleteRepeatSpritesNameof(string fullname)
        {
            collectAllPngFiles();
            string shortname = Path.GetFileNameWithoutExtension(fullname);
            for (int i = 0; i < allPngFiles.Count; i++)
            {
                string pngfullname = allPngFiles[i];
                if (fullname == pngfullname)
                {
                    continue;
                }
                string pngshortnane = Path.GetFileNameWithoutExtension(pngfullname);
                if (shortname == pngshortnane)
                {
                    resetPrefabSpite(fullname, pngfullname);
                    File.Delete(pngfullname);
                    Debug.Log("delete " + pngfullname);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion

        private static Sprite CreateSprite(Layer layer)
        {
            return CreateSprite(layer, PsdName);
        }
        
        private static Sprite CreateSprite(Layer layer, string packingTag)
        {
            Sprite sprite = null;

            if (layer.Children.Count == 0 && layer.Rect.width > 0)
            {
                string file = CreatePNG(layer);
                sprite = ImportSprite(GetRelativePath(file), packingTag);
            }
            
            return sprite;
        }
        
        private static Sprite ImportSprite(string relativePathToSprite, string packingTag)
        {
            AssetDatabase.ImportAsset(relativePathToSprite, ImportAssetOptions.ForceUpdate);

            // change the importer to make the texture a sprite
            TextureImporter textureImporter = AssetImporter.GetAtPath(relativePathToSprite) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.mipmapEnabled = false;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
                textureImporter.maxTextureSize = 2048;
                textureImporter.spritePixelsPerUnit = PixelsToUnits;
                textureImporter.spritePackingTag = packingTag;
                textureImporter.isReadable = true;
            }

            AssetDatabase.ImportAsset(relativePathToSprite, ImportAssetOptions.ForceUpdate);

            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(relativePathToSprite, typeof(Sprite));
            return sprite;
        }
        #endregion

        private static void AdjustUIRectByLayer(GameObject gameObject, Layer layer)
        {
            float x = layer.Rect.x / PixelsToUnits;
            float y = layer.Rect.y / PixelsToUnits;

            // Photoshop increase Y while going down. Unity increases Y while going up.  So, we need to reverse the Y position.
            y = (CanvasSize.y / PixelsToUnits) - y;

            // Photoshop uses the upper left corner as the pivot (0,0).  Unity defaults to use the center as (0,0), so we must offset the positions.
            x = x - ((CanvasSize.x / 2) / PixelsToUnits);
            y = y - ((CanvasSize.y / 2) / PixelsToUnits);

            float width = layer.Rect.width / PixelsToUnits;
            float height = layer.Rect.height / PixelsToUnits;

            gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            if (transform == null)
            {
                transform = gameObject.AddComponent<RectTransform>();
            }
            transform.sizeDelta = new Vector2(width, height);
            if (layer.IsTextLayer)
            {
                if (transform.sizeDelta.y < layer.FontSize + 20)   // 临时加的，目前无法找到更好的方案
                {
                    transform.sizeDelta = new Vector2(transform.sizeDelta.x, layer.FontSize + 20);
                }
            }
        }

        #region Unity UI
        private static void CreateUIEventSystem()
        {
            if (!GameObject.Find("EventSystem"))
            {
                GameObject gameObject = new GameObject("EventSystem");
                gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
            }
        }
        
        private static void CreateUICanvas()
        {
            Canvas = new GameObject(PsdName);

            Canvas canvas = Canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform transform = Canvas.GetComponent<RectTransform>();
            Vector2 scaledCanvasSize = new Vector2(CanvasSize.x / PixelsToUnits, CanvasSize.y / PixelsToUnits);
            transform.sizeDelta = scaledCanvasSize;

            CanvasScaler scaler = Canvas.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = PixelsToUnits;
            scaler.referencePixelsPerUnit = PixelsToUnits;

            Canvas.AddComponent<GraphicRaycaster>();
        }
        
        private static Image CreateUIImage(Layer layer)
        {
            layer.Name = layer.Name.ReplaceIgnoreCase("|Image", string.Empty);
            layer.Name = layer.Name.Replace('|', '_');
            GameObject gameObject = new GameObject(layer.Name.Trim());
            Image image = gameObject.AddComponent<Image>();
            AdjustUIRectByLayer(gameObject, layer);
            gameObject.transform.SetParent(currentGroupGameObject.transform, false);

            // if the current group object actually has a position (not a normal Photoshop folder layer), then offset the position accordingly
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + currentGroupGameObject.transform.position.x, gameObject.transform.position.y + currentGroupGameObject.transform.position.y, gameObject.transform.position.z);

            currentDepth -= depthStep;

            image.sprite = CreateSprite(layer);
            image.raycastTarget = false;
            return image;
        }
        
        private static TextMeshProUGUI CreateUIText(Layer layer)
        {
            layer.Name = layer.Name.ReplaceIgnoreCase("|Text", string.Empty);
            layer.Name = layer.Name.Replace('|', '_');
            GameObject gameObject = new GameObject(layer.Name.Trim());
            //Text textUI = gameObject.AddComponent<Text>();
            TextMeshProUGUI textUI = gameObject.AddComponent<TextMeshProUGUI>();

            AdjustUIRectByLayer(gameObject, layer);
            gameObject.transform.SetParent(currentGroupGameObject.transform, false);

            currentDepth -= depthStep;

            TMP_FontAsset font = null;
            if(layer.FontName == "LiSu")
            {
                font = ObjectPoolManager.GetSharedResource("FontsMaterials/SIMLI SDF", EResType.eResFontAsset) as TMP_FontAsset;
            }
            else //SimHei
            {
                font = ObjectPoolManager.GetSharedResource("FontsMaterials/Droid Sans Fallback SDF", EResType.eResFontAsset) as TMP_FontAsset;
            }
            textUI.font = font;
            textUI.text = layer.Text;
            //textUI.font = font;

            float fontSize = layer.FontSize / PixelsToUnits;
            float ceiling = Mathf.Ceil(fontSize);
            if (fontSize < ceiling)
            {
                // Unity UI Text doesn't support floating point font sizes, so we have to round to the next size and scale everything else
                float scaleFactor = ceiling / fontSize;
                textUI.fontSize = (int)ceiling;
                textUI.rectTransform.sizeDelta *= scaleFactor;
                textUI.rectTransform.localScale /= scaleFactor;
            }
            else
            {
                textUI.fontSize = (int)fontSize;
            }
            // 临时加的，目前无法找到更好的方案
            if (textUI.rectTransform.sizeDelta.y < fontSize + 20)
            {
                textUI.rectTransform.sizeDelta = new Vector2(textUI.rectTransform.sizeDelta.x, fontSize + 20);
            }

            textUI.color = new Color(layer.FillColor.r/255, layer.FillColor.g/255, layer.FillColor.b/255);
            textUI.alignment = TextAlignmentOptions.Center;

            switch (layer.Justification)
            {
                case TextJustification.Left:
                    textUI.alignment = TextAlignmentOptions.Left;
                    break;
                case TextJustification.Right:
                    textUI.alignment = TextAlignmentOptions.Right;
                    break;
                case TextJustification.Center:
                    textUI.alignment = TextAlignmentOptions.Center;
                    break;
            }
            textUI.raycastTarget = false;
            return textUI;
        }
        
        private static Button CreateUIButton(Layer layer)
        {
            // create an empty Image object with a Button behavior attached
            
            // look through the children for a clip rect
            ////Rectangle? clipRect = null;
            ////foreach (Layer child in layer.Children)
            ////{
            ////    if (child.Name.ContainsIgnoreCase("|ClipRect"))
            ////    {
            ////        clipRect = child.Rect;
            ////    }
            ////}

            Button button = null;

            Layer defaultLayer = layer.GetChildByTag("|Default");
            if (defaultLayer == null)
            {
                Debug.LogError("创建button出错，图层 " + layer.Name + "中 缺少 |Default标签");
                return null;
            }
            defaultLayer.Name = defaultLayer.Name.ReplaceIgnoreCase("|Default", string.Empty).Trim();
            if (defaultLayer.IsImageLayer)
            {
                Image image = CreateUIImage(defaultLayer);
                button = image.gameObject.AddComponent<Button>();
                button.targetGraphic = image;
            }
            else if (defaultLayer.IsTextLayer)
            {
                TextMeshProUGUI text = CreateUIText(defaultLayer);
                button = text.gameObject.AddComponent<Button>();
            }
            AdjustUIRectByLayer(button.gameObject, defaultLayer);
            button.gameObject.name = layer.Name.Trim();

            foreach (Layer child in layer.Children)
            {
                if (child == defaultLayer)
                {
                    continue;
                }
                if (child.IsImageLayer)
                {
                    if (child.Name.ContainsIgnoreCase("|Disabled") && defaultLayer.IsImageLayer)
                    {
                        child.Name = child.Name.ReplaceIgnoreCase("|Disabled", string.Empty).Trim();
                        button.transition = Selectable.Transition.SpriteSwap;

                        SpriteState spriteState = button.spriteState;
                        spriteState.disabledSprite = CreateSprite(child);
                        button.spriteState = spriteState;
                    }
                    if(child.Name.ContainsIgnoreCase("|Highlighted") && defaultLayer.IsImageLayer)
                    {
                        child.Name = child.Name.ReplaceIgnoreCase("|Highlighted", string.Empty).Trim();
                        button.transition = Selectable.Transition.SpriteSwap;

                        SpriteState spriteState = button.spriteState;
                        spriteState.highlightedSprite = CreateSprite(child);
                        button.spriteState = spriteState;
                    }
                    if (child.Name.ContainsIgnoreCase("|Pressed") && defaultLayer.IsImageLayer)
                    {
                        child.Name = child.Name.ReplaceIgnoreCase("|Pressed", string.Empty).Trim();
                        button.transition = Selectable.Transition.SpriteSwap;

                        SpriteState spriteState = button.spriteState;
                        spriteState.pressedSprite = CreateSprite(child);
                        button.spriteState = spriteState;
                    }
                    else
                    {
                        Image image = CreateUIImage(child);
                        image.transform.SetParent(button.transform, false);
                        AdjustUIRectByLayer(image.gameObject, child);
                        image.raycastTarget = false;
                    }
                }
                else if (child.IsTextLayer)
                {
                    TextMeshProUGUI text = CreateUIText(child);
                    text.transform.SetParent(button.transform, false);
                    AdjustUIRectByLayer(text.gameObject, child);
                }
                else if (child.IsFolderLayer)
                {
                    GameObject oldGroupObject = currentGroupGameObject;
                    currentGroupGameObject = button.gameObject;
                    ExportFolderLayer(child);
                    currentGroupGameObject = oldGroupObject;
                }
            }            
            return button;
        }
        
        private static InputField CreateUIInputField(Layer layer)
        {
            Layer textLayer = layer.GetChildByTag("|Text");
            if (textLayer == null)
            {
                Debug.LogError("创建inputField出错，图层 " + layer.Name + "中 缺少 |Text 标签");
                return null;
            }
            textLayer.Name = textLayer.Name.ReplaceIgnoreCase("|Text", string.Empty).Trim();

            InputField inputField = null;
            TextMeshProUGUI text = CreateUIText(textLayer);

            Layer backgroundLayer = layer.GetImageChild("|Background");
            if (backgroundLayer != null)    // 以background为主体
            {
                backgroundLayer.Name = backgroundLayer.Name.ReplaceIgnoreCase("|Background", string.Empty).Trim();
                Image background = CreateUIImage(backgroundLayer);
                inputField = background.gameObject.AddComponent<InputField>();

                text.transform.SetParent(inputField.transform, false);
                AdjustUIRectByLayer(text.gameObject, textLayer);
            }
            else
            {
                inputField = text.gameObject.AddComponent<InputField>();
                AdjustUIRectByLayer(inputField.gameObject, textLayer);
            }

            //inputField.textComponent = text;
            inputField.gameObject.name = layer.Name.Trim();
            
            foreach (Layer child in layer.Children)
            {
                if (child == textLayer || child == backgroundLayer)
                {
                    continue;
                }
                if (child.Name.ContainsIgnoreCase("|Placeholder"))
                {
                    child.Name = child.Name.ReplaceIgnoreCase("|Placeholder", string.Empty).Trim();
                    TextMeshProUGUI placeholderText = CreateUIText(child);
                    placeholderText.gameObject.transform.SetParent(inputField.gameObject.transform, false);
                    AdjustUIRectByLayer(placeholderText.gameObject, child);
                    inputField.placeholder = placeholderText;
                }
                else
                {
                    if (child.IsTextLayer)
                    {
                        TextMeshProUGUI textChild = CreateUIText(child);
                        textChild.transform.SetParent(inputField.transform, false);
                        AdjustUIRectByLayer(textChild.gameObject, child);
                    }
                    else if (child.IsImageLayer)
                    {
                        Image imageChild = CreateUIImage(child);
                        imageChild.transform.SetParent(inputField.transform, false);
                        AdjustUIRectByLayer(imageChild.gameObject, child);
                    }
                    else if (child.IsFolderLayer)
                    {
                        GameObject oldGroupObject = currentGroupGameObject;
                        currentGroupGameObject = inputField.gameObject;
                        ExportFolderLayer(child);
                        currentGroupGameObject = oldGroupObject;
                    }
                }                
            }
            return null;
        }
        
        private static Toggle CreateUIToggle(Layer layer)
        {
            Layer backgroundLayer = layer.GetChildByTag("|Background");
            Layer checkLayer = layer.GetChildByTag("|Check");
            if (backgroundLayer == null || checkLayer == null)
            {
                Debug.LogError("创建Toggle出错，图层 " + layer.Name + "中 缺少 |Background 或 |Check 标签");
                return null;
            }
            backgroundLayer.Name = backgroundLayer.Name.ReplaceIgnoreCase("|Background", string.Empty).Trim();
            checkLayer.Name = checkLayer.Name.ReplaceIgnoreCase("|Check", string.Empty).Trim();
            Toggle toggle = null;
            Image backgroundImg = CreateUIImage(backgroundLayer);
            toggle = backgroundImg.gameObject.AddComponent<Toggle>();
            AdjustUIRectByLayer(toggle.gameObject, backgroundLayer);

            Image checkImg = CreateUIImage(checkLayer);
            checkImg.transform.SetParent(backgroundImg.transform);
            AdjustUIRectByLayer(checkImg.gameObject, checkLayer);

            toggle.targetGraphic = backgroundImg;
            toggle.graphic = checkImg;

            foreach (Layer child in layer.Children)
            {
                if (child == backgroundLayer || child == checkLayer)
                {
                    continue;
                }
                if (child.IsTextLayer)
                {
                    TextMeshProUGUI textChild = CreateUIText(child);
                    textChild.transform.SetParent(toggle.transform, false);
                    AdjustUIRectByLayer(textChild.gameObject, child);
                }
                else if (child.IsImageLayer)
                {
                    Image imageChild = CreateUIImage(child);
                    imageChild.transform.SetParent(toggle.transform, false);
                    AdjustUIRectByLayer(imageChild.gameObject, child);
                }
                else if (child.IsFolderLayer)
                {
                    GameObject oldGroupObject = currentGroupGameObject;
                    currentGroupGameObject = toggle.gameObject;
                    ExportFolderLayer(child);
                    currentGroupGameObject = oldGroupObject;
                }
            }
            return toggle;
        }

        private static ScrollRect CreateUIScrollview(Layer layer)
        {
            Layer backgroundLayer = layer.GetImageChild("|Background");
            if (backgroundLayer == null)
            {
                Debug.LogError("创建scrollView出错，图层 " + layer.Name + "中 缺少 |Background 标签");
                return null;
            }

            ScrollRect scrollRect = null;
            backgroundLayer.Name = backgroundLayer.Name.ReplaceIgnoreCase("|Background", string.Empty).Trim();

            Image backgroundImg = CreateUIImage(backgroundLayer);
            backgroundImg.name = layer.Name.Trim();
            scrollRect = backgroundImg.gameObject.AddComponent<ScrollRect>();

            // viewport
            GameObject viewPort = new GameObject("ViewPort");
            viewPort.AddComponent<RectTransform>();
            viewPort.transform.SetParent(backgroundImg.transform, false);
            Image maskImgae = viewPort.AddComponent<Image>();
            maskImgae.sprite = CreateSprite(backgroundLayer);
            maskImgae.type = Image.Type.Sliced;            
            RectTransform rectViewPort = viewPort.GetComponent<RectTransform>();
            rectViewPort.sizeDelta = backgroundImg.GetComponent<RectTransform>().sizeDelta;
            rectViewPort.anchorMin = new Vector2(0, 0);
            rectViewPort.anchorMax = new Vector2(1, 1);
            rectViewPort.offsetMin = new Vector2(0, 0);
            rectViewPort.offsetMax = new Vector2(0, 0);
            viewPort.AddComponent<UnityEngine.UI.Mask>();

            // Content
            GameObject content = new GameObject("Content");
            viewPort.AddComponent<RectTransform>();
            content.transform.SetParent(viewPort.transform);
            VerticalLayoutGroup verticalLayerGroup = content.AddComponent<VerticalLayoutGroup>();
            verticalLayerGroup.spacing = 10;
            verticalLayerGroup.childForceExpandWidth = true;
            verticalLayerGroup.childForceExpandHeight = false;
            RectTransform rectContent = content.GetComponent<RectTransform>();
            rectContent.anchorMin = new Vector2(0, 1);
            rectContent.anchorMax = new Vector2(1, 1);
            rectContent.pivot = new Vector2(0.5f, 1);
            rectContent.offsetMax = new Vector2(0, 0);
            rectContent.offsetMin = new Vector2(0, 0 - backgroundLayer.Rect.height * 1.5f);

            // scrollrect
            scrollRect.content = rectContent;
            scrollRect.viewport = rectViewPort;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            GameObject.DestroyImmediate(backgroundImg);
            
            GameObject oldGroupObject = currentGroupGameObject;
            foreach (Layer child in layer.Children)
            {
                if (child == backgroundLayer)
                {
                    continue;
                }
                if (child.Name.ContainsIgnoreCase("|Element"))
                {
                    child.Name = child.Name.ReplaceIgnoreCase("|Element", string.Empty).Trim();
                    currentGroupGameObject = content;
                }
                else
                {
                    currentGroupGameObject = scrollRect.gameObject;
                }
                if (child.IsTextLayer)
                {
                    TextMeshProUGUI textChild = CreateUIText(child);
                    AdjustUIRectByLayer(textChild.gameObject, child);
                }
                if (child.IsFolderLayer)
                {
                    ExportFolderLayer(child);
                }
                if (child.IsImageLayer)
                {
                    Image imageChild = CreateUIImage(child);
                    AdjustUIRectByLayer(imageChild.gameObject, child);
                }
            }

            int elementCount = content.transform.childCount;
            for (int i = 0; i < elementCount; i++)
            {
                GameObject childObj = content.transform.GetChild(i).gameObject;
                RectTransform rect = childObj.GetComponent<RectTransform>();
                if (rect != null)
                {
                    LayoutElement layout = childObj.AddComponent<LayoutElement>();
                    layout.preferredHeight = rect.sizeDelta.y;
                    layout.preferredWidth = rect.sizeDelta.x;
                }
            }

            currentGroupGameObject = oldGroupObject;
            
            return scrollRect;
        }

        private static GameObject CreateUIProgressBar(Layer layer)
        {
            GameObject gameObject = new GameObject(layer.Name.Trim());
            gameObject.AddComponent<RectTransform>();
            gameObject.transform.SetParent(currentGroupGameObject.transform, false);

            Layer bg = layer.GetChildByTag("|Background");
            if (bg == null)
            {
                Debug.LogError("创建ProgressBar出错，图层 " + layer.Name + "中 缺少 |Background标签");
                return null;
            }
            Layer fg = layer.GetChildByTag("|Foreground");
            if (fg == null)
            {
                Debug.LogError("创建ProgressBar出错，图层 " + layer.Name + "中 缺少 |Foreground标签");
                return null;
            }
            bg.Name = bg.Name.ReplaceIgnoreCase("|Background", string.Empty).Trim();
            fg.Name = fg.Name.ReplaceIgnoreCase("|Foreground", string.Empty).Trim();
            // bg
            Image backgroundImg = CreateUIImage(bg);
            backgroundImg.transform.SetParent(gameObject.transform, false);
            AdjustUIRectByLayer(backgroundImg.gameObject, bg);

            Image foregroundImg = CreateUIImage(fg);
            foregroundImg.transform.SetParent(gameObject.transform, false);
            AdjustUIRectByLayer(foregroundImg.gameObject, fg);
            
            foregroundImg.transform.SetAsLastSibling();
            foregroundImg.type = Image.Type.Filled;
            foregroundImg.fillMethod = Image.FillMethod.Horizontal;

            return gameObject;
        }
        #endregion
    }
}