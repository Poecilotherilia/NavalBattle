using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using System.IO;
using protocol;

#if UNITY_EDITOR
namespace editor
{
    public class CreateProtoEditor
    {
        [MenuItem("CustomTool/My Test")]
        private static void Test()
        {
            LaunchBat(Application.dataPath + "/../protoToCs/protoToCs.bat",
                Application.dataPath + "/../protoToCs");
            CopyFolder(Application.dataPath + "/../protoToCs/generate/",
                Application.dataPath + "/Code/Net/Proto/");
        }
        [MenuItem("CustomTool/My Test1")]
        private static void Test1()
        {
            WriteCreateBufClass();
        }
        static void LaunchBat(string batName, string WorkongDir)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = batName;
            startInfo.WorkingDirectory = WorkongDir;
            Process.Start(startInfo);

        }

        static void WriteCreateBufClass()
        {
            using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Code/Net/CreateProtoBuf.cs", false))
            {
                sw.WriteLine("//动态修改，不要手动修改\n");
                sw.WriteLine("using protocol;");
                sw.WriteLine("public class CreateProtoBuf");
                sw.WriteLine("{");
                sw.WriteLine("  public static ProtoBuf.IExtensible GetProtoData(ProtoDefine protoId, byte[] msgData)");
                sw.WriteLine("  {");
                sw.WriteLine("      switch (protoId)");
                sw.WriteLine("      {");

                foreach (int value in Enum.GetValues(typeof(ProtoDefine)))
                {
                    string strName = Enum.GetName(typeof(ProtoDefine), value);//获取名称
                    sw.WriteLine(string.Format("            case ProtoDefine.{0}:", strName));
                    sw.WriteLine(string.Format("                return NetUtilcs.Deserialize<{0}>(msgData);", strName));
                }

                sw.WriteLine("          default:");
                sw.WriteLine("              return null;");
                sw.WriteLine("      }");
                sw.WriteLine("  }");
                sw.WriteLine("}");
            }
        }

        public static void CopyFolder(string srcFolderPath, string destFolderPath)
        {
            // 创建所有的对应目录
            foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(srcFolderPath, destFolderPath));
            }

            // 复制原文件夹下所有内容到目标文件夹，直接覆盖
            foreach (string newPath in Directory.GetFiles(srcFolderPath, "*.*", SearchOption.AllDirectories))
            {

                File.Copy(newPath, newPath.Replace(srcFolderPath, destFolderPath), true);
            }

            AssetDatabase.Refresh();

            UnityEngine.Debug.LogWarning("FileStaticAPI::CopyFolder is innored under wep player platfrom");
        }
    }
}
#endif