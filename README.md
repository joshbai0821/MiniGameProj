# MiniGameProj
## Assets/Editor 
编辑器目录，目前功能导出ab包，同一个文件夹下，全部统一打成一个ab包，ab包名称为将/替换_的相对路径的字符串
## Prefabs
预制件总目录，相关资源放在此目录下，当前地图数据的Prefab放在Prefabs/Map
## Resources
地图数据文件，音乐文件
## Scene
场景文件，Scene.unity
## Scripts
脚本文件：
总控制器GameManager，包含资源管理器和模块管理
资源控制器ResourceManager，使用引用计数进行控制，编辑器下使用正常资源，移动平台使用ab包。
登陆模块为LoginModule，主界面为MainMenuModule，场景为SceneModule。其他根据业务扩展
