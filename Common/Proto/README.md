# Protobuf-net
## Version
[Protobuf-net 3.2.8](https://github.com/protobuf-net)
Visual Studio Community 2022
保证下面三个项目没有报错，并且切换到成Release模式；
protobuf-net
protobuf-net.core
protogen
编译完成之后，
将protobuf-net/bin/Release/net462 或者其他版本的生成文件，全部拷贝到Unity中；
将protogen/bin/Release/net462 跟dll对应版本的文件，拷贝到工具目录下，使用protogen.exe 生成proto文件；

## 根据 .proto 生成 .cs 文件

protogen Game.proto --csharp_out=""

添加Option，所有Repeated字段生成List，并且带有Set访问器。
protogen +repeatedaslist=yes +listset=yes Game.proto --csharp_out=""

