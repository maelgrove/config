using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net.Json.Stores;
using Config.Net.Stores;
using Config.Net.Stores.Impl.CommandLine;
using Config.Net.Yaml.Stores;
using Xunit;

namespace Config.Net.Tests
{
    public class MultiStoreTest : AbstractTestFixture
    {
       [Fact]
       public void Read_config_values_in_correct_order()
       {
         using (IConfigStore iniFileConfigStore = new IniFileConfigStore(GetSamplePath("ini"), true, true))
         using (IConfigStore commandLineConfigStore = new CommandLineConfigStore(new[] { "-key0:value1" }))
         {
            IMultiStoreSettings settings = new ConfigurationBuilder<IMultiStoreSettings>()
               .UseConfigStore(iniFileConfigStore)
               .UseConfigStore(commandLineConfigStore)
               .Build();

            Assert.Equal("value1", settings.Key0);
         }
       }

       [Fact]
       public void Write_config_values_back_to_origin()
       {
         using (IConfigStore iniFileConfigStore = new IniFileConfigStore(GetSamplePath("ini"), true, true))
         using (IConfigStore yamlFileConfigStore = new YamlFileConfigStore(GetSamplePath("yml")))
         {
            IMultiStoreSettings settings = new ConfigurationBuilder<IMultiStoreSettings>()
               .UseConfigStore(iniFileConfigStore)
               .UseConfigStore(yamlFileConfigStore)
               .Build();

            settings.Key0 = "value2";

            Assert.Null(yamlFileConfigStore.Read("key0"));
         }
      }

       protected string GetSamplePath(string ext)
       {
          string dir = BuildDir.FullName;
          string src = Path.Combine(dir, "TestData", "sample." + ext);
          string testFile = Path.Combine(dir, src);
          src = Path.GetFullPath(testFile);
          string dest = Path.Combine(TestDir.FullName, "sample." + ext);

          File.Copy(src, dest, true);

          return dest;
       }

   }

   public interface IMultiStoreSettings
   {
      string Key0 { get; set; }
   }
}
