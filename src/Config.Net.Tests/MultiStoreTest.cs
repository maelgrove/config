using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net.Stores;
using Config.Net.Stores.Impl.CommandLine;
using Xunit;

namespace Config.Net.Tests
{
    public class MultiStoreTest : AbstractTestFixture, IDisposable
    {
       private string _testFilePath;
       private IniFileConfigStore _iniFileConfigStore;
       private CommandLineConfigStore _commandLineConfigStore;

       public MultiStoreTest()
       {
          //get back clean file
          string src = Path.Combine(BuildDir.FullName, "TestData", "sample.ini");
          _testFilePath = Path.Combine(TestDir.FullName, "sample.ini");
          File.Copy(src, _testFilePath, true);

          //create the stores
          _iniFileConfigStore = new IniFileConfigStore(_testFilePath, true, true);
          _commandLineConfigStore = new CommandLineConfigStore(new []{ "-key0:value1" });
       }

       [Fact]
       public void Read_config_values_in_correct_order()
       {
          IMultiStoreSettings settings = new ConfigurationBuilder<IMultiStoreSettings>()
             .UseConfigStore(_iniFileConfigStore)
             .UseConfigStore(_commandLineConfigStore)
             .Build();

          Assert.Equal("value1", settings.Key0);
       }

       /// <inheritdoc />
       public void Dispose()
       {
          _iniFileConfigStore.Dispose();
          _commandLineConfigStore.Dispose();
       }
    }

   public interface IMultiStoreSettings
   {
      string Key0 { get; }
   }
}
