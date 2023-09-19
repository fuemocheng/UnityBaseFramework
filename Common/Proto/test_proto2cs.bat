@echo off

REM 删除输出目录重新创建；
if exist out (
	rd /s /Q out
)

md out

REM 在 protogen inputfile.proto --csharp_out="out" 中，
REM 如果 inputfile.proto 输入带路径，则输出时会把路径也输出;
REM 如果 --csharp_out 输出路径为空，则会在原proto目录生成;
REM 所以按照以下方式生成结束后，再拷贝到输出目录;

for %%i in (proto\\*.proto) do (
	protogen\\protogen %%i --csharp_out=""
)


REM 把proto目录下生成的文件移到out下；
for %%i in (proto\\*.cs) do (
	if exist out ( 
		move %%i out > nul
		echo move %%i to out ...
	)
)

echo GEN PROTO CSHARP FILE SUCCEED

pause