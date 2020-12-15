## 文档生成工具

本工具的目的是生成Interface和AppService层的接口文档，方便测试人员编写接口测试用例使用。

支持：
- 排除不需要的服务
- 排除指定的属性
- 指定输出单个或多个方法

## 使用帮助

1、正确配置

修改售楼系统编译后的文件路径

```c#
<add key="SourcePath" value="F:\售楼系统_\src\00_根目录\bin"/>
```

需要加载的程序集文件

```c#
<!--这里只加载售楼相关的dll-->
<add key="Dll" value="Mysoft.Slxt.*.dll"/>
```

修改输出的模块的dll

```c#
<add key="OutputModule" value="Mysoft.Slxt.PriceMng.Interfaces.dll"/>
```

修改输出的方法，为空，则全部都输出

```c#
<add key="OutputMethod" value="PriceChgAudit"/>
```

完整示例：

```c#
<appSettings>
    <clear/>
    <add key="TemplatePath" value=""/>
    <!--文档输出目录-->
    <add key="OutputPath" value="md"/>
    <!--程序集、XML文件所在路径-->
    <add key="SourcePath" value="F:\售楼系统_\src\00_根目录\bin"/>
    <!--解析的Dll文件，多个用英文逗号分隔-->
    <add key="Dll" value="Mysoft.Slxt.*.dll"/>
    <!---->
    <add key="OutputModule" value="Mysoft.Slxt.PriceMng.Interfaces.dll"/>
    <!--输出方法-->
    <add key="OutputMethod" value="PriceChgAudit"/>
    <!--忽略的属性-->
    <add key="IgnoreProperty" value="Attributes,EntityName,EntityState,CreatedGUID,CreatedName,CreatedTime,ModifiedGUID,ModifiedName,ModifiedTime"/>
    <!--忽略的方法-->
    <add key="IgnoreMethods" value="RetryProcess,AddProcess,ValidateImportOpportunity,ValidateImportChanceClue,ImportChanceClueImp,ValidateUpdateRoom"/>
    <!--忽略的服务-->
    <add key="IgnoreServices" value="IAsyncTaskPublicService,ParamSetttingAppService,ProjectOverviewAppService,IParamSetttingPublicService,RoomGenerateAppService,IAppProcessPublicService"/>
  </appSettings>
```