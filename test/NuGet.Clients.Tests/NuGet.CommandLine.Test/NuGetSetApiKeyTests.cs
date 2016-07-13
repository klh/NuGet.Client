// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.CommandLine.Test
{
    public class NuGetSetApiKeyTests
    {
        [Fact]
        public void SetApiKey_DefaultSource()
        {
            var nugetexe = Util.GetNuGetExePath();

            using (var testFolder = TestFileSystemUtility.CreateRandomTestFolder())
            {
                var configFile = Path.Combine(testFolder, "nuget.config");
                Util.CreateFile(configFile, "<configuration/>");

                var apiKey = Guid.NewGuid().ToString();

                // Act
                var args = new [] { "setApiKey", apiKey, "-ConfigFile", configFile };
                var result = CommandRunner.Run(
                    nugetexe,
                    testFolder,
                    string.Join(" ", args),
                    waitForExit: true);

                // Assert
                Assert.Equal(0, result.Item1);
                
                var output = result.Item2;
                Assert.DoesNotContain("WARNING: No API Key was provided", output);

                var settings = Configuration.Settings.LoadDefaultSettings(
                    Path.GetDirectoryName(configFile),
                    Path.GetFileName(configFile),
                    null);
                var source = settings.GetValue("packageSources", "test_source");
                Assert.Equal("http://test_source", source);
            }
        }
    }
}
