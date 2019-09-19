using System.Collections.Generic;
namespace MiniProj
{
    public enum ModuleId
    {
        ErrorModule = -1,
        LoginModule = 0,
        MainMenuModule = 1,
        SceneModule = 2,
    }
    public class ModuleName
    {
        
        public static Dictionary<string, ModuleId> NameToID = new Dictionary<string, ModuleId>()
        {
            {"LoginModule", ModuleId.LoginModule},
            {"MainMenuModule", ModuleId.MainMenuModule},
            {"SceneModule", ModuleId.SceneModule },
        };
    }
}

