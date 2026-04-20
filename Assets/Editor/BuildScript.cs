using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public abstract class BuildScript
    {
        // Where your HTML wrappers expect the build files
        static string hostOutputDir   = "C:\\Users\\basni\\Desktop\\pensionplan-service\\Build\\host";
        static string clientOutputDir = "C:\\Users\\basni\\Desktop\\pensionplan-service\\Build\\client";

        // The product name Unity uses to name the output files
        static string hostProductName   = "webgl_build_host";
        static string clientProductName = "webgl_build";

        // Only these extensions are copied — TemplateData and index.html are skipped
        static string[] allowedExtensions = { ".js", ".wasm", ".data" };

        [MenuItem("Build/Build All (Host + Client)")]
        public static void BuildAll()
        {
            BuildTarget(
                scenes: new[] {
                    "Assets/Version1/Host/scene/HostScene.unity"
                },
                buildDir:    "Builds/Host",
                productName: hostProductName,
                outputDir:   hostOutputDir
            );

            BuildTarget(
                scenes: new[] {
                    "Assets/Version1/Phases/Login/scenes/LoginScene.unity",
                    "Assets/Version1/Phases/DonatePoints/scenes/DonatePointsScene.unity",
                    "Assets/Version1/Phases/Loading/scenes/Loading.unity",
                    "Assets/Version1/Phases/Trading/Scenes/MarketScene.unity",
                    "Assets/Version1/Phases/MoneyCorrection/scenes/MoneyCorrectionScene.unity",
                    "Assets/Version1/Phases/MoneyToPoint/scenes/MoneyToPointScene.unity",
                    "Assets/Version1/Phases/TakeALoan/scenes/TakeALoanScene.unity",
                    "Assets/Version1/Phases/End/scenes/EndScene.unity",
                    "Assets/Version1/Phases/PayDept/scenes/PayDebtScene.unity",
                    "Assets/Version1/Phases/Explanation/Bank/scenes/BankExplanationScene.unity",
                    "Assets/Version1/Phases/BankOverview/Scenes/BankOverviewScene.unity"
                },
                buildDir:    "Builds/Client",
                productName: clientProductName,
                outputDir:   clientOutputDir
            );

            Debug.Log("✅ Host + Client built and copied.");
        }

        static void BuildTarget(string[] scenes, string buildDir, string productName, string outputDir)
        {
            string previousName = PlayerSettings.productName;
            PlayerSettings.productName = productName;

            Debug.Log($"🔨 Building '{productName}' with scenes:");
            foreach (var s in scenes) Debug.Log($"   - {s}");

            var options = new BuildPlayerOptions
            {
                scenes           = scenes,
                locationPathName = buildDir,
                target           = UnityEditor.BuildTarget.WebGL,
                options          = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"📋 Build result: {report.summary.result} | Errors: {report.summary.totalErrors}");

            PlayerSettings.productName = previousName;

            string srcDir = Path.Combine(buildDir, "Build");
            Debug.Log($"📂 Copying from: {srcDir}");
            Debug.Log($"📂 Copying to:   {outputDir}");
            Debug.Log($"📂 Source files found:");
            foreach (var f in Directory.GetFiles(srcDir))
                Debug.Log($"   {Path.GetFileName(f)}");

            CopyBuildFiles(srcDir, outputDir);
        }

        static void CopyBuildFiles(string srcDir, string dstDir)
        {
            if (!Directory.Exists(dstDir))
                Directory.CreateDirectory(dstDir);

            foreach (var file in Directory.GetFiles(srcDir))
            {
                bool allowed = false;
                foreach (var allowedExt in allowedExtensions)
                {
                    if (file.EndsWith(allowedExt))
                    {
                        allowed = true;
                        break;
                    }
                }

                if (!allowed) continue;

                string dest = Path.Combine(dstDir, Path.GetFileName(file));
                File.Copy(file, dest, overwrite: true);
                Debug.Log($"  Copied: {Path.GetFileName(file)}");
            }
        }
    }
}
