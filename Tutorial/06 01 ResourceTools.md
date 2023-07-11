# ResourceTools 模块
## 0x01 Asset 封装一个 UnityEngine.GameObject

1. UnityBaseFramework.Editor.ResourceTools.Asset : IComparable<Asset>
2. Member variable
    ```cs
    string Guid;        //Guid
    string Name;        //UnityEditor.AssetDatabase.GUIDToAssetPath(Guid); 以路径为Name;
    Resource Resource;  //所在的Resource
    ```
3. Member function
    ```cs
    private Asset(string guid, Resource resource);                  //构造
    public static Asset Create(string guid);                        //创建
    public static Asset Create(string guid, Resource resource);     //创建
    ```
4. Asset : IComparable<Asset> 实现接口IComparable；
    ```cs
    //在使用SortedDictionary、SortedList...时可直接排序；
    public int CompareTo(Asset asset)
    {
        return string.Compare(Guid, asset.Guid, StringComparison.Ordinal);
    }
    ```

## 0x02 Resource 相当于封装一个 UnityEngine.AssetBundle

1. UnityBaseFramework.Editor.ResourceTools.Resource
2. Member variable
    ```cs
    private readonly List<Asset> m_Assets;          //存储的Asset的集合
    private readonly List<string> m_ResourceGroups; //包含在哪些ResourceGroup里

    public string Name;                 //Name
    public string Variant;              //变体
    public string FullName;             //Name.Variant
    public AssetType AssetType;         //Unknow(未知),Asset(资源),Scene(场景)
    public bool IsLoadFromBinary;       //是否使用二进制方式加载
    public string FileSystem;           //文件系统名
    public LoadType LoadType;           //资源加载方式，一般是LoadFromFile(使用文件方式加载)
    public bool Packed;                 //标记为随APP一起发布的资源
    ```
3. Member function
    ```cs
    public static Resource Create(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups);     //创建
    public Asset[] GetAssets();                             //ResourceTools.Asset
    public Asset GetFirstAsset();                           //获取第一个
    public void Rename(string name, string variant);        //重命名
    public void AssignAsset(Asset asset, bool isScene);     //重新分配资源，如果Asset之前在某个Resource里，从之前的Resource中删除，添加到当前的Resource中；
    public void UnassignAsset(Asset asset);                 //将Asset资源从当前Resource中删除；
    public string[] GetResourceGroups();                    //
    public bool HasResourceGroup(string resourceGroup);     //
    public void AddResourceGroup(string resourceGroup);     //
    public bool RemoveResourceGroup(string resourceGroup);  //
    public void Clear();                                    //将所有Asset资源的Resource置空，然后清空此Resource中数据；
    private int AssetComparer(Asset a, Asset b);            //分配Asset时，进行排序；
    ```

## 0x03 DependencyData 依赖数据
1. UnityBaseFramework.Editor.ResourceTools.DependencyData
2. Member variable
    ```cs
    private List<Resource> m_DependencyResources;
    private List<Asset> m_DependencyAssets;
    //被其他资源依赖的，但没有被记录在 ResourceCollection.xml 中的分散的资源；
    private List<string> m_ScatteredDependencyAssetNames;
    ```
3. Member function
    ```cs
    public void AddDependencyAsset(Asset asset);
    public void AddScatteredDependencyAsset(string dependencyAssetName);
    public Resource[] GetDependencyResources();
    public Asset[] GetDependencyAssets();
    public string[] GetScatteredDependencyAssetNames();
    public void RefreshData();
    ```


## 0x04 ResourceCollection 资源集合
1. UnityBaseFramework.Editor.ResourceTools.ResourceCollection
2. 存储结构
    ```cs
    //所有ResourceTools.Resource的集合
    private readonly SortedDictionary<string, Resource> m_Resources;    
    //所有ResourceTools.Asset的集合
    private readonly SortedDictionary<string, Asset> m_Assets;
    ```
3. 加载Load
    1. 加载 ResourceCollection.xml 配置文件；
    2. 将 Resources 节点的数据读取到 m_Resources 字典中；
    3. 将 Assets 节点数据读取到 m_Assets 字典中；
    
4. 保存Save
    1. 设置 xml 数据格式，创建出要保存的 xml 节点；
    2. 遍历 m_Resources ，将数据填充到 xmlElement 中，然后保存到 xmlResources 中；
    3. 遍历 m_Assets ，将数据填充到 xmlElement 中，然后保存到 xmlAssets 中；
    4. 将文件保存到 加载 ResourceCollection.xml 中；
    5. AssetDatabase.Refresh();

5. ResourceName名称限制
    ```cs
    //名称需符合如下正则
    ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
    //变体需符合如下正则
    ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");
    ```

## 0x05 ResourceAnalyzerController 资源分析控制器
1. UnityBaseFramework.Editor.ResourceTools.ResourceAnalyzerController
2. 存储结构
    ```cs
    //记录每个Asset的依赖，key为assetName(即 path)
    private readonly Dictionary<string, DependencyData> m_DependencyDatas; 
    //记录被其他资源依赖的，不在 ResourceCollection.xml 中记录的分散的资源；
    private readonly Dictionary<string, List<Asset>> m_ScatteredAssets;
    //记录循环依赖的数据；
    private readonly List<string[]> m_CircularDependencyDatas;
    //记录每一个单独的依赖项，比如某个prafab依赖了两个图片，则记录为两条；同时，依赖的依赖也会被记录下来；
    //Stamp结构为仅记录hostAssetName, dependencyAssetName的结构体；
    private readonly HashSet<Stamp> m_AnalyzedStamps;
    ```
3. ResourceAnalyzerController.Analyze()
    1. 清空缓存字典列表等数据；
    2. 获取 t:Script 所有资源，在分析资源时用于剔除脚本资源；
    3. 获取所有收集的Asset资源 m_ResourceCollection.GetAssets();
    4. 分析每个资源的依赖情况；AnalyzeAsset(string assetName, Asset hostAsset,...)
        1. 获取 assetName 的所有依赖，并遍历；
        2. 忽略对脚本的依赖，忽略对场景的依赖；
        3. 构造 Stamp stamp = new Stamp(hostAsset.Name, dependencyAssetName);添加到 m_AnalyzedStamps 中；
        4. 判断依赖是否记录在 ResourceCollection.xml 中；
            1. 如果记录在配置文件中，则添加到 m_DependencyDatas 中；
            2. 如果不在配置文件中，则添加到 m_ScatteredAssets 中，并分析此依赖资源；
        5. 依赖资源内部排序排序，dependencyData.RefreshData();
    5. 循环依赖资源的检查，CircularDependencyChecker.Check()
        1. Stamp[] m_Stamps = m_AnalyzedStamps.ToArray()为传入参数；
        2. 将所有 stamp.HostAssetName 记录在 HashSet集合 hosts 中；
        3. 遍历 hosts，对每个值 Check(host, route, visited)；
            1. visited.Add(host);
            2. route.AddLast(host);
            3. 在Check中，遍历 m_Stamps 每个值 stamp ；
                1. 忽略 host != stamp.HostAssetName 的情况，即需要的的是 host 的所有依赖项；
                2. 如果 host 的依赖项（即 stamp.DependencyAssetName）包含在了，visited（即已访问的 host） 中，代表了 host 的依赖中存在 host，即循环依赖了，则将其添加到 route 中，并返回true;
                3. 如果 2 中不成立，则继续迭代：Check(stamp.DependencyAssetName, route, visited)；直至返回true，或者进行下一步；
            4. route.RemoveLast();
            5. visited.Remove(host);
            6. return false；标着这个host的所有依赖，依赖的依赖...都没有循环依赖；
        4. 如果 3 中有循环依赖，则循环依赖的情况记录在了 route 中；
        5. 这里的值得学习的地方就是 3 中的迭代Check()了；
    6. 最终分析完成；
4. 主要流程集中在 3 中，分析完成数据存储在了 2 中，以供后续的使用，UI的展示；


## 0x06 ResourceBuilderController 资源构建控制器
1. UnityBaseFramework.Editor.ResourceTools.ResourceBuilderController
   
2. AssetData 
    class ResourceBuilderController.AssetData  
    增加了文件对应的字节流长度和HashCode
    ```cs
    private readonly string m_Guid;
    private readonly string m_Name;
    private readonly int m_Length;
    private readonly int m_HashCode;
    private readonly string[] m_DependencyAssetNames;

    public string Guid;
    public string Name;
    public int Length;
    public int HashCode;
    public string[] GetDependencyAssetNames();

    public AssetData(string guid, string name, int length, int hashCode, string[] dependencyAssetNames);
    ```
3.  ResourceCode  
    class ResourceBuilderController.ResourceCode
    ```cs
    private readonly Platform m_Platform;
    private readonly int m_Length;
    private readonly int m_HashCode;
    private readonly int m_CompressedLength;
    private readonly int m_CompressedHashCode;

    public Platform Platform;
    public int Length;
    public int HashCode;
    public int CompressedLength;
    public int CompressedHashCode;

    public ResourceCode(Platform platform, int length, int hashCode, int compressedLength, int compressedHashCode);
    ```
4.  ResourceData 对应一个 UnityEditor.AssetBundleBuild 的数据  
    class ResourceBuilderController.ResourceData
    ```cs
    private readonly string m_Name;
    private readonly string m_Variant;
    private readonly string m_FileSystem;
    private readonly LoadType m_LoadType;
    private readonly bool m_Packed;
    private readonly string[] m_ResourceGroups;
    private readonly List<AssetData> m_AssetDatas;
    private readonly List<ResourceCode> m_Codes;

    public string Name;
    public string Variant;
    public string FileSystem;
    public bool IsLoadFromBinary;
    public LoadType LoadType;
    public bool Packed;
    public int AssetCount;
    public string[] GetResourceGroups();
    public string[] GetAssetGuids();
    public string[] GetAssetNames();
    public AssetData[] GetAssetDatas();
    public AssetData GetAssetData(string assetName);
    public void AddAssetData(string guid, string name, int length, int hashCode, string[] dependencyAssetNames);
    public ResourceCode GetCode(Platform platform);
    public ResourceCode[] GetCodes();
    public void AddCode(Platform platform, int length, int hashCode, int compressedLength, int compressedHashCode);

    public ResourceData(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups);
    ```
5. BuildResources 顺序
    ResourceBuilder.BuildResources() -> ResourceBuilderController.BuildResources();
    ```md
    1. 判断各种路径是否存在；
    
    2. 获取 BuildAssetBundleOptions, 资源包构建选项；
    
    3. m_ResourceCollection.Load(); 重新加载资源集合；
    
    4. 判断平台是否设置；
    
    5. m_ResourceAnalyzerController.Analyze(); 
        分析资源依赖，是否有循环依赖的资源，分散的资源等；
    
    6. 准备构建数据 PrepareBuildData(...) 
        Resource[] resources = m_ResourceCollection.GetResources();
        遍历 resources 数据，构造 ResourceData 添加到 m_ResourceDatas 中；
        
        Asset[] assets = m_ResourceCollection.GetAssets();
        遍历 assets 数据，读取每个文件的字节流数据，计算HashCode，获取对应的依赖数据，构造 AssetData 添加到对应的 m_ResourceDatas[...] 数据中；

        foreach (ResourceData resourceData in m_ResourceDatas.Values)
        遍历 m_ResourceDatas，如果 resourceData 的加载方式是二进制加载，则将 resourceData 添加到 binaryResourceDatas 中；
        如果 resourceData 的加载方式是从文件加载，则 构造 UnityEditor.AssetBundleBuild 数据，并且将其加入 assetBundleBuildDatas 中，最后将 resourceData 加入 assetBundleResourceDatas 中；
    
    7. 构建资源 BuildResources(...)  
        判断平台是否正确；
        判断要输出的各个目录是否存在，不存在则创建；
        遍历要输出目录的文件，如果文件名不存在于 assetBundleResourceDatas ，则说明这个文件需要删除，则删除此文件，并删除对应 .manifest 文件，最后删除空目录；

        最后调用 UnityEditor.BuildPipeline.BuildAssetBundles(...) 来进行构建资源
        AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, assetBundleBuildDatas, buildAssetBundleOptions, GetBuildTarget(platform));

    8. 创建文件系统 CreateFileSystems(...)
        详见 0x05 FileSystem 的创建流程

    9. Process AssetBundles， 处理 AssetBundles；
        遍历 ResourceData[] assetBundleResourceDatas，处理每个AssetBundle，ProcessAssetBundle(...);

        读取 AssetBundles 字节数组，计算hashcode等，如果需要加密，则根据加载方式对字节数组进行加密；

        最后输出 ProcessOutput(...);
    
    10. Process Binaries， 处理 二进制数据；
        遍历 ResourceData[] binaryResourceDatas，处理每个二进制数据文件，ProcessBinary(...);

        读取 每个文件的 字节数组，计算hashcode等，如果需要加密，则根据加载方式对字节数组进行加密；

        最后输出 ProcessOutput(...);

    11. ProcessOutput(...)，把对应的资bytes经过加密或者不加密写入到文件夹或者文件系统中；
        如果是 OutputPackageSelected 单机模式，
            如果此文件不在文件系统中，则直接写入对应的 Package 文件夹中；
            否则将此文件写入对应的文件系统中；
        
        如果是 OutputPackedSelected 首包模式，并且此文件 resourceData.Packed 要打进首包里，
            如果此文件不在文件系统中，则直接写入对应的 Package 文件夹中；
            否则将此文件写入对应的文件系统中；
        
        如果是 OutputFullSelected 输出所有文件，
            如果此文件不在文件系统中，则直接写入对应的 Full 文件夹中；
            否则将此文件写入对应的文件系统中；

        最后 resourceData.AddCode(...)，此 resourceData 添加 ResourceCode 信息，用于记录文件的字节数字长度，HashCode，压缩后的长度，压缩后的HashCode；

    12. ProcessPackageVersionList(...) 单机模式资源版本列表
        1. 根据 m_ResourceCollection 构造下面数据；
            1. PackageVersionList.Asset[]
            2. PackageVersionList.Resource[]
            3. PackageVersionList.FileSystem[]
            4. PackageVersionList.ResourceGroup[]

        2. 序列化 PackageVersionList 信息，即序列化单机模式版本资源列表，这里提供了三种序列化工具：
            1. PackageVersionListSerializeCallback_V0
                1. 用随机数填充指定字节数组 s_CachedHashBytes ；
                2. 将下面的数据依次写入二进制写入器 binaryWriter ；
                    s_CachedHashBytes，随机数数组；
                    ApplicableGameVersion，游戏版本号（用随机数组加密）；
                    InternalResourceVersion，内部资源版本号；
                    assets.Length，Assets数组的长度；
                    resources.Length，Resource数组的长度；

                    遍历 resources ，依次写入：
                        resource.Name，（用随机数组加密）；
                        resource.Variant，（用随机数组加密）；
                        resource.LoadType；
                        resource.Length；
                        resource.HashCode；
                        assetIndexes.Length，该Resource包含的资源索引集合的长度；
                        遍历 assetIndexes，依次写入：
                            asset.Name，（用 hashBytes 加密）；
                            dependencyAssetIndexes.Length，依赖资源索引集合的长度；
                            遍历 dependencyAssetIndexes，依次写入：
                                assets[dependencyAssetIndex].Name，依赖名（加密）；
                    
                    resourceGroups.Length，资源组长度；

                    遍历 resourceGroups，依次写入：
                        resourceGroup.Name，（用随机数组加密）；
                        resourceIndexes.Length，资源索引集合的长度；
                        遍历 resourceIndexes，依次写入；
                            resourceIndex，资源索引；      

            2. PackageVersionListSerializeCallback_V1
                写入流程与 PackageVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    先写入所有 Asset，
                    再写入所有 Resource，
                    再写入所有 ResourceGroup；

                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

            3. PackageVersionListSerializeCallback_V2
                写入流程与 PackageVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    先写入所有 Asset，
                    再写入所有 Resource，
                    再写入所有 FileSystem，
                    再写入所有 ResourceGroup；

                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

        3. 将二进制流写入文件；

    13. ProcessUpdatableVersionList(...) 远程更新资源版本列表
        1. 根据 m_ResourceCollection 构造下面数据；
            1. UpdatableVersionList.Asset[]
            2. UpdatableVersionList.Resource[]
            3. UpdatableVersionList.FileSystem[]
            4. UpdatableVersionList.ResourceGroup[]

        2. 序列化 UpdatableVersionList 信息，即序列化可更新模式版本资源列表，这里提供了三种序列化工具：
            1. UpdatableVersionListSerializeCallback_V0
                1. 用随机数填充指定字节数组 s_CachedHashBytes ；
                2. 将下面的数据依次写入二进制写入器 binaryWriter ；
                    s_CachedHashBytes，随机数数组；
                    ApplicableGameVersion，游戏版本号（用随机数组加密）；
                    InternalResourceVersion，内部资源版本号；
                    assets.Length，Assets数组的长度；
                    resources.Length，Resource数组的长度；

                    遍历 resources ，依次写入：
                        resource.Name，（用随机数组加密）；
                        resource.Variant，（用随机数组加密）；
                        resource.LoadType；
                        resource.Length；
                        resource.HashCode；
                        resource.CompressedLength；
                        resource.CompressedHashCode；
                        assetIndexes.Length，该Resource包含的资源索引集合的长度；
                        遍历 assetIndexes，依次写入：
                            asset.Name，（用 hashBytes 加密）；
                            dependencyAssetIndexes.Length，依赖资源索引集合的长度；
                            遍历 dependencyAssetIndexes，依次写入：
                                assets[dependencyAssetIndex].Name，依赖名（加密）；
                    
                    resourceGroups.Length，资源组长度；

                    遍历 resourceGroups，依次写入：
                        resourceGroup.Name，（用随机数组加密）；
                        resourceIndexes.Length，资源索引集合的长度；
                        遍历 resourceIndexes，依次写入；
                            resourceIndex，资源索引；      

             2. UpdatableVersionListSerializeCallback_V1
                写入流程与 UpdatableVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    先写入所有 Asset，
                    再写入所有 Resource，
                    再写入所有 ResourceGroup；

                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

            3. UpdatableVersionListSerializeCallback_V2
                写入流程与 UpdatableVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    先写入所有 Asset，
                    再写入所有 Resource，
                    再写入所有 FileSystem，
                    再写入所有 ResourceGroup；

                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

        3. 将二进制流写入文件；

    14. ProcessReadOnlyVersionList(...) 可更新模式资源版本列表
        1. 根据 m_ResourceCollection 构造下面数据；
            1. LocalVersionList.Asset[]
            2. LocalVersionList.Resource[]
            3. LocalVersionList.FileSystem[]
            4. LocalVersionList.ResourceGroup[]

        2. 序列化 LocalVersionList 信息，即本地版本资源列表，这里提供了三种序列化工具：
            1. LocalVersionListSerializeCallback_V0
                1. 用随机数填充指定字节数组 s_CachedHashBytes ；
                2. 将下面的数据依次写入二进制写入器 binaryWriter ；
                    s_CachedHashBytes，随机数数组；
                    resources.Length，Resource数组的长度；

                    遍历 resources ，依次写入：
                        resource.Name，（用随机数组加密）；
                        resource.Variant，（用随机数组加密）；
                        resource.LoadType；
                        resource.Length；
                        resource.HashCode；
            
            2. LocalVersionListSerializeCallback_V1
                1. 用随机数填充指定字节数组 s_CachedHashBytes ；
                2. 将下面的数据依次写入二进制写入器 binaryWriter ；
                    s_CachedHashBytes，随机数数组；
                    resources.Length，Resource数组的长度；

                    遍历 resources ，依次写入：
                        resource.Name，（用随机数组加密）；
                        resource.Variant，（用随机数组加密）；
                        resource.Extension，扩展名（用随机数组加密）；
                        resource.LoadType；
                        resource.Length；
                        resource.HashCode；
                        
                写入流程与 LocalVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

            3. LocalVersionListSerializeCallback_V2
                1. 用随机数填充指定字节数组 s_CachedHashBytes ；
                2. 将下面的数据依次写入二进制写入器 binaryWriter ；
                    s_CachedHashBytes，随机数数组；
                    resources.Length，Resource数组的长度；

                    遍历 resources ，依次写入：
                        resource.Name，（用随机数组加密）；
                        resource.Variant，（用随机数组加密）；
                        resource.Extension，扩展名（用随机数组加密）；
                        resource.LoadType；
                        resource.Length；
                        resource.HashCode；
                    
                    fileSystems.Length，文件系统数组长度；

                    遍历 fileSystems ，依次写入：
                        fileSystem.Name，文件系统名（用随机数组加密）；
                        resourceIndexes.Length，资源索引数组长度；
                        遍历 resourceIndexes，依次写入：
                            resourceIndex，资源索引；
                        
                写入流程与 LocalVersionListSerializeCallback_V0 基本相同；
                不同之处：
                    写入整型用 binaryWriter.Write7BitEncodedInt32()，对整型进行了编码；

        3. 将二进制流写入文件；

    15. ProcessResourceComplete(...) 完成构建
    ```

6. ResourcePackBuilder 打资源包
    1. 以full文件夹中指定版本为基础，找到改变的资源。可以同时打多个版本的资源包，每个版本的资源包相互独立。
    2. BuildResourcePack函数实现打包两个版本之间的差异资源。
    3. 流程
        1. 读取两个版本的GameFrameworkVersion.*.data文件，每个版本内资源信息，保存到对应的UpdateableVersionList实例，对比实例中每个文件，计算需要打包的资源列表。
        2. 对每个源版本号，调用BuildResourcePack函数
            1. 读取GameFrameworkVersion，找到需要打包的资源列表，注意不只是资源变了，加载方式变了也要更新文件，所以要避免大bundle加载方式变化。
            2. 要打包的资源，用ResourcePackVersionList.Resource保存，序列化为一个或几个大的文件，大小上限可选。
            3. 打包后保存到ResourcePack对应的版本文件夹中，并上传到资源服务器。
    4. TODO

7. ResourceSyncTools
    1. TODO
    


