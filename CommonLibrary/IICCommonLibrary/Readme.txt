IICCommonLibrary中各个目录的说明

0.Library		目录中的Class不依赖于任何其他目录中的Class
1.Foundation	运行环境, 环境中的代码属于底层代码, 
2.Framework		大部分基础组件的框架代码, 依赖1.Foundation, 同级目录不会互相依赖
3.Extensions	依赖1.Foundation和2.Framework, 但不互相依赖, Framework层无法剥离的扩展部分会在这里IICCommonLibrary中各个目录的说明
