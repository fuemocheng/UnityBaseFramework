# FileSystem 模块

## 1.FileSystemAccess.cs 文件系统访问方式
```
public enum FileSystemAccess : byte
{
    Unspecified = 0,   //未指定
    Read = 1,          //只可读
    Write = 2,         //只可写
    ReadWrite = 3,     //可读写
}
```

## 2.FileInfo.cs 
FileInfo   [StructLayout(LayoutKind.Auto)]  
文件信息，具体文件信息
```
    string Name;        //名称
    long Offset;        //偏移
    int Length;         //长度（解密后）
```

## 3.HeaderData.cs
FileSystem.HeaderData  [StructLayout(LayoutKind.Sequential)]  
头数据，描述整个文件系统信息  
```
    byte Version;            //FileSystemVersion  
    int MaxFileCount;        //最大文件数  
    int MaxBlockCount;       //最大数据块数  
    int BlockCount;          //使用的数据块数  

    //MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)  
    byte[] m_Header;         // {(byte)'G'，(byte)'F'，(byte)'F'}  

    //MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)  
    byte[] m_EncryptBytes;   //加密信息，构造时用随机数填充 
```

## 4.StringData.cs
FileSystem.StringData  [StructLayout(LayoutKind.Sequential)]  
字符串数据，单个文件的String数据，目前存储的是：文件名（加密）
```
    //缓存字节数据，0.25KB 空间  
    static readonly byte[] s_CachedBytes = new byte[256]  

    byte m_Length;      //字节长度  

    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]  
    byte[] m_Bytes;   

    //将 m_Bytes 拷贝到 s_CachedBytes；  
    //使用 encryptBytes 对 s_CachedBytes 做异或运算，期间无GC；  
    //将 s_CachedBytes 使用UTF-8 编码转换成字符串，并返回；  
    public string GetString(byte[] encryptBytes);  
  
    //将UTF-8 编码的字符串转换成字节数组；  
    //使用 encryptBytes 对 s_CachedBytes 做异或运算，期间无GC  
    //将 s_CachedBytes 拷贝到 m_Bytes；  
    //构造并返回返回一个新的 new StringData((byte)length, m_Bytes);  
    public StringData SetString(string value, byte[] encryptBytes);  

    //返回一个新的 new StringData(0, m_Bytes);  
    public StringData Clear();  
```

## 5.BlockData.cs
FileSystem.BlockData  [StructLayout(LayoutKind.Sequential)]  
块数据，单个文件的信息  
```
    //空数据块  
    public static readonly BlockData Empty = new BlockData(0, 0);

    int StringIndex;     //StringData索引（文件名）
    int ClusterIndex;    //簇索引（用于计算偏移）
    int Length;          //文件长度
```

## 6.FileSystem.cs
单个文件系统，一般包含多个文件
```
const int ClusterSize = 1024 * 4;       //4096 byte = 4k
const int CachedBytesLength = 0x1000;   //4096 byte = 4k

static readonly string[] EmptyStringArray = new string[] { };
static readonly byte[] s_CachedBytes = new byte[CachedBytesLength];

static readonly int HeaderDataSize = Marshal.SizeOf(typeof(HeaderData));  //20
static readonly int BlockDataSize = Marshal.SizeOf(typeof(BlockData));    //12
static readonly int StringDataSize = Marshal.SizeOf(typeof(StringData));  //256

readonly string m_FullPath;
readonly FileSystemAccess m_Access;
readonly FileSystemStream m_Stream;

//Key-Value : 文件名 - BlockIndex
readonly Dictionary<string, int> m_FileDatas;

readonly List<BlockData> m_BlockDatas;

//Key-Value : Block.Length - ListRange<BlockIndex>
readonly GameFrameworkMultiDictionary<int, int> m_FreeBlockIndexes; 

readonly SortedDictionary<int, StringData> m_StringDatas;
readonly Queue<int> m_FreeStringIndexes;
readonly Queue<StringData> m_FreeStringDatas;

HeaderData m_HeaderData;
int m_BlockDataOffset;
int m_StringDataOffset;
int m_FileDataOffset;
```

### 6.1 存储方式，存储顺序
1. 假设最大文件数量 MaxFileCount = 2，最大块数量 MaxBlockCount = 2；

2. 数据先写入文件流 FileStream，后通过文件流保存到物理磁盘；

3. 先保存 HeaderData，  
   HeaderDataSize = 20;

4. 再保存 BlockData，  
   m_BlockDataOffset 数据偏移开始为20，  
   BlookDataSize = BlockDataSize * HeaderData.MaxBlockCount = 12 * 2 = 24;

5. 再保存 StringData，  
   m_StringDataOffset = m_BlockDataOffset + BlockDataSize * HeaderData.MaxBlockCount = 44;  
   StringDataSize = StringDataSize * HeaderData.MaxFileCount = 256 * 2 = 512;
   
6. 再保存 FileData，  
   m_FileDataOffset = (int)GetUpBoundClusterOffset(m_StringDataOffset + StringDataSize * HeaderData.MaxFileCount) = 4096;

7. 存储示例：  
   ![FileSystem_StorageStructure](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/Pics/FileSystem_StorageStructure.png?raw=true)

### 6.2 创建文件系统 Create(...)
1. FileSystem fileSystem = new FileSystem(fullPath, access, stream);  
   根据路径，文件系统访问方式，文件系统流初始化文件系统新实例；  
2. fileSystem.m_HeaderData = new HeaderData(maxFileCount, maxBlockCount);
   根据文件系统最大文件数，文件系统最大块数据数量，创建头数据；
3. CalcOffsets(fileSystem);  
   根据 6.1 中的存储方式，计算 BlockData 数据的偏移，StringData(加密文件名)数据的偏移，真实文件数据的存储偏移；
4. 创建时将HeaderData写入文件流，设置当前流的Length为m_FileDataOffset，为后续写入文件数据做准备；

### 6.3 加载文件系统 Load(...)
1. FileSystem fileSystem = new FileSystem(fullPath, access, stream);  
   根据路径，文件系统访问方式，文件系统流初始化文件系统新实例；  
2. 从传入的文件流 FileSystemStream stream 中读取 HeaderDataSize 大小的数据，并转换成 HeaderData 数据；
3. CalcOffsets(fileSystem);  
   根据 6.1 中的存储方式，计算 BlockData 数据的偏移，StringData(加密文件名)数据的偏移，真实文件数据的存储偏移；
4. 根据 HeaderData 中的数据块数 BlockCount，从流中读取 BlockData，并转换成BlockData数据；  
   未被使用的BlockData的数据，加入m_FreeBlockIndexes中；
5. 根据4中的数据，从流中读取 StringData(加密文件名数据)，并转换成 StringData 数据；  
   计算未被使用的StringData，并加入m_FreeStringIndexes；

### 6.4 写入指定文件 Write(...)
public bool WriteFile(string name, byte[] buffer, int startIndex, int length){}  
1. 做一些越界判断；
2. 申请Block索引；  
   int blockIndex = AllocBlock(length);  
     
   length = (int)GetUpBoundClusterOffset(length);  
   计算大于等于length的最小的4096(4k)的倍数，即为要申请的Block的大小；  
   
   先在 m_FreeBlockIndexes 查找是否有空闲的空间大于length的且空间最小的Block； 

   如果查找到，则使用这个BlockIndex，并且将剩余的空间（deltaLength = lengthFound - length），构造成空余空间，添加到 m_FreeBlockIndexes 中，即可复用；  
   并重新记录Block的ClusterIndex和长度；

   如果没查找到，则只用空的BlockIndex，int blockIndex = GetEmptyBlockIndex()，并将Stream流的长度设置为（当前Stream流的长度（fileLength = m_Stream.Length）+ length ）,代表写入此文件数据后，Stream流的总长度；  
   计算Block的簇索引（GetUpBoundClusterCount(fileLength)，大于等于 fileLength 的最小的4096(4k)的倍数），重新写入Block的ClusterIndex和长度；

   不管以前是否有这个文件，都重新申请新BlockIndex；旧Block如何回收在后面；

3. 根据申请的BlockData的簇索引，计算写入文件时Stream流的开始位置，为 ClusterIndex * 4096；  
   m_Stream.Position = GetClusterOffset(ClusterIndex);
   将二进制数据写入Stream流；
   m_Stream.Write(buffer, startIndex, length)；

4. ProcessWriteFile(name, hasFile, oldBlockIndex, blockIndex, length) 写入文件名，记录进 m_FileDatas；
   判断是否以前有这个文件文件；  

   如果以前有，则将就Block的StringIndex赋值给新的Block，重写Block信息，并且将OldBlock Free掉，然后尝试合并Free的Block，具体见TryCombineFreeBlocks()；

   如果以前没有，则申请新的StringIndex，int stringIndex = AllocString(name);  
   先从m_FreeStringIndexes中申请stringIndex，从m_FreeStringDatas申请新的stringData，没有则创建新的；  
   然后将数据，加密写入stringData；  
   最后将stringIndex和stringData合并写入Stream流中；

   申请完StringIndex，则重新写入Block数据中；  

   最后将数据记录进m_FileDatas；

### 6.5 int ReadFile(string name, byte[] buffer, int startIndex, int length)
1. FileInfo GetFileInfo(string name)
   从m_FileDatas中，根据文件名获取BlockIndex；
   根据BlockIndex获取文件文件的簇索引ClusterIndex（并计算文件在Stream流中的初始偏移）和文件长度；

2. 根据FileInfo中记录的文件偏移和长度，从Stream流中读取二进制数据；  
   m_Stream.Position = fileInfo.Offset;  
   m_Stream.Read(buffer, startIndex, length);

3. int ReadFileSegment(string name, int offset, byte[] buffer, int startIndex, int length) 读取指定文件的指定片段
   设置Stream流Position的时候加上偏移即可；  
   m_Stream.Position = fileInfo.Offset + offset;  
   m_Stream.Read(buffer, startIndex, length);  

### 6.6 bool RenameFile(string oldName, string newName) 重命名指定文件
1. 从m_FileDatas中，根据oldName，获取 blockIndex;  
   根据 blockIndex 获取到BlockData；  
   根据BlockData中的StringIndex，获取stringData；  
   重写 stringData;  
   m_FileData中添加newName，删除oldName;  

### 6.7 bool DeleteFile(string name)
1. m_FileDatas.Remove(name);  
   先将对应的stringData清空，将stringIndex添加到m_FreeStringIndexes，将stringData添加到m_FreeStringDatas，m_StringDatas删除stringData，然后重写stringData数据；  

2. 将对应的blockData Free，然后尝试合并空block（TryCombineFreeBlocks(blockIndex)）；  
   
   合并成功则完成删除；
   
   如果没有可以合并的，则将此block信息加入m_FreeBlockIndexes；  
   m_FreeBlockIndexes.Add(blockData.Length, blockIndex);

### 6.8 TryCombineFreeBlocks 合并FreeBlocks
   主要思想是遍历 m_FreeBlockIndexes，找出此block的前一个或者后一个也是free的block，将它们合并；  
   细节不再赘述；

## 7.FileSystemManager.cs 文件系统管理器
1. SetFileSystemHelper(IFileSystemHelper fileSystemHelper) 设置文件系统辅助器  
   m_FileSystemHelper = fileSystemHelper;  

2. IFileSystem CreateFileSystem(...)  
   
   通过 m_FileSystemHelper 创建 FileSystemStream;  
   FileSystemStream fileSystemStream = m_FileSystemHelper.CreateFileSystemStream(fullPath, access, true);  

   FileSystem fileSystem = FileSystem.Create(... , fileSystemStream, ...)

3. IFileSystem LoadFileSystem(...)
   
   通过 m_FileSystemHelper 创建 FileSystemStream;  
   FileSystemStream fileSystemStream = m_FileSystemHelper.CreateFileSystemStream(fullPath, access, true);  

   FileSystem fileSystem = FileSystem.Create(... , fileSystemStream, ...)

## 8.FileSystemStream.cs
1. CommonFileSystemStream : FileSystemStream, IDisposable  
   封装System.FileStream

2. AndroidFileSystemStream : FileSystemStream  
   封装AndroidJavaObject


